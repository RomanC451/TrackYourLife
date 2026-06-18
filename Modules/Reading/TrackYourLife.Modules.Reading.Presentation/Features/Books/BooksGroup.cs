namespace TrackYourLife.Modules.Reading.Presentation.Features.Books;

internal sealed class BooksGroup : Group
{
    public BooksGroup()
    {
        Configure(
            ApiRoutes.Books,
            ep =>
            {
                ep.Description(x => x.ProducesProblem(StatusCodes.Status401Unauthorized));
            }
        );
    }
}
