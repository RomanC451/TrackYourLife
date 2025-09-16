using FluentAssertions;
using NSubstitute;
using TrackYourLife.Modules.Nutrition.Application.Features.Recipes.Commands.UpdateRecipe;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;
using TrackYourLife.Modules.Nutrition.Domain.UnitTests.Utils;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Nutrition.Application.UnitTests.Features.Recipes.Commands.UpdateRecipe;

public class UpdateRecipeCommandHandlerTests
{
    private readonly UpdateRecipeCommandHandler _handler;
    private readonly IRecipeRepository _recipeRepository;
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly UserId _userId;

    public UpdateRecipeCommandHandlerTests()
    {
        _recipeRepository = Substitute.For<IRecipeRepository>();
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _userId = UserId.NewId();
        _userIdentifierProvider.UserId.Returns(_userId);

        _handler = new UpdateRecipeCommandHandler(_recipeRepository, _userIdentifierProvider);
    }

    [Fact]
    public async Task Handle_WhenRecipeExistsAndBelongsToUser_ShouldUpdateRecipe()
    {
        // Arrange
        var recipe = RecipeFaker.Generate(userId: _userId);
        var command = new UpdateRecipeCommand(
            RecipeId: recipe.Id,
            Name: "Updated Recipe",
            Portions: 4,
            Weight: 100f
        );

        _recipeRepository.GetByIdAsync(command.RecipeId, default).Returns(recipe);
        _recipeRepository
            .GetByNameAndUserIdAsync(command.Name, _userId, default)
            .Returns((Recipe?)null);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        recipe.Name.Should().Be(command.Name);
        recipe.Portions.Should().Be(command.Portions);
        _recipeRepository.Received(1).Update(recipe);
    }

    [Fact]
    public async Task Handle_WhenRecipeDoesNotExist_ShouldReturnFailure()
    {
        // Arrange
        var command = new UpdateRecipeCommand(
            RecipeId: RecipeId.NewId(),
            Name: "Updated Recipe",
            Portions: 4,
            Weight: 100f
        );

        _recipeRepository.GetByIdAsync(command.RecipeId, default).Returns((Recipe?)null);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Recipe.NotFound");
        _recipeRepository.DidNotReceive().Update(Arg.Any<Recipe>());
    }

    [Fact]
    public async Task Handle_WhenRecipeBelongsToDifferentUser_ShouldReturnFailure()
    {
        // Arrange
        var otherUserId = UserId.NewId();
        var recipe = RecipeFaker.Generate(userId: otherUserId);
        var command = new UpdateRecipeCommand(
            RecipeId: recipe.Id,
            Name: "Updated Recipe",
            Portions: 4,
            Weight: 100f
        );

        _recipeRepository.GetByIdAsync(command.RecipeId, default).Returns(recipe);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Recipe.NotOwned");
        _recipeRepository.DidNotReceive().Update(Arg.Any<Recipe>());
    }

    [Fact]
    public async Task Handle_WhenRecipeWithSameNameExists_ShouldReturnFailure()
    {
        // Arrange
        var recipe = RecipeFaker.Generate(userId: _userId);
        var existingRecipe = RecipeFaker.Generate(userId: _userId);
        var command = new UpdateRecipeCommand(
            RecipeId: recipe.Id,
            Name: existingRecipe.Name,
            Portions: 4,
            Weight: 100f
        );

        _recipeRepository.GetByIdAsync(command.RecipeId, default).Returns(recipe);
        _recipeRepository
            .GetByNameAndUserIdAsync(command.Name, _userId, default)
            .Returns(existingRecipe);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Recipe.AlreadyExists");
        _recipeRepository.DidNotReceive().Update(Arg.Any<Recipe>());
    }

    [Fact]
    public async Task Handle_WhenUpdateFails_ShouldReturnFailure()
    {
        // Arrange
        var recipe = RecipeFaker.Generate(userId: _userId);
        var command = new UpdateRecipeCommand(
            RecipeId: recipe.Id,
            Name: string.Empty,
            Portions: 4,
            Weight: 100f
        );

        _recipeRepository.GetByIdAsync(command.RecipeId, default).Returns(recipe);
        _recipeRepository
            .GetByNameAndUserIdAsync(command.Name, _userId, default)
            .Returns((Recipe?)null);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        _recipeRepository.DidNotReceive().Update(Arg.Any<Recipe>());
    }
}
