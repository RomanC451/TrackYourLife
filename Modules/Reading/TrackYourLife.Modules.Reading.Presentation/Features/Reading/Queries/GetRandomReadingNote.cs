using TrackYourLife.Modules.Reading.Application.Features.Reading.Queries.GetRandomReadingNote;
using TrackYourLife.Modules.Reading.Contracts.Dtos;

namespace TrackYourLife.Modules.Reading.Presentation.Features.Reading.Queries;

internal sealed class GetRandomReadingNote(ISender sender) : EndpointWithoutRequest<IResult>
{
    public override void Configure()
    {
        Get("/random-note");
        Group<ReadingGroup>();
        Description(x =>
            x.Produces<RandomReadingNoteDto>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status404NotFound)
        );
    }

    public override async Task<IResult> ExecuteAsync(CancellationToken ct)
    {
        return await Result
            .Create(new GetRandomReadingNoteQuery())
            .BindAsync(query => sender.Send(query, ct))
            .ToActionResultAsync();
    }
}
