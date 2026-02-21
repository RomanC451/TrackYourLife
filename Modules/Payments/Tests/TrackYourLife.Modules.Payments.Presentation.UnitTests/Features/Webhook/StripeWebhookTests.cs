using System.Text;
using MediatR;
using Microsoft.AspNetCore.Http;
using TrackYourLife.Modules.Payments.Application.Features.Webhooks.HandleStripeWebhook;
using TrackYourLife.Modules.Payments.Presentation.Features.Webhook;
using TrackYourLife.SharedLib.Domain.Results;
using TrackYourLife.SharedLib.Presentation.UnitTests;

namespace TrackYourLife.Modules.Payments.Presentation.UnitTests.Features.Webhook;

public sealed class StripeWebhookTests
{
    private readonly ISender _sender;
    private readonly StripeWebhook _endpoint;

    public StripeWebhookTests()
    {
        _sender = Substitute.For<ISender>();
        _endpoint = new StripeWebhook(_sender);
    }

    [Fact]
    public async Task ExecuteAsync_WhenBodyIsEmpty_ShouldReturnBadRequest()
    {
        SetHttpContext(body: "", signature: "sig");

        var result = await _endpoint.ExecuteAsync(CancellationToken.None);

        result.Should().NotBeNull();
        result.Should().BeOfType<Microsoft.AspNetCore.Http.HttpResults.BadRequest>();
        await _sender
            .DidNotReceive()
            .Send(Arg.Any<HandleStripeWebhookCommand>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_WhenSignatureHeaderMissing_ShouldReturnBadRequest()
    {
        SetHttpContext(body: "{}", signature: null);

        var result = await _endpoint.ExecuteAsync(CancellationToken.None);

        result.Should().NotBeNull();
        result.Should().BeOfType<Microsoft.AspNetCore.Http.HttpResults.BadRequest>();
        await _sender
            .DidNotReceive()
            .Send(Arg.Any<HandleStripeWebhookCommand>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_WhenBodyAndSignaturePresent_ShouldSendCommandAndReturnOk()
    {
        const string json = "{\"type\":\"checkout.session.completed\"}";
        const string signature = "whsec_abc";
        SetHttpContext(body: json, signature: signature);

        _sender
            .Send(Arg.Any<HandleStripeWebhookCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        var result = await _endpoint.ExecuteAsync(CancellationToken.None);

        result.Should().NotBeNull();
        result.Should().BeOfType<Microsoft.AspNetCore.Http.HttpResults.Ok>();
        await _sender
            .Received(1)
            .Send(
                Arg.Is<HandleStripeWebhookCommand>(c =>
                    c.JsonPayload == json && c.SignatureHeader == signature
                ),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task ExecuteAsync_WhenHandlerFails_ShouldReturnBadRequest()
    {
        SetHttpContext(body: "{}", signature: "sig");
        _sender
            .Send(Arg.Any<HandleStripeWebhookCommand>(), Arg.Any<CancellationToken>())
            .Returns(
                Result.Failure(
                    new SharedLib.Domain.Errors.Error("Webhook.Invalid", "Invalid signature")
                )
            );

        var result = await _endpoint.ExecuteAsync(CancellationToken.None);

        result.Should().NotBeNull();
        result.Should().BeOfType<Microsoft.AspNetCore.Http.HttpResults.BadRequest>();
    }

    private void SetHttpContext(string body, string? signature)
    {
        var context = new DefaultHttpContext();
        context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(body));
        if (signature is not null)
        {
            context.Request.Headers["Stripe-Signature"] = signature;
        }

        _endpoint.SetHttpContext(context);
    }
}
