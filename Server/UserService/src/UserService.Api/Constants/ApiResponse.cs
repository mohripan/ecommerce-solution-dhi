namespace UserService.Api.Constants
{
    public static class ApiResponse
    {
        public const string Success = "SUCCESS";
        public const string NotFound = "NOT_FOUND";
        public const string ValidationError = "VALIDATION_ERROR";
        public const string InternalServerError = "INTERNAL_SERVER_ERROR";
        public const string AuthenticationFailed = "AUTHERNTICATION_FAILED";

        public static readonly Dictionary<string, string> Messages = new()
        {
            { Success, "Operation completed successfully." },
            { NotFound, "The requested resource was not found." },
            { ValidationError, "Validation error occurred." },
            { InternalServerError, "An unexpected error occurred. Please try again later." }
        };
    }
}
