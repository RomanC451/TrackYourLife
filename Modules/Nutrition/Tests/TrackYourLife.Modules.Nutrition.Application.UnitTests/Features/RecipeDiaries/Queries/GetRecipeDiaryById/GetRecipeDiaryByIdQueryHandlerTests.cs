using TrackYourLife.Modules.Nutrition.Application.Features.RecipeDiaries.Queries.GetRecipeDiaryById;
using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.RecipeDiaries;
using TrackYourLife.Modules.Nutrition.Domain.UnitTests.Utils;
using TrackYourLife.SharedLib.Domain.Errors;

namespace TrackYourLife.Modules.Nutrition.Application.UnitTests.Features.RecipeDiaries.Queries.GetRecipeDiaryById;

public class GetRecipeDiaryByIdQueryHandlerTests
{
    private readonly IRecipeDiaryQuery _recipeDiaryQuery;
    private readonly GetRecipeDiaryByIdQueryHandler _handler;

    public GetRecipeDiaryByIdQueryHandlerTests()
    {
        _recipeDiaryQuery = Substitute.For<IRecipeDiaryQuery>();
        _handler = new GetRecipeDiaryByIdQueryHandler(_recipeDiaryQuery);
    }

    [Fact]
    public async Task Handle_WhenRecipeDiaryExists_ShouldReturnSuccessWithRecipeDiary()
    {
        // Arrange
        var diaryId = NutritionDiaryId.NewId();
        var query = new GetRecipeDiaryByIdQuery(diaryId);
        var recipeDiary = RecipeDiaryFaker.GenerateReadModel(id: diaryId);

        _recipeDiaryQuery.GetByIdAsync(diaryId, Arg.Any<CancellationToken>()).Returns(recipeDiary);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(recipeDiary);
    }

    [Fact]
    public async Task Handle_WhenRecipeDiaryDoesNotExist_ShouldReturnFailure()
    {
        // Arrange
        var diaryId = NutritionDiaryId.NewId();
        var query = new GetRecipeDiaryByIdQuery(diaryId);

        _recipeDiaryQuery
            .GetByIdAsync(diaryId, Arg.Any<CancellationToken>())
            .Returns((RecipeDiaryReadModel?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(RecipeDiaryErrors.NotFound(diaryId));
    }
}
