using Supabase;
using TrackYourLife.Modules.Common.Application.Core.Abstraction;

namespace TrackYourLife.Modules.Common.Infrastructure.Services;

internal class SupaBaseClient(
    string supabaseUrl,
    string? supabaseKey,
    SupabaseOptions? options = null
) : Client(supabaseUrl, supabaseKey, options), ISupabaseClient { }
