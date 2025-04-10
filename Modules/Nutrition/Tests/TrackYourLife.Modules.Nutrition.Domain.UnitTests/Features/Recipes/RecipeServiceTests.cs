using TrackYourLife.Modules.Nutrition.Domain.Core;
using TrackYourLife.Modules.Nutrition.Domain.Features.RecipeDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Nutrition.Domain.UnitTests.Features.Recipes;

public class RecipeServiceTests
{
    private readonly IRecipeDiaryQuery _recipeDiaryQuery;
    private readonly IRecipeDiaryRepository _recipeDiaryRepository;
    private readonly IRecipeRepository _recipeRepository;
    private readonly INutritionUnitOfWork _unitOfWork;
    private readonly RecipeService _sut;

    public RecipeServiceTests()
    {
        _recipeDiaryQuery = Substitute.For<IRecipeDiaryQuery>();
        _recipeDiaryRepository = Substitute.For<IRecipeDiaryRepository>();
        _recipeRepository = Substitute.For<IRecipeRepository>();
        _unitOfWork = Substitute.For<INutritionUnitOfWork>();
        _sut = new RecipeService(
            _recipeDiaryQuery,
            _recipeDiaryRepository,
            _recipeRepository,
            _unitOfWork
        );
    }

    [Fact]
    public async Task CloneIfUsed_WhenRecipeNotUsed_ShouldReturnSuccess()
    {
        // Arrange
        var recipe = Recipe.Create(RecipeId.NewId(), UserId.NewId(), "Test Recipe").Value;
        var userId = UserId.NewId();
        var cancellationToken = CancellationToken.None;

        _recipeDiaryQuery.AnyByRecipeIdAsync(userId, recipe.Id, cancellationToken).Returns(false);

        // Act
        var result = await _sut.CloneIfUsed(recipe, userId, cancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _recipeRepository
            .DidNotReceive()
            .AddAsync(Arg.Any<Recipe>(), Arg.Any<CancellationToken>());
        await _recipeDiaryRepository
            .DidNotReceive()
            .BulkUpdateRecipeId(
                Arg.Any<RecipeId>(),
                Arg.Any<RecipeId>(),
                Arg.Any<CancellationToken>()
            );
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CloneIfUsed_WhenRecipeUsed_ShouldCloneAndUpdateDiaries()
    {
        // Arrange
        var recipe = Recipe.Create(RecipeId.NewId(), UserId.NewId(), "Test Recipe").Value;
        var userId = UserId.NewId();
        var cancellationToken = CancellationToken.None;

        _recipeDiaryQuery.AnyByRecipeIdAsync(userId, recipe.Id, cancellationToken).Returns(true);

        // Act
        var result = await _sut.CloneIfUsed(recipe, userId, cancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _recipeRepository
            .Received(1)
            .AddAsync(Arg.Is<Recipe>(r => r.IsOld), cancellationToken);
        await _recipeDiaryRepository
            .Received(1)
            .BulkUpdateRecipeId(recipe.Id, Arg.Any<RecipeId>(), cancellationToken);
        await _unitOfWork.Received(2).SaveChangesAsync(cancellationToken);
    }
}
