using TrackYourLife.Modules.Payments.Infrastructure.Options;
using TrackYourLife.Modules.Payments.Infrastructure.Services;

namespace TrackYourLife.Modules.Payments.Infrastructure.UnitTests.Services;

public sealed class StripeServiceTests
{
    private static StripeService CreateService(
        string? proPriceIdMonthly = "price_monthly_configured",
        string? proPriceIdYearly = "price_yearly_configured"
    )
    {
        var options = Microsoft.Extensions.Options.Options.Create(new StripeOptions
        {
            SecretKey = "sk_test_unit",
            WebhookSecret = "whsec_unit",
            ProPriceIdMonthly = proPriceIdMonthly ?? "",
            ProPriceIdYearly = proPriceIdYearly ?? "",
        });
        return new StripeService(options);
    }

    [Fact]
    public void ResolvePriceId_WhenNull_ReturnsProPriceIdMonthly()
    {
        var service = CreateService("price_m", "price_y");

        var result = service.ResolvePriceId(null);

        result.Should().Be("price_m");
    }

    [Fact]
    public void ResolvePriceId_WhenEmpty_ReturnsProPriceIdMonthly()
    {
        var service = CreateService("price_m", "price_y");

        var result = service.ResolvePriceId("");

        result.Should().Be("price_m");
    }

    [Fact]
    public void ResolvePriceId_WhenWhitespace_ReturnsProPriceIdMonthly()
    {
        var service = CreateService("price_m", "price_y");

        var result = service.ResolvePriceId("   ");

        result.Should().Be("price_m");
    }

    [Fact]
    public void ResolvePriceId_WhenPriceMonthly_ReturnsProPriceIdMonthly()
    {
        var service = CreateService("price_m", "price_y");

        var result = service.ResolvePriceId("price_monthly");

        result.Should().Be("price_m");
    }

    [Fact]
    public void ResolvePriceId_WhenPriceMonthlyCaseInsensitive_ReturnsProPriceIdMonthly()
    {
        var service = CreateService("price_m", "price_y");

        var result = service.ResolvePriceId("PRICE_MONTHLY");

        result.Should().Be("price_m");
    }

    [Fact]
    public void ResolvePriceId_WhenPriceYearly_ReturnsProPriceIdYearly()
    {
        var service = CreateService("price_m", "price_y");

        var result = service.ResolvePriceId("price_yearly");

        result.Should().Be("price_y");
    }

    [Fact]
    public void ResolvePriceId_WhenActualStripePriceId_ReturnsAsIs()
    {
        var service = CreateService("price_m", "price_y");

        var result = service.ResolvePriceId("price_1ABC123");

        result.Should().Be("price_1ABC123");
    }

    [Fact]
    public void NormalizePeriodEnd_WhenNull_ReturnsNull()
    {
        var result = StripeService.NormalizePeriodEnd(null);
        result.Should().BeNull();
    }

    [Theory]
    [InlineData(1970, 1, 1)]
    [InlineData(1979, 12, 31)]
    public void NormalizePeriodEnd_WhenBefore1980_ReturnsNull(int year, int month, int day)
    {
        var value = new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Utc);

        var result = StripeService.NormalizePeriodEnd(value);

