using System;
using System.Threading.Tasks;

namespace StaffAPI.Repository.Interfaces
{
    /// <summary>
    /// Unit of work interface
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        IAboutRepository aboutRepository { get; }
        IFAQRepository fAQRepository { get; }
        IServiceRepository serviceRepository { get; }
        IArticleRepository articleRepository { get; }
        ICategoriesRepository categoriesRepository { get; }
        INewsCategoriesRepository newsCategoriesRepository { get; }
        IParamSettingRepository paramSettingRepository { get; }
        IProductRepository productRepository { get; }
        IAdvRepository advRepository { get; }
        IContactRepository contactRepository { get; }
        IInvoiceSaveRepository invoiceSaveRepository { get; }
        IOrderStatusRepository orderStatusRepository { get; }
        INoticeRepository noticeRepository { get; }
        IContractRepository contractRepository { get; }

        void Save();
        Task SaveAsync();
    }

}
