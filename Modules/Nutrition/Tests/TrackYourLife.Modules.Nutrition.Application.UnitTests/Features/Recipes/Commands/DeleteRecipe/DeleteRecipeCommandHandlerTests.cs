using TrackYourLife.Modules.Nutrition.Application.Features.Recipes.Commands.DeleteRecipe;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;
using TrackYourLife.Modules.Nutrition.Domain.UnitTests.Utils;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Nutrition.Application.UnitTests.Features.Recipes.Commands.DeleteRecipe;

public class DeleteRecipeCommandHandlerTests
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly DeleteRecipeCommandHandler _handler;

    public DeleteRecipeCommandHandlerTests()
    {
        _recipeRepository = Substitute.For<IRecipeRepository>();
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _handler = new DeleteRecipeCommandHandler(_recipeRepository, _userIdentifierProvider);
    }

    [Fact]
    public async Task Handle_WhenRecipeExistsAndBelongsToUser_ShouldMarkRecipeAsOldAndReturnSuccess()
    {
        // Arrange
        var userId = UserId.NewId();
        var recipeId = RecipeId.NewId();
        var recipe = RecipeFaker.Generate(id: recipeId, userId: userId);
        var command = new DeleteRecipeCommand(recipeId);

        _userIdentifierProvider.UserId.Returns(userId);
        _recipeRepository.GetByIdAsync(recipeId, Arg.Any<CancellationToken>()).Returns(recipe);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        recipe.IsOld.Should().BeTrue();
        _recipeRepository.Received(1).Update(recipe);
    }

    [Fact]
    public async Task Handle_WhenRecipeDoesNotExist_ShouldReturnFailure()
    {
        // Arrange
        var userId = UserId.NewId();
        var recipeId = RecipeId.NewId();
        var command = new DeleteRecipeCommand(recipeId);

        _userIdentifierProvider.UserId.Returns(userId);
        _recipeRepository
            .GetByIdAsync(recipeId, Arg.Any<CancellationToken>())
            .Returns((Recipe?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(RecipeErrors.NotFound(recipeId));
        _recipeRepository.DidNotReceive().Update(Arg.Any<Recipe>());
    }

    [Fact]
    public async Task Handle_WhenRecipeBelongsToDifferentUser_ShouldReturnFailure()
    {
        // Arrange
        var userId = UserId.NewId();
        var otherUserId = UserId.NewId();
        var recipeId = RecipeId.NewId();
        var recipe = RecipeFaker.Generate(id: recipeId, userId: otherUserId);
        var command = new DeleteRecipeCommand(recipeId);

        _userIdentifierProvider.UserId.Returns(userId);
        _recipeRepository.GetByIdAsync(recipeId, Arg.Any<CancellationToken>()).Returns(recipe);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(RecipeErrors.NotOwned(recipeId));
        _recipeRepository.DidNotReceive().Update(Arg.Any<Recipe>());
    }
}
