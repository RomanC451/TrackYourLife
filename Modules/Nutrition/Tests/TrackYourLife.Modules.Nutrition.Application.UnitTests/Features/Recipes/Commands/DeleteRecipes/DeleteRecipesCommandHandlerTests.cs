using TrackYourLife.Modules.Nutrition.Application.Features.Recipes.Commands.DeleteRecipes;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;
using TrackYourLife.Modules.Nutrition.Domain.UnitTests.Utils;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Nutrition.Application.UnitTests.Features.Recipes.Commands.DeleteRecipes;

public class DeleteRecipesCommandHandlerTests
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly DeleteRecipesCommandHandler _handler;

    public DeleteRecipesCommandHandlerTests()
    {
        _recipeRepository = Substitute.For<IRecipeRepository>();
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _handler = new DeleteRecipesCommandHandler(_recipeRepository, _userIdentifierProvider);
    }

    [Fact]
    public async Task Handle_WhenAllRecipesExistAndBelongToUser_ShouldMarkRecipesAsOldAndReturnSuccess()
    {
        // Arrange
        var userId = UserId.NewId();
        var recipeIds = new[] { RecipeId.NewId(), RecipeId.NewId() };
        var recipes = recipeIds.Select(id => RecipeFaker.Generate(id: id, userId: userId)).ToList();
        var command = new DeleteRecipesCommand(recipeIds);

        _userIdentifierProvider.UserId.Returns(userId);
        foreach (var recipe in recipes)
        {
            _recipeRepository.GetByIdAsync(recipe.Id, Arg.Any<CancellationToken>()).Returns(recipe);
        }

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        recipes.ForEach(r => r.IsOld.Should().BeTrue());
        foreach (var recipe in recipes)
        {
            _recipeRepository.Received(1).Update(recipe);
        }
    }

    [Fact]
    public async Task Handle_WhenOneRecipeDoesNotExist_ShouldReturnFailure()
    {
        // Arrange
        var userId = UserId.NewId();
        var existingRecipeId = RecipeId.NewId();
        var nonExistingRecipeId = RecipeId.NewId();
        var existingRecipe = RecipeFaker.Generate(id: existingRecipeId, userId: userId);
        var command = new DeleteRecipesCommand([existingRecipeId, nonExistingRecipeId]);

        _userIdentifierProvider.UserId.Returns(userId);
        _recipeRepository
            .GetByIdAsync(existingRecipeId, Arg.Any<CancellationToken>())
            .Returns(existingRecipe);
        _recipeRepository
            .GetByIdAsync(nonExistingRecipeId, Arg.Any<CancellationToken>())
            .Returns((Recipe?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(RecipeErrors.NotFound(nonExistingRecipeId));
        _recipeRepository.DidNotReceive().Update(Arg.Any<Recipe>());
    }

    [Fact]
    public async Task Handle_WhenOneRecipeBelongsToDifferentUser_ShouldReturnFailure()
    {
        // Arrange
        var userId = UserId.NewId();
        var otherUserId = UserId.NewId();
        var userRecipeId = RecipeId.NewId();
        var otherUserRecipeId = RecipeId.NewId();
        var userRecipe = RecipeFaker.Generate(id: userRecipeId, userId: userId);
        var otherUserRecipe = RecipeFaker.Generate(id: otherUserRecipeId, userId: otherUserId);
        var command = new DeleteRecipesCommand([userRecipeId, otherUserRecipeId]);

        _userIdentifierProvider.UserId.Returns(userId);
        _recipeRepository
            .GetByIdAsync(userRecipeId, Arg.Any<CancellationToken>())
            .Returns(userRecipe);
        _recipeRepository
            .GetByIdAsync(otherUserRecipeId, Arg.Any<CancellationToken>())
            .Returns(otherUserRecipe);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(RecipeErrors.NotOwned(otherUserRecipeId));
        _recipeRepository.DidNotReceive().Update(Arg.Any<Recipe>());
    }
}
