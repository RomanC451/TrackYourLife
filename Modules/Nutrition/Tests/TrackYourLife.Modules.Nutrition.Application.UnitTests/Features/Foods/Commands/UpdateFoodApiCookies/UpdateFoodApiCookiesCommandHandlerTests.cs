using System.Net;
using Microsoft.AspNetCore.Http;
using TrackYourLife.Modules.Nutrition.Application.Core.Abstraction.Services;
using TrackYourLife.Modules.Nutrition.Application.Features.Foods.Commands.UpdateFoodApiCookies;
using TrackYourLife.Modules.Nutrition.Domain.Core;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Nutrition.Application.UnitTests.Features.Foods.Commands.UpdateFoodApiCookies;

public class UpdateFoodApiCookiesCommandHandlerTests
{
    private readonly IFoodApiCookiesManager _foodApiCookiesManager;
    private readonly FoodApiCookieContainer _foodApiCookieContainer;
    private readonly UpdateFoodApiCookiesCommandHandler _handler;

    public UpdateFoodApiCookiesCommandHandlerTests()
    {
        _foodApiCookiesManager = Substitute.For<IFoodApiCookiesManager>();
        _foodApiCookieContainer = new FoodApiCookieContainer();
        _handler = new UpdateFoodApiCookiesCommandHandler(
            _foodApiCookiesManager,
            _foodApiCookieContainer
        );
    }

    [Fact]
    public async Task Handle_WhenAddCookiesFails_ShouldReturnFailure()
    {
        // Arrange
        var formFile = Substitute.For<IFormFile>();
        var command = new UpdateFoodApiCookiesCommand(formFile);
        var expectedError = new Error("Test", "Test error");

        _foodApiCookiesManager
            .AddCookiesFromFilesToDbAsync(formFile, Arg.Any<CancellationToken>())
            .Returns(Result.Failure<List<Cookie>>(expectedError));

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(expectedError);
        _foodApiCookieContainer.Count.Should().Be(0);
    }

    [Fact]
    public async Task Handle_WhenAddCookiesSucceeds_ShouldAddCookiesToContainerAndReturnSuccess()
    {
        // Arrange
        var formFile = Substitute.For<IFormFile>();
        var command = new UpdateFoodApiCookiesCommand(formFile);
        var cookies = new List<Cookie>
        {
            new("cookie1", "value1", "/", "example.com"),
            new("cookie2", "value2", "/", "example.com"),
        };

        _foodApiCookiesManager
            .AddCookiesFromFilesToDbAsync(formFile, Arg.Any<CancellationToken>())
            .Returns(Result.Success<List<Cookie>>(cookies));

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _foodApiCookieContainer.Count.Should().Be(cookies.Count);
    }
}
