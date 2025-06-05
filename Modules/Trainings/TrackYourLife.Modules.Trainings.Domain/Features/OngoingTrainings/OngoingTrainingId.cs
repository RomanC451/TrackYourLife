using System.ComponentModel;
using System.Text.Json.Serialization;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;

[JsonConverter(typeof(StronglyTypedGuidJsonConverter<OngoingTrainingId>))]
[TypeConverter(typeof(StronglyTypedGuidTypeConverter<OngoingTrainingId>))]
public sealed record OngoingTrainingId : StronglyTypedGuid<OngoingTrainingId>
{
    public OngoingTrainingId() { }

    public OngoingTrainingId(Guid Value)
        : base(Value) { }

    public static bool TryParse(string? input, out OngoingTrainingId? output)
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
