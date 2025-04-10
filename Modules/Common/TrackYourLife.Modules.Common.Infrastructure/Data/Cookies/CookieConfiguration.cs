using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackYourLife.Modules.Common.Domain.Features.Cookies;
using TrackYourLife.Modules.Common.Infrastructure.Data.Constants;

namespace TrackYourLife.Modules.Common.Infrastructure.Data.Cookies;

internal sealed class CookieConfiguration : IEntityTypeConfiguration<Cookie>
{
    public void Configure(EntityTypeBuilder<Cookie> builder)
    {
        builder.ToTable(TableNames.Cookie);

        builder.HasKey(x => x.Id);
    }
}
