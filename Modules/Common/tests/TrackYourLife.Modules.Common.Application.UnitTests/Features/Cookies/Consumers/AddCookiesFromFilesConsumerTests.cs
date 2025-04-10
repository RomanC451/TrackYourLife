using MassTransit;
using Serilog;
using TrackYourLife.Modules.Common.Application.Core.Abstraction.Services;
using TrackYourLife.Modules.Common.Application.Features.Cookies.Consumers;
using TrackYourLife.Modules.Common.Domain.Core;
using TrackYourLife.Modules.Common.Domain.Features.Cookies;
using TrackYourLife.SharedLib.Contracts.Integration.Common;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Results;
using Cookie = System.Net.Cookie;

namespace TrackYourLife.Modules.Common.Application.UnitTests.Features.Cookies.Consumers;

public sealed class AddCookiesFromFilesConsumerTest : IDisposable
{
    private readonly AddCookiesFromFilesConsumer sut;
    private readonly ICookiesReader cookieReader = Substitute.For<ICookiesReader>();
    private readonly ICookieRepository cookieRepository = Substitute.For<ICookieRepository>();
    private readonly ILogger logger = Substitute.For<ILogger>();
    private readonly ICommonUnitOfWork unitOfWork = Substitute.For<ICommonUnitOfWork>();

    public AddCookiesFromFilesConsumerTest()
    {
        sut = new AddCookiesFromFilesConsumer(cookieReader, cookieRepository, logger, unitOfWork);
    }

