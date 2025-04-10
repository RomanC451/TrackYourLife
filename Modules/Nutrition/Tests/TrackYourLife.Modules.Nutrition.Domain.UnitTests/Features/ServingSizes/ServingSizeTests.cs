using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;
using TrackYourLife.SharedLib.Domain.Errors;

namespace TrackYourLife.Modules.Nutrition.Domain.UnitTests.Features.ServingSizes;

public class ServingSizeTests
{
    private readonly ServingSizeId _id;
    private readonly float _nutritionMultiplier;
    private readonly string _unit;
    private readonly float _value;
    private readonly long? _apiId;

    public ServingSizeTests()
    {
        _id = ServingSizeId.NewId();
        _nutritionMultiplier = 1.0f;
        _unit = "g";
        _value = 100.0f;
        _apiId = 12345L;
    }

    [Fact]
    public void Create_WithValidData_ShouldCreateServingSize()
    {
        // Act
        var result = ServingSize.Create(_id, _nutritionMultiplier, _unit, _value, _apiId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Id.Should().Be(_id);
        result.Value.NutritionMultiplier.Should().Be(_nutritionMultiplier);
        result.Value.Unit.Should().Be(_unit);
        result.Value.Value.Should().Be(_value);
        result.Value.ApiId.Should().Be(_apiId);
    }

    [Fact]
    public void Create_WithoutApiId_ShouldCreateServingSize()
    {
        // Act
        var result = ServingSize.Create(_id, _nutritionMultiplier, _unit, _value);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Id.Should().Be(_id);
        result.Value.NutritionMultiplier.Should().Be(_nutritionMultiplier);
        result.Value.Unit.Should().Be(_unit);
        result.Value.Value.Should().Be(_value);
        result.Value.ApiId.Should().BeNull();
    }

    [Fact]
    public void Create_WithEmptyId_ShouldFail()
    {
        // Act
        var result = ServingSize.Create(ServingSizeId.Empty, _nutritionMultiplier, _unit, _value);

        // Assert
        result.IsFailure.Should().BeTrue();
        result
            .Error.Code.Should()
            .Be($"{nameof(ServingSize)}.{nameof(ServingSize.Id).ToCapitalCase()}.Empty");
    }

    [Fact]
    public void Create_WithNegativeNutritionMultiplier_ShouldFail()
    {
        // Act
        var result = ServingSize.Create(_id, -1.0f, _unit, _value);

        // Assert
        result.IsFailure.Should().BeTrue();
        result
            .Error.Code.Should()
            .Be(
                $"{nameof(ServingSize)}.{nameof(ServingSize.NutritionMultiplier).ToCapitalCase()}.NotPositive"
            );
    }

    [Fact]
    public void Create_WithEmptyUnit_ShouldFail()
    {
        // Act
        var result = ServingSize.Create(_id, _nutritionMultiplier, string.Empty, _value);

        // Assert
        result.IsFailure.Should().BeTrue();
        result
            .Error.Code.Should()
            .Be($"{nameof(ServingSize)}.{nameof(ServingSize.Unit).ToCapitalCase()}.Empty");
    }

    [Fact]
    public void Create_WithNegativeValue_ShouldFail()
    {
        // Act
        var result = ServingSize.Create(_id, _nutritionMultiplier, _unit, -100.0f);

        // Assert
        result.IsFailure.Should().BeTrue();
        result
            .Error.Code.Should()
            .Be($"{nameof(ServingSize)}.{nameof(ServingSize.Value).ToCapitalCase()}.NotPositive");
    }
}
