// using TrackYourLife.Modules.Trainings.Application.Features.Exercises.Queries.GetExerciseImage;

// namespace TrackYourLife.Modules.Trainings.Presentation.Features.Exercises.Queries;

// internal sealed record GetExerciseImageRequest(string ImageUrl);

// internal sealed class GetExerciseImage(ISender sender) : Endpoint<GetExerciseImageRequest>
// {
//     public override void Configure()
//     {
//         Get("images/{imageUrl}");
//         Group<ExercisesGroup>();
//         Description(x =>
//             x.Produces<FormFile>(StatusCodes.Status200OK)
//                 .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
//         );
//         AllowFileUploads();
//     }

//     public override async Task HandleAsync(GetExerciseImageRequest req, CancellationToken ct)
//     {
//         var result = await sender.Send(new GetExerciseImageQuery(req.ImageUrl), ct);

//         if (result.IsSuccess)
//         {
//             await SendStreamAsync(
//                 stream: result.Value.OpenReadStream(),
//                 fileName: result.Value.FileName,
//                 fileLengthBytes: result.Value.Length,
//                 contentType: result.Value.ContentType,
//                 cancellation: ct
//             );
//         }
//         else
//         {
//             await SendErrorsAsync(400, ct);
//         }
//     }
// }
