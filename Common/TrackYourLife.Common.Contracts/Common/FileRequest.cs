using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace TrackYourLife.Common.Contracts.Common;

public class FileRequest
{
    [Required]
    public IFormFile File { get; set; } = default!;
}
