using Microsoft.AspNetCore.Http;

namespace Paygate.OnePay
{
    public static class Tools
    {
        public const int StartIdOrder = 0;
        #region Paygate

        public static string MerchantUrl(this HttpContext context, string MerchantPath)
        {
            return context.Request.Scheme + "://" + context.Request.Host + MerchantPath;
        }

        public static string MerchantUrl(this HttpContext context)
        {
            return context.Request.Scheme + "://" + context.Request.Host;
        }

        public static string MerchantUrl(this PaygateInfo paygateInfo, string MerchantPath)
        {
            return paygateInfo.Domain + MerchantPath;
        }

        public static string MerchantUrl(this PaygateInfo paygateInfo)
        {
            return paygateInfo.Domain;
        }

        // vpc_ticket
        public static string GetRemoteIp(this HttpContext context)
        {
            return context.Connection.RemoteIpAddress.ToString();
        }

        public static string CreatePay(this VPCRequest conn, HttpContext context, PaygateInfo paygateInfo, PaymentIn t)
        {
            // Khoi tao lop thu vien va gan gia tri cac tham so gui sang cong thanh toan
            conn.SetSecureSecret(paygateInfo._SECURE_SECRET);
            // Add the Digital Order Fields for the functionality you wish to use
            // Core Transaction Fields
            conn.AddDigitalOrderField("AgainLink", paygateInfo.MerchantUrl());
            conn.AddDigitalOrderField("Title", paygateInfo._Title);
            conn.AddDigitalOrderField("vpc_Locale", paygateInfo._PaygateLanguage);//Chon ngon ngu hien thi tren cong thanh toan (vn/en)
            conn.AddDigitalOrderField("vpc_Version", paygateInfo._PaygateVersion.ToString());
            conn.AddDigitalOrderField("vpc_Command", paygateInfo._CreatePayCommand);
            conn.AddDigitalOrderField("vpc_Merchant", paygateInfo._MerchantID);
            conn.AddDigitalOrderField("vpc_AccessCode", paygateInfo._MerchantAccessCode);
            conn.AddDigitalOrderField("vpc_MerchTxnRef", t.vpc_MerchTxnRef);//new Random().NextDouble().ToString()
            conn.AddDigitalOrderField("vpc_OrderInfo", t.vpc_OrderInfo);
            conn.AddDigitalOrderField("vpc_Amount", t.vpc_Amount + "00");
            conn.AddDigitalOrderField("vpc_ReturnURL", paygateInfo.MerchantUrl(paygateInfo._MerchantUrl));

            //// Thong tin them ve khach hang. De trong neu khong co thong tin
            //conn.AddDigitalOrderField("vpc_SHIP_Street01", t.vpc_SHIP_Street01);
            //conn.AddDigitalOrderField("vpc_SHIP_Provice", t.vpc_SHIP_Provice);
            //conn.AddDigitalOrderField("vpc_SHIP_City", t.vpc_SHIP_City);
            //conn.AddDigitalOrderField("vpc_SHIP_Country", t.vpc_SHIP_Country);
            //conn.AddDigitalOrderField("vpc_Customer_Phone", t.vpc_Customer_Phone);
            //conn.AddDigitalOrderField("vpc_Customer_Email", t.vpc_Customer_Email);
            //conn.AddDigitalOrderField("vpc_Customer_Id", t.vpc_Customer_Id);
            // Dia chi IP cua khach hang
            conn.AddDigitalOrderField("vpc_TicketNo", context.GetRemoteIp());
            // Chuyen huong trinh duyet sang cong thanh toan
            var url = conn.Create3PartyQueryString();
            return url;
        }
        #endregion
    }
}
