namespace EntityFramework.API.Constants
{
    public static class UserRoles
    {
        public const string Admin = "Admin";
        public const string User = "User";
    }

    public static class TableConsts
    {
        public const int PageSize = 10;

        public const string Order = "_Order";
        public const string OrderStatus = "_OrderStatus";

        public const string About = "About";
        public const string Service = "Service";
        public const string FAQ = "FAQ";
        public const string Contact = "Contact";
        public const string ParamSetting = "ParamSetting";
    }

    public static class CommonFields
    {
        public const string CreatedBy = "UserCreator";
        public const string CreatedOn = "DateCreator";
        public const string UpdatedBy = "UserModify";
        public const string UpdatedOn = "DateModify";
        public const string IsDeleted = "IsDeleted";
        public const string UserDeleted = "UserDeleted";
        public const string DateDeleted = "DateDeleted";
    }
}
