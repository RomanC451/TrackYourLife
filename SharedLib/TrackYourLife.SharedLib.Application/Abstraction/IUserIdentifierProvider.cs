using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.SharedLib.Application.Abstraction;

public interface IUserIdentifierProvider
{
    UserId UserId { get; }
}
