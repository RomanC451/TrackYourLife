namespace TrackYourLifeDotnet.Domain.Enums
{
    public class UserTokensMinutesToExpires
    {
        
    private static readonly Dictionary<string, int> MinutesToExpireDict = new Dictionary<string, int>
    {
        { "RefreshToken", 10 },
        { "EmailVerificationToken", 30 },
    };

        public static int GetMinutesToExpire(string key)
    {
        return MinutesToExpireDict[key];
    }
    }


}