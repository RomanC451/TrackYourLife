using MassTransit;
using Serilog;
using TrackYourLife.Modules.Common.Application.Core.Abstractions.Services;
using TrackYourLife.Modules.Common.Application.Features.Cookies.Consumers;
using TrackYourLife.Modules.Common.Domain.Core;
using TrackYourLife.Modules.Common.Domain.Features.Cookies;
using TrackYourLife.SharedLib.Contracts.Integration.Common;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Results;

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
        AddCookiesFromFilesErrorResponse response = null!;

        var context = Substitute.For<ConsumeContext<AddCookiesFromFilesRequest>>();
        context.Message.Returns(new AddCookiesFromFilesRequest([], [], []));

        await context.RespondAsync(Arg.Do<AddCookiesFromFilesErrorResponse>(x => response = x));

        cookieReader
            .GetCookiesAsync(Arg.Any<byte[]>(), Arg.Any<CancellationToken>())
            .Returns(
                Result.Failure<List<System.Net.Cookie>>(
                    InfrastructureErrors.CookieReader.FailedToReadCookies
                )
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
        cookieRepository.DidNotReceive().Update(Arg.Any<Cookie>());
        await unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Consume_WhenCookiesFound_ShouldAddOrUpdateCookies()
    {
        // Arrange
        Cookie addedCookie = null!;
        Cookie updatedCookie = null!;
        AddCookiesFromFilesResponse response = null!;

        List<System.Net.Cookie> cookies =
        [
            new("name1", "value1-2", "domain", "path"),
            new("name2", "value2", "domain", "path")
        ];

        var context = Substitute.For<ConsumeContext<AddCookiesFromFilesRequest>>();
        context.Message.Returns(new AddCookiesFromFilesRequest([], [], []));
        await context.RespondAsync(Arg.Do<AddCookiesFromFilesResponse>(x => response = x));

        cookieReader
            .GetCookiesAsync(Arg.Any<byte[]>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(cookies));

        cookieRepository
            .GetByNameAndDomainAsync(
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<CancellationToken>()
            )
            .Returns((Cookie)null!);

        cookieRepository
            .GetByNameAndDomainAsync(
                cookies[0].Name,
                cookies[0].Domain,
                Arg.Any<CancellationToken>()
            )
            .Returns(Cookie.Create(CookieId.NewId(), "name1", "value1", "domain", "path").Value);

        await cookieRepository.AddAsync(
            Arg.Do<Cookie>(x => addedCookie = x),
            Arg.Any<CancellationToken>()
        );

        cookieRepository.Update(Arg.Do<Cookie>(x => updatedCookie = x));

        // Act
        await sut.Consume(context);

        // Assert
        addedCookie.Name.Should().Be("name2");
        addedCookie.Value.Should().Be("value2");

        updatedCookie.Value.Should().Be("value1-2");

        response.Cookies.Count.Should().Be(2);

        await unitOfWork.Received().SaveChangesAsync(Arg.Any<CancellationToken>());

        cookieRepository.Received(1).Update(updatedCookie);
        await cookieRepository.Received(1).AddAsync(addedCookie, Arg.Any<CancellationToken>());
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
