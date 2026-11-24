using FluentAssertions;
using NSubstitute;
using TrackYourLife.Modules.Users.Application.Features.PersonalAccessTokens.Commands.RevokePersonalAccessToken;
using TrackYourLife.Modules.Users.Domain.Features.PersonalAccessTokens;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;
using Xunit;

namespace TrackYourLife.Modules.Users.Application.UnitTests.Features.PersonalAccessTokens.Commands.RevokePersonalAccessToken;

public class RevokePersonalAccessTokenCommandHandlerTests
{
    private readonly IPersonalAccessTokenRepository _repository;
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly RevokePersonalAccessTokenCommandHandler _handler;

    public RevokePersonalAccessTokenCommandHandlerTests()
    {
        _repository = Substitute.For<IPersonalAccessTokenRepository>();
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _handler = new RevokePersonalAccessTokenCommandHandler(
            _repository,
            _userIdentifierProvider
        );
    }

    [Fact]
    public async Task Handle_WhenTokenOwned_RevokesSuccessfully()
    {
        var userId = UserId.NewId();
        var token = CreateToken(userId);
        _userIdentifierProvider.UserId.Returns(userId);
        _repository.GetByIdAsync(token.Id, Arg.Any<CancellationToken>()).Returns(token);

        var result = await _handler.Handle(
            new RevokePersonalAccessTokenCommand(token.Id),
            CancellationToken.None
        );

        result.IsSuccess.Should().BeTrue();
        token.IsRevoked.Should().BeTrue();
        _repository.Received(1).Update(token);
    }

    [Fact]
    public async Task Handle_WhenTokenMissing_ReturnsNotFound()
    {
        var userId = UserId.NewId();
        var tokenId = PersonalAccessTokenId.NewId();
        _userIdentifierProvider.UserId.Returns(userId);
        _repository
            .GetByIdAsync(tokenId, Arg.Any<CancellationToken>())
            .Returns((PersonalAccessToken?)null);

        var result = await _handler.Handle(
            new RevokePersonalAccessTokenCommand(tokenId),
            CancellationToken.None
        );

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PersonalAccessTokenErrors.NotFound(tokenId));
    }

    [Fact]
    public async Task Handle_WhenTokenNotOwned_ReturnsNotOwned()
    {
        var ownerId = UserId.NewId();
        var otherUserId = UserId.NewId();
        var token = CreateToken(ownerId);
        _userIdentifierProvider.UserId.Returns(otherUserId);
        _repository.GetByIdAsync(token.Id, Arg.Any<CancellationToken>()).Returns(token);

        var result = await _handler.Handle(
            new RevokePersonalAccessTokenCommand(token.Id),
            CancellationToken.None
        );

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PersonalAccessTokenErrors.NotOwned(token.Id));
        _repository.DidNotReceive().Update(Arg.Any<PersonalAccessToken>());
    }

    private static PersonalAccessToken CreateToken(UserId userId)
    {
        return PersonalAccessToken
            .Create(
                PersonalAccessTokenId.NewId(),
                userId,
                "Home Assistant",
                "tyl_abcdefgh",
                "hashvalue",
                DateTime.UtcNow,
                DateTime.UtcNow.AddYears(10)
            )
            .Value;
    }
}
