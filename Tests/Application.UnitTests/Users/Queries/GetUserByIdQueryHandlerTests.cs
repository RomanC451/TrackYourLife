using Moq;
using TrackYourLifeDotnet.Application.Users.Queries.GetUserById;
using TrackYourLifeDotnet.Domain.Errors;
using TrackYourLifeDotnet.Domain.Repositories;
using TrackYourLifeDotnet.Domain.ValueObjects;
using TrackYourLifeDotnet.Domain.Entities;
using Xunit;

namespace TrackYourLifeDotnet.Application.UnitTests.Users.Queries;

public class GetUserByIdQueryHandlerTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly GetUserByIdQueryHandler _sut;

    public GetUserByIdQueryHandlerTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _sut = new GetUserByIdQueryHandler(_mockUserRepository.Object);
    }

    [Fact]
    public async Task Handle_WithValidId_ReturnsSuccessResultWithUserData()
    {
        // Arranges
        var query = new GetUserByIdQuery(Guid.NewGuid());
        var user = User.Create(
            Guid.NewGuid(),
            Email.Create("johndoe@example.com").Value,
            new HashedPassword("password"),
            Name.Create("John").Value,
            Name.Create("Doe").Value
        );
        _mockUserRepository.Setup(r => r.GetByIdAsync(query.Id, default)).ReturnsAsync(user);

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
        var query = new GetUserByIdQuery(Guid.NewGuid());
        _mockUserRepository.Setup(r => r.GetByIdAsync(query.Id, default)).ReturnsAsync((User?)null);

        // Act
        var result = await _sut.Handle(query, default);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(DomainErrors.User.NotFound(query.Id), result.Error);
    }
}
