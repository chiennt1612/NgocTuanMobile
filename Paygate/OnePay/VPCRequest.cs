using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Paygate.OnePay
{
    public interface IVPCRequest
    {
        void SetSecureSecret(String secret);
        void AddDigitalOrderField(String key, String value);
        String GetResultField(String key, String defValue);
        String GetResultField(String key);
        //String GetRequestRaw();
        string GetTxnResponseCode();
        String Create3PartyQueryString();
        string Process3PartyResponse(System.Collections.Specialized.NameValueCollection nameValueCollection);
    }
    public class VPCRequest : IVPCRequest
    {
        private readonly PaygateInfo paygateInfo;
        private readonly ILogger ilogger;
        Uri _address;
        SortedList<String, String> _requestFields = new SortedList<String, String>(new VPCStringComparer());
#pragma warning disable CS0169 // The field 'VPCRequest._rawResponse' is never used
        String _rawResponse;
#pragma warning restore CS0169 // The field 'VPCRequest._rawResponse' is never used
        SortedList<String, String> _responseFields = new SortedList<String, String>(new VPCStringComparer());
        String _secureSecret;


        public VPCRequest(PaygateInfo paygateInfo, ILogger ilogger)
        {
            this.paygateInfo = paygateInfo;
            this.ilogger = ilogger;
            _address = new Uri(this.paygateInfo._PaygateURL);
            try
            {
                ilogger.LogInformation($"Create object VPCRequest with {JsonConvert.SerializeObject(paygateInfo)}");
            }
            catch (Exception ex)
            {
                ilogger.LogError($"Create object VPCRequest with {JsonConvert.SerializeObject(ex.Message)}");
            }
        }

        public void SetSecureSecret(String secret)
        {
            _secureSecret = secret;
            ilogger.LogInformation($"SetSecureSecret object VPCRequest with {_secureSecret}");
        }

        public void AddDigitalOrderField(String key, String value)
        {
            ilogger.LogInformation($"AddDigitalOrderField key {key} with value {value}");
            if (!String.IsNullOrEmpty(value))
            {
                _requestFields.Add(key, value);
            }
        }

        public String GetResultField(String key, String defValue)
        {
            String value;
            if (_responseFields.TryGetValue(key, out value))
            {
                ilogger.LogInformation($"GetResultField key {key} with value {value}");
                return value;
            }
            else
            {
                ilogger.LogInformation($"GetResultField key {key} with value {defValue}");
                return defValue;
            }
        }

        public String GetResultField(String key)
        {
            return GetResultField(key, "");
        }

        private String GetRequestRaw()
        {
            StringBuilder data = new StringBuilder();
            foreach (KeyValuePair<string, string> kvp in _requestFields)
            {
                if (!String.IsNullOrEmpty(kvp.Value))
                {
                    data.Append(kvp.Key + "=" + HttpUtility.UrlEncode(kvp.Value) + "&");
                }
            }
            //remove trailing & from string
            if (data.Length > 0)
                data.Remove(data.Length - 1, 1);
            var s = data.ToString();
            data.Clear();
            data = null;
            ilogger.LogInformation($"GetRequestRaw value {s}");
            return s;
        }

        public string GetTxnResponseCode()
        {
            return GetResultField("vpc_TxnResponseCode");
        }

        //_____________________________________________________________________________________________________
        // Three-Party order transaction processing

        public String Create3PartyQueryString()
        {
            StringBuilder url = new StringBuilder();
            //Payment Server URL
            url.Append(_address);
            url.Append("?");
            //Create URL Encoded request string from request fields 
            url.Append(GetRequestRaw());
            //Hash the request fields
            url.Append("&vpc_SecureHash=");
            url.Append(CreateSHA256Signature(true));

            var _url = url.ToString();
            url.Clear();
            url = null;
            ilogger.LogInformation($"Create3PartyQueryString value {_url}");
            return _url;
        }

        public string Process3PartyResponse(System.Collections.Specialized.NameValueCollection nameValueCollection)
        {
            foreach (string item in nameValueCollection)
            {
                ilogger.LogInformation($"Process3PartyResponse key {item} value {nameValueCollection[item]}");
                if (!item.Equals("vpc_SecureHash") && !item.Equals("vpc_SecureHashType"))
                {
                    _responseFields.Add(item, nameValueCollection[item]);
                }

            }

            if (!nameValueCollection["vpc_TxnResponseCode"].Equals("0") && !String.IsNullOrEmpty(nameValueCollection["vpc_Message"]))
            {
                if (!String.IsNullOrEmpty(nameValueCollection["vpc_SecureHash"]))
                {
                    if (!CreateSHA256Signature(false).Equals(nameValueCollection["vpc_SecureHash"]))
                    {
                        return "INVALIDATED";
                    }
                    ilogger.LogInformation($"Process3PartyResponse Is corrected");
                    return "CORRECTED";
                }
                ilogger.LogInformation($"Process3PartyResponse Is corrected");
                return "CORRECTED";
            }

            if (String.IsNullOrEmpty(nameValueCollection["vpc_SecureHash"]))
            {
                return "INVALIDATED";//no sercurehash response
            }
            if (!CreateSHA256Signature(false).Equals(nameValueCollection["vpc_SecureHash"]))
            {
                return "INVALIDATED";
            }
            ilogger.LogInformation($"Process3PartyResponse Is corrected");
            return "CORRECTED";
        }

        //_____________________________________________________________________________________________________

        private class VPCStringComparer : IComparer<String>
        {
            /*
             <summary>Customised Compare Class</summary>
             <remarks>
             <para>
             The Virtual Payment Client need to use an Ordinal comparison to Sort on 
             the field names to create the SHA256 Signature for validation of the message. 
             This class provides a Compare method that is used to allow the sorted list 
             to be ordered using an Ordinal comparison.
             </para>
             </remarks>
             */

            public int Compare(String a, String b)
            {
                /*
                 <summary>Compare method using Ordinal comparison</summary>
                 <param name="a">The first string in the comparison.</param>
                 <param name="b">The second string in the comparison.</param>
                 <returns>An int containing the result of the comparison.</returns>
                 */

                // Return if we are comparing the same object or one of the 
                // objects is null, since we don't need to go any further.
                if (a == b) return 0;
                if (a == null) return -1;
                if (b == null) return 1;

                // Ensure we have string to compare
                string sa = a as string;
                string sb = b as string;

                // Get the CompareInfo object to use for comparing
                System.Globalization.CompareInfo myComparer = System.Globalization.CompareInfo.GetCompareInfo("en-US");
                if (sa != null && sb != null)
                {
                    // Compare using an Ordinal Comparison.
                    return myComparer.Compare(sa, sb, System.Globalization.CompareOptions.Ordinal);
                }
                throw new ArgumentException("a and b should be strings.");
            }
        }

        //______________________________________________________________________________
        // SHA256 Hash Code

        public string CreateSHA256Signature(bool useRequest)
        {
            // Hex Decode the Secure Secret for use in using the HMACSHA256 hasher
            // hex decoding eliminates this source of error as it is independent of the character encoding
            // hex decoding is precise in converting to a byte array and is the preferred form for representing binary values as hex strings. 
            byte[] convertedHash = new byte[_secureSecret.Length / 2];
            for (int i = 0; i < _secureSecret.Length / 2; i++)
            {
                convertedHash[i] = (byte)Int32.Parse(_secureSecret.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber);
            }

            // Build string from collection in preperation to be hashed
            StringBuilder sb = new StringBuilder();
            SortedList<String, String> list = (useRequest ? _requestFields : _responseFields);
            foreach (KeyValuePair<string, string> kvp in list)
            {
                if (kvp.Key.StartsWith("vpc_") || kvp.Key.StartsWith("user_"))
                    sb.Append(kvp.Key + "=" + kvp.Value + "&");
            }
            // remove trailing & from string
            if (sb.Length > 0)
                sb.Remove(sb.Length - 1, 1);

            // Create secureHash on string
            string hexHash = "";
            using (HMACSHA256 hasher = new HMACSHA256(convertedHash))
            {
                byte[] hashValue = hasher.ComputeHash(Encoding.UTF8.GetBytes(sb.ToString()));
                foreach (byte b in hashValue)
                {
                    hexHash += b.ToString("X2");
                }
            }
            return hexHash;
        }
    }
}
