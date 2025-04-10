using Supabase.Gotrue;
using Supabase.Realtime;
using Supabase.Storage;

namespace TrackYourLife.Modules.Common.Application.Core.Abstraction;

public interface ISupabaseClient
    : Supabase.Interfaces.ISupabaseClient<
        User,
        Session,
        RealtimeSocket,
        RealtimeChannel,
        Bucket,
        FileObject
    >;
