using TrackYourLife.Modules.Nutrition.Application.Features.Recipes.Commands.CreateRecipe;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;
using TrackYourLife.Modules.Nutrition.Domain.UnitTests.Utils;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Nutrition.Application.UnitTests.Features.Recipes.Commands.CreateRecipe;

public class CreateRecipeCommandHandlerTests
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly CreateRecipeCommandHandler _handler;

    public CreateRecipeCommandHandlerTests()
    {
        _recipeRepository = Substitute.For<IRecipeRepository>();
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _handler = new CreateRecipeCommandHandler(_recipeRepository, _userIdentifierProvider);
    }

    [Fact]
    public async Task Handle_WhenAllConditionsAreMet_ShouldCreateRecipeAndReturnSuccess()
    {
        // Arrange
        var userId = UserId.NewId();
        var recipeName = "Test Recipe";
        var command = new CreateRecipeCommand(recipeName);

        _userIdentifierProvider.UserId.Returns(userId);
        _recipeRepository
            .GetByNameAndUserIdAsync(recipeName, userId, Arg.Any<CancellationToken>())
            .Returns((Recipe?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        await _recipeRepository
            .Received(1)
            .AddAsync(Arg.Is<Recipe>(r => r.Name == recipeName), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenRecipeWithSameNameExists_ShouldReturnFailure()
    {
        // Arrange
        var userId = UserId.NewId();
        var recipeName = "Test Recipe";
        var command = new CreateRecipeCommand(recipeName);
        var existingRecipe = RecipeFaker.Generate(name: recipeName, userId: userId);

        _userIdentifierProvider.UserId.Returns(userId);
        _recipeRepository
            .GetByNameAndUserIdAsync(recipeName, userId, Arg.Any<CancellationToken>())
            .Returns(existingRecipe);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(RecipeErrors.AlreadyExists(recipeName));
        await _recipeRepository
            .DidNotReceive()
            .AddAsync(Arg.Any<Recipe>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenRecipeCreationFails_ShouldReturnFailure()
    {
        // Arrange
        var userId = UserId.NewId();
        var command = new CreateRecipeCommand(string.Empty);

        _userIdentifierProvider.UserId.Returns(userId);
        _recipeRepository
            .GetByNameAndUserIdAsync(string.Empty, userId, Arg.Any<CancellationToken>())
            .Returns((Recipe?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        await _recipeRepository
            .DidNotReceive()
            .AddAsync(Arg.Any<Recipe>(), Arg.Any<CancellationToken>());
    }
}
