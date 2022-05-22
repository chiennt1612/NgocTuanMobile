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
        public const string OrderItem = "_OrderItem";
        public const string Address = "_Address";
        public const string OrderStatus = "_OrderStatus";

        public const string About = "About";
        public const string Service = "Service";
        public const string FAQ = "FAQ";
        public const string Contact = "Contact";
        public const string Product = "Product";
        public const string Article = "Article";
        public const string Categories = "Categories";
        public const string NewsCategories = "NewsCategories";
        public const string ParamSetting = "ParamSetting";

        public const string Adv = "Adv";
        public const string AdvPosition = "AdvPosition";
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
