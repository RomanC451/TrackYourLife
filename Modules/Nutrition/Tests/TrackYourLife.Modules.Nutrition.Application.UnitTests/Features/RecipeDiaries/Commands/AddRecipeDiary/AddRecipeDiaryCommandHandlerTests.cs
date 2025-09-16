using TrackYourLife.Modules.Nutrition.Application.Features.RecipeDiaries.Commands.AddRecipeDiary;
using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.RecipeDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;
using TrackYourLife.Modules.Nutrition.Domain.UnitTests.Utils;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Nutrition.Application.UnitTests.Features.RecipeDiaries.Commands.AddRecipeDiary;

public class AddRecipeDiaryCommandHandlerTests
{
    private readonly IRecipeDiaryRepository _recipeDiaryRepository;
    private readonly IRecipeRepository _recipeRepository;
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly AddRecipeDiaryCommandHandler _handler;

    public AddRecipeDiaryCommandHandlerTests()
    {
        _recipeDiaryRepository = Substitute.For<IRecipeDiaryRepository>();
        _recipeRepository = Substitute.For<IRecipeRepository>();
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _handler = new AddRecipeDiaryCommandHandler(
            _recipeDiaryRepository,
            _recipeRepository,
            _userIdentifierProvider
        );
    }

    [Fact]
    public async Task Handle_WhenRecipeExists_ShouldCreateRecipeDiaryAndReturnSuccess()
    {
        // Arrange
        var userId = UserId.NewId();
        var recipeId = RecipeId.NewId();
        var command = new AddRecipeDiaryCommand(
            recipeId,
            MealTypes.Breakfast,
            1.0f,
            DateOnly.FromDateTime(DateTime.Now),
            ServingSizeId.NewId()
        );

        var recipe = RecipeFaker.Generate(id: recipeId, userId: userId);
        _userIdentifierProvider.UserId.Returns(userId);
        _recipeRepository.GetByIdAsync(recipeId, Arg.Any<CancellationToken>()).Returns(recipe);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        await _recipeDiaryRepository
            .Received(1)
            .AddAsync(
                Arg.Is<RecipeDiary>(rd =>
                    rd.RecipeId == recipeId
                    && rd.UserId == userId
                    && Math.Abs(rd.Quantity - command.Quantity) < 0.0001f
                    && rd.Date == command.EntryDate
                    && rd.MealType == command.MealType
                ),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task Handle_WhenRecipeDoesNotExist_ShouldReturnFailure()
    {
        // Arrange
        var recipeId = RecipeId.NewId();
        var command = new AddRecipeDiaryCommand(
            recipeId,
            MealTypes.Breakfast,
            1.0f,
            DateOnly.FromDateTime(DateTime.Now),
            ServingSizeId.NewId()
        );

        _recipeRepository
            .GetByIdAsync(recipeId, Arg.Any<CancellationToken>())
            .Returns((Recipe?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(RecipeErrors.NotFound(recipeId));
        await _recipeDiaryRepository
            .DidNotReceive()
            .AddAsync(Arg.Any<RecipeDiary>(), Arg.Any<CancellationToken>());
    }
}
