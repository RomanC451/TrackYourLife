using TrackYourLife.SharedLib.FunctionalTests;

namespace TrackYourLife.Modules.Trainings.FunctionalTests;

[CollectionDefinition("Trainings Integration Tests")]
public class TrainingsFunctionalTestCollection(TrainingsFunctionalTestWebAppFactory factory)
    : FunctionalTestCollection(factory),
        ICollectionFixture<TrainingsFunctionalTestWebAppFactory>;
