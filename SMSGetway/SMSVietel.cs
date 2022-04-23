using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net;
using System.Xml;

namespace SMSGetway
{
    public interface ISMSVietel
    {
        string CheckBalance();
        string SendSMS(string Mobile, string Content);
    }
    public class SMSVietel : ISMSVietel
    {
        private readonly ILogger<SMSVietel> _logger;
        private readonly IConfiguration _configuration;

        public SMSVietel(ILogger<SMSVietel> _logger, IConfiguration _configuration)
        {
            this._logger = _logger;
            this._configuration = _configuration;
            //_rijndael = new RijndaelEnhanced(_configuration["SMSVietel:User"], _configuration["Decryptor:initVector"]);
        }

        private string tempXMLRequest = "" +
            "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:impl=\"http://impl.bulkSms.ws/\">" +
            "   <soapenv:Header/>" +
            "   <soapenv:Body>" +
            "      <impl:[FUNCTIONNAME]>" +
            "      [FUNCTIONPARAM]" +
            "      </impl:[FUNCTIONNAME]>" +
            "   </soapenv:Body>" +
            "</soapenv:Envelope>";

        public string SendSMS(string Mobile, string Content)
        {
            string strKq = "";
            string ParamName;
            string ParamValue;
            ParamName = "User^Password^CPCode^RequestID^UserID^ReceiverID^ServiceID^CommandCode^Content^ContentType";
            ParamValue = $"{_configuration["SMSVietel:User"]}^{_configuration["SMSVietel:Password"]}^{_configuration["SMSVietel:CPCode"]}" +
                $"^{_configuration["SMSVietel:RequestID"]}^{FormatMobile(Mobile)}^{FormatMobile(Mobile)}^{_configuration["SMSVietel:ServiceID"]}" +
                $"^{_configuration["SMSVietel:CommandCode"]}^{Content}^{_configuration["SMSVietel:ContentType"]}";
            strKq = GetByWebServiceRequest("wsCpMt", ParamName, ParamValue);   // call hàm wsCpMt
            return strKq;
        }
        public string CheckBalance()
        {
            string strKq = ""; string ParamName; string ParamValue;
            ParamName = "User^Password^CPCode";
            ParamValue = $"{_configuration["SMSVietel:User"]}^{_configuration["SMSVietel:Password"]}^{_configuration["SMSVietel:CPCode"]}";
            strKq = GetByWebServiceRequest("checkBalance", ParamName, ParamValue);   // call hàm checkBalance
            return strKq;
        }
        private string FormatMobile(string Mobile)
        {
            string strLeft2;
            Mobile = Mobile.Replace(" ", "");
            strLeft2 = Mobile.Substring(0, 2);
            switch (strLeft2)
            {
                case "01":
                    //Mobile = Mobile + "00000000000"; Mobile.Substring (0, 11);
                    Mobile = "841" + Mobile.Substring(2); break;
                case "03":
                    //Mobile = Mobile + "00000000000"; Mobile.Substring (0, 10);
                    Mobile = "843" + Mobile.Substring(2); break;
                case "04":
                    //Mobile = Mobile + "00000000000"; Mobile.Substring (0, 10);
                    Mobile = "844" + Mobile.Substring(2); break;
                case "05":
                    //Mobile = Mobile + "00000000000"; Mobile.Substring (0, 10);
                    Mobile = "845" + Mobile.Substring(2); break;
                case "06":
                    //Mobile = Mobile + "00000000000"; Mobile.Substring (0, 10);
                    Mobile = "846" + Mobile.Substring(2); break;
                case "07":
                    //Mobile = Mobile + "00000000000"; Mobile.Substring (0, 10);
                    Mobile = "847" + Mobile.Substring(2); break;
                case "08":
                    //Mobile = Mobile + "00000000000"; Mobile.Substring (0, 10);
                    Mobile = "848" + Mobile.Substring(2); break;
                case "09":
                    //Mobile = Mobile + "00000000000"; Mobile.Substring (0, 10);
                    Mobile = "849" + Mobile.Substring(2); break;
                case "84":
                    break;
                default:
                    Mobile = "84" + Mobile;
                    break;
            }
            return Mobile;
        }
        private string GetByWebServiceRequest(string FuncName, string ParamName, string ParamValue)
        {
            string logMessage;
            string sReturn; XmlDocument soapEnvelopeXml; string iXMLReq; string iXMLResp;
            string iParam; string[] a1; string[] a2; int i;
            HttpWebRequest webReq; HttpWebResponse iResponse;
            sReturn = "Thất bại";
            iParam = "";
            iXMLReq = tempXMLRequest.Replace("[FUNCTIONNAME]", FuncName);
            a1 = ParamName.Split(new string[] { "^" }, StringSplitOptions.None);
            a2 = ParamValue.Split(new string[] { "^" }, StringSplitOptions.None);
            for (i = 0; i < a2.Length; i++)
            {
                iParam = iParam + "<" + a1[i] + ">" + a2[i] + "</" + a1[i] + ">";
            }
            iXMLReq = iXMLReq.Replace("[FUNCTIONPARAM]", iParam);
            soapEnvelopeXml = new XmlDocument();
            soapEnvelopeXml.LoadXml(iXMLReq);
            logMessage = Environment.NewLine + "urlAPI: " + _configuration["SMSVietel:urlAPI"] + Environment.NewLine;
            logMessage = logMessage + "FuncName: " + FuncName + Environment.NewLine;
            logMessage = logMessage + "Action: SOAP:Action" + Environment.NewLine;
            logMessage = logMessage + "ContentType: text/xml;charset=\"utf-8\"" + Environment.NewLine;
            logMessage = logMessage + "Method: POST" + Environment.NewLine;
            logMessage = logMessage + "iXMLReq: " + iXMLReq + Environment.NewLine;
            try
            {
                webReq = (HttpWebRequest)WebRequest.Create(_configuration["SMSVietel:urlAPI"]);
                webReq.Headers.Add("SOAP:Action");
                webReq.ContentType = "text/xml;charset=\"utf-8\"";
                webReq.Accept = "text/xml";
                webReq.Method = "POST";
                webReq.Proxy = new WebProxy();//no proxy
                soapEnvelopeXml.Save(webReq.GetRequestStream());
                iResponse = (HttpWebResponse)webReq.GetResponse();
                //InitiateSSLTrust();//bypass SSL
                if (iResponse.StatusCode == HttpStatusCode.OK)
                {
                    StreamReader rd = new StreamReader(iResponse.GetResponseStream());
                    XmlNodeList root;
                    soapEnvelopeXml = new XmlDocument();
                    iXMLResp = rd.ReadToEnd();
                    soapEnvelopeXml.LoadXml(iXMLResp);
                    root = soapEnvelopeXml.GetElementsByTagName("return");
                    sReturn = root[0].InnerXml;
                }
            }
            catch (Exception ex)
            {
                sReturn = "(urlAPI:" + _configuration["SMSVietel:urlAPI"] + "?op=" + FuncName + "||" + iParam + ")" + ex.ToString();
            }
            logMessage = logMessage + "result: " + sReturn + Environment.NewLine;
            _logger.LogError($"SMS {FuncName}: {logMessage} => {sReturn}");
            return sReturn;
        }

    }
}
