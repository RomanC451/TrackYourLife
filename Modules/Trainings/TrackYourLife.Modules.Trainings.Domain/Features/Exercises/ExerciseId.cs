using System.ComponentModel;
using System.Text.Json.Serialization;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Trainings.Domain.Features.Exercises;

[JsonConverter(typeof(StronglyTypedGuidJsonConverter<ExerciseId>))]
[TypeConverter(typeof(StronglyTypedGuidTypeConverter<ExerciseId>))]
public sealed record ExerciseId : StronglyTypedGuid<ExerciseId>
{
    public ExerciseId() { }

    public ExerciseId(Guid Value)
        : base(Value) { }

    public static bool TryParse(string? input, out ExerciseId? output)
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
