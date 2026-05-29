using MassTransit;
using TrackYourLife.Modules.Users.Application.Core.Abstraction.Services;
using TrackYourLife.Modules.Users.Application.Features.Users.Consumers;
using TrackYourLife.SharedLib.Contracts.Integration.Users;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Users.Application.UnitTests.Features.Users.Consumers;

public sealed class SendYoutubeSettingsPasswordResetEmailConsumerTests
{
    private readonly IEmailService _emailService;
    private readonly SendYoutubeSettingsPasswordResetEmailConsumer _consumer;

    public SendYoutubeSettingsPasswordResetEmailConsumerTests()
    {
        _emailService = Substitute.For<IEmailService>();
        _consumer = new SendYoutubeSettingsPasswordResetEmailConsumer(_emailService);
    }

    [Fact]
    public async Task Consume_WhenEmailSucceeds_ShouldRespondWithNoErrors()
    {
        _emailService
            .SendYoutubeSettingsPasswordResetEmail("user@example.com", "NewPass1!")
            .Returns(Result.Success());

        var context = Substitute.For<ConsumeContext<SendYoutubeSettingsPasswordResetEmailRequest>>();
        context.Message.Returns(
            new SendYoutubeSettingsPasswordResetEmailRequest("user@example.com", "NewPass1!")
        );

        SendYoutubeSettingsPasswordResetEmailResponse? response = null;
        await context.RespondAsync(
            Arg.Do<SendYoutubeSettingsPasswordResetEmailResponse>(r => response = r)
        );

        await _consumer.Consume(context);

        response.Should().NotBeNull();
        response!.Errors.Should().BeEmpty();
        _emailService
            .Received(1)
            .SendYoutubeSettingsPasswordResetEmail("user@example.com", "NewPass1!");
    }
}
