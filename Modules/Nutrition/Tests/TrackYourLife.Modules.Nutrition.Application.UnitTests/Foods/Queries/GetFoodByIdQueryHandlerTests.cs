using FluentAssertions;
using NSubstitute;
using NSubstitute.ClearExtensions;
using TrackYourLife.Modules.Nutrition.Application.Core.Abstraction;
using TrackYourLife.Modules.Nutrition.Application.Features.Foods.Queries.GetFoodById;
using TrackYourLife.Modules.Nutrition.Application.UnitTests.Utils;
using TrackYourLife.Modules.Nutrition.Contracts.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.UnitTests.Utils;

namespace TrackYourLife.Modules.Nutrition.Application.UnitTests.Foods.Queries;

public class GetFoodByIdQueryHandlerTests : IDisposable
{
    private readonly IFoodQuery foodQuery = Substitute.For<IFoodQuery>();

    private readonly GetFoodByIdQueryHandler sut;

    public GetFoodByIdQueryHandlerTests()
    {
        sut = new GetFoodByIdQueryHandler(foodQuery);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        foodQuery.ClearSubstitute();
    }

    [Fact]
    public async Task Handle_WhenFoodExists_ShouldReturnSuccess()
    {
        // Arrange
        var query = new GetFoodByIdQuery(FoodId.NewId());
        var food = FoodFaker.GenerateReadModel();

        foodQuery.GetByIdAsync(query.FoodId, Arg.Any<CancellationToken>()).Returns(food);

        // Act
        var result = await sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(food);

        await foodQuery.Received(1).GetByIdAsync(query.FoodId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenFoodDoesNotExist_ShouldReturnFailure()
    {
        // Arrange
        var query = new GetFoodByIdQuery(FoodId.NewId());

        foodQuery
            .GetByIdAsync(query.FoodId, Arg.Any<CancellationToken>())
            .Returns((FoodReadModel)null!);

        // Act
        var result = await sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(FoodErrors.NotFoundById(query.FoodId));

        await foodQuery.Received(1).GetByIdAsync(query.FoodId, Arg.Any<CancellationToken>());
    }
}
