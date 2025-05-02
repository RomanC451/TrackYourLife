using TrackYourLife.SharedLib.FunctionalTests;

namespace TrackYourLife.Modules.Users.FunctionalTests;

[CollectionDefinition("Users Integration Tests")]
public class UsersFucntionalTestCollection(UsersFunctionalTestWebAppFactory factory)
    : FunctionalTestCollection(factory),
        ICollectionFixture<UsersFunctionalTestWebAppFactory>;
