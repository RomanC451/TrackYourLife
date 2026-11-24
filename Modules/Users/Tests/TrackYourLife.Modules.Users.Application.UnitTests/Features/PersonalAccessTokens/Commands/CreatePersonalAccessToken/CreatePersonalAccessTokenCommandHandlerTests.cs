using FluentAssertions;
using NSubstitute;
using TrackYourLife.Modules.Users.Application.Core.Abstraction.Authentication;
using TrackYourLife.Modules.Users.Application.Features.PersonalAccessTokens.Commands.CreatePersonalAccessToken;
using TrackYourLife.Modules.Users.Domain.Features.PersonalAccessTokens;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;
using Xunit;

namespace TrackYourLife.Modules.Users.Application.UnitTests.Features.PersonalAccessTokens.Commands.CreatePersonalAccessToken;

public class CreatePersonalAccessTokenCommandHandlerTests
{
    private readonly IPersonalAccessTokenRepository _repository;
    private readonly IPersonalAccessTokenHasher _hasher;
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly CreatePersonalAccessTokenCommandHandler _handler;

    public CreatePersonalAccessTokenCommandHandlerTests()
    {
        _repository = Substitute.For<IPersonalAccessTokenRepository>();
        _hasher = Substitute.For<IPersonalAccessTokenHasher>();
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _handler = new CreatePersonalAccessTokenCommandHandler(
            _repository,
            _hasher,
            _userIdentifierProvider
        );
    }

    [Fact]
    public async Task Handle_WithValidName_ReturnsRawTokenOnceAndPersistsHash()
    {
        var userId = UserId.NewId();
        const string rawToken = "tyl_abcdefghijklmnopqrstuvwxyz012345";
        const string hash = "abc123hash";
        const string prefix = "tyl_abcdefgh";

        _userIdentifierProvider.UserId.Returns(userId);
        _hasher.GenerateRawToken().Returns(rawToken);
        _hasher.Hash(rawToken).Returns(hash);
        _hasher.GetVisiblePrefix(rawToken).Returns(prefix);

        var result = await _handler.Handle(
            new CreatePersonalAccessTokenCommand("Home Assistant"),
            CancellationToken.None
        );

        result.IsSuccess.Should().BeTrue();
        result.Value.Token.Should().Be(rawToken);
        result.Value.Name.Should().Be("Home Assistant");
        result.Value.TokenPrefix.Should().Be(prefix);

        await _repository
            .Received(1)
            .AddAsync(
                Arg.Is<PersonalAccessToken>(token =>
                    token.UserId == userId
                    && token.TokenHash == hash
                    && token.TokenPrefix == prefix
                    && token.Name == "Home Assistant"
                ),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task Handle_WithEmptyName_ReturnsFailure()
    {
        var userId = UserId.NewId();
        _userIdentifierProvider.UserId.Returns(userId);
        _hasher.GenerateRawToken().Returns("tyl_abcdefghijklmnopqrstuvwxyz012345");
        _hasher.Hash(Arg.Any<string>()).Returns("hash");
        _hasher.GetVisiblePrefix(Arg.Any<string>()).Returns("tyl_abcdefgh");

        var result = await _handler.Handle(
            new CreatePersonalAccessTokenCommand("  "),
            CancellationToken.None
        );

        result.IsFailure.Should().BeTrue();
        await _repository
            .DidNotReceive()
            .AddAsync(Arg.Any<PersonalAccessToken>(), Arg.Any<CancellationToken>());
    }
}
