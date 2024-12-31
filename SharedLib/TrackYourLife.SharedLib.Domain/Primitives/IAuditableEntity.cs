namespace TrackYourLife.SharedLib.Domain.Primitives;

public interface IAuditableEntity
{
    DateTime CreatedOnUtc { get; }

    DateTime? ModifiedOnUtc { get; }
}
