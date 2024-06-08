using System.Text.Json.Serialization;

namespace TrackYourLife.Common.Domain.Shared;

[JsonConverter(typeof(StronglyTypedGuidJsonConverter<IStronglyTypedGuid>))]
public interface IStronglyTypedGuid
{
    Guid Value { get; }
}