    [Fact]
    public async Task Consume_WhenNoCookiesFound_ShouldRespondWithError()
    {
        // Arrange
        AddCookiesFromFilesResponse response = null!;
        var context = Substitute.For<ConsumeContext<AddCookiesFromFilesRequest>>();
        context.Message.Returns(new AddCookiesFromFilesRequest([]));
        await context.RespondAsync(Arg.Do<AddCookiesFromFilesResponse>(x => response = x));

        cookieReader
            .GetCookiesAsync(Arg.Any<byte[]>(), Arg.Any<CancellationToken>())
            .Returns(
                Result.Failure<List<Cookie>>(InfrastructureErrors.CookieReader.FailedToReadCookies)
            );

        // Act
        await sut.Consume(context);

        // Assert
        response.Error.Should().Be(InfrastructureErrors.CookieReader.FailedToReadCookies);
        await cookieRepository
            .DidNotReceive()
            .GetByNameAndDomainAsync(
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<CancellationToken>()
            );
        cookieRepository.DidNotReceive().Update(Arg.Any<Domain.Features.Cookies.Cookie>());
        await unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Consume_WhenCookiesFound_ShouldAddOrUpdateCookies()
    {
        // Arrange
        Domain.Features.Cookies.Cookie addedCookie = null!;
        Domain.Features.Cookies.Cookie updatedCookie = null!;
        AddCookiesFromFilesResponse response = null!;
        var cookies = new List<Cookie>
        {
            new Cookie("name1", "value1-2") { Domain = "domain", Path = "path" },
            new Cookie("name2", "value2") { Domain = "domain", Path = "path" },
        };

        var context = Substitute.For<ConsumeContext<AddCookiesFromFilesRequest>>();
        context.Message.Returns(new AddCookiesFromFilesRequest(new byte[0]));
        await context.RespondAsync(Arg.Do<AddCookiesFromFilesResponse>(x => response = x));

        cookieReader
            .GetCookiesAsync(Arg.Any<byte[]>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(cookies));

        cookieRepository
            .GetByNameAndDomainAsync(
                cookies[0].Name,
                cookies[0].Domain,
                Arg.Any<CancellationToken>()
            )
            .Returns(
                Domain
                    .Features.Cookies.Cookie.Create(
                        CookieId.NewId(),
                        "name1",
                        "value1",
                        "domain",
                        "path"
                    )
                    .Value
            );

        await cookieRepository.AddAsync(
            Arg.Do<Domain.Features.Cookies.Cookie>(x => addedCookie = x),
            Arg.Any<CancellationToken>()
        );
        cookieRepository.Update(Arg.Do<Domain.Features.Cookies.Cookie>(x => updatedCookie = x));

        // Act
        await sut.Consume(context);

        // Assert
        addedCookie.Name.Should().Be("name2");
        addedCookie.Value.Should().Be("value2");
        updatedCookie.Value.Should().Be("value1-2");
        response.Data.Should().HaveCount(2);
        await unitOfWork.Received().SaveChangesAsync(Arg.Any<CancellationToken>());
        cookieRepository.Received(1).Update(updatedCookie);
        await cookieRepository.Received(1).AddAsync(addedCookie, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Consume_WhenFileDataIsEmpty_ShouldReturnError()
    {
        // Arrange
        AddCookiesFromFilesResponse response = null!;
        var context = Substitute.For<ConsumeContext<AddCookiesFromFilesRequest>>();
        context.Message.Returns(new AddCookiesFromFilesRequest(Array.Empty<byte>()));
        await context.RespondAsync(Arg.Do<AddCookiesFromFilesResponse>(x => response = x));

        cookieReader
            .GetCookiesAsync(Arg.Any<byte[]>(), Arg.Any<CancellationToken>())
            .Returns(
                Result.Failure<List<Cookie>>(InfrastructureErrors.CookieReader.FailedToReadCookies)
            );

        // Act
        await sut.Consume(context);

        // Assert
        response.Error.Should().Be(InfrastructureErrors.CookieReader.FailedToReadCookies);
        await cookieRepository
            .DidNotReceive()
            .GetByNameAndDomainAsync(
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<CancellationToken>()
            );
        cookieRepository.DidNotReceive().Update(Arg.Any<Domain.Features.Cookies.Cookie>());
        await unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Consume_WhenFileFormatIsInvalid_ShouldReturnError()
    {
        // Arrange
        AddCookiesFromFilesResponse response = null!;
        var invalidData = new byte[] { 0x00, 0x01, 0x02 }; // Invalid cookie file format
        var context = Substitute.For<ConsumeContext<AddCookiesFromFilesRequest>>();
        context.Message.Returns(new AddCookiesFromFilesRequest(invalidData));
        await context.RespondAsync(Arg.Do<AddCookiesFromFilesResponse>(x => response = x));

        cookieReader
            .GetCookiesAsync(Arg.Any<byte[]>(), Arg.Any<CancellationToken>())
            .Returns(
                Result.Failure<List<Cookie>>(InfrastructureErrors.CookieReader.FailedToReadCookies)
            );

        // Act
        await sut.Consume(context);

        // Assert
        response.Error.Should().Be(InfrastructureErrors.CookieReader.FailedToReadCookies);
        await cookieRepository
            .DidNotReceive()
            .GetByNameAndDomainAsync(
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<CancellationToken>()
            );
        cookieRepository.DidNotReceive().Update(Arg.Any<Domain.Features.Cookies.Cookie>());
        await unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Consume_WhenDuplicateCookiesFound_ShouldUpdateExistingOnes()
    {
        // Arrange
        Domain.Features.Cookies.Cookie updatedCookie = null!;
        AddCookiesFromFilesResponse response = null!;
        var cookies = new List<Cookie>
        {
            new Cookie("name1", "value1") { Domain = "domain", Path = "path" },
            new Cookie("name1", "value2") { Domain = "domain", Path = "path" }, // Duplicate
        };

        var context = Substitute.For<ConsumeContext<AddCookiesFromFilesRequest>>();
        context.Message.Returns(new AddCookiesFromFilesRequest(new byte[0]));
        await context.RespondAsync(Arg.Do<AddCookiesFromFilesResponse>(x => response = x));

        cookieReader
            .GetCookiesAsync(Arg.Any<byte[]>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(cookies));

        var existingCookie = Domain
            .Features.Cookies.Cookie.Create(
                CookieId.NewId(),
                "name1",
                "old-value",
                "domain",
                "path"
            )
            .Value;

        cookieRepository
            .GetByNameAndDomainAsync("name1", "domain", Arg.Any<CancellationToken>())
            .Returns(existingCookie);

        cookieRepository.Update(Arg.Do<Domain.Features.Cookies.Cookie>(x => updatedCookie = x));

        // Act
        await sut.Consume(context);

        // Assert
        updatedCookie.Value.Should().Be("value2"); // Should use the last value
        response.Data.Should().NotBeNull();
        response.Data!.Should().HaveCount(1);
        response.Data![0].Name.Should().Be("name1");
        response.Data![0].Value.Should().Be("value2");
        await unitOfWork.Received().SaveChangesAsync(Arg.Any<CancellationToken>());
        cookieRepository.Received(1).Update(updatedCookie);
        await cookieRepository
            .DidNotReceive()
            .AddAsync(Arg.Any<Domain.Features.Cookies.Cookie>(), Arg.Any<CancellationToken>());
    }

    public void Dispose()
    {
        cookieReader.ClearSubstitute();
        cookieRepository.ClearSubstitute();
        logger.ClearSubstitute();
        unitOfWork.ClearSubstitute();
        GC.SuppressFinalize(this);
    }
}
