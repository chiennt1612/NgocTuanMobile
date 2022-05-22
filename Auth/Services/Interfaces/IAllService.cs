namespace Auth.Services.Interfaces
{
    public interface IAllService
    {
        IAboutServices aboutServices { get; }
        IFAQServices fAQServices { get; }
        IServiceServices serviceServices { get; }
        IAdvServices advServices { get; }
        IArticleServices articleServices { get; }
        ICategoriesServices categoriesServices { get; }
        INewsCategoriesServices newsCategoriesServices { get; }
        IParamSettingServices paramSettingServices { get; }
        IProductServices productServices { get; }
        IContactServices contactServices { get; }
    }

}
