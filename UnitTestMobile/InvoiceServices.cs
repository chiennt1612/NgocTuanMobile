using Auth.Services;
using Auth.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using Utils.Repository;
using Utils.Repository.Interfaces;

namespace UnitTestMobile
{
    public class UnitTest_InvoiceServices
    {
        private IConfiguration configuration;
        private IInvoiceServices _invoiceService;
        private IInvoiceRepository unitOfWork;
        [SetUp]
        public void Setup()
        {
            configuration = new ConfigurationBuilder()
                            .AddJsonFile(@"appsettings.json")
                            .Build();
            unitOfWork = new InvoiceRepository(null, configuration);
            _invoiceService = new InvoiceServices(unitOfWork);
        }

        #region CheckPayInvoice function
        [Test]
        public void UnitTest_CheckPayInvoice_1()
        {
            Assert.Pass();
        }
        #endregion

        #region GetInvoice function
        [Test]
        public void UnitTest_GetInvoice_1()
        {
            Assert.Pass();
        }
        #endregion

        #region CheckInvoice function
        [Test]
        public void UnitTest_CheckInvoice_1()
        {
            Assert.Pass();
        }
        #endregion

        #region CheckAllInvoice function
        [Test]
        public void UnitTest_CheckAllInvoice_1()
        {
            Assert.Pass();
        }
        #endregion

        #region GetInvoiceA function
        [Test]
        public void UnitTest_GetInvoiceA_1()
        {
            Assert.Pass();
        }
        #endregion
        #region GetInvoiceAllA function
        [Test]
        public void UnitTest_GetInvoiceAllA_1()
        {
            Assert.Pass();
        }
        #endregion
        #region getCustomerInfo function
        [Test]
        public void UnitTest_getCustomerInfo_1()
        {
            Assert.Pass();
        }
        #endregion
        #region getInvoiceByQRCode function
        [Test]
        public void UnitTest_getInvoiceByQRCode_1()
        {
            Assert.Pass();
        }
        #endregion
        #region GetInvoiceAll function
        [Test]
        public void UnitTest_GetInvoiceAll_1()
        {
            Assert.Pass();
        }
        #endregion
        #region PayInvoice function
        [Test]
        public void UnitTest_PayInvoice_1()
        {
            Assert.Pass();
        }
        #endregion
        #region UndoPayInvoice function
        [Test]
        public void UnitTest_UndoPayInvoice_1()
        {
            Assert.Pass();
        }
        #endregion
    }
}