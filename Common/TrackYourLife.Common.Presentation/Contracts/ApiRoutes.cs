namespace TrackYourLife.Common.Presentation.Contracts;

public sealed class ApiRoutes
{
    private const string Root = "api";

    public static class Authentication
    {
        private const string DefaultRoute = $"{Root}/auth";
        public const string Register = $"{DefaultRoute}/register";
        public const string LogIn = $"{DefaultRoute}/login";
        public const string LogOut = $"{DefaultRoute}/logout";
        public const string RefreshToken = $"{DefaultRoute}/refresh-token";
    }

    public static class User
    {
        private const string DefaultRoute = $"{Root}/user";
        public const string GetCurrentUserData = $"{DefaultRoute}";
        public const string Update = $"{DefaultRoute}";
        public const string Delete = $"{DefaultRoute}";
        public const string VerifyEmail = $"{DefaultRoute}/verify-email/{{token}}";
        public const string ResendVerificationEmail =
            $"{DefaultRoute}/resend-verification-email/{{email}}";

        public const string UploadProfileImage = $"{DefaultRoute}/upload-profile-image";
    }

    public static class Food
    {
        private const string DefaultRoute = $"{Root}/food";
        public const string GetList = $"{DefaultRoute}/search";
        public const string GetById = $"{DefaultRoute}/{{id:guid}}";
    }

    public static class FoodDiary
    {
        private const string DefaultRoute = $"{Root}/foodDiary";
        public const string GetByDate = $"{DefaultRoute}/by-date";
        public const string AddEntry = $"{DefaultRoute}";
        public const string UpdateEntry = $"{DefaultRoute}";
        public const string DeleteEntry = $"{DefaultRoute}/{{id:guid}}";
        public const string GetTotalCalories = $"{DefaultRoute}/total-calories";
    }

    public static class UserGoal
    {
        private const string DefaultRoute = $"{Root}/userGoal";
        public const string GetActiveGoal = $"{DefaultRoute}";
        public const string AddGoal = $"{DefaultRoute}";
        public const string UpdateGoal = $"{DefaultRoute}";
    }
}
