using TrackYourLife.Modules.Nutrition.Application.Features.NutritionDiaries.Queries.GetNutritionDiariesByDate;
using TrackYourLife.Modules.Nutrition.Domain.Features.FoodDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.RecipeDiaries;
using TrackYourLife.Modules.Nutrition.Domain.UnitTests.Utils;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Nutrition.Application.UnitTests.Features.NutritionDiaries.Queries.GetNutritionDiariesByDate;

public class GetNutritionDiariesByDateQueryHandlerTests
{
    private readonly IFoodDiaryQuery _foodDiaryQuery;
    private readonly IRecipeDiaryQuery _recipeDiaryQuery;
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly GetNutritionDiariesByDateQueryHandler _handler;

    private readonly UserId _userId;
    private readonly DateOnly _date;
    private readonly GetNutritionDiariesByDateQuery _validQuery;

    public GetNutritionDiariesByDateQueryHandlerTests()
    {
        _foodDiaryQuery = Substitute.For<IFoodDiaryQuery>();
        _recipeDiaryQuery = Substitute.For<IRecipeDiaryQuery>();
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _handler = new GetNutritionDiariesByDateQueryHandler(
            _foodDiaryQuery,
            _recipeDiaryQuery,
            _userIdentifierProvider
        );

        _userId = UserId.NewId();
        _date = DateOnly.FromDateTime(DateTime.Today);
        _validQuery = new GetNutritionDiariesByDateQuery(_date);

        _userIdentifierProvider.UserId.Returns(_userId);
    }

    [Fact]
    public async Task Handle_WhenNoDiariesExist_ShouldReturnEmptyDictionaries()
    {
        // Arrange
        _foodDiaryQuery
            .GetByDateAsync(_userId, _date, Arg.Any<CancellationToken>())
            .Returns(Enumerable.Empty<FoodDiaryReadModel>());

        _recipeDiaryQuery
            .GetByDateAsync(_userId, _date, Arg.Any<CancellationToken>())
            .Returns(Enumerable.Empty<RecipeDiaryReadModel>());

        // Act
        var result = await _handler.Handle(_validQuery, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Item1.Should().NotBeNull();
        result.Value.Item2.Should().NotBeNull();
        result.Value.Item1.Values.Should().AllSatisfy(list => list.Should().BeEmpty());
        result.Value.Item2.Values.Should().AllSatisfy(list => list.Should().BeEmpty());
    }

    [Fact]
    public async Task Handle_WhenDiariesExist_ShouldReturnGroupedDiaries()
    {
        // Arrange
        var foodDiary = FoodDiaryFaker.GenerateReadModel(userId: _userId, date: _date);
        var recipeDiary = RecipeDiaryFaker.GenerateReadModel(userId: _userId, date: _date);

        _foodDiaryQuery
            .GetByDateAsync(_userId, _date, Arg.Any<CancellationToken>())
            .Returns(new[] { foodDiary });

        _recipeDiaryQuery
            .GetByDateAsync(_userId, _date, Arg.Any<CancellationToken>())
            .Returns(new[] { recipeDiary });

        // Act
        var result = await _handler.Handle(_validQuery, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Item1[foodDiary.MealType].Should().Contain(foodDiary);
        result.Value.Item2[recipeDiary.MealType].Should().Contain(recipeDiary);
    }
}
