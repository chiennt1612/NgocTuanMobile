using Auth.Repository;
using Auth.Repository.Interfaces;
using Auth.Services;
using Auth.Services.Interfaces;
using EntityFramework.API.DBContext;
using NUnit.Framework;

namespace UnitTestMobile
{
    public class UnitTest_ContactServices
    {
        private IContactServices _invoiceSaveService;
        private IUnitOfWork unitOfWork;
        [SetUp]
        public void Setup()
        {
            unitOfWork = new UnitOfWork(new AppDbContext("Data Source=(LocalDb)\\MSSQLLocalDB;Initial Catalog=NuocNgocTuan"));
            _invoiceSaveService = new ContactServices(unitOfWork, null);
        }

        #region AddAsync function
        [Test]
        public void UnitTest_AddAsync_1()
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

        #region Update function
        [Test]
        public void UnitTest_Update_1()
        {
            Assert.Pass();
        }
        #endregion
    }
}