using System.ComponentModel.DataAnnotations;

namespace TrackYourLifeDotnet.Application.Foods.Queries.GetFoodList;

public sealed record GetFoodListRequest(
    [Required] string? SearchParam,
    [Required] int? Page,
    [Required] int? PageSize
);
