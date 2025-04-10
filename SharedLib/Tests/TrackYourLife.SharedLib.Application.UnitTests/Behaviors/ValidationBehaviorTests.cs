using FluentAssertions;
using FluentValidation;
using MediatR;
using TrackYourLife.SharedLib.Application.Behaviors;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.SharedLib.Application.UnitTests.Behaviors;

public class ValidationTestRequest<TResponse> : IRequest<TResponse>
{
    public string? Name { get; set; }
    public int? Age { get; set; }
    public string? Email { get; set; }
}

public class NameValidator : AbstractValidator<ValidationTestRequest<Result>>
{
    public NameValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required");
    }
}

public class EmailValidator : AbstractValidator<ValidationTestRequest<Result>>
{
    public EmailValidator()
    {
        RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required");
    }
}

public class AgeValidator : AbstractValidator<ValidationTestRequest<Result>>
{
    public AgeValidator()
    {
        RuleFor(x => x.Age).GreaterThan(0).WithMessage("Age must be greater than 0");
    }
}

public class ValidationBehaviorTests
{
    private readonly ValidationBehavior<ValidationTestRequest<Result>, Result> _sut;
    private readonly IValidator<ValidationTestRequest<Result>> _validator;

    public ValidationBehaviorTests()
    {
        _validator = new NameValidator();
        _sut = new ValidationBehavior<ValidationTestRequest<Result>, Result>(new[] { _validator });
    }

    [Fact]
    public async Task Handle_WithValidRequest_ShouldProceed()
    {
        // Arrange
        var request = new ValidationTestRequest<Result>
        {
            Name = "Test",
            Age = 25,
            Email = "test@example.com",
        };
        var expectedResponse = Result.Success();

        // Act
        var result = await _sut.Handle(
            request,
            () => Task.FromResult(expectedResponse),
            CancellationToken.None
        );

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Should().Be(expectedResponse);
    }

    [Fact]
    public async Task Handle_WithInvalidRequest_ShouldReturnValidationErrorWithAllErrors()
    {
        // Arrange
        var request = new ValidationTestRequest<Result>
        {
            Name = null,
            Age = 25,
            Email = null,
        };
        var expectedResponse = Result.Success();
        var nameValidator = new NameValidator();
        var emailValidator = new EmailValidator();
        var sut = new ValidationBehavior<ValidationTestRequest<Result>, Result>(
            [nameValidator, emailValidator]
        );

        // Act
        var result = await sut.Handle(
            request,
            () => Task.FromResult(expectedResponse),
            CancellationToken.None
        );

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error.Code.Should().Be("ValidationError");
        result.Should().BeOfType<ValidationResult>();
        var validationResult = (ValidationResult)result;
        validationResult.Should().NotBeNull();
        validationResult.Errors.Should().HaveCount(2);
        validationResult.Errors[0].Code.Should().Be("Name");
        validationResult.Errors[0].Message.Should().Be("Name is required");
        validationResult.Errors[1].Code.Should().Be("Email");
        validationResult.Errors[1].Message.Should().Be("Email is required");
    }

    [Fact]
    public async Task Handle_WithMultipleValidators_ShouldCombineErrors()
    {
        // Arrange
        var request = new ValidationTestRequest<Result>
        {
            Name = null,
            Age = 0,
            Email = "test@example.com",
        };
        var expectedResponse = Result.Success();
        var nameValidator = new NameValidator();
        var ageValidator = new AgeValidator();
        var sut = new ValidationBehavior<ValidationTestRequest<Result>, Result>(
            [nameValidator, ageValidator]
        );

        // Act
        var result = await sut.Handle(
            request,
            () => Task.FromResult(expectedResponse),
            CancellationToken.None
        );

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error.Code.Should().Be("ValidationError");
        result.Should().BeOfType<ValidationResult>();
        var validationResult = (ValidationResult)result;
        validationResult.Should().NotBeNull();
        validationResult.Errors.Should().HaveCount(2);
        validationResult
            .Errors.Should()
            .BeEquivalentTo(
                [
                    new Error("Name", "Name is required"),
                    new Error("Age", "Age must be greater than 0"),
                ],
                options => options.Including(x => x.Code).Including(x => x.Message)
            );
    }
}
