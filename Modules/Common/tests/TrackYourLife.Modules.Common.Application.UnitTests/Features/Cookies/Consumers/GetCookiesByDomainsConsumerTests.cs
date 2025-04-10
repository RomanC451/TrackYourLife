using MassTransit;
using TrackYourLife.Modules.Common.Application.Features.Cookies.Consumers;
using TrackYourLife.Modules.Common.Domain.Features.Cookies;
using TrackYourLife.SharedLib.Contracts.Integration.Common;

namespace TrackYourLife.Modules.Common.Application.UnitTests.Features.Cookies.Consumers;

public sealed class GetCookiesByDomainsConsumerTest : IDisposable
{
    private readonly GetCookiesByDomainsConsumer sut;
    private readonly ICookieQuery cookieQuery = Substitute.For<ICookieQuery>();

    public GetCookiesByDomainsConsumerTest()
    {
        sut = new GetCookiesByDomainsConsumer(cookieQuery);
    }

    [Fact]
    public async Task Consume_WhenDomainsAreValid_ShouldReturnCookies()
    {
        // Arrange
        GetCookiesByDomainsResponse response = null!;

        var cookies = new List<CookieReadModel>
        {
            new(CookieId.NewId(), "name", "value", "domain.com", "/"),
        };

        var request = new GetCookiesByDomainsRequest(["domain.com"]);
        var context = Substitute.For<ConsumeContext<GetCookiesByDomainsRequest>>();
        context.Message.Returns(request);
        await context.RespondAsync(Arg.Do<GetCookiesByDomainsResponse>(x => response = x));

        cookieQuery.GetCookiesByDomainAsync("domain.com", CancellationToken.None).Returns(cookies);

        // Act
        await sut.Consume(context);

        // Assert
        response.Cookies.Should().HaveCount(1);
        response.Cookies[0].Name.Should().Be("name");
        response.Cookies[0].Value.Should().Be("value");
        response.Cookies[0].Domain.Should().Be("domain.com");
        response.Cookies[0].Path.Should().Be("/");

        await cookieQuery.Received(1).GetCookiesByDomainAsync("domain.com", CancellationToken.None);
    }

    [Fact]
    public async Task Consume_WhenDomainIsEmpty_ShouldReturnEmptyList()
    {
        // Arrange
        GetCookiesByDomainsResponse response = null!;

        var request = new GetCookiesByDomainsRequest([]);
        var context = Substitute.For<ConsumeContext<GetCookiesByDomainsRequest>>();
        context.Message.Returns(request);
        await context.RespondAsync(Arg.Do<GetCookiesByDomainsResponse>(x => response = x));

        // Act
        await sut.Consume(context);

        // Assert
        response.Cookies.Should().BeEmpty();
    }

    [Fact]
    public async Task Consume_WhenCookiesAreNotFound_ShouldReturnEmptyList()
    {
        // Arrange
        GetCookiesByDomainsResponse response = null!;

        var request = new GetCookiesByDomainsRequest(new[] { "example.com" });
        var context = Substitute.For<ConsumeContext<GetCookiesByDomainsRequest>>();
        context.Message.Returns(request);
        await context.RespondAsync(Arg.Do<GetCookiesByDomainsResponse>(x => response = x));

        cookieQuery.GetCookiesByDomainAsync("example.com", CancellationToken.None).Returns([]);

        // Act
        await sut.Consume(context);

        // Assert
        response.Cookies.Should().BeEmpty();
    }

    [Fact]
    public async Task Consume_WhenMultipleDomainsProvided_ShouldReturnCookiesForAllDomains()
    {
        // Arrange
        GetCookiesByDomainsResponse response = null!;

        var cookies1 = new List<CookieReadModel>
        {
            new(CookieId.NewId(), "name1", "value1", "domain1.com", "/"),
        };

        var cookies2 = new List<CookieReadModel>
        {
            new(CookieId.NewId(), "name2", "value2", "domain2.com", "/"),
        };

        var request = new GetCookiesByDomainsRequest(["domain1.com", "domain2.com"]);
        var context = Substitute.For<ConsumeContext<GetCookiesByDomainsRequest>>();
        context.Message.Returns(request);
        await context.RespondAsync(Arg.Do<GetCookiesByDomainsResponse>(x => response = x));

        cookieQuery
            .GetCookiesByDomainAsync("domain1.com", CancellationToken.None)
            .Returns(cookies1);
        cookieQuery
            .GetCookiesByDomainAsync("domain2.com", CancellationToken.None)
            .Returns(cookies2);

        // Act
        await sut.Consume(context);

        // Assert
        response.Cookies.Should().HaveCount(2);
        response.Cookies.Should().Contain(x => x.Name == "name1" && x.Domain == "domain1.com");
        response.Cookies.Should().Contain(x => x.Name == "name2" && x.Domain == "domain2.com");

        await cookieQuery
            .Received(1)
            .GetCookiesByDomainAsync("domain1.com", CancellationToken.None);
        await cookieQuery
            .Received(1)
            .GetCookiesByDomainAsync("domain2.com", CancellationToken.None);
    }

    [Fact]
    public async Task Consume_WhenDomainsContainNull_ShouldSkipNullDomains()
    {
        // Arrange
        GetCookiesByDomainsResponse response = null!;

        var cookies = new List<CookieReadModel>
        {
            new(CookieId.NewId(), "name", "value", "domain.com", "/"),
        };

        var request = new GetCookiesByDomainsRequest(["domain.com", null!]);
        var context = Substitute.For<ConsumeContext<GetCookiesByDomainsRequest>>();
        context.Message.Returns(request);
        await context.RespondAsync(Arg.Do<GetCookiesByDomainsResponse>(x => response = x));

        cookieQuery.GetCookiesByDomainAsync("domain.com", CancellationToken.None).Returns(cookies);

        // Act
        await sut.Consume(context);

        // Assert
        response.Cookies.Should().HaveCount(1);
        response.Cookies[0].Name.Should().Be("name");
        response.Cookies[0].Domain.Should().Be("domain.com");

        await cookieQuery.Received(1).GetCookiesByDomainAsync("domain.com", CancellationToken.None);
        await cookieQuery
            .DidNotReceive()
            .GetCookiesByDomainAsync(null!, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Consume_WhenDomainsContainInvalidFormat_ShouldSkipInvalidDomains()
    {
        // Arrange
        GetCookiesByDomainsResponse response = null!;

        var cookies = new List<CookieReadModel>
        {
            new(CookieId.NewId(), "name", "value", "domain.com", "/"),
        };

        var request = new GetCookiesByDomainsRequest(["domain.com", "invalid domain"]);
        var context = Substitute.For<ConsumeContext<GetCookiesByDomainsRequest>>();
        context.Message.Returns(request);
        await context.RespondAsync(Arg.Do<GetCookiesByDomainsResponse>(x => response = x));

        cookieQuery.GetCookiesByDomainAsync("domain.com", CancellationToken.None).Returns(cookies);

        // Act
        await sut.Consume(context);

        // Assert
        response.Cookies.Should().HaveCount(1);
        response.Cookies[0].Name.Should().Be("name");
        response.Cookies[0].Domain.Should().Be("domain.com");

        await cookieQuery.Received(1).GetCookiesByDomainAsync("domain.com", CancellationToken.None);
        await cookieQuery
            .DidNotReceive()
            .GetCookiesByDomainAsync("invalid domain", Arg.Any<CancellationToken>());
    }

    public void Dispose()
    {
        cookieQuery.ClearSubstitute();
        GC.SuppressFinalize(this);
    }
}
