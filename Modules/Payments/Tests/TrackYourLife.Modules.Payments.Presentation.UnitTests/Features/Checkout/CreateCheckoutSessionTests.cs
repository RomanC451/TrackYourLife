using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using TrackYourLife.Modules.Payments.Application.Features.Checkout.CreateCheckoutSession;
using TrackYourLife.Modules.Payments.Presentation.Features.Checkout;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Payments.Presentation.UnitTests.Features.Checkout;

public sealed class CreateCheckoutSessionTests
{
    private readonly ISender _sender;
    private readonly CreateCheckoutSession _endpoint;

    public CreateCheckoutSessionTests()
    {
        _sender = Substitute.For<ISender>();
        _endpoint = new CreateCheckoutSession(_sender);
    }

    [Fact]
    public async Task ExecuteAsync_WhenCommandSucceeds_ShouldReturnOkWithCheckoutUrl()
    {
        const string checkoutUrl = "https://checkout.stripe.com/session/abc";
        var request = new CreateCheckoutSessionRequest(
            "https://app/success",
            "https://app/cancel",
            "price_123"
        );

        _sender
            .Send(Arg.Any<CreateCheckoutSessionCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(checkoutUrl));

        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        result.Should().NotBeNull();
        var okResult = result.Should().BeOfType<Ok<string>>().Subject;
        okResult.Value.Should().Be(checkoutUrl);
        await _sender
            .Received(1)
            .Send(
                Arg.Is<CreateCheckoutSessionCommand>(c =>
                    c.SuccessUrl == request.SuccessUrl
                    && c.CancelUrl == request.CancelUrl
                    && c.PriceId == request.PriceId
                ),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task ExecuteAsync_WhenCommandFails_ShouldReturnBadRequest()
    {
        var request = new CreateCheckoutSessionRequest(
            "https://app/success",
            "https://app/cancel",
            "price_123"
        );
        _sender
            .Send(Arg.Any<CreateCheckoutSessionCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure<string>(new SharedLib.Domain.Errors.Error("Checkout.AlreadySubscribed", "Already subscribed.", 400)));

        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        result.Should().NotBeNull();
        result.Should().BeOfType<BadRequest<Microsoft.AspNetCore.Mvc.ProblemDetails>>();
    }
}
