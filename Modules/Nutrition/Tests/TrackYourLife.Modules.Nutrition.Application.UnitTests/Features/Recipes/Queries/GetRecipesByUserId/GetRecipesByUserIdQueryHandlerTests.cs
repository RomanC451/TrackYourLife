using FluentAssertions;
using NSubstitute;
using TrackYourLife.Modules.Nutrition.Application.Features.Recipes.Queries.GetRecipesByUserId;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;
using TrackYourLife.Modules.Nutrition.Domain.UnitTests.Utils;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Nutrition.Application.UnitTests.Features.Recipes.Queries.GetRecipesByUserId;

public class GetRecipesByUserIdQueryHandlerTests
{
    private readonly GetRecipesByUserIdQueryHandler _handler;
    private readonly IRecipeQuery _recipeQuery;
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly UserId _userId;

    public GetRecipesByUserIdQueryHandlerTests()
    {
        _recipeQuery = Substitute.For<IRecipeQuery>();
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _userId = UserId.NewId();
        _userIdentifierProvider.UserId.Returns(_userId);

        _handler = new GetRecipesByUserIdQueryHandler(_recipeQuery, _userIdentifierProvider);
    }

    [Fact]
    public async Task Handle_WhenRecipesExist_ShouldReturnRecipes()
    {
        // Arrange
        var recipes = new List<RecipeReadModel>
        {
            RecipeFaker.GenerateReadModel(userId: _userId),
            RecipeFaker.GenerateReadModel(userId: _userId),
        };

        _recipeQuery.GetByUserIdAsync(_userId, default).Returns(recipes);

        // Act
        var result = await _handler.Handle(new GetRecipesByUserIdQuery(), default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(recipes);
    }

    [Fact]
    public async Task Handle_WhenNoRecipesExist_ShouldReturnEmptyList()
    {
        // Arrange
        _recipeQuery
            .GetByUserIdAsync(_userId, default)
            .Returns(Enumerable.Empty<RecipeReadModel>());

        // Act
        var result = await _handler.Handle(new GetRecipesByUserIdQuery(), default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }
}
