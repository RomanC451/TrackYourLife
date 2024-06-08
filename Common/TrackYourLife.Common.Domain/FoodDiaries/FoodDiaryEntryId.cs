using System.Text.Json.Serialization;
using TrackYourLife.Common.Domain.Shared;

namespace TrackYourLife.Common.Domain.FoodDiaries;

[JsonConverter(typeof(StronglyTypedGuidJsonConverter<FoodDiaryEntryId>))]
public class FoodDiaryEntryId : StronglyTypedGuid<FoodDiaryEntryId>, IStronglyTypedGuid;

