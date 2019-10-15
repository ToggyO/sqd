using System.Net;

namespace Squadio.Common.Models.Errors
{
    public static class ErrorCodes
    {
        public static HttpStatusCode UnprocessableEntity = (HttpStatusCode) 422;

        public static class Business
        {
            public const string CountryCodeNotValid = "bus.country_code_not_valid";
            public const string EmailExists = "bus.email_already_exists";
            public const string RoleNotValid = "bus.role_not_valid";
            public const string RoleUserExists = "bus.user_role_already_exists";
            public const string UserDoesNotExists = "bus.user_does_not_exists";
            public const string FieldUserId = "bus.field_user_id";
            public const string SponsorStudyDoesntExist = "bus.sponsor_study_doesnt_exist";
            public const string ResearchSiteDoesntExist = "bus.research_site_doesnt_exist";
            public const string PasswordResetRequestInvalid = "bus.password_reset_request_invalid";
            public const string OldPasswordInvalid = "bus.old_password_invalid";
            public const string IncorrectPassword = "bus.incorrect_password";
            public const string AccessRequestNotFound = "bus.access_request_not_exists";
            public const string AccessRequestNotExpired = "bus.access_request_not_expired";
            public const string AccessRequestHasCounter = "bus.access_request_counter";
            public const string AccessRequestAlreadyOverriden = "bus.access_request_overriden";
            public const string AccessRequestAlreadyGranted = "bus.access_request_granted";
            public const string AtLeastOneLocationRequired = "bus.at_least_one_Location_required";
            public const string AtLeastOneStudyRequired = "bus.at_least_one_study_required";
            public const string CannotSendVerificationRequest = "bus.cannot_send_verification_request";
            public const string AtLeastOneLocationRequiredError = "bus.at_least_one_Location_required";
            public const string AtLeastOneStudyRequiredError = "bus.at_least_one_study_required";
            public const string CannotRemovePrincipalLocation = "bus.cannot_remove_principal_location";
            public const string UserIsDeleted = "bus.user_is_deleted";
            public const string UserIsBlocked = "bus.user_is_blocked";
        }

        public static class Global
        {
            public const string BusinessConflict = "business_conflict";
        }

        public static class Common
        {
            public const string FieldNotBlank = "common.field_not_blank";
            public const string FieldBlank = "common.field_blank";
            public const string FieldSizeMax = "common.field_size_max";
            public const string FieldSizeMin = "common.field_size_min";
            public const string FieldInvalidLength = "common.field_invalid_length";
            public const string FieldInvalid = "common.field_invalid";
            public const string FieldNotValidChars = "common.field_not_valid_chars";
            public const string FieldMax = "common.field_max";
            public const string FieldMin = "common.field_min";
            public const string FieldFuture = "common.field_future";
            public const string FieldPast = "common.field_past";
            public const string FieldEmail = "common.field_email";
            public const string FieldCountryCode = "common.field_contry_code";
            public const string FieldCardNumber = "common.field_card_number";
            public const string FieldPhone = "common.field_phone";
            public const string FieldDuplicate = "common.field_duplicate";
            public const string FileFormatInvalid = "common.file_format_invalid";
            public const string FieldRoleInvalid = "common.field_role_invalid";
            public const string FieldRoleUserExists = "common.field_role_user_exists";
            public const string UnprocessableEntity = "common.unprocessable_entity";
            public const string NotFound = "common.not_found";
        }

        public static class Security
        {
            public const string Forbidden = "sec.forbidden";
            public const string FieldAccessToken = "accessToken";
            public const string Unauthorized = "sec.security_error";
            public const string AuthDataInvalid = "sec.auth_data_invalid";
            public const string AccessTokenInvalid = "sec.access_token_invalid";
            public const string RefreshTokenInvalid = "sec.refresh_token_invalid";
        }

        public static class System
        {
            public const string InternalError = "sys.internal_error";
        }
    }
}