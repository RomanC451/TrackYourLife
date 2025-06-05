using System.ComponentModel;
using System.Text.Json.Serialization;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Trainings.Domain.Features.Trainings;

[JsonConverter(typeof(StronglyTypedGuidJsonConverter<TrainingId>))]
[TypeConverter(typeof(StronglyTypedGuidTypeConverter<TrainingId>))]
public sealed record TrainingId : StronglyTypedGuid<TrainingId>
{
    public TrainingId() { }

    public TrainingId(Guid Value)
        : base(Value) { }

    public static bool TryParse(string? input, out TrainingId? output)
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
};
