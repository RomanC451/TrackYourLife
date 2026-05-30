using MassTransit;
using MassTransit.Testing;
using TrackYourLife.Modules.Payments.Application.Abstraction;
using TrackYourLife.Modules.Payments.Application.Services;
using TrackYourLife.SharedLib.Contracts.Integration.Users;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Payments.Application.UnitTests.Services;

public sealed class StripeCustomerIdResolverTests : IAsyncLifetime
{
    private InMemoryTestHarness _harness = null!;
    private IBus _bus = null!;
    private readonly IStripeService _stripeService;
    private StripeCustomerIdResolver _resolver = null!;
    private SetStripeCustomerIdRequest? _persistRequest;

    public StripeCustomerIdResolverTests()
    {
        _stripeService = Substitute.For<IStripeService>();
    }

    public async Task InitializeAsync()
    {
        _harness = new InMemoryTestHarness();
        _harness.OnConfigureBus += configurator =>
        {
            configurator.ReceiveEndpoint(
                "set-stripe-customer-id",
                e =>
                {
                    e.Handler<SetStripeCustomerIdRequest>(async context =>
                    {
                        _persistRequest = context.Message;
                        await context.RespondAsync(new SetStripeCustomerIdResponse([]));
                    });
                }
            );
        };
        await _harness.Start();
        _bus = _harness.Bus;
        _resolver = new StripeCustomerIdResolver(_stripeService, _bus);
    }

    public async Task DisposeAsync() => await _harness.Stop();

    [Fact]
    public async Task ResolveAndPersistAsync_WhenExistingCustomerIsValid_DoesNotPersist()
    {
        var userId = UserId.NewId();
        _stripeService
            .GetOrCreateCustomerIdAsync("cus_valid", "user@example.com", "user@example.com", Arg.Any<CancellationToken>())
            .Returns("cus_valid");

        var result = await _resolver.ResolveAndPersistAsync(
            userId,
            "cus_valid",
            "user@example.com",
            CancellationToken.None
        );

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("cus_valid");
        _persistRequest.Should().BeNull();
    }

    [Fact]
    public async Task ResolveAndPersistAsync_WhenCustomerIsReplaced_PersistsNewId()
    {
        var userId = UserId.NewId();
        _stripeService
            .GetOrCreateCustomerIdAsync("cus_stale", "user@example.com", "user@example.com", Arg.Any<CancellationToken>())
            .Returns("cus_new");

        var result = await _resolver.ResolveAndPersistAsync(
            userId,
            "cus_stale",
            "user@example.com",
            CancellationToken.None
        );

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("cus_new");
        _persistRequest.Should().NotBeNull();
        _persistRequest!.UserId.Should().Be(userId);
        _persistRequest.StripeCustomerId.Should().Be("cus_new");
    }

    [Fact]
    public async Task ResolveAndPersistAsync_WhenNoExistingCustomer_PersistsCreatedId()
    {
        var userId = UserId.NewId();
        _stripeService
            .GetOrCreateCustomerIdAsync(null, "user@example.com", "user@example.com", Arg.Any<CancellationToken>())
            .Returns("cus_created");

        var result = await _resolver.ResolveAndPersistAsync(
            userId,
            null,
            "user@example.com",
            CancellationToken.None
        );

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("cus_created");
        _persistRequest.Should().NotBeNull();
        _persistRequest!.StripeCustomerId.Should().Be("cus_created");
    }

    [Fact]
    public async Task ResolveAndPersistAsync_WhenPersistFails_ReturnsFailure()
    {
        var userId = UserId.NewId();
        await _harness.Stop();
        _harness = new InMemoryTestHarness();
        _harness.OnConfigureBus += configurator =>
        {
            configurator.ReceiveEndpoint(
                "set-stripe-customer-id-fail",
                e =>
                {
                    e.Handler<SetStripeCustomerIdRequest>(async context =>
                    {
                        await context.RespondAsync(
                            new SetStripeCustomerIdResponse(
                                [new SharedLib.Domain.Errors.Error("User.NotFound", "User not found.", 404)]
                            )
                        );
                    });
                }
            );
        };
        await _harness.Start();
        _resolver = new StripeCustomerIdResolver(_stripeService, _harness.Bus);

        _stripeService
            .GetOrCreateCustomerIdAsync(null, "user@example.com", "user@example.com", Arg.Any<CancellationToken>())
            .Returns("cus_created");

        var result = await _resolver.ResolveAndPersistAsync(
            userId,
            null,
            "user@example.com",
            CancellationToken.None
        );

        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("User.NotFound");
    }
}
