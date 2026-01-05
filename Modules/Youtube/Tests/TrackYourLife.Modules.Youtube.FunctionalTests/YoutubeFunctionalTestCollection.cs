using TrackYourLife.SharedLib.FunctionalTests;

namespace TrackYourLife.Modules.Youtube.FunctionalTests;

[CollectionDefinition("Youtube Integration Tests")]
public class YoutubeFunctionalTestCollection(YoutubeFunctionalTestWebAppFactory factory)
    : FunctionalTestCollection(factory),
        ICollectionFixture<YoutubeFunctionalTestWebAppFactory>;
