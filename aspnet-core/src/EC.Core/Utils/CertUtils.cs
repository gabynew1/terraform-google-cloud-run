using EC.Utils.Dto;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace EC.Utils
{
    public class CertUtils
    {
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();
        public static X509Certificate2 getX509Certificate(string certSerial, StoreLocation location = StoreLocation.LocalMachine)
        {
            X509Store x509Store = new X509Store(StoreName.My, location);
            x509Store.Open(OpenFlags.ReadOnly);
            try
            {
                X509Certificate2Enumerator enumerator = x509Store.Certificates.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    X509Certificate2 current = enumerator.Current;
                    if (current.SerialNumber.ToUpper().Equals(certSerial.ToUpper()))
                    {
                        return current;
                    }
                }

                return null;
            }
            finally
            {
                x509Store.Close();
            }
        }



    }
}
