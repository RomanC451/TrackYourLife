using MassTransit;
using TrackYourLife.Modules.Users.Domain.Features.Users;
using TrackYourLife.SharedLib.Contracts.Integration.Users;
using TrackYourLife.SharedLib.Domain.Errors;

namespace TrackYourLife.Modules.Users.Application.Features.Users.Consumers;

public sealed class GetUserAccountByIdConsumer(IUserQuery userQuery)
    : IConsumer<GetUserAccountByIdRequest>
{
    public async Task Consume(ConsumeContext<GetUserAccountByIdRequest> context)
    {
        var user = await userQuery.GetByIdAsync(context.Message.UserId, context.CancellationToken);

        if (user is null)
        {
            await context.RespondAsync(
                new GetUserAccountByIdResponse(null, [UserErrors.NotFound(context.Message.UserId)])
            );
            return;
        }

        var data = new UserAccountDto(user.Email, user.VerifiedOnUtc);
        await context.RespondAsync(new GetUserAccountByIdResponse(data, []));
    }
}
