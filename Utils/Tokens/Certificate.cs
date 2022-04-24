using Microsoft.IdentityModel.Tokens;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace Utils.Tokens
{
    public static class Certificate
    {
        public static X509SigningCredentials Get()
        {
            var fileName = Directory.GetCurrentDirectory() + @"\SSO.pfx";
            if (!File.Exists(fileName))
            {
                return null;
            }
            var cert = new X509Certificate2(fileName, "IdentityServer", X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.Exportable);
            X509SigningCredentials clientSigningCredentials = new X509SigningCredentials(cert);
            return clientSigningCredentials;
        }

        public static X509SigningCredentials GetFromMachine()
        {
            try
            {
                X509Store storex = new X509Store(StoreName.My, StoreLocation.CurrentUser);
                storex.Open(OpenFlags.ReadOnly);

                X509Certificate2 certificatex = storex.Certificates[0];

                storex.Close();
                X509SigningCredentials clientSigningCredentials = new X509SigningCredentials(certificatex);
                return clientSigningCredentials;
            }
            catch
            {
                return null;
            }
        }
    }
}
