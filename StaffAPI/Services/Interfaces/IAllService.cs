namespace StaffAPI.Services.Interfaces
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

        INoticeServices noticeServices { get; }
        IInvoiceServices invoiceServices { get; }
        IInvoiceSaveServices iInvoiceSaveServices { get; }
        IContractServices contractServices { get; }
    }

}
