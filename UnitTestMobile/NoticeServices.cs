using Auth.Repository;
using Auth.Repository.Interfaces;
using Auth.Services;
using Auth.Services.Interfaces;
using EntityFramework.API.DBContext;
using NUnit.Framework;

namespace UnitTestMobile
{
    public class UnitTest_NoticeServices
    {
        private INoticeServices _invoiceSaveService;
        private IUnitOfWork unitOfWork;
        [SetUp]
        public void Setup()
        {
            unitOfWork = new UnitOfWork(new AppDbContext("Data Source=(LocalDb)\\MSSQLLocalDB;Initial Catalog=NuocNgocTuan"));
            _invoiceSaveService = new NoticeServices(unitOfWork, null);
        }

        #region AddAsync function
        [Test]
        public void UnitTest_AddAsync_1()
        {
            Assert.Pass();
        }
        #endregion

        #region DeleteAsync function
        [Test]
        public void UnitTest_DeleteAsync_1()
        {
            Assert.Pass();
        }
        #endregion

        #region GetByIdAsync function
        [Test]
        public void UnitTest_GetByIdAsync_1()
        {
            Assert.Pass();
        }
        #endregion

        #region GetListAsync function
        [Test]
        public void UnitTest_GetListAsync_1()
        {
            Assert.Pass();
        }
        #endregion

        #region GetAllAsync function
        [Test]
        public void UnitTest_GetAllAsync_1()
        {
            Assert.Pass();
        }
        #endregion

        #region AddManyAsync function
        [Test]
        public void UnitTest_AddManyAsync_1()
        {
            Assert.Pass();
        }
        #endregion
    }
}