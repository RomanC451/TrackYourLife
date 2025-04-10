using FluentAssertions;
using NSubstitute;
using TrackYourLife.Modules.Nutrition.Application.Features.FoodDiaries.Queries.GetFoodDiaryById;
using TrackYourLife.Modules.Nutrition.Domain.Features.FoodDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;
using TrackYourLife.Modules.Nutrition.Domain.UnitTests.Utils;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Nutrition.Application.UnitTests.Features.FoodDiaries.Queries.GetFoodDiaryById;

public class GetFoodDiaryByIdQueryHandlerTests
{
    private readonly IFoodDiaryQuery _foodDiaryQuery;
    private readonly GetFoodDiaryByIdQueryHandler _handler;

    private readonly UserId _userId;
    private readonly NutritionDiaryId _foodDiaryId;
    private readonly GetFoodDiaryByIdQuery _validQuery;

    public GetFoodDiaryByIdQueryHandlerTests()
    {
        _foodDiaryQuery = Substitute.For<IFoodDiaryQuery>();
        _handler = new GetFoodDiaryByIdQueryHandler(_foodDiaryQuery);

        _userId = UserId.NewId();
        _foodDiaryId = NutritionDiaryId.NewId();
        _validQuery = new GetFoodDiaryByIdQuery(_foodDiaryId);
    }

    [Fact]
    public async Task Handle_WhenFoodDiaryNotFound_ShouldReturnFailure()
    {
        // Arrange
        _foodDiaryQuery
            .GetByIdAsync(_foodDiaryId, Arg.Any<CancellationToken>())
            .Returns((FoodDiaryReadModel?)null);

        // Act
        var result = await _handler.Handle(_validQuery, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(FoodDiaryErrors.NotFound(_foodDiaryId));
    }

    [Fact]
    public async Task Handle_WhenAllDataIsValid_ShouldReturnSuccess()
    {
        // Arrange
        var foodDiary = FoodDiaryFaker.GenerateReadModel(id: _foodDiaryId, userId: _userId);

        _foodDiaryQuery.GetByIdAsync(_foodDiaryId, Arg.Any<CancellationToken>()).Returns(foodDiary);

        // Act
        var result = await _handler.Handle(_validQuery, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(foodDiary);
    }
}
