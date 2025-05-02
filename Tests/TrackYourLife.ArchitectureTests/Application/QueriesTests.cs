using System.Reflection;
using FluentValidation;
using MediatR;
using NetArchTest.Rules;

namespace TrackYourLife.ArchitectureTests.Application;

public class QueriesTests : BaseArchitectureTest
{
    private static IEnumerable<Type> QueryTypes =>
        Types
            .InAssemblies(ApplicationAssemblies.Assemblies)
            .That()
            .ImplementInterface(typeof(IRequest<>))
            .And()
            .AreNotInterfaces()
            .And()
            .HaveNameEndingWith("Query")
            .GetTypes();

    private static IEnumerable<Type> ValidatedQueries =>
        Types
            .InAssemblies(ApplicationAssemblies.Assemblies)
            .That()
            .Inherit(typeof(AbstractValidator<>))
            .And()
            .HaveNameEndingWith("QueryValidator")
            .GetTypes()
            .Select(t => t.BaseType!.GetGenericArguments()[0]);

    private static IEnumerable<Type> Validators =>
        Types
            .InAssemblies(ApplicationAssemblies.Assemblies)
            .That()
            .Inherit(typeof(AbstractValidator<>))
            .And()
            .HaveNameEndingWith("QueryValidator")
            .GetTypes();

    [Fact]
    public void Queries_ShouldBeSealed() => ShouldBeSealed(QueryTypes);

    [Fact]
    public void Queries_ShouldHaveQueryPostfix() => ShouldHavePostfix(QueryTypes, "Query");

    [Fact]
    public void Queries_ShouldBeDefinedAsRecords() => ShouldBeDefinedAsRecords(QueryTypes);

    [Fact]
    public void Queries_ShouldHaveValidator() =>
        CustomTest(QueryTypes, (t) => ValidatedQueries.Contains(t));

    [Fact]
    public void Queries_ShouldHaveCommandProperties_ShouldHaveValidationRules() =>
        CustomTest(
            Validators,
            (t) =>
            {
                Type commandType = t.BaseType!.GetGenericArguments()[0];
                var commandProperties = commandType.GetProperties(
                    BindingFlags.Public | BindingFlags.Instance
                );

                var validator = (IValidator)Activator.CreateInstance(t)!;

                foreach (var property in commandProperties)
                {
                    // Act
                    if (!validator.CreateDescriptor().GetRulesForMember(property.Name).Any())
                    {
                        return false;
                    }
                }
                return true;
            }
        );
}
