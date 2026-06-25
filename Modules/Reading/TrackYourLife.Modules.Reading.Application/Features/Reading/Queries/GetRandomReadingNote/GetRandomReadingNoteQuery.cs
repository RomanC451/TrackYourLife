using TrackYourLife.Modules.Reading.Contracts.Dtos;
using TrackYourLife.SharedLib.Application.Abstraction;

namespace TrackYourLife.Modules.Reading.Application.Features.Reading.Queries.GetRandomReadingNote;

public sealed record GetRandomReadingNoteQuery : IQuery<RandomReadingNoteDto>;
