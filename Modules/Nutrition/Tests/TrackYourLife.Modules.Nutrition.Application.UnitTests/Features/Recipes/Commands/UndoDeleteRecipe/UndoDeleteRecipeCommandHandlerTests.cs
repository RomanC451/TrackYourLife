using FluentAssertions;
using NSubstitute;
using TrackYourLife.Modules.Nutrition.Application.Features.Recipes.Commands.UndoDeleteRecipe;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;
using TrackYourLife.Modules.Nutrition.Domain.UnitTests.Utils;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Nutrition.Application.UnitTests.Features.Recipes.Commands.UndoDeleteRecipe;

public class UndoDeleteRecipeCommandHandlerTests
{
    private readonly UndoDeleteRecipeCommandHandler _handler;
    private readonly IRecipeRepository _recipeRepository;
    private readonly IUserIdentifierProvider _userIdentifierProvider;

    public UndoDeleteRecipeCommandHandlerTests()
    {
        _recipeRepository = Substitute.For<IRecipeRepository>();
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _handler = new UndoDeleteRecipeCommandHandler(_recipeRepository, _userIdentifierProvider);
    }

    [Fact]
    public async Task Handle_WhenRecipeExistsAndBelongsToUser_ShouldRemoveOldMark()
    {
        // Arrange
        var recipeId = RecipeId.NewId();
        var userId = UserId.NewId();
        var command = new UndoDeleteRecipeCommand(recipeId);

        var recipe = RecipeFaker.Generate(id: recipeId, userId: userId);
        recipe.MarkAsOld();

        _userIdentifierProvider.UserId.Returns(userId);
        _recipeRepository.GetOldByIdAsync(recipeId, Arg.Any<CancellationToken>()).Returns(recipe);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        recipe.IsOld.Should().BeFalse();
        _recipeRepository.Received(1).Update(recipe);
    }

    [Fact]
    public async Task Handle_WhenRecipeDoesNotExist_ShouldReturnFailure()
    {
        // Arrange
        var recipeId = RecipeId.NewId();
        var userId = UserId.NewId();
        var command = new UndoDeleteRecipeCommand(recipeId);

        _userIdentifierProvider.UserId.Returns(userId);
        _recipeRepository
            .GetOldByIdAsync(recipeId, Arg.Any<CancellationToken>())
            .Returns((Recipe?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Recipe.NotFound");
        _recipeRepository.DidNotReceive().Update(Arg.Any<Recipe>());
    }

    [Fact]
    public async Task Handle_WhenRecipeBelongsToDifferentUser_ShouldReturnFailure()
    {
        // Arrange
        var recipeId = RecipeId.NewId();
        var userId = UserId.NewId();
        var otherUserId = UserId.NewId();
        var command = new UndoDeleteRecipeCommand(recipeId);

        var recipe = RecipeFaker.Generate(id: recipeId, userId: otherUserId);
        recipe.MarkAsOld();

        _userIdentifierProvider.UserId.Returns(userId);
        _recipeRepository.GetOldByIdAsync(recipeId, Arg.Any<CancellationToken>()).Returns(recipe);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Recipe.NotOwned");
        _recipeRepository.DidNotReceive().Update(Arg.Any<Recipe>());
    }
}
