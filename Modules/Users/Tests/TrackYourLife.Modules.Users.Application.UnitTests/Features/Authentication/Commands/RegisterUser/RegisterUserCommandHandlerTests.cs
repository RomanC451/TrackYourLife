using TrackYourLife.Modules.Users.Application.Core;
using TrackYourLife.Modules.Users.Application.Core.Abstraction.Authentication;
using TrackYourLife.Modules.Users.Application.Features.Authentication.Commands.RegisterUser;
using TrackYourLife.Modules.Users.Domain.Core;
using TrackYourLife.Modules.Users.Domain.Features.Users;
using TrackYourLife.Modules.Users.Domain.Features.Users.ValueObjects;

namespace TrackYourLife.Modules.Users.Application.UnitTests.Features.Authentication.Commands.RegisterUser;

public class RegisterUserCommandHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly IUsersUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly UsersFeatureManagement _featureManager;
    private readonly RegisterUserCommandHandler _handler;

    public RegisterUserCommandHandlerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _unitOfWork = Substitute.For<IUsersUnitOfWork>();
        _passwordHasher = Substitute.For<IPasswordHasher>();
        _featureManager = Substitute.For<UsersFeatureManagement>();
        _handler = new RegisterUserCommandHandler(
            _userRepository,
            _passwordHasher,
            _featureManager
        );
    }

    [Fact]
    public async Task Handle_WithValidInput_CreatesUser()
    {
        // Arrange
        var command = new RegisterUserCommand(
            "test@example.com",
            "ValidPassword123!",
            "John",
            "Doe"
        );

        _userRepository
            .IsEmailUniqueAsync(Arg.Any<Email>(), Arg.Any<CancellationToken>())
            .Returns(true);

        _passwordHasher.Hash(Arg.Any<string>()).Returns(new HashedPassword("hashed_password"));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _userRepository.Received(1).AddAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithExistingEmail_ReturnsFailure()
    {
        // Arrange
        var command = new RegisterUserCommand(
            "existing@example.com",
            "ValidPassword123!",
            "John",
            "Doe"
        );

        _userRepository
            .IsEmailUniqueAsync(Arg.Any<Email>(), Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.Email.AlreadyUsed);
    }

    [Fact]
    public async Task Handle_WithExistingEmail_DoesNotCreateUser()
    {
        // Arrange
        var command = new RegisterUserCommand(
            "existing@example.com",
            "ValidPassword123!",
            "John",
            "Doe"
        );

        _userRepository
            .IsEmailUniqueAsync(Arg.Any<Email>(), Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _userRepository
            .DidNotReceive()
            .AddAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithExistingEmail_DoesNotSaveChanges()
    {
        // Arrange
        var command = new RegisterUserCommand(
            "existing@example.com",
            "ValidPassword123!",
            "John",
            "Doe"
        );

        _userRepository
            .IsEmailUniqueAsync(Arg.Any<Email>(), Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithSkipEmailVerification_VerifiesEmail()
    {
        // Arrange
        var command = new RegisterUserCommand(
            "test@example.com",
            "ValidPassword123!",
            "John",
            "Doe"
        );

        _userRepository
            .IsEmailUniqueAsync(Arg.Any<Email>(), Arg.Any<CancellationToken>())
            .Returns(true);

        _passwordHasher.Hash(Arg.Any<string>()).Returns(new HashedPassword("hashed_password"));

        _featureManager.SkipEmailVerification = true;

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _userRepository
            .Received(1)
            .AddAsync(Arg.Is<User>(u => u.VerifiedOnUtc != null), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithoutSkipEmailVerification_DoesNotVerifyEmail()
    {
        // Arrange
        var command = new RegisterUserCommand(
            "test@example.com",
            "ValidPassword123!",
            "John",
            "Doe"
        );

        _userRepository
            .IsEmailUniqueAsync(Arg.Any<Email>(), Arg.Any<CancellationToken>())
            .Returns(true);

        _passwordHasher.Hash(Arg.Any<string>()).Returns(new HashedPassword("hashed_password"));

        _featureManager.SkipEmailVerification = false;

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _userRepository
            .Received(1)
            .AddAsync(Arg.Is<User>(u => u.VerifiedOnUtc == null), Arg.Any<CancellationToken>());
    }
}
