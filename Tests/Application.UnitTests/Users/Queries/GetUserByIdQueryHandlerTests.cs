using Moq;
using TrackYourLifeDotnet.Application.Users.Queries.GetUserData;
using TrackYourLifeDotnet.Domain.Errors;

using Xunit;
using TrackYourLifeDotnet.Application.Abstractions.Services;
using TrackYourLifeDotnet.Domain.Users.Repositories;
using TrackYourLifeDotnet.Domain.Users;
using TrackYourLifeDotnet.Domain.Users.ValueObjects;
using TrackYourLifeDotnet.Domain.Users.StrongTypes;

namespace TrackYourLifeDotnet.Application.UnitTests.Users.Queries;

public class GetUserByIdQueryHandlerTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IAuthService> _mockAuthService;
    private readonly GetUserDataQueryHandler _sut;

    public GetUserByIdQueryHandlerTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockAuthService = new Mock<IAuthService>();
        _sut = new GetUserDataQueryHandler(_mockUserRepository.Object, _mockAuthService.Object);
    }

    [Fact]
    public async Task Handle_WithValidId_ReturnsSuccessResultWithUserData()
    {
        // Arranges
        var query = new GetUserDataQuery();
        var user = User.Create(
            UserId.NewId(),
            Email.Create("johndoe@example.com").Value,
            new HashedPassword("password"),
            Name.Create("John").Value,
            Name.Create("Doe").Value
        );
        _mockUserRepository.Setup(r => r.GetByIdAsync(user.Id, default)).ReturnsAsync(user);

        // Act
        var result = await _sut.Handle(query, default);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(user.Id, result.Value.Id);
        Assert.Equal(user.Email.Value, result.Value.Email);
        Assert.Equal(user.FirstName.Value, result.Value.FirstName);
        Assert.Equal(user.LastName.Value, result.Value.LastName);
    }

    [Fact]
    public async Task Handle_WithInvalidId_ReturnsFailureResultWithNotFoundError()
    {
        // Arrange
        var query = new GetUserDataQuery();
        var userId = UserId.NewId();
        _mockUserRepository.Setup(r => r.GetByIdAsync(userId, default)).ReturnsAsync((User?)null);

        // Act
        var result = await _sut.Handle(query, default);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(DomainErrors.User.NotFound(userId), result.Error);
    }
}
