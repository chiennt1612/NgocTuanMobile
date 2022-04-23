using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Utils
{
    public static class Tools
    {
        public const string NumberSepr = ",";
        public const string ClaimCustomerCode = "username";

        public static string GetClaimValue(this IEnumerable<System.Security.Claims.Claim> Claims, string Key)
        {
            var a = Claims.Where(u => u.Type == Key).FirstOrDefault();
            if (a != null)
            {
                return a.Value;
            }
            else
            {
                return "";
            }
        }

        #region cache dist
        public static async Task<T> GetAsync<T>(this IDistributedCache _cache, string Key, CancellationToken token = default)
        {
            string r = await _cache.GetStringAsync(Key, token);
            if (String.IsNullOrEmpty(r))
                return default;
            else
                return DeserializeFromBase64String<T>(r);
        }
        public static async Task SetAsync<T>(this IDistributedCache _cache, string Key, T _obj, long _time = 1, CancellationToken token = default)
        {
            DistributedCacheEntryOptions options = new DistributedCacheEntryOptions() { AbsoluteExpiration = DateTime.Now.AddMinutes(_time) };
            string r = SerializeToBase64String<T>(_obj);
            await _cache.SetStringAsync(Key, r, options, token);
        }
        #endregion

        #region Base64
        public static bool IsBase64String(this string s)
        {
            s = s.Trim();
            return (s.Length % 4 == 0) && Regex.IsMatch(s, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None);

        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static string SerializeToBase64String<T>(T obj)
        {
            var a = JsonConvert.SerializeObject(obj);
            return Base64Encode(a);
        }

        public static T DeserializeFromBase64String<T>(string content)
        {
            var obj = JsonConvert.DeserializeObject<T>(Base64Decode(content));
            return obj;
        }

        public static string Base64UrlEncode(byte[] s)
        {
            return Base64UrlTextEncoder.Encode(s);
        }
        public static byte[] Base64UrlDecode(string s)
        {
            return Base64UrlTextEncoder.Decode(s);
        }
        #endregion

        #region Zip
        public static string Zip(string s)
        {
            MemoryStream m = new MemoryStream();
            GZipStream gz = new GZipStream(m, CompressionMode.Compress);
            StreamWriter sw = new StreamWriter(gz);
            sw.Write(s); sw.Close();
            byte[] b = m.ToArray();
            string zip = Base64UrlEncode(b);
            return zip;
        }
        public static string UnZip(string s)
        {
            string r = "";
            try
            {
                byte[] r1;
                r1 = Base64UrlDecode(s);
                MemoryStream m = new MemoryStream(r1);
                GZipStream gz = new GZipStream(m, CompressionMode.Decompress);
                StreamReader sr = new StreamReader(gz);
                r = sr.ReadToEnd();
                sr.Close();
            }
            catch
            {
                r = "";
            }
            return r;
        }
        #endregion

        #region Number
        private static string InsertSepr(string d0)
        {
            var d = "" + d0; // convert to string
            var i = 0;
            var d2 = "";
            var ic = 0;
            var ofs = d.Length - 1;
            var decimalpoint = d.IndexOf('.');
            if (decimalpoint >= 0) ofs = decimalpoint - 1;
            for (i = ofs; i >= 0; i--)
            {
                if (d[i].ToString() != NumberSepr)
                {
                    if (ic++ % 3 == 0 && i != ofs && d[i] != '-') d2 = NumberSepr + d2;
                    d2 = d[i] + d2;
                }
            }

            if (decimalpoint >= 0)
            {
                for (i = decimalpoint; i < d.Length; i++)
                    d2 += d[i];
            }
            return d2;
        }

        public static string RemNumSepr(string num)
        {
            return Regex.Replace(num.Replace(NumberSepr, ""), "[^0-9]", "");
        }

        public static string FormatNumber(this string num, int iRound = 0)
        {
            double a = 0;
            try
            {
                a = double.Parse(num);
                a = Math.Round(a, iRound);
            }
            catch
            {
                a = 0;
            }
            //if (a == 0) return "";
            return InsertSepr(a.ToString());
        }

        public static string NumberReading(string number)
        {
            string strReturn = "";
            string s = number;
            while (s.Length > 0 && s.Substring(0, 1) == "0")
            {
                s = s.Substring(1);
            }
            string[] so = new string[] { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };
            string[] hang = new string[] { "", "nghìn", "triệu", "tỷ" };
            int i, j, donvi, chuc, tram;

            bool booAm = false;
            decimal decS = 0;

            try
            {
                decS = Convert.ToDecimal(s.ToString());
            }
            catch { }

            if (decS < 0)
            {
                decS = -decS;
                //s = decS.ToString();
                booAm = true;
            }
            i = s.Length;
            if (i == 0)
                strReturn = so[0] + strReturn;
            else
            {
                j = 0;
                while (i > 0)
                {
                    donvi = Convert.ToInt32(s.Substring(i - 1, 1));
                    i--;
                    if (i > 0)
                        chuc = Convert.ToInt32(s.Substring(i - 1, 1));
                    else
                        chuc = -1;
                    i--;
                    if (i > 0)
                        tram = Convert.ToInt32(s.Substring(i - 1, 1));
                    else
                        tram = -1;
                    i--;
                    if ((donvi > 0) || (chuc > 0) || (tram > 0) || (j == 3))
                        strReturn = hang[j] + strReturn;
                    j++;
                    if (j > 3) j = 1;   //Tránh lỗi, nếu dưới 13 số thì không có vấn đề.
                    //Hàm này chỉ dùng để đọc đến 9 số nên không phải bận tâm
                    if ((donvi == 1) && (chuc > 1))
                        strReturn = "mốt " + strReturn;
                    else
                    {
                        if ((donvi == 5) && (chuc > 0))
                            strReturn = "lăm " + strReturn;
                        else if (donvi > 0)
                            strReturn = so[donvi] + " " + strReturn;
                    }
                    if (chuc < 0) break;//Hết số
                    else
                    {
                        if ((chuc == 0) && (donvi > 0)) strReturn = "linh " + strReturn;
                        if (chuc == 1) strReturn = "mười " + strReturn;
                        if (chuc > 1) strReturn = so[chuc] + " mươi " + strReturn;
                    }
                    if (tram < 0) break;//Hết số
                    else
                    {
                        if ((tram > 0) || (chuc > 0) || (donvi > 0)) strReturn = so[tram] + " trăm " + strReturn;
                    }
                    strReturn = " " + strReturn;
                }
            }
            if (booAm) strReturn = "Âm " + strReturn;
            string result = strReturn.Trim().Substring(0, 1).ToUpper() + strReturn.Trim().Substring(1) + " đồng chẵn";
            return result;
        }
        public static string NumberTo00N(this int num, int round)
        {
            return ("0000000000" + num.ToString()).Right(round);
        }

        public static string Left(this string s, int n)
        {
            if (s.Length > n)
            {
                s = s.Substring(0, n);
            }
            return s;
        }

        public static string Right(this string s, int n, bool IsN = true)
        {
            if (IsN)
            {
                if (s.Length > n)
                {
                    s = s.Substring(s.Length - n, n);
                }
            }
            else
            {
                s = s.Substring(n);
            }
            return s;
        }
        #endregion

        #region Cookie
        public static T ReadCookie<T>(this HttpContext context, string key)
        {
            var a = context.Request.Cookies[key];
            if (!String.IsNullOrEmpty(a))
            {
                if (IsBase64String(a))
                {
                    return DeserializeFromBase64String<T>(a);
                }
            }
            return default;
        }

        public static void WriteCookie<T>(this HttpContext context, string key, T obj, string _Path = "/", long _time = 31536000000)
        {
            string h = "";
            int i = context.Request.Host.Value.IndexOf(":");
            if (i > 1)
            {
                h = context.Request.Host.Value.Substring(0, i);
            }
            else
            {
                h = context.Request.Host.Value;
            }
            var a = SerializeToBase64String(obj);
            context
                .Response
                .Cookies
                .Append(key, a, new CookieOptions()
                {
                    Expires = DateTime.Now.AddMilliseconds(_time),
                    SameSite = SameSiteMode.Strict,
                    Secure = true,
                    HttpOnly = true,
                    Domain = h,
                    Path = _Path
                });
        }

        public static string GetQueryString(this HttpContext httpContext, string Param, bool XSS = true)
        {
            string r = "";
            try
            {
                r = httpContext.Request.Query[Param];
            }
            catch
            {
            }
            if (String.IsNullOrEmpty(r)) r = "";

            if (XSS)
                return r.ReplaceXSS();
            else
                return r;
        }

        public static string GetFormValue(this HttpContext httpContext, string Param, bool XSS = true)
        {
            string r = "";
            try
            {
                r = httpContext.Request.Form[Param];
            }
            catch
            {
            }
            if (String.IsNullOrEmpty(r)) r = "";

            if (XSS)
                return r.ReplaceXSS();
            else
                return r;
        }
        #endregion

        #region WebUtility
        public static string ReplaceXSS(this string s)
        {
            s = s.UrlDecode().Trim();
            s = s.Replace("??", "?")
                    .Replace("'", "’")
                    .Replace("\"", "’’")
                    .Replace("%27", "’")
                    .Replace("%22", "")
                    .Replace("\\\\", "")
                    .Replace("\\", "")
                    .Replace("(", "")
                    .Replace("[", "")
                    .Replace("]", "")
                    .Replace("{", "")
                    .Replace("$", "")
                    .Replace("%24", "")
                    .Replace("%25", "")
                    .Replace("%23", "")
                    .Replace("javascript:", "")
                    .Replace("%", "")
                    .Replace("#", "")
                    .Replace("--", "-")
                    .Replace(")", "")
                    .Replace("<", "«")
                    .Replace(">", "»")
                    ;

            return s;
        }

        public static bool IsEncode(this string s, bool IsUrlEncode = true)
        {
            if (IsUrlEncode)
                return (System.Net.WebUtility.UrlDecode(s) != s);
            else
                return (System.Net.WebUtility.HtmlDecode(s) != s);
        }

        public static string UrlEncode(this string s)
        {
            var r = s;
            if (!IsEncode(r)) r = System.Net.WebUtility.UrlEncode(r);
            return r;
        }

        public static string UrlDecode(this string s)
        {
            var r = s;
            while (IsEncode(r)) r = System.Net.WebUtility.UrlDecode(r);
            return r;
        }

        public static string HtmlEncode(this string s)
        {
            var r = s;
            if (!IsEncode(r, false)) r = System.Net.WebUtility.HtmlEncode(r);
            return r;
        }

        public static string HtmlDecode(this string s)
        {
            var r = s;
            while (!IsEncode(r, false)) r = System.Net.WebUtility.HtmlDecode(r);
            return r;
        }
        #endregion

        #region string Vietnamese
        private static readonly string[] VietnameseSigns = new string[]
        {

            "aAeEoOuUiIdDyY",

            "áàạảãâấầậẩẫăắằặẳẵ",

            "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",

            "éèẹẻẽêếềệểễ",

            "ÉÈẸẺẼÊẾỀỆỂỄ",

            "óòọỏõôốồộổỗơớờợởỡ",

            "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",

            "úùụủũưứừựửữ",

            "ÚÙỤỦŨƯỨỪỰỬỮ",

            "íìịỉĩ",

            "ÍÌỊỈĨ",

            "đ",

            "Đ",

            "ýỳỵỷỹ",

            "ÝỲỴỶỸ"
        };
        public static string RemoveSign4VietnameseString(this string str)
        {
            if (String.IsNullOrEmpty(str)) return "";
            str = str
                .ToLower()
                .Trim()
                .Replace("\t", "-")
                .Replace("\n", "-")
                .Replace(" ", "-")
                .Replace("--", "-");

            for (int i = 1; i < VietnameseSigns.Length; i++)
            {
                for (int j = 0; j < VietnameseSigns[i].Length; j++)
                    str = str.Replace(VietnameseSigns[i][j], VietnameseSigns[0][i - 1]);
            }
            str = Regex.Replace(str, "[^a-zA-Z0-9-]", String.Empty);
            return str;
        }
        #endregion

        #region Set-Get-ViewData
        public static void SetData<T>(this ViewDataDictionary viewData, string Key, T obj) where T : class
        {
            viewData[Key] = obj;
        }
        public static T GetData<T>(this ViewDataDictionary viewData, string Key) where T : class
        {
            return viewData[Key] as T;
        }
        #endregion
    }
}
