using MassTransit;
using TrackYourLife.Modules.Users.Application.Features.Goals.Consumers;
using TrackYourLife.Modules.Users.Domain.Features.Goals;
using TrackYourLife.Modules.Users.Domain.UnitTests.Utils;
using TrackYourLife.SharedLib.Contracts.Integration.Users;
using TrackYourLife.SharedLib.Domain.Enums;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Users.Application.UnitTests.Features.Goals.Consumers;

public sealed class GetNutritionGoalsByUserIdConsumerTests
{
    private readonly GetNutritionGoalsByUserIdConsumer sut;
    private readonly IGoalQuery goalQuery = Substitute.For<IGoalQuery>();

    public GetNutritionGoalsByUserIdConsumerTests()
    {
        sut = new GetNutritionGoalsByUserIdConsumer(goalQuery);
    }

    [Fact]
    public async Task Consume_WhenAllGoalsExist_ShouldReturnNutritionGoals()
    {
        // Arrange
        GetNutritionGoalsByUserIdResponse response = null!;
        var userId = UserId.NewId();

        var date = DateOnly.FromDateTime(DateTime.UtcNow);

        var caloriesGoal = GoalFaker.GenerateReadModel(
            userId: userId,
            type: GoalType.Calories,
            value: 2000,
            startDate: date,
            endDate: date.AddDays(1)
        );

        var carbohydratesGoal = GoalFaker.GenerateReadModel(
            userId: userId,
            type: GoalType.Carbohydrates,
            value: 250,
            startDate: date,
            endDate: date.AddDays(1)
        );

        var fatGoal = GoalFaker.GenerateReadModel(
            userId: userId,
            type: GoalType.Fats,
            value: 70,
            startDate: date,
            endDate: date.AddDays(1)
        );

        var proteinGoal = GoalFaker.GenerateReadModel(
            userId: userId,
            type: GoalType.Protein,
            value: 150,
            startDate: date,
            endDate: date.AddDays(1)
        );

        var request = new GetNutritionGoalsByUserIdRequest(userId, date);
        var context = Substitute.For<ConsumeContext<GetNutritionGoalsByUserIdRequest>>();
        context.Message.Returns(request);
        await context.RespondAsync(Arg.Do<GetNutritionGoalsByUserIdResponse>(x => response = x));

        goalQuery
            .GetGoalByTypeAndDateAsync(
                userId,
                GoalType.Calories,
                date,
                Arg.Any<CancellationToken>()
            )
            .Returns(caloriesGoal);

        goalQuery
            .GetGoalByTypeAndDateAsync(
                userId,
                GoalType.Carbohydrates,
                date,
                Arg.Any<CancellationToken>()
            )
            .Returns(carbohydratesGoal);

        goalQuery
            .GetGoalByTypeAndDateAsync(userId, GoalType.Fats, date, Arg.Any<CancellationToken>())
            .Returns(fatGoal);

        goalQuery
            .GetGoalByTypeAndDateAsync(userId, GoalType.Protein, date, Arg.Any<CancellationToken>())
            .Returns(proteinGoal);

        // Act
        await sut.Consume(context);

        // Assert
        response.Data.Should().NotBeNull();
        response.Data!.CaloriesGoal.Should().Be(2000);
        response.Data!.CarbohydratesGoal.Should().Be(250);
        response.Data!.FatGoal.Should().Be(70);
        response.Data!.ProteinGoal.Should().Be(150);
        response.Errors.Should().BeEmpty();

        await goalQuery
            .Received(1)
            .GetGoalByTypeAndDateAsync(
                userId,
                GoalType.Calories,
                date,
                Arg.Any<CancellationToken>()
            );
        await goalQuery
            .Received(1)
            .GetGoalByTypeAndDateAsync(
                userId,
                GoalType.Carbohydrates,
                date,
                Arg.Any<CancellationToken>()
            );
        await goalQuery
            .Received(1)
            .GetGoalByTypeAndDateAsync(userId, GoalType.Fats, date, Arg.Any<CancellationToken>());
        await goalQuery
            .Received(1)
            .GetGoalByTypeAndDateAsync(
                userId,
                GoalType.Protein,
                date,
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task Consume_WhenSomeGoalsAreMissing_ShouldReturnErrors()
    {
        // Arrange
        GetNutritionGoalsByUserIdResponse response = null!;
        var userId = UserId.NewId();
        var date = DateOnly.FromDateTime(DateTime.UtcNow);

        var caloriesGoal = GoalFaker.GenerateReadModel(
            userId: userId,
            type: GoalType.Calories,
            value: 2000,
            startDate: date,
            endDate: date.AddDays(1)
        );

        var proteinGoal = GoalFaker.GenerateReadModel(
            userId: userId,
            type: GoalType.Protein,
            value: 150,
            startDate: date,
            endDate: date.AddDays(1)
        );

        var request = new GetNutritionGoalsByUserIdRequest(userId, date);
        var context = Substitute.For<ConsumeContext<GetNutritionGoalsByUserIdRequest>>();
        context.Message.Returns(request);
        await context.RespondAsync(Arg.Do<GetNutritionGoalsByUserIdResponse>(x => response = x));

        goalQuery
            .GetGoalByTypeAndDateAsync(
                userId,
                GoalType.Calories,
                date,
                Arg.Any<CancellationToken>()
            )
            .Returns(caloriesGoal);

        goalQuery
            .GetGoalByTypeAndDateAsync(
                userId,
                GoalType.Carbohydrates,
                date,
                Arg.Any<CancellationToken>()
            )
            .Returns((GoalReadModel?)null);

        goalQuery
            .GetGoalByTypeAndDateAsync(userId, GoalType.Fats, date, Arg.Any<CancellationToken>())
            .Returns((GoalReadModel?)null);

        goalQuery
            .GetGoalByTypeAndDateAsync(userId, GoalType.Protein, date, Arg.Any<CancellationToken>())
            .Returns(proteinGoal);

        // Act
        await sut.Consume(context);

        // Assert
        response.Data.Should().BeNull();
        response.Errors.Should().HaveCount(2);
        response.Errors.Should().Contain(GoalErrors.NotExisting(GoalType.Carbohydrates));
        response.Errors.Should().Contain(GoalErrors.NotExisting(GoalType.Fats));

        await goalQuery
            .Received(1)
            .GetGoalByTypeAndDateAsync(
                userId,
                GoalType.Calories,
                date,
                Arg.Any<CancellationToken>()
            );
        await goalQuery
            .Received(1)
            .GetGoalByTypeAndDateAsync(
                userId,
                GoalType.Carbohydrates,
                date,
                Arg.Any<CancellationToken>()
            );
        await goalQuery
            .Received(1)
            .GetGoalByTypeAndDateAsync(userId, GoalType.Fats, date, Arg.Any<CancellationToken>());
        await goalQuery
            .Received(1)
            .GetGoalByTypeAndDateAsync(
                userId,
                GoalType.Protein,
                date,
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task Consume_WhenAllGoalsAreMissing_ShouldReturnAllErrors()
    {
        // Arrange
        GetNutritionGoalsByUserIdResponse response = null!;
        var userId = UserId.NewId();
        var date = DateOnly.FromDateTime(DateTime.UtcNow);

        var request = new GetNutritionGoalsByUserIdRequest(userId, date);
        var context = Substitute.For<ConsumeContext<GetNutritionGoalsByUserIdRequest>>();
        context.Message.Returns(request);
        await context.RespondAsync(Arg.Do<GetNutritionGoalsByUserIdResponse>(x => response = x));

        goalQuery
            .GetGoalByTypeAndDateAsync(
                Arg.Any<UserId>(),
                Arg.Any<GoalType>(),
                Arg.Any<DateOnly>(),
                Arg.Any<CancellationToken>()
            )
            .Returns((GoalReadModel?)null);

        // Act
        await sut.Consume(context);

        // Assert
        response.Data.Should().BeNull();
        response.Errors.Should().HaveCount(4);
        response.Errors.Should().Contain(GoalErrors.NotExisting(GoalType.Calories));
        response.Errors.Should().Contain(GoalErrors.NotExisting(GoalType.Carbohydrates));
        response.Errors.Should().Contain(GoalErrors.NotExisting(GoalType.Fats));
        response.Errors.Should().Contain(GoalErrors.NotExisting(GoalType.Protein));

        await goalQuery
            .Received(1)
            .GetGoalByTypeAndDateAsync(
                userId,
                GoalType.Calories,
                date,
                Arg.Any<CancellationToken>()
            );
        await goalQuery
            .Received(1)
            .GetGoalByTypeAndDateAsync(
                userId,
                GoalType.Carbohydrates,
                date,
                Arg.Any<CancellationToken>()
            );
        await goalQuery
            .Received(1)
            .GetGoalByTypeAndDateAsync(userId, GoalType.Fats, date, Arg.Any<CancellationToken>());
        await goalQuery
            .Received(1)
            .GetGoalByTypeAndDateAsync(
                userId,
                GoalType.Protein,
                date,
                Arg.Any<CancellationToken>()
            );
    }
}
