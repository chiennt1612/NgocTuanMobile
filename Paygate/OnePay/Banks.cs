using System.Collections.Generic;

namespace Paygate.OnePay
{
    public static class Banks
    {
        public static List<BankCode> GetAll()
        {
            var a = new List<BankCode>();
            a.Add(new BankCode() { BIN = 970407, BankName = "Techcombank" });
            a.Add(new BankCode() { BIN = 970423, BankName = "Tienphongbank" });
            a.Add(new BankCode() { BIN = 970415, BankName = "Vietinbank" });
            a.Add(new BankCode() { BIN = 970441, BankName = "VIB" });
            a.Add(new BankCode() { BIN = 970427, BankName = "VietA" });
            a.Add(new BankCode() { BIN = 970426, BankName = "MSB" });
            a.Add(new BankCode() { BIN = 970431, BankName = "EXIM" });
            a.Add(new BankCode() { BIN = 970443, BankName = "SHB" });
            a.Add(new BankCode() { BIN = 970437, BankName = "HDBank" });
            a.Add(new BankCode() { BIN = 970436, BankName = "Vietcombank" });
            a.Add(new BankCode() { BIN = 970406, BankName = "DongAbank" });
            a.Add(new BankCode() { BIN = 970422, BankName = "MB" });
            a.Add(new BankCode() { BIN = 970428, BankName = "NAB" });
            a.Add(new BankCode() { BIN = 970440, BankName = "SEABANK" });
            a.Add(new BankCode() { BIN = 970414, BankName = "OCEANBANK" });
            a.Add(new BankCode() { BIN = 970418, BankName = "BIDV" });
            a.Add(new BankCode() { BIN = 970409, BankName = "BACA" });
            a.Add(new BankCode() { BIN = 970432, BankName = "VPBANK" });
            a.Add(new BankCode() { BIN = 970419, BankName = "NCB(NAVIBANK)" });
            a.Add(new BankCode() { BIN = 970405, BankName = "AGRIBANK" });
            a.Add(new BankCode() { BIN = 970429, BankName = "SAIGONBANK(SCB)" });
            a.Add(new BankCode() { BIN = 970403, BankName = "SACOMBANK" });
            a.Add(new BankCode() { BIN = 970425, BankName = "AnBinhBank" });
            a.Add(new BankCode() { BIN = 970412, BankName = "PVComBank" });
            a.Add(new BankCode() { BIN = 970454, BankName = "VCCB(Bản Việt)" });
            return a;
        }
    }
}
