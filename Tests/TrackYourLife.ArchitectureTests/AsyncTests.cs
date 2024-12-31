using System.Reflection;
using FluentAssertions;
using MassTransit;
using MediatR;
using NetArchTest.Rules;
using Quartz;
using TrackYourLife.Modules.Nutrition.Application.Core.Abstraction.Messaging;
using TrackYourLife.SharedLib.Application.Behaviors;

namespace TrackYourLife.ArchitectureTests;

public class AsyncTests : BaseArchitectureTest
{
    private static IEnumerable<Type> AllTypes =>
        Types
            .InAssemblies(AllAssemblies)
            .That()
            .AreClasses()
            .And()
            .DoNotImplementInterface(typeof(ICommandHandler<>))
            .And()
            .DoNotImplementInterface(typeof(ICommandHandler<,>))
            .And()
            .DoNotImplementInterface(typeof(IQueryHandler<,>))
            .And()
            .DoNotImplementInterface(typeof(INotificationHandler<>))
            .And()
            .DoNotImplementInterface(typeof(IPipelineBehavior<,>))
            .And()
            .DoNotImplementInterface(typeof(IJob))
            .And()
            .DoNotImplementInterface(typeof(IConsumer<>))
            .And()
            .DoNotInherit(typeof(GenericUnitOfWorkBehavior<,,>))
            .GetTypes();

    [Fact]
    public void AllMethodsReturningTask_ShouldHaveAsyncSuffix() =>
        CustomTest(
            AllTypes,
            (Type type) =>
            {
                var methods = type.GetMethods(
                        BindingFlags.Public
                            | BindingFlags.NonPublic
                            | BindingFlags.Instance
                            | BindingFlags.Static
                            | BindingFlags.DeclaredOnly
                    )
                    .Where(m =>
                        m.ReturnType == typeof(Task)
                        || (
                            m.ReturnType.IsGenericType
                            && m.ReturnType.GetGenericTypeDefinition() == typeof(Task<>)
                        )
                    )
                    .ToList();

                if (type.Name.Contains("ResultExtensions"))
                {
                    // Your code here
                    Console.WriteLine("NutritionUnitOfWorkBehavior");
                }

                foreach (var method in methods)
                {
                    if (!method.Name.EndsWith("Async"))
                    {
                        return false;
                    }
                }
                return true;
            }
        );
}