        result.Should().BeNull();
    }

    [Fact]
    public void NormalizePeriodEnd_WhenUtc1980OrLater_ReturnsSameUtc()
    {
        var value = new DateTime(2000, 6, 15, 12, 0, 0, DateTimeKind.Utc);

        var result = StripeService.NormalizePeriodEnd(value);

        result.Should().Be(value);
    }

    [Fact]
    public void NormalizePeriodEnd_WhenUnspecifiedKind_ConvertsToUtc()
    {
        var value = new DateTime(2000, 6, 15, 12, 0, 0, DateTimeKind.Unspecified);

        var result = StripeService.NormalizePeriodEnd(value);

        result.Should().NotBeNull();
        result!.Value.Kind.Should().Be(DateTimeKind.Utc);
        result.Value.Year.Should().Be(2000);
    }

    [Fact]
    public void GetCurrentPeriodEndFromFirstItemInPayload_WhenValidJson_ReturnsUnixTimestampAsUtc()
    {
        // Unix timestamp for 2000-06-15 12:00:00 UTC
        var json = """
            {"data":{"object":{"items":{"data":[{"current_period_end":961070400}]}}}}
            """;

        var result = StripeService.GetCurrentPeriodEndFromFirstItemInPayload(json);

        result.Should().NotBeNull();
        result!.Value.Year.Should().Be(2000);
        result.Value.Month.Should().Be(6);
        result.Value.Day.Should().Be(15);
    }

    [Fact]
    public void GetCurrentPeriodEndFromFirstItemInPayload_WhenEmptyDataArray_ReturnsNull()
    {
        var json = """{"data":{"object":{"items":{"data":[]}}}}""";

        var result = StripeService.GetCurrentPeriodEndFromFirstItemInPayload(json);

        result.Should().BeNull();
    }

    [Fact]
    public void GetCurrentPeriodEndFromFirstItemInPayload_WhenMissingPath_ReturnsNull()
    {
        var json = """{"data":{}}""";

        var result = StripeService.GetCurrentPeriodEndFromFirstItemInPayload(json);

        result.Should().BeNull();
    }

    [Fact]
    public void GetCurrentPeriodEndFromFirstItemInPayload_WhenInvalidJson_ReturnsNull()
    {
        var result = StripeService.GetCurrentPeriodEndFromFirstItemInPayload("not json {");

        result.Should().BeNull();
    }

    [Fact]
    public void GetCurrentPeriodEndFromFirstItemInPayload_WhenCurrentPeriodEndZero_ReturnsNull()
    {
        var json = """{"data":{"object":{"items":{"data":[{"current_period_end":0}]}}}}""";

        var result = StripeService.GetCurrentPeriodEndFromFirstItemInPayload(json);

        result.Should().BeNull();
    }

    [Fact]
    public void GetCurrentPeriodEndFromFirstItemInPayload_WhenCurrentPeriodEndNotNumber_ReturnsNull()
    {
        var json = """{"data":{"object":{"items":{"data":[{"current_period_end":"not_a_number"}]}}}}""";

        var result = StripeService.GetCurrentPeriodEndFromFirstItemInPayload(json);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetOrCreateCustomerIdAsync_WhenExistingCustomerIdProvided_ReturnsIt()
    {
        var service = CreateService();
        const string existingId = "cus_existing123";

        var result = await service.GetOrCreateCustomerIdAsync(
            existingId,
            "user@example.com",
            "User Name",
            CancellationToken.None
        );

        result.Should().Be(existingId);
    }

    [Fact]
    public async Task CustomerHasActiveSubscriptionForPriceAsync_WhenResolvedPriceIdEmpty_ReturnsFalse()
    {
        var service = CreateService(proPriceIdMonthly: "", proPriceIdYearly: "");

        var result = await service.CustomerHasActiveSubscriptionForPriceAsync(
            "cus_any",
            "price_monthly",
            CancellationToken.None
        );

        result.Should().BeFalse();
    }

    [Fact]
    public async Task CreateCheckoutSessionAsync_WhenResolvedPriceIdEmpty_Throws()
    {
        var service = CreateService(proPriceIdMonthly: "", proPriceIdYearly: "");

        var act = () => service.CreateCheckoutSessionAsync(
            null,
            "user@example.com",
            "user-id",
            "price_monthly",
            "https://success",
            "https://cancel",
            CancellationToken.None
        );

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*No Stripe Price ID configured*");
    }

    [Fact]
    public void TryParseWebhookEvent_WhenSignatureInvalid_ReturnsError()
    {
        var service = CreateService();
        const string payload = """{"id":"evt_1","type":"checkout.session.completed"}""";
        const string invalidSignature = "t=1234567890,v1=invalid_signature";

        var (payloadResult, error) = service.TryParseWebhookEvent(payload, invalidSignature);

        payloadResult.Should().BeNull();
        error.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void TryParseWebhookEvent_WhenPayloadEmpty_ReturnsError()
    {
        var service = CreateService();

        var (payloadResult, error) = service.TryParseWebhookEvent("", "t=1,v1=abc");

        payloadResult.Should().BeNull();
        error.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task HasProcessedEventAsync_BeforeMark_ReturnsFalse()
    {
        var service = CreateService();
        var eventId = "evt_" + Guid.NewGuid().ToString("N");

        var result = await service.HasProcessedEventAsync(eventId);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task HasProcessedEventAsync_AfterMark_ReturnsTrue()
    {
        var service = CreateService();
        var eventId = "evt_" + Guid.NewGuid().ToString("N");

        await service.MarkEventProcessedAsync(eventId);
        var result = await service.HasProcessedEventAsync(eventId);

        result.Should().BeTrue();
    }
}
