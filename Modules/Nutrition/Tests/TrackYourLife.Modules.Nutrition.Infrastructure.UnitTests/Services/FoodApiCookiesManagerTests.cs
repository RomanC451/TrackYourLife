using System.Net;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using TrackYourLife.Modules.Nutrition.Infrastructure.Options;
using TrackYourLife.Modules.Nutrition.Infrastructure.Services;
using TrackYourLife.SharedLib.Contracts.Integration.Common;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Nutrition.Infrastructure.UnitTests.Services;

public class FoodApiCookiesManagerTests
{
    private readonly IRequestClient<GetCookiesByDomainsRequest> _getCookiesClient;
    private readonly IRequestClient<AddCookiesRequest> _addCookiesClient;
    private readonly IRequestClient<AddCookiesFromFilesRequest> _addCookiesFromFileClient;
    private readonly IOptions<FoodApiOptions> _foodApiOptions;
    private readonly FoodApiCookiesManager _sut;

    public FoodApiCookiesManagerTests()
    {
        _getCookiesClient = Substitute.For<IRequestClient<GetCookiesByDomainsRequest>>();
        _addCookiesClient = Substitute.For<IRequestClient<AddCookiesRequest>>();
        _addCookiesFromFileClient = Substitute.For<IRequestClient<AddCookiesFromFilesRequest>>();
        _foodApiOptions = Microsoft.Extensions.Options.Options.Create(
            new FoodApiOptions { CookieDomains = ["example.com"] }
        );

        _sut = new FoodApiCookiesManager(
            _getCookiesClient,
            _addCookiesClient,
            _addCookiesFromFileClient,
            _foodApiOptions
        );
    }

    [Fact]
    public async Task GetCookiesFromDbAsync_ShouldReturnCookiesFromResponse()
    {
        // Arrange
        var expectedCookies = new List<Cookie>
        {
            new("cookie1", "value1", "/", "example.com"),
            new("cookie2", "value2", "/", "example.com"),
        };

        var response = Substitute.For<Response<GetCookiesByDomainsResponse>>();
        response.Message.Returns(new GetCookiesByDomainsResponse(expectedCookies));

        _getCookiesClient
            .GetResponse<GetCookiesByDomainsResponse>(Arg.Any<GetCookiesByDomainsRequest>())
            .Returns(response);

        // Act
        var result = await _sut.GetCookiesFromDbAsync();

        // Assert
        result.Should().BeEquivalentTo(expectedCookies);
        await _getCookiesClient
            .Received(1)
            .GetResponse<GetCookiesByDomainsResponse>(
                Arg.Is<GetCookiesByDomainsRequest>(r => r.Domains.Contains("example.com"))
            );
    }

    [Fact]
    public async Task AddCookiesToDbAsync_WhenSuccessful_ShouldReturnSuccess()
    {
        // Arrange
        var cookies = new List<Cookie>
        {
            new("cookie1", "value1", "/", "example.com"),
            new("cookie2", "value2", "/", "example.com"),
        };

        var response = Substitute.For<Response<Result>>();
        response.Message.Returns(Result.Success());

        _addCookiesClient
            .GetResponse<Result>(Arg.Any<AddCookiesRequest>(), Arg.Any<CancellationToken>())
            .Returns(response);

        // Act
        var result = await _sut.AddCookiesToDbAsync(cookies, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _addCookiesClient
            .Received(1)
            .GetResponse<Result>(
                Arg.Is<AddCookiesRequest>(r => r.Cookies == cookies),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task AddCookiesToDbAsync_WhenFailed_ShouldReturnFailure()
    {
        // Arrange
        var cookies = new List<Cookie>
        {
            new("cookie1", "value1", "/", "example.com"),
            new("cookie2", "value2", "/", "example.com"),
        };

        var response = Substitute.For<Response<Result>>();
        response.Message.Returns(Result.Failure(IntegrationErrors.MassTransit.FailedRequest));

        _addCookiesClient
            .GetResponse<Result>(Arg.Any<AddCookiesRequest>(), Arg.Any<CancellationToken>())
            .Returns(response);

        // Act
        var result = await _sut.AddCookiesToDbAsync(cookies, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeEquivalentTo(IntegrationErrors.MassTransit.FailedRequest);
        await _addCookiesClient
            .Received(1)
            .GetResponse<Result>(
                Arg.Is<AddCookiesRequest>(r => r.Cookies == cookies),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task AddCookiesFromFilesToDbAsync_WhenSuccessful_ShouldReturnCookies()
    {
        // Arrange
        var expectedCookies = new List<Cookie>
        {
            new("cookie1", "value1", "/", "example.com"),
            new("cookie2", "value2", "/", "example.com"),
        };

        var fileBytes = new byte[] { 1, 2, 3, 4, 5 };
        var memoryStream = new MemoryStream(fileBytes);
        var formFile = Substitute.For<IFormFile>();
        formFile.OpenReadStream().Returns(memoryStream);
        formFile.Length.Returns(fileBytes.Length);

        var response = Substitute.For<Response<AddCookiesFromFilesResponse>>();
        response.Message.Returns(new AddCookiesFromFilesResponse(expectedCookies, Error.None));

        _addCookiesFromFileClient
            .GetResponse<AddCookiesFromFilesResponse>(
                Arg.Any<AddCookiesFromFilesRequest>(),
                Arg.Any<CancellationToken>()
            )
            .Returns(response);

        // Act
        var result = await _sut.AddCookiesFromFilesToDbAsync(formFile, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(expectedCookies);
        await _addCookiesFromFileClient
            .Received(1)
            .GetResponse<AddCookiesFromFilesResponse>(
                Arg.Any<AddCookiesFromFilesRequest>(),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task AddCookiesFromFilesToDbAsync_WhenFailed_ShouldReturnFailure()
    {
        // Arrange
        var fileBytes = new byte[] { 1, 2, 3, 4, 5 };
        var memoryStream = new MemoryStream(fileBytes);
        var formFile = Substitute.For<IFormFile>();
        formFile.OpenReadStream().Returns(memoryStream);
        formFile.Length.Returns(fileBytes.Length);

        var response = Substitute.For<Response<AddCookiesFromFilesResponse>>();
        response.Message.Returns(
            new AddCookiesFromFilesResponse(null, IntegrationErrors.MassTransit.FailedRequest)
        );

        _addCookiesFromFileClient
            .GetResponse<AddCookiesFromFilesResponse>(
                Arg.Any<AddCookiesFromFilesRequest>(),
                Arg.Any<CancellationToken>()
            )
            .Returns(response);

        // Act
        var result = await _sut.AddCookiesFromFilesToDbAsync(formFile, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeEquivalentTo(IntegrationErrors.MassTransit.FailedRequest);
        await _addCookiesFromFileClient
            .Received(1)
            .GetResponse<AddCookiesFromFilesResponse>(
                Arg.Any<AddCookiesFromFilesRequest>(),
                Arg.Any<CancellationToken>()
            );
    }
}
