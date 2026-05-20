using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeCategories;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.FunctionalTests;

namespace TrackYourLife.Modules.Youtube.FunctionalTests.Features;

[Collection("Youtube Integration Tests")]
public class UserRegistrationYoutubeCategoriesTests(YoutubeFunctionalTestWebAppFactory factory)
    : YoutubeBaseIntegrationTest(factory)
{
    [Fact]
    public async Task RegisterUser_ShouldSeedDefaultYoutubeCategoriesViaIntegrationEvent()
    {
        var client = CreateUnauthorizedClient();
        var user = await client.RegisterAndLoginNewUserAsync();

        await WaitForOutboxEventsToBeHandledAsync(_usersWriteDbContext.OutboxMessages);

        var categories = await WaitForDefaultYoutubeCategoriesAsync(user.Id);

        categories.Should().HaveCount(2);
        categories[0].Name.Should().Be(YoutubeCategoryDefaults.EntertainmentName);
        categories[0]
            .MaxVideosPerDay.Should()
            .Be(YoutubeCategoryDefaults.EntertainmentMaxVideosPerDay);
        categories[0].DisplayOrder.Should().Be(YoutubeCategoryDefaults.EntertainmentDisplayOrder);
        categories[1].Name.Should().Be(YoutubeCategoryDefaults.EducationalName);
        categories[1]
            .MaxVideosPerDay.Should()
            .Be(YoutubeCategoryDefaults.EducationalMaxVideosPerDay);
        categories[1].DisplayOrder.Should().Be(YoutubeCategoryDefaults.EducationalDisplayOrder);
    }

    private async Task<List<YoutubeCategory>> WaitForDefaultYoutubeCategoriesAsync(UserId userId)
    {
        var timeout = TimeSpan.FromSeconds(20);
        var startTime = DateTime.UtcNow;

        while (DateTime.UtcNow - startTime < timeout)
        {
            var categories = await _youtubeWriteDbContext
                .YoutubeCategories.AsNoTracking()
                .Where(c => c.UserId == userId)
                .OrderBy(c => c.DisplayOrder)
                .ToListAsync();

            if (categories.Count >= 2)
            {
                return categories;
            }

            await Task.Delay(100);
        }

        throw new TimeoutException(
            $"Default YouTube categories were not seeded for user {userId} within {timeout.TotalSeconds}s."
        );
    }
}
