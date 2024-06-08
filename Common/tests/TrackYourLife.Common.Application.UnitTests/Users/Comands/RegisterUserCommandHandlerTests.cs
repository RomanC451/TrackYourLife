// using Moq;
// using TrackYourLifeDotnet.Application.Abstractions.Authentication;
// using TrackYourLifeDotnet.Application.Abstractions.Services;
// using TrackYourLifeDotnet.Application.Users.Commands.Register;
// using TrackYourLifeDotnet.Domain.Errors;
// using TrackYourLifeDotnet.Domain.Repositories;
// using TrackYourLifeDotnet.Domain.Shared;
// using Microsoft.FeatureManagement;
// using TrackYourLifeDotnet.Domain.Users.Repositories;
// using TrackYourLifeDotnet.Domain.Users.ValueObjects;
// using TrackYourLifeDotnet.Domain.Users;

// namespace TrackYourLifeDotnet.Application.UnitTests.Users.Comands;

// public class RegisterUserHandlerTests
// {
//     private readonly Mock<IUserRepository> _userRepository = new();
//     private readonly Mock<IUnitOfWork> _unitOfWork = new();
//     private readonly Mock<IPasswordHasher> _passwordHasher = new();
//     private readonly Mock<IFeatureManager> _featureManager = new();
//     private readonly RegisterUserCommandHandler _sut;

//     public RegisterUserHandlerTests()
//     {
//         _sut = new RegisterUserCommandHandler(
//             _userRepository.Object,
//             _unitOfWork.Object,
//             _passwordHasher.Object,
//             _featureManager.Object
//         );
//     }

//     [Fact]
//     public async Task Handler_WithValidCommand_ShouldCreateNewUser()
//     {
//         // Arrange
//         RegisterUserCommand command = new("test@test.com", "Testtest.1234", "John", "Doe");

//         HashedPassword passwordHash = new("HashedPassword");

//         _passwordHasher.Setup(hasher => hasher.Hash(command.Password)).Returns(passwordHash);

//         Email email = Email.Create(command.Email).Value;

//         Name firstName = Name.Create(command.FirstName).Value;
//         Name lastName = Name.Create(command.LastName).Value;

//         _userRepository
//             .Setup(repo => repo.IsEmailUniqueAsync(email, It.IsAny<CancellationToken>()))
//             .ReturnsAsync(true);

//         _unitOfWork.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()));

//         Domain.Users.User createdUser = null!;

//         _userRepository
//             .Setup(repo => repo.AddAsync(It.IsAny<Domain.Users.User>(), CancellationToken.None))
//             .Callback<Domain.Users.User, CancellationToken>((user, token) => createdUser = user);

//         // Act
//         Result<RegisterUserResult> result = await _sut.Handle(command, CancellationToken.None);

//         // Assert
//         Assert.True(result.IsSuccess);

//         RegisterUserResult response = result.Value;

//         Assert.NotNull(response);
//         Assert.Equal(createdUser.Id.Value, response.UserId.Value);

//         _userRepository.Verify(
//             repo => repo.AddAsync(It.IsAny<Domain.Users.User>(), CancellationToken.None),
//             Times.Once
//         );
//         _userRepository.Verify(
//             repo => repo.IsEmailUniqueAsync(email, It.IsAny<CancellationToken>()),
//             Times.Once
//         );
//         _unitOfWork.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
//         _passwordHasher.Verify(hasher => hasher.Hash(command.Password), Times.Once);
//     }

//     [Fact]
//     public async Task Handler_WithInvalidEmail_ShouldReturnError()
//     {
//         // Arrange
//         RegisterUserCommand command = new("invalidemail", "TestTest.1234", "John", "Doe");

//         // Act
//         Result<RegisterUserResult> result = await _sut.Handle(command, CancellationToken.None);

//         // Assert
//         Assert.True(result.IsFailure);
//         Assert.Contains(DomainErrors.Email.InvalidFormat, result.Error);

//         _userRepository.Verify(
//             repo => repo.IsEmailUniqueAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()),
//             Times.Never
//         );
//         _userRepository.Verify(
//             repo => repo.AddAsync(It.IsAny<Domain.Users.User>(), CancellationToken.None),
//             Times.Never
//         );
//         _unitOfWork.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
//         _passwordHasher.Verify(hasher => hasher.Hash(command.Password), Times.Never);
//     }

