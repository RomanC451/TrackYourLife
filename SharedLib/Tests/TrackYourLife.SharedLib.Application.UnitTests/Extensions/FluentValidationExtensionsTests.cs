using FluentAssertions;
using FluentValidation;
using FluentValidation.TestHelper;
using NSubstitute;
using TrackYourLife.SharedLib.Application.Extensions;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.SharedLib.Application.UnitTests.Extensions;

public record TestId : StronglyTypedGuid<TestId>
{
    public TestId()
        : base(Guid.Empty) { }

    public TestId(Guid value)
        : base(value) { }
}

public class TestModel
{
    public TestId Id { get; set; } = new TestId();
}

public class TestModelValidator : AbstractValidator<TestModel>
{
    public TestModelValidator()
    {
        RuleFor(x => x.Id).NotEmptyId();
    }
}

public class FluentValidationExtensionsTests
{
    private readonly TestModelValidator _validator;
    private readonly TestModel _model;

    public FluentValidationExtensionsTests()
    {
        _validator = new TestModelValidator();
        _model = new TestModel { Id = new TestId() };
    }

    [Fact]
    public void NotEmptyId_WhenIdIsEmptyGuid_ShouldHaveValidationError()
    {
        // Arrange
        _model.Id = new TestId();

        // Act
        var result = _validator.TestValidate(_model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id);
        result.Errors.Should().Contain(x => x.ErrorMessage == "The ID must not be empty.");
    }

    [Fact]
    public void NotEmptyId_WhenIdIsValid_ShouldNotHaveValidationError()
    {
        // Arrange
        _model.Id = new TestId(Guid.NewGuid());

        // Act
        var result = _validator.TestValidate(_model);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void NotEmptyId_ShouldConfigureRuleBuilderCorrectly()
    {
        // Arrange
        var validator = new InlineValidator<TestModel>();
        var ruleBuilder = validator.RuleFor(x => x.Id);

        // Act
        ruleBuilder.NotEmptyId();

        // Assert
        var model = new TestModel { Id = new TestId() };
        var validationResult = validator.Validate(model);
        validationResult.IsValid.Should().BeFalse();
        validationResult
            .Errors.Should()
            .ContainSingle()
            .Which.ErrorMessage.Should()
            .Be("The ID must not be empty.");

        model.Id = new TestId(Guid.NewGuid());
        validationResult = validator.Validate(model);
        validationResult.IsValid.Should().BeTrue();
        validationResult.Errors.Should().BeEmpty();
    }
}
