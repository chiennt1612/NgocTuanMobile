using Auth.Repository.Interfaces;
using EntityFramework.API.DBContext;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace Auth.Repository
{
    /// <inheritdoc />
    /// <summary>
    /// Unit of work class
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        #region inject field variables
        private readonly AppDbContext _appDbContext;
        #endregion

        #region data members

        private readonly IHttpContextAccessor _httpContext;
        private IAboutRepository _AboutRepository;
        private IFAQRepository _FAQRepository;
        private IServiceRepository _ServiceRepository;
        private IArticleRepository _ArticleRepository;
        private ICategoriesRepository _CategoriesRepository;
        private INewsCategoriesRepository _NewsCategoriesRepository;
        private IParamSettingRepository _ParamSettingRepository;
        private IProductRepository _ProductRepository;
        private IAdvRepository _AdvRepository;

        private InvoiceSaveRepository _InvoiceSaveRepository;
        private IContactRepository _ContactRepository;
        private IOrderStatusRepository _OrderStatusRepository;
        private INoticeRepository _NoticeRepository;
        private IContractRepository _ContractRepository;
        #endregion

        /// <summary>
        /// Unit of work constructor
        /// </summary>
        /// <param name="appDbContext"></param>
        /// <param name="contextAccessor"></param>
        //public UnitOfWork(AppDbContext _appDbContext, IHttpContextAccessor _httpContext,
        //    IAboutRepository _AboutRepository,
        //    IFAQRepository _FAQRepository,
        //    IServiceRepository _ServiceRepository,
        //    IArticleRepository _ArticleRepository,
        //    ICategoriesRepository _CategoriesRepository,
        //    INewsCategoriesRepository _NewsCategoriesRepository,
        //    IMenuMainFooterRepository _MenuMainFooterRepository,
        //    IMenuSubFooterRepository _MenuSubFooterRepository,
        //    IParamSettingRepository _ParamSettingRepository,
        //    IProductRepository _ProductRepository,
        //    IAdvRepository _AdvRepository,
        //    IContactRepository _ContactRepository,
        //    IOrderStatusRepository _OrderStatusRepository)
        //{
        //    this._appDbContext = _appDbContext;
        //    this._httpContext = _httpContext;
        //    this._AboutRepository = _AboutRepository;
        //    this._FAQRepository = _FAQRepository;
        //    this._ServiceRepository = _ServiceRepository;
        //    this._ArticleRepository = _ArticleRepository;
        //    this._CategoriesRepository = _CategoriesRepository;
        //    this._NewsCategoriesRepository = _NewsCategoriesRepository;
        //    this._MenuMainFooterRepository = _MenuMainFooterRepository;
        //    this._MenuSubFooterRepository = _MenuSubFooterRepository;
        //    this._ParamSettingRepository = _ParamSettingRepository;
        //    this._ProductRepository = _ProductRepository;
        //    this._AdvRepository = _AdvRepository;
        //    this._ContactRepository = _ContactRepository;
        //    this._OrderStatusRepository = _OrderStatusRepository;
        //}

        public UnitOfWork(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public UnitOfWork(AppDbContext appDbContext, IHttpContextAccessor contextAccessor)
        {
            _appDbContext = appDbContext;
            _httpContext = contextAccessor;
        }
        #region Properties
        /// <summary>
        /// Get AppDbContext
        /// </summary>
        public AppDbContext AppDbContext => _appDbContext;

        /// <summary>
        /// Get UserRepository
        /// </summary>
        public IContactRepository contactRepository
        {
            get
            {
                return _ContactRepository = _ContactRepository ?? new ContactRepository(_appDbContext, _httpContext);
            }
        }

        public INoticeRepository noticeRepository
        {
            get
            {
                return _NoticeRepository = _NoticeRepository ?? new NoticeRepository(_appDbContext, _httpContext);
            }
        }

        public IContractRepository contractRepository
        {
            get
            {
                return _ContractRepository = _ContractRepository ?? new ContractRepository(_appDbContext, _httpContext);
            }
        }

        public IInvoiceSaveRepository invoiceSaveRepository
        {
            get
            {
                return _InvoiceSaveRepository = _InvoiceSaveRepository ?? new InvoiceSaveRepository(_appDbContext, _httpContext);
            }
        }

        public IAdvRepository advRepository
        {
            get
            {
                return _AdvRepository = _AdvRepository ?? new AdvRepository(_appDbContext, _httpContext);
            }
        }

        public IOrderStatusRepository orderStatusRepository
        {
            get
            {
                return _OrderStatusRepository = _OrderStatusRepository ?? new OrderStatusRepository(_appDbContext, _httpContext);
            }
        }

        public IArticleRepository articleRepository
        {
            get
            {
                return _ArticleRepository = _ArticleRepository ?? new ArticleRepository(_appDbContext, _httpContext);
            }
        }

        public IAboutRepository aboutRepository
        {
            get
            {
                return _AboutRepository = _AboutRepository ?? new AboutRepository(_appDbContext, _httpContext);
            }
        }

        public IFAQRepository fAQRepository
        {
            get
            {
                return _FAQRepository = _FAQRepository ?? new FAQRepository(_appDbContext, _httpContext);
            }
        }

        public IServiceRepository serviceRepository
        {
            get
            {
                return _ServiceRepository = _ServiceRepository ?? new ServiceRepository(_appDbContext, _httpContext);
            }
        }

        public ICategoriesRepository categoriesRepository
        {
            get
            {
                return _CategoriesRepository = _CategoriesRepository ?? new CategoriesRepository(_appDbContext, _httpContext);
            }
        }

        public INewsCategoriesRepository newsCategoriesRepository
        {
            get
            {
                return _NewsCategoriesRepository = _NewsCategoriesRepository ?? new NewsCategoriesRepository(_appDbContext, _httpContext);
            }
        }

        public IParamSettingRepository paramSettingRepository
        {
            get
            {
                return _ParamSettingRepository = _ParamSettingRepository ?? new ParamSettingRepository(_appDbContext, _httpContext);
            }
        }

        public IProductRepository productRepository
        {
            get
            {
                return _ProductRepository = _ProductRepository ?? new ProductRepository(_appDbContext, _httpContext);
            }
        }

        /// <summary>
        /// Save
        /// </summary>
        public void Save()
        {
            _appDbContext.SaveChanges();
        }
        /// <summary>
        /// Save Async
        /// </summary>
        public async Task SaveAsync()
        {
            await _appDbContext.SaveChangesAsync();
        }
        #endregion

        #region dispose
        private bool _disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _appDbContext.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }

}
