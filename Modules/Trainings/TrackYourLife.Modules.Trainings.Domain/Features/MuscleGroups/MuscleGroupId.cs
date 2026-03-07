using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using NJsonSchema;
using NJsonSchema.Annotations;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Trainings.Domain.Features.MuscleGroups;

[JsonConverter(typeof(StronglyTypedGuidJsonConverter<MuscleGroupId>))]
[TypeConverter(typeof(StronglyTypedGuidTypeConverter<MuscleGroupId>))]
public sealed record MuscleGroupId : StronglyTypedGuid<MuscleGroupId>
{
    public MuscleGroupId() { }

    public MuscleGroupId(Guid Value)
        : base(Value) { }

    public static bool TryParse(string? input, out MuscleGroupId? output)
    {
        output = null;

        if (string.IsNullOrEmpty(input))
        {
            return false;
        }

        if (!Guid.TryParse(input, out var guid))
        {
            return false;
        }

        output = Create(guid);

        return true;
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}