//     [Fact]
//     public async Task Handler_WithInvalidPassword_ShouldReturnError()
//     {
//         // Arrange
//         RegisterUserCommand command = new("test2@test.com", "invalidpassword", "John", "Doe");
//         var email = Email.Create(command.Email).Value;
//         _userRepository
//             .Setup(repo => repo.IsEmailUniqueAsync(email, It.IsAny<CancellationToken>()))
//             .ReturnsAsync(true);

//         // Act
//         Result<RegisterUserResult> result = await _sut.Handle(command, CancellationToken.None);

//         // Assert
//         Assert.True(result.IsFailure);
//         Assert.Contains(DomainErrors.Password.UpperCase, result.Error);

//         _userRepository.Verify(
//             repo => repo.IsEmailUniqueAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()),
//             Times.Once
//         );
//         _userRepository.Verify(
//             repo => repo.AddAsync(It.IsAny<Domain.Users.User>(), CancellationToken.None),
//             Times.Never
//         );
//         _unitOfWork.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
//         _passwordHasher.Verify(hasher => hasher.Hash(command.Password), Times.Never);
//     }

//     [Fact]
//     public async Task Handler_WithExistingEmail_ShouldReturnError()
//     {
//         // Arrange
//         RegisterUserCommand command = new("test@test.com", "TestTest.1234", "John", "Doe");

//         _userRepository
//             .Setup(
//                 repo => repo.IsEmailUniqueAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>())
//             )
//             .ReturnsAsync(false);

//         // Act
//         Result<RegisterUserResult> result = await _sut.Handle(command, CancellationToken.None);

//         // Assert
//         Assert.True(result.IsFailure);
//         Assert.Contains(DomainErrors.Email.AlreadyUsed, result.Error);

//         _userRepository.Verify(
//             repo => repo.IsEmailUniqueAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()),
//             Times.Once
//         );
//         _userRepository.Verify(
//             repo => repo.AddAsync(It.IsAny<Domain.Users.User>(), CancellationToken.None),
//             Times.Never
//         );
//         _unitOfWork.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
//         _passwordHasher.Verify(hasher => hasher.Hash(command.Password), Times.Never);
//     }

//     [Fact]
//     public async Task Handler_WithInvalidLastName_ShouldReturnError()
//     {
//         // Arrange
//         RegisterUserCommand command = new("test@test.com", "TestTest.1234", "John", "");

//         _userRepository
//             .Setup(
//                 repo => repo.IsEmailUniqueAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>())
//             )
//             .ReturnsAsync(true);

//         // Act
//         Result<RegisterUserResult> result = await _sut.Handle(command, CancellationToken.None);

//         // Assert
//         Assert.True(result.IsFailure);
//         Assert.Contains(DomainErrors.Name.Empty, result.Error);

//         _userRepository.Verify(
//             repo => repo.IsEmailUniqueAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()),
//             Times.Never
//         );
//         _userRepository.Verify(
//             repo => repo.AddAsync(It.IsAny<Domain.Users.User>(), CancellationToken.None),
//             Times.Never
//         );
//         _unitOfWork.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
//         _passwordHasher.Verify(hasher => hasher.Hash(command.Password), Times.Never);
//     }

//     [Fact]
//     public async Task Handler_WithInvalidFirstName_ShouldReturnError()
//     {
//         // Arrange
//         RegisterUserCommand command = new("test@test.com", "TestTest.1234", "", "Doe");

//         _userRepository
//             .Setup(
//                 repo => repo.IsEmailUniqueAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>())
//             )
//             .ReturnsAsync(true);

//         // Act
//         Result<RegisterUserResult> result = await _sut.Handle(command, CancellationToken.None);

//         // Assert
//         Assert.True(result.IsFailure);
//         Assert.Contains(DomainErrors.Name.Empty, result.Error);

//         _userRepository.Verify(
//             repo => repo.IsEmailUniqueAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()),
//             Times.Never
//         );
//         _userRepository.Verify(
//             repo => repo.AddAsync(It.IsAny<Domain.Users.User>(), CancellationToken.None),
//             Times.Never
//         );
//         _unitOfWork.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

//         _passwordHasher.Verify(hasher => hasher.Hash(command.Password), Times.Never);
//     }
// }
