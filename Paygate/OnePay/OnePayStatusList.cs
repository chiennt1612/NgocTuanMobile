﻿using System.Collections.Generic;

namespace Paygate.OnePay
{
    public static class OnePayStatusList
    {
        public static List<OnePayStatus> GetAll()
        {
            var a = new List<OnePayStatus>();
            a.Add(new OnePayStatus() { Code = "0", NameEn = "Transaction is successful", NameVi = "Giao dịch thành công" });
            a.Add(new OnePayStatus() { Code = "1", NameEn = "The transaction is unsuccessful.\nThis transaction has been declined by issuer bank or card have been not registered online payment services.\nPlease contact your bank for further clarification.", NameVi = "Giao dịch không thành công,\nNgân hàng phát hành thẻ không cấp phép cho giao dịch hoặc thẻ chưa được kích hoạt dịch vụ thanh toán trên Internet. Vui lòng liên hệ ngân hàng theo số điện thoại sau mặt thẻ được hỗ trợ chi tiết." });
            a.Add(new OnePayStatus() { Code = "2", NameEn = "The transaction is unsuccessful.\nThis transaction has been declined by issuer bank.\nPlease contact your bank for further clarification.", NameVi = "Giao dịch không thành công,\nNgân hàng phát hành thẻ từ chối cấp phép cho giao dịch. Vui lòng liên hệ ngân hàng theo số điện thoại sau mặt thẻ để biết chính xác nguyên nhân Ngân hàng từ chối." });
            a.Add(new OnePayStatus() { Code = "3", NameEn = "The transaction is unsuccessful.\nOnePAY did not received payment result from Issuer bank.\nPlease contact your bank for details and try again.", NameVi = "Giao dịch không thành công,\nCổng thanh toán không nhận được kết quả trả về từ ngân hàng phát hành thẻ. Vui lòng liên hệ với ngân hàng theo số điện thoại sau mặt thẻ để biết chính xác trạng thái giao dịch và thực hiện thanh toán lại" });
            a.Add(new OnePayStatus() { Code = "4", NameEn = "The transaction is unsuccessful.\nYour card is expired or You have entered incorrect expired date.\nPlease check and try again.", NameVi = "Giao dịch không thành công do thẻ hết hạn sử dụng hoặc nhập sai thông tin tháng/ năm hết hạn của thẻ. Vui lòng kiểm tra lại thông tin và thanh toán lại" });
            a.Add(new OnePayStatus() { Code = "5", NameEn = "The transaction is unsuccessful.\nThis transaction cannot be processed due to insufficient funds.\nPlease try another card.", NameVi = "Giao dịch không thành công,\nThẻ không đủ hạn mức hoặc tài khoản không đủ số dư để thanh toán. Vui lòng kiểm tra lại thông tin và thanh toán lại" });
            a.Add(new OnePayStatus() { Code = "6", NameEn = "The transaction is unsuccessful.\nAn error was encountered while processing your transaction.\nPlease contact your bank for further clarification.", NameVi = "Giao dịch không thành công,\nQuá trình xử lý giao dịch phát sinh lỗi từ ngân hàng phát hành thẻ. Vui lòng liên hệ ngân hàng theo số điện thoại sau mặt thẻ được hỗ trợ chi tiết." });
            a.Add(new OnePayStatus() { Code = "7", NameEn = "The transaction is unsuccessful.\nAn error was encountered while processing your transaction. Please contact your bank for further clarification.", NameVi = "Giao dịch không thành công,\nĐã có lỗi phát sinh trong quá trình xử lý giao dịch. Vui lòng thực hiện thanh toán lại." });
            a.Add(new OnePayStatus() { Code = "8", NameEn = "The transaction is unsuccessful.\nYou have entered incorrect card number. Please try again.", NameVi = "Giao dịch không thành công. Số thẻ không đúng. Vui lòng kiểm tra và thực hiện thanh toán lại" });
            a.Add(new OnePayStatus() { Code = "9", NameEn = "The transaction is unsuccessful.\nYou have entered incorrect card holder name.\nPlease try again.", NameVi = "Giao dịch không thành công. Tên chủ thẻ không đúng. Vui lòng kiểm tra và thực hiện thanh toán lại" });
            a.Add(new OnePayStatus() { Code = "10", NameEn = "The transaction is unsuccessful.\nThe card is expired/locked.\nPlease try again.", NameVi = "Giao dịch không thành công. Thẻ hết hạn/Thẻ bị khóa. Vui lòng kiểm tra và thực hiện thanh toán lại" });
            a.Add(new OnePayStatus() { Code = "11", NameEn = "The transaction is unsuccessful.\nYou have been not registered online payment services.\nPlease contact your bank for details.", NameVi = "Giao dịch không thành công. Thẻ chưa đăng ký sử dụng dịch vụ thanh toán trên Internet. Vui lòng liên hê ngân hàng theo số điện thoại sau mặt thẻ để được hỗ trợ." });
            a.Add(new OnePayStatus() { Code = "12", NameEn = "The transaction is unsuccessful.\nYou have entered incorrect Issue date or Expire date.\nPlease try again.", NameVi = "Giao dịch không thành công. Ngày phát hành/Hết hạn không đúng. Vui lòng kiểm tra và thực hiện thanh toán lại" });
            a.Add(new OnePayStatus() { Code = "13", NameEn = "The transaction is unsuccessful.\nThe transaction amount exceeds the maximum transaction/amount limit. Please try another card.", NameVi = "Giao dịch không thành công. thẻ/ tài khoản đã vượt quá hạn mức thanh toán. Vui lòng kiểm tra và thực hiện thanh toán lại" });
            a.Add(new OnePayStatus() { Code = "21", NameEn = "The transaction is unsuccessful.\nThis transaction cannot be processed due to insufficient funds in your account.\nPlease try another card.", NameVi = "Giao dịch không thành công. Số tiền không đủ để thanh toán. Vui lòng kiểm tra và thực hiện thanh toán lại" });
            a.Add(new OnePayStatus() { Code = "22", NameEn = "The transaction is unsuccessful.\nThis transaction cannot be processed due to invalid account.\nPlease try again.", NameVi = "Giao dịch không thành công. Thông tin tài khoản không đúng. Vui lòng kiểm tra và thực hiện thanh toán lại" });
            a.Add(new OnePayStatus() { Code = "23", NameEn = "The transaction is unsuccessful.\nThis transaction cannot be processed due to account locked.\nPlease contact your bank for further clarification.", NameVi = "Giao dịch không thành công. Tài khoản bị khóa. Vui lòng liên hê ngân hàng theo số điện thoại sau mặt thẻ để được hỗ trợ" });
            a.Add(new OnePayStatus() { Code = "24", NameEn = "The transaction is unsuccessful.\nYou have entered incorrect card number. Please try again", NameVi = "Giao dịch không thành công. Thông tin thẻ không đúng. Vui lòng kiểm tra và thực hiện thanh toán lại" });
            a.Add(new OnePayStatus() { Code = "25", NameEn = "The transaction is unsuccessful.\nYou have entered incorrect OTP. Please try again.", NameVi = "Giao dịch không thành công. OTP không đúng. Vui lòng kiểm tra và thực hiện thanh toán lại" });
            a.Add(new OnePayStatus() { Code = "253", NameEn = "The transaction is unsuccessful.\nTransaction timed out.\nPlease try again.", NameVi = "Giao dịch không thành công. Quá thời gian thanh toán. Vui lòng thực hiện thanh toán lại" });
            a.Add(new OnePayStatus() { Code = "99", NameEn = "The transaction is unsuccessful.\nThe transaction has been cancelled by card holder.\nPlease try again.", NameVi = "Giao dịch không thành công. Người sử dụng hủy giao dịch" });
            a.Add(new OnePayStatus() { Code = "B", NameEn = "The transaction is unsuccessful.\nThe card used in this transaction is not authorized 3D-Secure complete.\nPlease contact your bank for further clarification.", NameVi = "Giao dịch không thành công do không xác thực được 3D-Secure. Vui lòng liên hệ ngân hàng theo số điện thoại sau mặt thẻ được hỗ trợ chi tiết." });
            a.Add(new OnePayStatus() { Code = "E", NameEn = "The transaction is unsuccessful.\nYou have entered wrong CSC or Issuer Bank declided transaction.\nPlease contact your bank for further clarification.", NameVi = "Giao dịch không thành công do nhập sai CSC (Card Security Card) hoặc ngân hàng từ chối cấp phép cho giao dịch. Vui lòng liên hệ ngân hàng theo số điện thoại sau mặt thẻ được hỗ trợ chi tiết." });
            a.Add(new OnePayStatus() { Code = "F", NameEn = "The transaction is unsuccessful.\nDue to 3D Secure Authentication Failed.\nPlease contact your bank for further clarification.", NameVi = "Giao dịch không thành công do không xác thực được 3D-Secure. Vui lòng liên hệ ngân hàng theo số điện thoại sau mặt thẻ được hỗ trợ chi tiết." });
            a.Add(new OnePayStatus() { Code = "Z", NameEn = "The transaction is unsuccessful.\nTransaction restricted due to OFD’s policies.\nPlease contact OnePAY for details (Hotline 1900 633 927)", NameVi = "Giao dịch không thành công do vi phạm quy định của hệ thống. Vui lòng liên hệ với OnePAY để được hỗ trợ (Hotline: 1900 633 927)" });
            a.Add(new OnePayStatus() { Code = "Oth er", NameEn = "The transaction is unsuccessful. Please contact OnePAY for details (Hotline 1900 633 927)", NameVi = "Giao dịch không thành công. Vui lòng liên hệ với OnePAY để được hỗ trợ (Hotline: 1900 633 927)" });
            return a;
        }
    }
}