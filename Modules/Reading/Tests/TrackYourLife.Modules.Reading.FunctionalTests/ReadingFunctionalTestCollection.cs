using TrackYourLife.SharedLib.FunctionalTests;

namespace TrackYourLife.Modules.Reading.FunctionalTests;

[CollectionDefinition("Reading Integration Tests")]
public class ReadingFunctionalTestCollection(ReadingFunctionalTestWebAppFactory factory)
    : FunctionalTestCollection(factory),
        ICollectionFixture<ReadingFunctionalTestWebAppFactory>;
