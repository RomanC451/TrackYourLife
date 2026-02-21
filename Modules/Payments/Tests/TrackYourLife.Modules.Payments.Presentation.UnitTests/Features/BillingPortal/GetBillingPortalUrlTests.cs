using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using TrackYourLife.Modules.Payments.Application.Features.BillingPortal.GetBillingPortalUrl;
using TrackYourLife.Modules.Payments.Presentation.Features.BillingPortal;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Payments.Presentation.UnitTests.Features.BillingPortal;

public sealed class GetBillingPortalUrlTests
{
    private readonly ISender _sender;
    private readonly GetBillingPortalUrl _endpoint;

    public GetBillingPortalUrlTests()
    {
        _sender = Substitute.For<ISender>();
        _endpoint = new GetBillingPortalUrl(_sender);
    }

    [Fact]
    public async Task ExecuteAsync_WhenQuerySucceeds_ShouldReturnOkWithUrl()
    {
        const string portalUrl = "https://billing.stripe.com/session/xyz";
        var request = new GetBillingPortalUrlRequest("https://app.example.com/billing");

        _sender
            .Send(Arg.Any<GetBillingPortalUrlQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(portalUrl));

        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        result.Should().NotBeNull();
        var okResult = result.Should().BeOfType<Ok<string>>().Subject;
        okResult.Value.Should().Be(portalUrl);
        await _sender
            .Received(1)
            .Send(
                Arg.Is<GetBillingPortalUrlQuery>(q => q.ReturnUrl == request.ReturnUrl),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task ExecuteAsync_WhenQueryFails_ShouldReturnProblemResult()
    {
        var request = new GetBillingPortalUrlRequest("https://app.example.com/billing");
        _sender
            .Send(Arg.Any<GetBillingPortalUrlQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure<string>(new SharedLib.Domain.Errors.Error("BillingPortal.NoStripeCustomer", "No Stripe customer.", 404)));

        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        result.Should().NotBeNull();
        result.Should().BeOfType<NotFound<Microsoft.AspNetCore.Mvc.ProblemDetails>>();
    }
}
