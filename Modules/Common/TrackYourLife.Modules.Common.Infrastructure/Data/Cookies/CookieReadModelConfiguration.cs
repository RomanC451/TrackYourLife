using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackYourLife.Modules.Common.Domain.Features.Cookies;
using TrackYourLife.Modules.Common.Infrastructure.Data.Constants;

namespace TrackYourLife.Modules.Common.Infrastructure.Data.Cookies;

internal sealed class CookieReadModelConfiguration : IEntityTypeConfiguration<CookieReadModel>
{
    public void Configure(EntityTypeBuilder<CookieReadModel> builder)
    {
        builder.ToTable(TableNames.Cookie);

        builder.HasKey(x => x.Id);
    }
}
