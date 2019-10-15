namespace Squadio.Common.Models.Errors
{
    public static class ErrorMessages
    {
        public static class Common
        {
            public const string UnprocessableEntity = "Invalid entity or business request";
            public const string InvalidField = "Invalid field";
            public const string NotFound = "Not found";
        }

        public static class Business
        {
            public const string PhoneNumberNotValid = "Phone number is invalid";
            public const string CountryCodeNotValid = "Country code is not valid";
            public const string EmailExists = "Email is already registered";
            public const string RoleNotValid = "Role incorrect";
            public const string RoleUserExists = "The role already belongs to the user";
            public const string UserDoesNotExists = "User does not exists";
            public const string SponsorStudyDoesntExist = "Sponsor study doesnt exist";
            public const string ResearchSiteDoesntExist = "Research site doesnt exist";
            public const string PasswordResetRequestInvalid = "Password reset request is invalid";
            public const string PasswordResetUserDoesntExist = "Invalid email";
            public const string OldPasswordInvalid = "Password is invalid";
            public const string AccessRequestNotFound = "Access request invalid";
            public const string AccessRequestNotExpired = "Access request are expired";
            public const string AccessRequestHasCounter = "You have a counter access request";
            public const string AccessRequestAlreadyOverriden = "Access request already overriden.";
            public const string AccessRequestAlreadyGranted = "Access request already granted.";
            public const string AtLeastOneLocationRequired = "At least one location required";
            public const string CannotSendVerificationRequest = "You can not send verification request.";
            public const string CannotRemovePrincipalLocation = "Cannot remove principal location";
            public const string AtLeastOneStudyRequired = "At least one study required";
            public const string UserIsDeleted = "You don't have access to Anova system - your account is deleted";
            public const string UserIsBlocked = "You cannot request profile information from Research site(s) - your account is blocked";
        }

        public static class Global
        {
            public const string BusinessConflict = "Business Conflict";
        }
        
        public static class Security
        {
            public const string Forbidden = "Forbidden";
            public const string Unauthorized = "Unauthorized";
            public const string AuthDataInvalid = "Auth data invalid";
            public const string AccessTokenInvalid = "Access token invalid";
            public const string RefreshTokenInvalid = "Refresh token invalid";
        }

        public static class System
        {
            public const string InternalError = "Internal error";
        }
    }
}