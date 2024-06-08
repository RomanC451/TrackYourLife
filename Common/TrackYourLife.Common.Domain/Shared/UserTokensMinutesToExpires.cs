namespace TrackYourLife.Common.Domain.Shared
{
    public class UserTokensMinutesToExpires
    {
        private static readonly Dictionary<string, int> MinutesToExpireDict = new Dictionary<
            string,
            int
        >
        {
            { "RefreshToken", 360 },
            { "EmailVerificationToken", 360 },
        };

        public static int GetMinutesToExpire(string key)
        {
            return MinutesToExpireDict[key];
        }
    }
}
