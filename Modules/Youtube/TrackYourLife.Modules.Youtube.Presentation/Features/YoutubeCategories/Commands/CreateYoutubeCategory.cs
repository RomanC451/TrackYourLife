using TrackYourLife.Modules.Youtube.Application.Features.YoutubeCategories.Commands.CreateYoutubeCategory;
using TrackYourLife.Modules.Youtube.Presentation.Contracts;
using TrackYourLife.SharedLib.Contracts.Shared;

namespace TrackYourLife.Modules.Youtube.Presentation.Features.YoutubeCategories.Commands;

internal sealed record CreateYoutubeCategoryRequest(string Name, int MaxVideosPerDay);

internal sealed class CreateYoutubeCategory(ISender sender)
    : Endpoint<CreateYoutubeCategoryRequest, IResult>
{
    public override void Configure()
    {
        Post("");
        Group<YoutubeCategoriesGroup>();
        Description(x =>
            x.Produces<IdResponse>(StatusCodes.Status201Created)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status409Conflict)
        );
    }

    public override async Task<IResult> ExecuteAsync(CreateYoutubeCategoryRequest req, CancellationToken ct)
    {
        return await Result
            .Create(new CreateYoutubeCategoryCommand(req.Name, req.MaxVideosPerDay))
            .BindAsync(command => sender.Send(command, ct))
            .ToCreatedActionResultAsync(id => $"{ApiRoutes.SettingsCategories}/{id.Value}");
    }
}
