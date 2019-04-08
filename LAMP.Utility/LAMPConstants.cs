using System;

namespace LAMP.Utility
{
    /// <summary>
    /// LAMPConstants
    /// </summary>
    public class LAMPConstants
    {
        public static string LoggedInUser = "LoggedInUser";
        public static string Cookie_Name = "LAMP_Provider_Name";
        public static string UserName_Cookie = "UserName";
        public static string Password_Cookie = "Password";
        public static string SessionToken = "SessionToken";

        public static int API_SUCCESS = 1;
        public static int API_SUCCESS_CODE = 0;
        public static int API_FAIL = 0;
        public static int API_ERROR_CODE = 1;

        public static int API_UNEXPECTED_ERROR = 2030;
        public static int API_INVALID_INPUT_DATA = 2031;
        public static int API_SESSION_TOKEN_UPDATION_FAILED = 2033;
        public static int API_USER_LOGGED_IN_FAILED = 2034;
        public static int API_INVALID_USERNAME = 2035;
        public static int API_USER_ALREADY_REGISTRED = 2036;
        public static int API_USER_SESSION_EXPIRED = 2037;
        public static int API_INVALID_USER = 2040;
        public static int API_EMPTY_IMAGE_STRING = 2043;
        public static int API_USER_INACTIVE = 2044;
        public static int API_USER_ACCOUNT_DELETED = 2050;
        public static int API_UNEXPECTED_ERROR_FOR_BATCH_SCHEDULE = 1000;

        public static string MSG_USER_PASSWORD_SEND_SUCCESSFULLY = "MSG_USER_PASSWORD_SEND_SUCCESSFULLY"; 
        public const string MSG_SPECIFY_EMAIL_ADDRESS = "MSG_SPECIFY_EMAIL_ADDRESS";
        public const string MSG_INVALID_EMAIL = "MSG_INVALID_EMAIL";
        public const string MSG_SPECIFY_NEW_PASSWORD = "MSG_SPECIFY_NEW_PASSWORD";
        public const string MSG_SPECIFY_DIFFERENT_PASSWORDS = "MSG_SPECIFY_DIFFERENT_PASSWORDS";
        public const string MSG_SPECIFY_CONFIRM_PASSWORD = "MSG_SPECIFY_CONFIRM_PASSWORD";
        public const string MSG_SPECIFY_SAME_PASSWORDS = "MSG_SPECIFY_SAME_PASSWORDS";
        public const string MSG_NEW_PASSWORD = "MSG_NEW_PASSWORD";
        public const string MSG_CONFIRM_PASSWORD = "MSG_CONFIRM_PASSWORD";
        public const string MSG_SPECIFY_PASSWORD = "MSG_SPECIFY_PASSWORD";
        public const string MSG_INVALID_CREDENTIALS = "MSG_INVALID_CREDENTIALS";
        public static string MSG_RESET_PASSWORD_LINK_EMAIL_SEND = "MSG_RESET_PASSWORD_LINK_EMAIL_SEND";
        public static string MSG_PASSWORD_CHANGED_SUCCESSFULLY = "MSG_PASSWORD_CHANGED_SUCCESSFULLY";
        public static string MAIL_CANNOT_SEND = "MAIL_CANNOT_SEND";
        public static string ADMIN_NOT_REGISTERED = "ADMIN_NOT_REGISTERED";
        public static short SUCCESS_CODE = 0;
        public static short ERROR_CODE = 1;
        public static short UNEXPECTED_ERROR = 2030;
        public static short USER_EMAIL_ALREADY_EXIST = 2041;
        public static short INVALID_EMAIL = 2042;
        public static short EMPTY_PASSWORD = 2045;
        public const short MSG_CURRENT_PASSWORD = 2046;
        public const short MSG_WRONG_OLD_PASSWORD = 2047;
        public const short MSG_INVALID_USER = 2048;
        public const short MSG_INVALID_DOB = 2049;
        public static short INVALID_STUDY_ID = 2051;

        public static string MSG_USER_DETAILS_SAVED_SUCCESSFULLY = "MSG_USER_DETAILS_SAVED_SUCCESSFULLY";
        public static string MSG_USER_DETAILS_SAVED_SUCCESSFULLY_IMAGE_NOT_SAVED = "MSG_USER_DETAILS_SAVED_SUCCESSFULLY_IMAGE_NOT_SAVED";
        public static string MSG_USER_EMAIL_ALREADY_EXIST = "MSG_USER_DETAILS_SAVED_SUCCESSFULLY";
        public static string TIP_SAVED_SUCCESSFULLY = "TIP_SAVED_SUCCESSFULLY";
        public static string BLOG_SAVED_SUCCESSFULLY = "BLOG_SAVED_SUCCESSFULLY";
        public static string BLOG_UPDATED_SUCCESSFULLY = "BLOG_UPDATED_SUCCESSFULLY";
        public static string APP_HELP_SAVED_SUCCESSFULLY = "APP_HELP_SAVED_SUCCESSFULLY";
        public static string APP_HELP_UPDATED_SUCCESSFULLY = "APP_HELP_UPDATED_SUCCESSFULLY";
        public static string ADMIN_SAVED_SUCCESSFULLY = "ADMIN_SAVED_SUCCESSFULLY";
        public static string ADMIN_UPDATED_SUCCESSFULLY = "ADMIN_UPDATED_SUCCESSFULLY";
        public static string ADMIN_NOT_SAVED = "ADMIN_NOT_SAVED";
        public static string SCHEDULE_SAVED_SUCCESSFULLY = "SCHEDULE_SAVED_SUCCESSFULLY";
        public static string SCHEDULE_UPDATED_SUCCESSFULLY = "SCHEDULE_UPDATED_SUCCESSFULLY";
        public static string DISTRACTION_SURVEY_SAVED_SUCCESSFULLY = "DISTRACTION_SURVEY_SAVED_SUCCESSFULLY";


        public const string HTML_RESOURCE_FORGOTPASSWORD = "~/Resources/EmailTemplates/ForgotPassword.html";
        public const string HTML_RESOURCE_ADMINFORGOTPASSWORD = "~/Resources/EmailTemplates/AdminForgotPassword.html";
        public const string HTML_RESOURCE_USER_STUDYID_PWD = "~/Resources/EmailTemplates/LampUser.html";
        public const string HTML_RESOURCE_ADMIN_PWD = "~/Resources/EmailTemplates/NewAdminCredentials.html";
        public const string CLINICAL_PROFILE_PATH = "~/Content/Default/clinicalProfile";
        public const string LOGO_PATH = "/Resources/EmailTemplates/logo.png";
        
        public const short LAMP_PAGE_SIZE = 10;
        public const short BLOG_PAGE_SIZE = 3;
        public const short APPHELP_PAGE_SIZE = 2;
        public const short RATING_NIL = -1;
        public const string UN_REGISTERED_USER_MESSAGE = "UN_REGISTERED_USER_MESSAGE";
        public const string UN_REGISTERED_ADMIN_MESSAGE = "UN_REGISTERED_ADMIN_MESSAGE";

        public static string MSG_SURVEY_SAVED_SUCCESSFULLY = "MSG_SURVEY_SAVED_SUCCESSFULLY";
        public static string MSG_SURVEY_SAVE_FAILED = "MSG_SURVEY_SAVE_FAILED";
        public static string MSG_SURVEY_QUESTIONS_REQUIRED = "MSG_SURVEY_QUESTIONS_REQUIRED";
        public static string DEFAULT_LANGUAGE = "en";
    }
}
