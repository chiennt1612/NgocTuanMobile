using System;
using System.Threading.Tasks;

namespace Auth.Repository.Interfaces
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
        IOrderStatusRepository orderStatusRepository { get; }

        void Save();
        Task SaveAsync();
    }

}
