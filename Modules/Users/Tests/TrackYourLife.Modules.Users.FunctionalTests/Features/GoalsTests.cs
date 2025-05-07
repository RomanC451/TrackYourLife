using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.Users.Contracts.Dtos;
using TrackYourLife.Modules.Users.Domain.Features.Goals;
using TrackYourLife.Modules.Users.Domain.Features.Goals.Enums;
using TrackYourLife.Modules.Users.Domain.UnitTests.Utils;
using TrackYourLife.Modules.Users.Presentation.Features.Goals.Commands;
using TrackYourLife.SharedLib.Domain.Enums;
using TrackYourLife.SharedLib.FunctionalTests.Utils;

namespace TrackYourLife.Modules.Users.FunctionalTests.Features;

[Collection("Users Integration Tests")]
public class GoalsTests(UsersFunctionalTestWebAppFactory factory)
    : UsersBaseIntegrationTest(factory)
{
    [Fact]
    public async Task AddGoal_WithValidData_ShouldReturnGoalId()
    {
        // Arrange
        var request = new AddGoalRequest(
            Value: 2000,
            Type: GoalType.Calories,
            Period: GoalPeriod.Day,
            StartDate: DateOnly.FromDateTime(DateTime.UtcNow),
            Force: false,
            EndDate: null
        );

        // Act
        var response = await _client.PostAsJsonAsync("/api/goals", request);

        // Assert
        var result = await response.ShouldHaveStatusCodeAndContent<TestIdResponse>(
            HttpStatusCode.Created
        );
        result.Should().NotBeNull();
        result!.id.Should().NotBe(Guid.Empty);

        // Verify goal was created
        var goal = await _usersWriteDbContext.Goals.FirstOrDefaultAsync(g =>
            g.Id == GoalId.Create(result.id)
        );
        goal.Should().NotBeNull();
        goal!.Type.Should().Be(request.Type);
        goal.Value.Should().Be(request.Value);
        goal.Period.Should().Be(request.Period);
        goal.StartDate.Should().Be(request.StartDate);
    }

    [Fact]
    public async Task UpdateGoal_WithValidData_ShouldReturnNoContent()
    {
        // Arrange
        var goal = GoalFaker.Generate(userId: _user.Id);
        await _usersWriteDbContext.Goals.AddAsync(goal);
        await _usersWriteDbContext.SaveChangesAsync();

        var request = new UpdateGoalRequest(
            Id: goal.Id,
            Type: GoalType.Calories,
            Value: 2500,
            Period: GoalPeriod.Day,
            StartDate: DateOnly.FromDateTime(DateTime.UtcNow),
            Force: false,
            EndDate: null
        );

        // Act
        var response = await _client.PutAsJsonAsync("/api/goals", request);

        // Assert
        await response.ShouldHaveStatusCode(HttpStatusCode.NoContent);

        // Verify goal was updated
        var updatedGoal = await _usersWriteDbContext
            .Goals.AsNoTracking()
            .FirstOrDefaultAsync(g => g.Id == goal.Id);
        updatedGoal.Should().NotBeNull();
        updatedGoal!.Value.Should().Be(request.Value);
        updatedGoal.Type.Should().Be(request.Type);
    }

    [Fact]
    public async Task GetGoal_WithValidTypeAndDate_ShouldReturnGoal()
    {
        // Arrange
        var goal = GoalFaker.Generate(userId: _user.Id);
        await _usersWriteDbContext.Goals.AddAsync(goal);
        await _usersWriteDbContext.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync(
            $"/api/goals?goalType={goal.Type}&date={goal.StartDate:yyyy-MM-dd}"
        );

        // Assert
        var result = await response.ShouldHaveStatusCodeAndContent<GoalDto>(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.Id.Should().Be(goal.Id);
        result.Type.Should().Be(goal.Type);
        result.Value.Should().Be(goal.Value);
    }

    [Fact]
    public async Task GetNutritionGoals_WithValidDate_ShouldReturnNutritionGoals()
    {
        // Arrange

        var goals = new[]
        {
            GoalFaker.Generate(type: GoalType.Calories, userId: _user.Id),
            GoalFaker.Generate(type: GoalType.Protein, userId: _user.Id),
            GoalFaker.Generate(type: GoalType.Carbohydrates, userId: _user.Id),
            GoalFaker.Generate(type: GoalType.Fats, userId: _user.Id),
        };

        foreach (var goal in goals)
        {
            await _usersWriteDbContext.Goals.AddAsync(goal);
        }
        await _usersWriteDbContext.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync(
            $"/api/goals/nutrition-goals?date={DateTime.UtcNow:yyyy-MM-dd}"
        );

        // Assert
        var result = await response.ShouldHaveStatusCodeAndContent<List<GoalDto>>(
            HttpStatusCode.OK
        );
        result.Should().NotBeNull();
        result.Should().HaveCount(4);
        result.Should().Contain(g => g.Type == GoalType.Calories);
        result.Should().Contain(g => g.Type == GoalType.Protein);
        result.Should().Contain(g => g.Type == GoalType.Carbohydrates);
        result.Should().Contain(g => g.Type == GoalType.Fats);
    }

    [Fact]
    public async Task CalculateNutritionGoals_WithValidData_ShouldReturnNoContent()
    {
        // Arrange
        var request = new CalculateNutritionGoalsRequest(
            Age: 30,
            Weight: 70,
            Height: 175,
            Gender: Gender.Male,
            ActivityLevel: ActivityLevel.ModeratelyActive,
            FitnessGoal: FitnessGoal.Maintain,
            Force: false
        );

        // Act
        var response = await _client.PutAsJsonAsync("/api/goals/nutrition", request);

        // Assert
        await response.ShouldHaveStatusCode(HttpStatusCode.NoContent);

        // Verify nutrition goals were created
        var goals = await _usersWriteDbContext
            .Goals.Where(g =>
                g.Type == GoalType.Calories
                || g.Type == GoalType.Protein
                || g.Type == GoalType.Carbohydrates
                || g.Type == GoalType.Fats
            )
            .ToListAsync();
        goals.Should().HaveCount(4);
    }

    [Fact]
    public async Task UpdateNutritionGoals_WithValidData_ShouldReturnNoContent()
    {
        // Arrange
        var request = new UpdateNutritionGoalsRequest(
            Calories: 2000,
            Protein: 150,
            Carbohydrates: 200,
            Fats: 70,
            Force: false
        );

        // Act
        var response = await _client.PutAsJsonAsync("/api/goals/nutrition/update", request);

        // Assert
        await response.ShouldHaveStatusCode(HttpStatusCode.NoContent);

        // Verify nutrition goals were updated
        var goals = await _usersWriteDbContext
            .Goals.Where(g =>
                g.Type == GoalType.Calories
                || g.Type == GoalType.Protein
                || g.Type == GoalType.Carbohydrates
                || g.Type == GoalType.Fats
            )
            .ToListAsync();
        goals.Should().HaveCount(4);
        goals.Should().Contain(g => g.Type == GoalType.Calories && g.Value == request.Calories);
        goals.Should().Contain(g => g.Type == GoalType.Protein && g.Value == request.Protein);
        goals
            .Should()
            .Contain(g => g.Type == GoalType.Carbohydrates && g.Value == request.Carbohydrates);
        goals.Should().Contain(g => g.Type == GoalType.Fats && g.Value == request.Fats);
    }
}
