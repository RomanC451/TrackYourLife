
using TrackYourLife.Common.Domain.Users;

namespace TrackYourLife.Common.Application.Core.Abstractions.Authentication;

public interface IUserIdentifierProvider
{
    UserId UserId { get; }
}
