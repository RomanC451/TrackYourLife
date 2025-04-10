using TrackYourLife.Modules.Nutrition.Application.Features.Recipes.Commands.AddIngredient;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.Ingredients;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;
using TrackYourLife.Modules.Nutrition.Domain.UnitTests.Utils;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Nutrition.Application.UnitTests.Features.Recipes.Commands.AddIngredient;

public class AddIngredientCommandHandlerTests
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly IFoodRepository _foodRepository;
    private readonly IServingSizeQuery _servingSizeQuery;
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IRecipeService _recipeService;
    private readonly AddIngredientCommandHandler _handler;

    public AddIngredientCommandHandlerTests()
    {
        _recipeRepository = Substitute.For<IRecipeRepository>();
        _foodRepository = Substitute.For<IFoodRepository>();
        _servingSizeQuery = Substitute.For<IServingSizeQuery>();
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _recipeService = Substitute.For<IRecipeService>();
        _handler = new AddIngredientCommandHandler(
            _recipeRepository,
            _foodRepository,
            _servingSizeQuery,
            _userIdentifierProvider,
            _recipeService
        );
    }

    [Fact]
    public async Task Handle_WhenAllConditionsAreMet_ShouldAddIngredientAndReturnSuccess()
    {
        // Arrange
        var userId = UserId.NewId();
        var recipeId = RecipeId.NewId();
        var foodId = FoodId.NewId();
        var servingSizeId = ServingSizeId.NewId();
        var quantity = 1.0f;

        var command = new AddIngredientCommand(recipeId, foodId, servingSizeId, quantity);

        var recipe = RecipeFaker.Generate(id: recipeId, userId: userId);
        var servingSize = ServingSizeFaker.GenerateReadModel(id: servingSizeId);
        var food = FoodFaker.Generate(
            id: foodId,
            foodServingSizes:
            [
                FoodServingSizeFaker.Generate(
                    index: 0,
                    foodId: foodId,
                    servingSize: ServingSizeFaker.Generate(id: servingSizeId)
                ),
            ]
        );

        _userIdentifierProvider.UserId.Returns(userId);
        _recipeRepository.GetByIdAsync(recipeId, Arg.Any<CancellationToken>()).Returns(recipe);
        _foodRepository.GetByIdAsync(foodId, Arg.Any<CancellationToken>()).Returns(food);
        _servingSizeQuery
            .GetByIdAsync(servingSizeId, Arg.Any<CancellationToken>())
            .Returns(servingSize);
        _recipeService
            .CloneIfUsed(recipe, userId, Arg.Any<CancellationToken>())
            .Returns(Result.Success(recipe));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        _recipeRepository.Received(1).Update(Arg.Is<Recipe>(r => r.Id == recipeId));
    }

    [Fact]
    public async Task Handle_WhenRecipeDoesNotExist_ShouldReturnFailure()
    {
        // Arrange
        var recipeId = RecipeId.NewId();
        var command = new AddIngredientCommand(
            recipeId,
            FoodId.NewId(),
            ServingSizeId.NewId(),
            1.0f
        );

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
        var command = new AddIngredientCommand(
            recipeId,
            FoodId.NewId(),
            ServingSizeId.NewId(),
            1.0f
        );

        var recipe = RecipeFaker.Generate(id: recipeId, userId: otherUserId);
        _userIdentifierProvider.UserId.Returns(userId);
        _recipeRepository.GetByIdAsync(recipeId, Arg.Any<CancellationToken>()).Returns(recipe);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(RecipeErrors.NotOwned(recipeId));
        _recipeRepository.DidNotReceive().Update(Arg.Any<Recipe>());
    }

    [Fact]
    public async Task Handle_WhenFoodDoesNotExist_ShouldReturnFailure()
    {
        // Arrange
        var userId = UserId.NewId();
        var recipeId = RecipeId.NewId();
        var foodId = FoodId.NewId();
        var command = new AddIngredientCommand(recipeId, foodId, ServingSizeId.NewId(), 1.0f);

        var recipe = RecipeFaker.Generate(id: recipeId, userId: userId);
        _userIdentifierProvider.UserId.Returns(userId);
        _recipeRepository.GetByIdAsync(recipeId, Arg.Any<CancellationToken>()).Returns(recipe);
        _foodRepository.GetByIdAsync(foodId, Arg.Any<CancellationToken>()).Returns((Food?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(FoodErrors.NotFoundById(foodId));
        _recipeRepository.DidNotReceive().Update(Arg.Any<Recipe>());
    }

    [Fact]
    public async Task Handle_WhenFoodDoesNotHaveServingSize_ShouldReturnFailure()
    {
        // Arrange
        var userId = UserId.NewId();
        var recipeId = RecipeId.NewId();
        var foodId = FoodId.NewId();
        var servingSizeId = ServingSizeId.NewId();
        var command = new AddIngredientCommand(recipeId, foodId, servingSizeId, 1.0f);

        var recipe = RecipeFaker.Generate(id: recipeId, userId: userId);
        var food = FoodFaker.Generate(id: foodId);
        _userIdentifierProvider.UserId.Returns(userId);
        _recipeRepository.GetByIdAsync(recipeId, Arg.Any<CancellationToken>()).Returns(recipe);
        _foodRepository.GetByIdAsync(foodId, Arg.Any<CancellationToken>()).Returns(food);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(FoodErrors.ServingSizeNotFound(foodId, servingSizeId));
        _recipeRepository.DidNotReceive().Update(Arg.Any<Recipe>());
    }

    [Fact]
    public async Task Handle_WhenServingSizeDoesNotExist_ShouldReturnFailure()
    {
        // Arrange
        var userId = UserId.NewId();
        var recipeId = RecipeId.NewId();
        var foodId = FoodId.NewId();
        var servingSizeId = ServingSizeId.NewId();
        var command = new AddIngredientCommand(recipeId, foodId, servingSizeId, 1.0f);

        var recipe = RecipeFaker.Generate(id: recipeId, userId: userId);
        var food = FoodFaker.Generate(id: foodId);
        food.FoodServingSizes.Add(
            FoodServingSizeFaker.Generate(0, foodId, ServingSizeFaker.Generate(id: servingSizeId))
        );

        _userIdentifierProvider.UserId.Returns(userId);
        _recipeRepository.GetByIdAsync(recipeId, Arg.Any<CancellationToken>()).Returns(recipe);
        _foodRepository.GetByIdAsync(foodId, Arg.Any<CancellationToken>()).Returns(food);
        _servingSizeQuery
            .GetByIdAsync(servingSizeId, Arg.Any<CancellationToken>())
            .Returns((ServingSizeReadModel?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ServingSizeErrors.NotFound(servingSizeId));
        _recipeRepository.DidNotReceive().Update(Arg.Any<Recipe>());
    }

    [Fact]
    public async Task Handle_WhenRecipeCloneFails_ShouldReturnFailure()
    {
        // Arrange
        var userId = UserId.NewId();
        var recipeId = RecipeId.NewId();
        var foodId = FoodId.NewId();
        var servingSizeId = ServingSizeId.NewId();
        var quantity = 1.0f;

        var command = new AddIngredientCommand(recipeId, foodId, servingSizeId, quantity);

        var recipe = RecipeFaker.Generate(id: recipeId, userId: userId);
        var food = FoodFaker.Generate(id: foodId);
        food.FoodServingSizes.Add(
            FoodServingSizeFaker.Generate(0, foodId, ServingSizeFaker.Generate(id: servingSizeId))
        );
        var servingSize = ServingSizeFaker.GenerateReadModel(id: servingSizeId);

        _userIdentifierProvider.UserId.Returns(userId);
        _recipeRepository.GetByIdAsync(recipeId, Arg.Any<CancellationToken>()).Returns(recipe);
        _foodRepository.GetByIdAsync(foodId, Arg.Any<CancellationToken>()).Returns(food);
        _servingSizeQuery
            .GetByIdAsync(servingSizeId, Arg.Any<CancellationToken>())
            .Returns(servingSize);
        _recipeService
            .CloneIfUsed(recipe, userId, Arg.Any<CancellationToken>())
            .Returns(Result.Failure(RecipeErrors.Old));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(RecipeErrors.Old);
        _recipeRepository.DidNotReceive().Update(Arg.Any<Recipe>());
    }
}
