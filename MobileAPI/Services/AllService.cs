using MobileAPI.Services.Interfaces;

namespace MobileAPI.Services
{
    /// <inheritdoc />
    /// <summary>
    /// Unit of work class
    /// </summary>
    public class AllService : IAllService
    {
        #region data members
        public IAboutServices aboutServices { get; }
        public IFAQServices fAQServices { get; }
        public IServiceServices serviceServices { get; }
        public IAdvServices advServices { get; }
        public IArticleServices articleServices { get; }
        public ICategoriesServices categoriesServices { get; }
        public INewsCategoriesServices newsCategoriesServices { get; }
        public IParamSettingServices paramSettingServices { get; }
        public IProductServices productServices { get; }
        public IContactServices contactServices { get; }

        public INoticeServices noticeServices { get; }
        public IInvoiceServices invoiceServices { get; }
        public IInvoiceSaveServices iInvoiceSaveServices { get; }
        public IContractServices contractServices { get; }
        #endregion

        /// <summary>
        /// Unit of work constructor
        /// </summary>
        /// <param name="appDbContext"></param>
        /// <param name="contextAccessor"></param>
        public AllService(IAboutServices aboutServices,
            IServiceServices serviceServices,
            IFAQServices fAQServices,
            IAdvServices advServices,
        IArticleServices articleServices,
        ICategoriesServices categoriesServices,
        INewsCategoriesServices newsCategoriesServices,
        IParamSettingServices paramSettingServices,
        IProductServices productServices,
        IContactServices contactServices,
        INoticeServices noticeServices,
        IInvoiceServices invoiceServices,
        IInvoiceSaveServices iInvoiceSaveServices,
        IContractServices iContractServices)
        {
            this.fAQServices = fAQServices;
            this.serviceServices = serviceServices;
            this.aboutServices = aboutServices;
            this.articleServices = articleServices;
            this.categoriesServices = categoriesServices;
            this.categoriesServices = categoriesServices;
            this.newsCategoriesServices = newsCategoriesServices;
            this.paramSettingServices = paramSettingServices;
            this.productServices = productServices;
            this.contactServices = contactServices;
            this.advServices = advServices;
            this.noticeServices = noticeServices;
            this.invoiceServices = invoiceServices;
            this.iInvoiceSaveServices = iInvoiceSaveServices;
            this.contractServices = iContractServices;
        }
    }

}
