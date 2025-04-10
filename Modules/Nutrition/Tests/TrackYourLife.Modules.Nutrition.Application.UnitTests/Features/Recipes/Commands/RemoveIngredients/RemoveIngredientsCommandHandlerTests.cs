using Serilog;
using TrackYourLife.Modules.Nutrition.Application.Features.Recipes.Commands.RemoveIngredients;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.Ingredients;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;
using TrackYourLife.Modules.Nutrition.Domain.UnitTests.Utils;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Nutrition.Application.UnitTests.Features.Recipes.Commands.RemoveIngredients;

public class RemoveIngredientsCommandHandlerTests
{
    private readonly RemoveIngredientsCommandHandler _handler;
    private readonly IRecipeRepository _recipeRepository;
    private readonly IFoodRepository _foodRepository;
    private readonly IServingSizeQuery _servingSizeQuery;
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IRecipeService _recipeService;
    private readonly ILogger _logger;

    public RemoveIngredientsCommandHandlerTests()
    {
        _recipeRepository = Substitute.For<IRecipeRepository>();
        _foodRepository = Substitute.For<IFoodRepository>();
        _servingSizeQuery = Substitute.For<IServingSizeQuery>();
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _recipeService = Substitute.For<IRecipeService>();
        _logger = Substitute.For<ILogger>();
        _handler = new RemoveIngredientsCommandHandler(
            _recipeRepository,
            _foodRepository,
            _servingSizeQuery,
            _userIdentifierProvider,
            _recipeService,
            _logger
        );
    }

    [Fact]
    public async Task Handle_WhenRecipeExistsAndBelongsToUser_ShouldRemoveIngredients()
    {
        // Arrange
        Recipe updatedRecipe = null!;

        var recipeId = RecipeId.NewId();
        var userId = UserId.NewId();
        var ingredientIds = new List<IngredientId> { IngredientId.NewId(), IngredientId.NewId() };
        var command = new RemoveIngredientsCommand(recipeId, ingredientIds);

        var recipe = RecipeFaker.Generate(
            id: recipeId,
            userId: userId,
            ingredients:
            [
                IngredientFaker.Generate(id: ingredientIds[0]),
                IngredientFaker.Generate(id: ingredientIds[1]),
            ]
        );
        var food = FoodFaker.Generate();
        var servingSize = ServingSizeFaker.GenerateReadModel();

        _userIdentifierProvider.UserId.Returns(userId);
        _recipeRepository.GetByIdAsync(recipeId, Arg.Any<CancellationToken>()).Returns(recipe);
        _recipeRepository
            .When(r => r.Update(Arg.Any<Recipe>()))
            .Do(r => updatedRecipe = r.Arg<Recipe>());
        _foodRepository.GetByIdAsync(Arg.Any<FoodId>(), Arg.Any<CancellationToken>()).Returns(food);
        _servingSizeQuery
            .GetByIdAsync(Arg.Any<ServingSizeId>(), Arg.Any<CancellationToken>())
            .Returns(servingSize);
        _recipeService
            .CloneIfUsed(recipe, userId, Arg.Any<CancellationToken>())
            .Returns(Result.Success(recipe));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        updatedRecipe.Should().NotBeNull();
        updatedRecipe.Ingredients.Should().BeEmpty();
        _recipeRepository.Received(1).Update(recipe);
    }

    [Fact]
    public async Task Handle_WhenRecipeDoesNotExist_ShouldReturnFailure()
    {
        // Arrange
        var recipeId = RecipeId.NewId();
        var userId = UserId.NewId();
        var ingredientIds = new List<IngredientId> { IngredientId.NewId() };
        var command = new RemoveIngredientsCommand(recipeId, ingredientIds);

        _userIdentifierProvider.UserId.Returns(userId);
        _recipeRepository
            .GetByIdAsync(recipeId, Arg.Any<CancellationToken>())
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
        var ingredientIds = new List<IngredientId> { IngredientId.NewId() };
        var command = new RemoveIngredientsCommand(recipeId, ingredientIds);

        var recipe = RecipeFaker.Generate(id: recipeId, userId: otherUserId);
        var food = FoodFaker.Generate();
        var servingSize = ServingSizeFaker.GenerateReadModel();
        var ingredient = Ingredient
            .Create(otherUserId, ingredientIds[0], food.Id, servingSize.Id, 100f)
            .Value;
        recipe.AddIngredient(ingredient, food, servingSize);

        _userIdentifierProvider.UserId.Returns(userId);
        _recipeRepository.GetByIdAsync(recipeId, Arg.Any<CancellationToken>()).Returns(recipe);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Recipe.NotOwned");
        _recipeRepository.DidNotReceive().Update(Arg.Any<Recipe>());
    }

    [Fact]
    public async Task Handle_WhenSomeIngredientsDoNotExist_ShouldRemoveOnlyExistingIngredients()
    {
        // Arrange
        var recipeId = RecipeId.NewId();
        var userId = UserId.NewId();
        var existingIngredientId = IngredientId.NewId();
        var nonExistingIngredientId = IngredientId.NewId();
        var ingredientIds = new List<IngredientId>
        {
            existingIngredientId,
            nonExistingIngredientId,
        };
        var command = new RemoveIngredientsCommand(recipeId, ingredientIds);

        var recipe = RecipeFaker.Generate(
            id: recipeId,
            userId: userId,
            ingredients: [IngredientFaker.Generate(id: existingIngredientId)]
        );
        var food = FoodFaker.Generate();
        var servingSize = ServingSizeFaker.GenerateReadModel();

        _userIdentifierProvider.UserId.Returns(userId);
        _recipeRepository.GetByIdAsync(recipeId, Arg.Any<CancellationToken>()).Returns(recipe);
        _foodRepository.GetByIdAsync(Arg.Any<FoodId>(), Arg.Any<CancellationToken>()).Returns(food);
        _servingSizeQuery
            .GetByIdAsync(Arg.Any<ServingSizeId>(), Arg.Any<CancellationToken>())
            .Returns(servingSize);
        _recipeService
            .CloneIfUsed(recipe, userId, Arg.Any<CancellationToken>())
            .Returns(Result.Success(recipe));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        recipe.Ingredients.Should().BeEmpty();
        _recipeRepository.Received(1).Update(recipe);
        _logger
            .Received(1)
            .Error(
                "Ingredient with id {IngredientId} not found in recipe with id {RecipeId}",
                nonExistingIngredientId,
                recipeId
            );
    }
}
