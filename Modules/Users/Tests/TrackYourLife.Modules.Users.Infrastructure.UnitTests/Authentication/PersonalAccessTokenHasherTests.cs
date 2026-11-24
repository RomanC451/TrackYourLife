using FluentAssertions;
using TrackYourLife.Modules.Users.Domain.Features.PersonalAccessTokens;
using TrackYourLife.Modules.Users.Infrastructure.Authentication;
using Xunit;

namespace TrackYourLife.Modules.Users.Infrastructure.UnitTests.Authentication;

public class PersonalAccessTokenHasherTests
{
    private readonly PersonalAccessTokenHasher _hasher = new();

    [Fact]
    public void GenerateRawToken_StartsWithPrefixAndIsUnique()
    {
        var first = _hasher.GenerateRawToken();
        var second = _hasher.GenerateRawToken();

        first.Should().StartWith(PersonalAccessToken.TokenPrefixMarker);
        second.Should().StartWith(PersonalAccessToken.TokenPrefixMarker);
        first.Should().NotBe(second);
    }

    [Fact]
    public void Hash_IsDeterministicAndDifferentFromRawToken()
    {
        var rawToken = _hasher.GenerateRawToken();

        var first = _hasher.Hash(rawToken);
        var second = _hasher.Hash(rawToken);

        first.Should().Be(second);
        first.Should().NotBe(rawToken);
        first.Should().HaveLength(64);
    }

    [Fact]
    public void GetVisiblePrefix_ReturnsMarkerPlusEightCharacters()
    {
        var rawToken = "tyl_abcdefghijklmnop";

        var prefix = _hasher.GetVisiblePrefix(rawToken);

        prefix.Should().Be("tyl_abcdefgh");
    }
}
