//using NetArchTest.Rules;
//using TrackYourLife.Common.Presentation.Abstractions;

//namespace TrackYourLife.ArchitectureTests.Presentation;

//public class ControllerTests : BaseArchitectureTest
//{
//    private static IEnumerable<Type> CommandTypes =>
//        Types
//            .InAssemblies(PresentationAssemblies.Assemblies)
//            .That()
//            .Inherit(typeof(ApiController))
//            .GetTypes();

//    [Fact]
//    public void Commands_ShouldBeInternal() => ShouldBeInternal(CommandTypes);

//    [Fact]
//    public void Commands_ShouldBeSealed() => ShouldBeSealed(CommandTypes);

//    [Fact]
//    public void Commands_ShouldHaveCommandPostfix() =>
//        ShouldHavePostfix(CommandTypes, "Controller");
//}
