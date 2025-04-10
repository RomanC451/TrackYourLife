using TrackYourLife.Modules.Nutrition.Application.Features.Recipes.Queries.GetRecipeById;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;
using TrackYourLife.Modules.Nutrition.Domain.UnitTests.Utils;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Nutrition.Application.UnitTests.Features.Recipes.Queries.GetRecipeById;

public class GetRecipeByIdQueryHandlerTests
{
    private readonly GetRecipeByIdQueryHandler _handler;
    private readonly IRecipeQuery _recipeQuery;
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly UserId _userId;

    public GetRecipeByIdQueryHandlerTests()
    {
        _recipeQuery = Substitute.For<IRecipeQuery>();
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _userId = UserId.NewId();
        _userIdentifierProvider.UserId.Returns(_userId);

        _handler = new GetRecipeByIdQueryHandler(_recipeQuery, _userIdentifierProvider);
    }

    [Fact]
    public async Task Handle_WhenRecipeExistsAndBelongsToUser_ShouldReturnRecipe()
    {
        // Arrange
        var recipe = RecipeFaker.GenerateReadModel(userId: _userId);
        var query = new GetRecipeByIdQuery(recipe.Id);

        _recipeQuery.GetByIdAsync(query.Id, default).Returns(recipe);

        // Act
        var result = await _handler.Handle(query, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(recipe);
    }

    [Fact]
    public async Task Handle_WhenRecipeDoesNotExist_ShouldReturnFailure()
    {
        // Arrange
        var query = new GetRecipeByIdQuery(RecipeId.NewId());

        _recipeQuery.GetByIdAsync(query.Id, default).Returns((RecipeReadModel?)null);

        // Act
        var result = await _handler.Handle(query, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Recipe.NotFound");
    }

    [Fact]
    public async Task Handle_WhenRecipeBelongsToDifferentUser_ShouldReturnFailure()
    {
        // Arrange
        var otherUserId = UserId.NewId();
        var recipe = RecipeFaker.GenerateReadModel(userId: otherUserId);
        var query = new GetRecipeByIdQuery(recipe.Id);

        _recipeQuery.GetByIdAsync(query.Id, default).Returns(recipe);

        // Act
        var result = await _handler.Handle(query, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Recipe.NotOwned");
    }
}
