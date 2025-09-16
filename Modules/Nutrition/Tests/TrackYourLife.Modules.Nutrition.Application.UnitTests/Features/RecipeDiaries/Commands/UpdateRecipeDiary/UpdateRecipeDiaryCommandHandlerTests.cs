using TrackYourLife.Modules.Nutrition.Application.Features.RecipeDiaries.Commands.UpdateRecipeDiary;
using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.RecipeDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;
using TrackYourLife.Modules.Nutrition.Domain.UnitTests.Utils;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Nutrition.Application.UnitTests.Features.RecipeDiaries.Commands.UpdateRecipeDiary;

public class UpdateRecipeDiaryCommandHandlerTests
{
    private readonly IRecipeDiaryRepository _recipeDiaryRepository;
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly UpdateRecipeDiaryCommandHandler _handler;

    public UpdateRecipeDiaryCommandHandlerTests()
    {
        _recipeDiaryRepository = Substitute.For<IRecipeDiaryRepository>();
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _handler = new UpdateRecipeDiaryCommandHandler(
            _recipeDiaryRepository,
            _userIdentifierProvider
        );
    }

    [Fact]
    public async Task Handle_WhenRecipeDiaryExists_ShouldUpdateRecipeDiaryAndReturnSuccess()
    {
        // Arrange
        var userId = UserId.NewId();
        var nutritionDiaryId = NutritionDiaryId.NewId();
        var newQuantity = 2.0f;
        var newMealType = MealTypes.Lunch;
        var command = new UpdateRecipeDiaryCommand(
            nutritionDiaryId,
            newQuantity,
            newMealType,
            ServingSizeId.NewId(),
            DateOnly.FromDateTime(DateTime.Now)
        );

        var recipeDiary = RecipeDiaryFaker.Generate(id: nutritionDiaryId, userId: userId);
        _userIdentifierProvider.UserId.Returns(userId);
        _recipeDiaryRepository
            .GetByIdAsync(nutritionDiaryId, Arg.Any<CancellationToken>())
            .Returns(recipeDiary);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        recipeDiary.Quantity.Should().Be(newQuantity);
        recipeDiary.MealType.Should().Be(newMealType);
        _recipeDiaryRepository
            .Received(1)
            .Update(Arg.Is<RecipeDiary>(rd => rd.Id == nutritionDiaryId));
    }

    [Fact]
    public async Task Handle_WhenRecipeDiaryDoesNotExist_ShouldReturnFailure()
    {
        // Arrange
        var nutritionDiaryId = NutritionDiaryId.NewId();
        var command = new UpdateRecipeDiaryCommand(
            nutritionDiaryId,
            2.0f,
            MealTypes.Lunch,
            ServingSizeId.NewId(),
            DateOnly.FromDateTime(DateTime.Now)
        );

        _recipeDiaryRepository
            .GetByIdAsync(nutritionDiaryId, Arg.Any<CancellationToken>())
            .Returns((RecipeDiary?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(RecipeDiaryErrors.NotFound(nutritionDiaryId));
        _recipeDiaryRepository.DidNotReceive().Update(Arg.Any<RecipeDiary>());
    }

    [Fact]
    public async Task Handle_WhenRecipeDiaryBelongsToDifferentUser_ShouldReturnFailure()
    {
        // Arrange
        var userId = UserId.NewId();
        var otherUserId = UserId.NewId();
        var nutritionDiaryId = NutritionDiaryId.NewId();
        var command = new UpdateRecipeDiaryCommand(
            nutritionDiaryId,
            2.0f,
            MealTypes.Lunch,
            ServingSizeId.NewId(),
            DateOnly.FromDateTime(DateTime.Now)
        );

        var recipeDiary = RecipeDiaryFaker.Generate(id: nutritionDiaryId, userId: otherUserId);
        _userIdentifierProvider.UserId.Returns(userId);
        _recipeDiaryRepository
            .GetByIdAsync(nutritionDiaryId, Arg.Any<CancellationToken>())
            .Returns(recipeDiary);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(RecipeDiaryErrors.NotOwned(nutritionDiaryId));
        _recipeDiaryRepository.DidNotReceive().Update(Arg.Any<RecipeDiary>());
    }

    [Fact]
    public async Task Handle_WhenUpdateFails_ShouldReturnFailure()
    {
        // Arrange
        var userId = UserId.NewId();
        var nutritionDiaryId = NutritionDiaryId.NewId();
        var command = new UpdateRecipeDiaryCommand(
            nutritionDiaryId,
            -1.0f,
            MealTypes.Lunch,
            ServingSizeId.NewId(),
            DateOnly.FromDateTime(DateTime.Now)
        );

        var recipeDiary = RecipeDiaryFaker.Generate(id: nutritionDiaryId, userId: userId);
        _userIdentifierProvider.UserId.Returns(userId);
        _recipeDiaryRepository
            .GetByIdAsync(nutritionDiaryId, Arg.Any<CancellationToken>())
            .Returns(recipeDiary);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result
            .Error.Code.Should()
            .Be(
                $"{nameof(NutritionDiary)}.{nameof(RecipeDiary.Quantity).ToCapitalCase()}.NotPositive"
            );
        _recipeDiaryRepository.DidNotReceive().Update(Arg.Any<RecipeDiary>());
    }
}
