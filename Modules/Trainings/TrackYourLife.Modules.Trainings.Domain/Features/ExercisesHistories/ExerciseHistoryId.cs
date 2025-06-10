using System.ComponentModel;
using System.Text.Json.Serialization;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories;

[JsonConverter(typeof(StronglyTypedGuidJsonConverter<ExerciseHistoryId>))]
[TypeConverter(typeof(StronglyTypedGuidTypeConverter<ExerciseHistoryId>))]
public sealed record ExerciseHistoryId : StronglyTypedGuid<ExerciseHistoryId>
{
    public ExerciseHistoryId() { }

    public ExerciseHistoryId(Guid Value)
        : base(Value) { }

    public static bool TryParse(string? input, out ExerciseHistoryId? output)
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
