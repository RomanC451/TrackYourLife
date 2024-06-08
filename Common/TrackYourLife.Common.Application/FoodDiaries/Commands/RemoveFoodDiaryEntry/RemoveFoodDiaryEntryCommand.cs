using TrackYourLife.Common.Application.Core.Abstractions.Messaging;
using TrackYourLife.Common.Domain.FoodDiaries;

namespace TrackYourLife.Common.Application.FoodDiaries.Commands.RemoveFoodDiaryEntry;

public sealed record RemoveFoodDiaryEntryCommand(FoodDiaryEntryId FoodDiaryEntryId) : ICommand;
