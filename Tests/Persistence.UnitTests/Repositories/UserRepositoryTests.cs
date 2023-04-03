using Microsoft.EntityFrameworkCore;
using TrackYourLife.Persistence.Repositories;
using TrackYourLifeDotnet.Domain.Entities;
using TrackYourLifeDotnet.Domain.Errors;
using TrackYourLifeDotnet.Domain.Repositories;
using TrackYourLifeDotnet.Domain.ValueObjects;

namespace TrackYourLifeDotnet.Persistence.UnitTests.Repositories;

public class UserRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly IUserRepository _userRepository;

    public UserRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        _context = new ApplicationDbContext(options, null);

        _userRepository = new UserRepository(_context);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsUser_IfExists()
    {
        // Arrange
        var user = User.Create(
            Guid.NewGuid(),
            Email.Create("john.doe@example.com").Value,
            PasswordHash.Create("password").Value,
            FirstName.Create("John").Value,
            LastName.Create("Doe").Value
        );

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        // Act
        var userFromDb = await _userRepository.GetByIdAsync(user.Id, CancellationToken.None);

        // Assert
        Assert.NotNull(userFromDb);
        Assert.Equal(user, userFromDb);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_IfDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var result = await _userRepository.GetByIdAsync(userId, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByEmailAsync_ReturnsUser_IfExists()
    {
        // Arrange
        var email = Email.Create("john.doe2@example.com").Value;
        var user = User.Create(
            Guid.NewGuid(),
            email,
            PasswordHash.Create("Password").Value,
            FirstName.Create("John").Value,
            LastName.Create("Doe").Value
        );
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        // Act
        var userFromDb = await _userRepository.GetByEmailAsync(email, CancellationToken.None);

        // Assert
        Assert.NotNull(userFromDb);
        Assert.Equal(user, userFromDb);
    }

    [Fact]
    public async Task GetByEmailAsync_ReturnsNull_IfDoesNotExist()
    {
        // Arrange
        var email = Email.Create("john.doe3@example.com").Value;

        // Act
        var result = await _userRepository.GetByEmailAsync(email, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task IsEmailUniqueAsync_ReturnsTrue_IfEmailIsUnique()
    {
        // Arrange
        var email = Email.Create("john.doe4@example.com").Value;

        // Act
        var result = await _userRepository.IsEmailUniqueAsync(email, CancellationToken.None);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task IsEmailUniqueAsync_ReturnsFalse_IfEmailIsNotUnique()
    {
        // Arrange
        var email = Email.Create("john.doe5@example.com").Value;
        var user = User.Create(
            Guid.NewGuid(),
            email,
            PasswordHash.Create("Password").Value,
            FirstName.Create("John").Value,
            LastName.Create("Doe").Value
        );
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _userRepository.IsEmailUniqueAsync(email, CancellationToken.None);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task Add_ReturnsSuccess_WhenUserIsAdded()
    {
        // Arrange
        var email = Email.Create("john.doe6@example.com").Value;
        var user = User.Create(
            Guid.NewGuid(),
            email,
            PasswordHash.Create("Password").Value,
            FirstName.Create("John").Value,
            LastName.Create("Doe").Value
        );
        // Act
        _userRepository.Add(user);
        _context.SaveChanges();

        // Assert
        var userFromDb = await _context.Users.FindAsync(user.Id);

        Assert.NotNull(userFromDb);
        Assert.Equal(user, userFromDb);
    }

    [Fact]
    public void Add_Should_ThrowException_WhenUserWithDuplicateIdIsAdded()
    {
        // Arrange
        var email = Email.Create("john.doe6@example.com").Value;
        var user = User.Create(
            Guid.NewGuid(),
            email,
            PasswordHash.Create("Password").Value,
            FirstName.Create("John").Value,
            LastName.Create("Doe").Value
        );

        _context.Users.Add(user);
        _context.SaveChanges();

        var email2 = Email.Create("john.doe6@example.com").Value;
        var user2 = User.Create(
            user.Id,
            email2,
            PasswordHash.Create("Password").Value,
            FirstName.Create("John").Value,
            LastName.Create("Doe").Value
        );

        // Act and assert
        _userRepository.Add(user);
        Assert.Throws<ArgumentException>(() => _context.SaveChanges());
    }

    [Fact]
    public async Task Update_User_Should_Update_User_In_Database()
    {
        // Arrange
        var email = Email.Create("john.doe7@example.com").Value;
        var user = User.Create(
            Guid.NewGuid(),
            email,
            PasswordHash.Create("Password").Value,
            FirstName.Create("John").Value,
            LastName.Create("Doe").Value
        );

        _context.Users.Add(user);
        _context.SaveChanges();

        user.ChangeName(FirstName.Create("John2").Value, LastName.Create("Doe2").Value);

        // Act
        _userRepository.Update(user);
        _context.SaveChanges();

        // Assert
        var userFromDb = await _context.Users.FindAsync(user.Id);

        Assert.NotNull(userFromDb);
        Assert.Equal(user, userFromDb);
    }

    [Fact]
    public void Update_Should_ThrowException_WhenUserDoesNotExist()
    {
        // Arrange
        var email = Email.Create("john.doe7@example.com").Value;
        var user = User.Create(
            Guid.NewGuid(),
            email,
            PasswordHash.Create("Password").Value,
            FirstName.Create("John").Value,
            LastName.Create("Doe").Value
        );

        // Act and assert
        _userRepository.Update(user);
        Assert.Throws<DbUpdateConcurrencyException>(() => _context.SaveChanges());
    }

    [Fact]
    public void Remove_User_Should_Remove_User_From_Database()
    {
        // Arrange
        var email = Email.Create("john.doe9@example.com").Value;
        var user = User.Create(
            Guid.NewGuid(),
            email,
            PasswordHash.Create("Password").Value,
            FirstName.Create("John").Value,
            LastName.Create("Doe").Value
        );

        _context.Users.Add(user);
        _context.SaveChanges();

        // Act
        _userRepository.Remove(user);
        _context.SaveChanges();
        // Assert

        Assert.DoesNotContain(user, _context.Users);
    }

    [Fact]
    public void Remove_ShouldThrowException_WhenUserDoesNotExist()
    {
        // Arrange
        var email = Email.Create("john.doe7@example.com").Value;
        var user = User.Create(
            Guid.NewGuid(),
            email,
            PasswordHash.Create("Password").Value,
            FirstName.Create("John").Value,
            LastName.Create("Doe").Value
        );

        // Act and assert
        _userRepository.Remove(user);
        Assert.Throws<DbUpdateConcurrencyException>(() => _context.SaveChanges());
    }

    [Fact]
    public void Dispose()
    {
        _context?.Dispose();
        GC.SuppressFinalize(this);
    }
}
