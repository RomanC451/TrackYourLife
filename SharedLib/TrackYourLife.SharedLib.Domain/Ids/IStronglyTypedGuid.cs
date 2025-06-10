using System.Text.Json.Serialization;

namespace TrackYourLife.SharedLib.Domain.Ids;

[JsonConverter(typeof(StronglyTypedGuidJsonConverter<IStronglyTypedGuid>))]
public interface IStronglyTypedGuid
{
    Guid Value { get; }
}
