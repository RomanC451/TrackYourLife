using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace TrackYourLife.SharedLib.Domain.Ids;

[JsonConverter(typeof(StronglyTypedGuidJsonConverter<IStronglyTypedGuid>))]
public interface IStronglyTypedGuid
{
    Guid Value { get; }
}
