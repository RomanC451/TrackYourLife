using System.Net;
using System.Net.Http.Json;
using System.Reflection;
using System.Text;
using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.Payments.FunctionalTests;
using TrackYourLife.Modules.Users.Domain.Features.Users;
using TrackYourLife.SharedLib.FunctionalTests;
using TrackYourLife.SharedLib.FunctionalTests.Utils;

namespace TrackYourLife.Modules.Payments.FunctionalTests.Features;

[Collection("Payments Integration Tests")]
public class PaymentsTests(PaymentsFunctionalTestWebAppFactory factory)
    : PaymentsBaseIntegrationTest(factory)
{
    [Fact]
    public async Task GetBillingPortalUrl_WithoutStripeCustomer_ShouldReturnNotFound()
    {
        await ClearCurrentUserStripeCustomerIdAsync();

        var response = await _client.GetAsync(
            "/api/payments/portal?ReturnUrl=https://app.example.com/billing"
        );

        await response.ShouldHaveStatusCode(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetBillingPortalUrl_WithStripeCustomer_ShouldReturnPortalUrl()
    {
        await SetCurrentUserStripeCustomerIdAsync(MockStripeService.FakeCustomerId);

        var response = await _client.GetAsync(
            "/api/payments/portal?ReturnUrl=https://app.example.com/billing"
        );

        var url = await response.ShouldHaveStatusCodeAndContent<string>(HttpStatusCode.OK);
        url.Should().Be(MockStripeService.FakePortalUrl);
    }

    [Fact]
    public async Task CreateCheckoutSession_WithValidRequest_ShouldReturnCheckoutUrl()
    {
        var request = new
        {
            SuccessUrl = "https://app.example.com/success",
            CancelUrl = "https://app.example.com/cancel",
            PriceId = "price_monthly",
        };

        var response = await _client.PostAsJsonAsync(
            "/api/payments/checkout-session",
            request
        );

        var url = await response.ShouldHaveStatusCodeAndContent<string>(HttpStatusCode.OK);
        url.Should().Be(MockStripeService.FakeCheckoutUrl);
    }

    [Fact]
    public async Task CreateCheckoutSession_Unauthorized_ShouldReturn401()
    {
        var client = CreateUnauthorizedClient();
        var request = new
        {
            SuccessUrl = "https://app.example.com/success",
            CancelUrl = "https://app.example.com/cancel",
            PriceId = "price_monthly",
        };

        var response = await client.PostAsJsonAsync(
            "/api/payments/checkout-session",
            request
        );

        await response.ShouldHaveStatusCode(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task StripeWebhook_WithValidPayload_ShouldReturnOk()
    {
        var client = CreateUnauthorizedClient();
        // Use event type that does not require user lookup so the pipeline succeeds with the mock.
        var body = """{"type":"invoice.payment_failed"}""";
        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/payments/webhook")
        {
            Content = new StringContent(body, Encoding.UTF8, "application/json"),
        };
        request.Headers.TryAddWithoutValidation("Stripe-Signature", "whsec_test_signature");

        var response = await client.SendAsync(request);

        await response.ShouldHaveStatusCode(HttpStatusCode.OK);
    }

    [Fact]
    public async Task StripeWebhook_WithEmptyBody_ShouldReturnBadRequest()
    {
        var client = CreateUnauthorizedClient();
        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/payments/webhook")
        {
            Content = new StringContent("", Encoding.UTF8, "application/json"),
        };
        request.Headers.TryAddWithoutValidation("Stripe-Signature", "whsec_test");

        var response = await client.SendAsync(request);

        await response.ShouldHaveStatusCode(HttpStatusCode.BadRequest);
    }

    private async Task ClearCurrentUserStripeCustomerIdAsync()
    {
        var user = await _usersWriteDbContext.Users.FirstAsync(u => u.Id == _user.Id);
        var prop = typeof(User).GetProperty("StripeCustomerId", BindingFlags.Public | BindingFlags.Instance)!;
        prop.SetValue(user, null);
        await _usersWriteDbContext.SaveChangesAsync();
    }

    private async Task SetCurrentUserStripeCustomerIdAsync(string stripeCustomerId)
    {
        var trackedUser = await _usersWriteDbContext.Users.FirstAsync(u => u.Id == _user.Id);
        trackedUser.SetStripeCustomerId(stripeCustomerId);
        await _usersWriteDbContext.SaveChangesAsync();
    }
}
