namespace Paygate.OnePay
{
    public class PaymentIn
    {
        //public string vpc_SercueSecureSecret { get; set; } // set cứng
        //public string AgainLink { get; set; } // set cứng context.MerchantUrl
        //public string Title { get; set; } // set cứng
        //public string vpc_Locale { get; set; } // set cứng _PaygateLanguage
        //public string vpc_Version { get; set; } // set cứng _PaygateVersion
        //public string vpc_Command { get; set; } = "Pay"; // set cứng Pay
        //public string vpc_Merchant { get; set; } // set cứng
        //public string vpc_AccessCode { get; set; } // set cứng
        public string vpc_MerchTxnRef { get; set; } // ID giao dịch
        public string vpc_OrderInfo { get; set; } // Mã giao dịch
        public string vpc_Amount { get; set; }
        //public string vpc_ReturnURL { get; set; }// set cứng context.MerchantUrl(_MerchantUrl)
        // Thong tin them ve khach hang. De trong neu khong co thong tin
        public string vpc_SHIP_Street01 { get; set; }
        public string vpc_SHIP_Provice { get; set; }
        public string vpc_SHIP_City { get; set; }
        public string vpc_SHIP_Country { get; set; }
        public string vpc_Customer_Phone { get; set; }
        public string vpc_Customer_Email { get; set; }
        public string vpc_Customer_Id { get; set; }
        // Dia chi IP cua khach hang
        //public string vpc_TicketNo { get; set; } // set cứng context.GetRemoteIp()
    }

    public class PaymentOut
    {
        public string vpc_Command { get; set; }
        public string vpc_Locale { get; set; }
        public string vpc_CurrencyCode { get; set; }
        public string vpc_MerchTxnRef { get; set; }
        public string vpc_Merchant { get; set; }
        public string vpc_OrderInfo { get; set; }
        public string vpc_Amount { get; set; }
        public string vpc_TxnResponseCode { get; set; }
        public string vpc_TransactionNo { get; set; }
        public string vpc_Message { get; set; }
        public string vpc_Card { get; set; }
        public string vpc_PayChannel { get; set; }
        public string vpc_CardUid { get; set; }
        public string vpc_CardHolderName { get; set; }
        public string vpc_ItaBank { get; set; }
        public string vpc_ItaFeeAmount { get; set; }
        public string vpc_ItaTime { get; set; }
        public string vpc_OrderAmount { get; set; }
        public string vpc_ItaMobile { get; set; }
        public string vpc_ItaEmail { get; set; }
        public string vpc_SecureHash { get; set; }
    }

    public class QueryDR
    {
        public string vpc_AccessCode { get; set; }
        public string vpc_Command { get; set; } = "queryDR";
        public string vpc_MerchTxnRef { get; set; }
        public string vpc_Merchant { get; set; }
        public string vpc_User { get; set; }
        public string vpc_Password { get; set; }
        public string vpc_Version { get; set; } = "1";
    }

    public class QueryDROut
    {
        //	Xác định giao dịch tồn tại hay không
        //   N: Không tồn tại giao dịch
        //   Y: Có tồn tại giao dịch thanh toán
        public string vpc_DRExists { get; set; }
        //Mã trả lời, xác định giao dịch thành công hay không
        //   0: Giao dịch thanh toán thành công
        //   <> 0: Giao dịch không thanh toán thành công
        //   300: Giao dịch pending
        //   100: Giao dịch đang tiến hành hoặc chưa thanh toán
        public string vpc_TxnResponseCode { get; set; }
        //Giá trị của đối số vpc_MerchTxnRef của giao dịch	
        public string vpc_MerchTxnRef { get; set; }
        //Giá trị của đối số vpc_Merchant của giao dịch	
        public string vpc_Merchant { get; set; }
        //Giá trị của đối số vpc_OrderInfo của giao dịch	
        public string vpc_OrderInfo { get; set; }
        //Giá trị của đối số vpc_Amount của giao dịch	
        public string vpc_Amount { get; set; }
        //Là một số duy nhất được sinh ra từ cổng thanh toán cho mỗi giao dịch
        public string vpc_TransactionNo { get; set; }
        //Mô tả lỗi giao dịch khi thanh toán
        public string vpc_Message { get; set; }
        //Loại thẻ đã thanh toán
        //+ Quốc tế: VC, MC, JC, AE, CUP
        //+ Nội địa: 6 số đầu định danh thẻ, xem danh sách ở mục
        public string vpc_Card { get; set; }
        //Kênh thanh toán: WEB: Thanh toán qua website
        public string vpc_PayChannel { get; set; }
        //Chữ ký kiểm tra toàn vẹn dữ liệu
        public string vpc_SecureHash { get; set; }
    }

    public class PaygateInfo //: IPaygateInfo
    {
        public string _PaygateURL { get; set; }
        public int _PaygateVersion { get; set; }
        public string _PaygateLanguage { get; set; }
        public string _MerchantUrl { get; set; }
        public string _MerchantIPN { get; set; }

        public string _SECURE_SECRET { get; set; }
        public string _MerchantAccessCode { get; set; }
        public string _MerchantID { get; set; }
        public string _Title { get; set; }
        public string _User { get; set; }
        public string _Password { get; set; }
        public string _CreatePayCommand { get; set; }
    }

    public interface IPaygateInfo
    {
        public string _PaygateURL { get; set; }
        public int _PaygateVersion { get; set; }
        public string _PaygateLanguage { get; set; }
        public string _MerchantUrl { get; set; }
        public string _MerchantIPN { get; set; }

        public string _SECURE_SECRET { get; set; }
        public string _MerchantAccessCode { get; set; }
        public string _MerchantID { get; set; }
        public string _Title { get; set; }
        public string _User { get; set; }
        public string _Password { get; set; }
        public string _CreatePayCommand { get; set; }
    }

}
