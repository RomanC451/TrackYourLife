using MassTransit;
using Serilog;
using TrackYourLife.Modules.Common.Application.Features.Cookies.Consumers;
using TrackYourLife.Modules.Common.Domain.Core;
using TrackYourLife.Modules.Common.Domain.Features.Cookies;
using TrackYourLife.SharedLib.Contracts.Integration.Common;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Common.Application.UnitTests.Features.Cookies.Consumers;

public sealed class AddCookiesConsumerTest : IDisposable
{
    private readonly AddCookiesConsumer sut;
    private readonly ICookieRepository cookieRepository = Substitute.For<ICookieRepository>();
    private readonly ILogger logger = Substitute.For<ILogger>();
    private readonly ICommonUnitOfWork unitOfWork = Substitute.For<ICommonUnitOfWork>();

    public AddCookiesConsumerTest()
    {
        sut = new AddCookiesConsumer(cookieRepository, logger, unitOfWork);
    }

    [Fact]
    public async Task Consume_WhenCookieExists_UpdatesCookie()
    {
        // Arrange
        AddCookiesResponse response = null!;
        Cookie updatedCookie = null!;

        var request = new AddCookiesRequest([new("name", "value2", "path", "domain")]);
        var context = Substitute.For<ConsumeContext<AddCookiesRequest>>();
        context.Message.Returns(request);
        cookieRepository.Update(Arg.Do<Cookie>(x => updatedCookie = x));

        await context.RespondAsync(Arg.Do<AddCookiesResponse>(x => response = x));

        var existingCookie = Cookie
            .Create(CookieId.NewId(), "name", "value1", "path", "domain")
            .Value;

        cookieRepository
            .GetByNameAndDomainAsync("name", "domain", Arg.Any<CancellationToken>())
            .Returns(existingCookie);

        // Act
        await sut.Consume(context);

        // Assert
        response.Should().BeEquivalentTo(new AddCookiesResponse(true));

        updatedCookie.Value.Should().Be("value2");

        cookieRepository.Received(1).Update(existingCookie);
        await unitOfWork.Received(1).SaveChangesAsync(CancellationToken.None);
    }

    [Fact]
    public async Task Consume_WhenCookieDoesNotExist_AddsNewCookie()
    {
        // Arrange
        Cookie addedCookie = null!;
        AddCookiesResponse response = null!;

        var request = new AddCookiesRequest([new("name", "value", "path", "domain")]);
        var context = Substitute.For<ConsumeContext<AddCookiesRequest>>();
        context.Message.Returns(request);
        await cookieRepository.AddAsync(
            Arg.Do<Cookie>(x => addedCookie = x),
            Arg.Any<CancellationToken>()
        );

        await context.RespondAsync(Arg.Do<AddCookiesResponse>(x => response = x));

        cookieRepository
            .GetByNameAndDomainAsync("name", "domain", CancellationToken.None)
            .Returns((Cookie)null!);

        // Act
        await sut.Consume(context);

        // Assert
        response.Should().BeEquivalentTo(new AddCookiesResponse(true));

        addedCookie.Name.Should().Be("name");
        addedCookie.Value.Should().Be("value");
        addedCookie.Path.Should().Be("path");
        addedCookie.Domain.Should().Be("domain");

        await cookieRepository.Received(1).AddAsync(Arg.Any<Cookie>(), CancellationToken.None);
        await unitOfWork.Received(1).SaveChangesAsync(CancellationToken.None);
    }

    [Fact]
    public async Task Consume_WhenCookieCreationFails_LogsError()
    {
        // Arrange
        AddCookiesResponse response = null!;

        var request = new AddCookiesRequest([new("name", "", "domain", "path")]);
        var context = Substitute.For<ConsumeContext<AddCookiesRequest>>();
        context.Message.Returns(request);

        await context.RespondAsync(Arg.Do<AddCookiesResponse>(x => response = x));

        // Act
        await sut.Consume(context);

        // Assert
        response.Should().BeEquivalentTo(new AddCookiesResponse(true));

        logger
            .Received(1)
            .Error(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<Error>());

        await unitOfWork.Received(1).SaveChangesAsync(CancellationToken.None);
    }

    [Fact]
    public async Task Consume_WhenMultipleCookiesProvided_HandlesAllCookies()
    {
        // Arrange
        List<Cookie> addedCookies = new();
        AddCookiesResponse response = null!;

        var request = new AddCookiesRequest(
            [new("name1", "value1", "path1", "domain1"), new("name2", "value2", "path2", "domain2")]
        );
        var context = Substitute.For<ConsumeContext<AddCookiesRequest>>();
        context.Message.Returns(request);
        await cookieRepository.AddAsync(
            Arg.Do<Cookie>(x => addedCookies.Add(x)),
            Arg.Any<CancellationToken>()
        );

        await context.RespondAsync(Arg.Do<AddCookiesResponse>(x => response = x));

        cookieRepository
            .GetByNameAndDomainAsync(
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<CancellationToken>()
            )
            .Returns((Cookie)null!);

        // Act
        await sut.Consume(context);

        // Assert
        response.Should().BeEquivalentTo(new AddCookiesResponse(true));
        addedCookies.Should().HaveCount(2);
        addedCookies.Should().Contain(x => x.Name == "name1" && x.Value == "value1");
        addedCookies.Should().Contain(x => x.Name == "name2" && x.Value == "value2");

        await cookieRepository
            .Received(2)
            .AddAsync(Arg.Any<Cookie>(), Arg.Any<CancellationToken>());
        await unitOfWork.Received(1).SaveChangesAsync(CancellationToken.None);
    }

    [Fact]
    public async Task Consume_WhenCookieValueIsNull_LogsErrorAndContinues()
    {
        // Arrange
        AddCookiesResponse response = null!;

        var request = new AddCookiesRequest([new("name", null!, "path", "domain")]);
        var context = Substitute.For<ConsumeContext<AddCookiesRequest>>();
        context.Message.Returns(request);

        await context.RespondAsync<AddCookiesResponse>(
            Arg.Do<AddCookiesResponse>(x => response = x)
        );

        // Act
        await sut.Consume(context);

        // Assert
        response.Should().BeEquivalentTo(new AddCookiesResponse(true));
        logger
            .Received(1)
            .Error(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<Error>());
        await unitOfWork.Received(1).SaveChangesAsync(CancellationToken.None);
    }

    [Fact]
    public async Task Consume_WhenCookieDataIsInvalid_LogsErrorAndContinues()
    {
        // Arrange
        AddCookiesResponse response = null!;

        var request = new AddCookiesRequest([new("name", "", "path", "domain")]);
        var context = Substitute.For<ConsumeContext<AddCookiesRequest>>();
        context.Message.Returns(request);

        await context.RespondAsync(Arg.Do<AddCookiesResponse>(x => response = x));

        // Act
        await sut.Consume(context);

        // Assert
        response.Should().BeEquivalentTo(new AddCookiesResponse(true));
        logger
            .Received(1)
            .Error(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<Error>());
        await unitOfWork.Received(1).SaveChangesAsync(CancellationToken.None);
    }

    public void Dispose()
    {
        cookieRepository.ClearSubstitute();
        logger.ClearSubstitute();
        unitOfWork.ClearSubstitute();
        GC.SuppressFinalize(this);
    }
}
