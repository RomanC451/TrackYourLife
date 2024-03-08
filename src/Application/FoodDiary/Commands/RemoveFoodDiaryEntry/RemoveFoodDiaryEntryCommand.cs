using TrackYourLifeDotnet.Application.Abstractions.Messaging;
using TrackYourLifeDotnet.Domain.FoodDiaries;

namespace TrackYourLifeDotnet.Application.FoodDiary.Commands.RemoveFoodDiaryEntry;

public sealed record RemoveFoodDiaryEntryCommand(FoodDiaryEntryId FoodDiaryEntryId) : ICommand;
