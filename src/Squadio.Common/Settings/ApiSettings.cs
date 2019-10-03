using System;
using System.Collections.Generic;
using System.Text;

namespace Squadio.Common.Settings
{
    public class ApiSettings
    {
        public byte[] PublicKeyBytes => Encoding.UTF8.GetBytes($"{PublicKey}");
        public string PublicKey { set; get; }
        public int AccessTokenExpiresInMinutes { set; get; }
        public int RefreshTokenExpiresInMinutes { set; get; }
        public string ISSUER { set; get; }
        public string AUDIENCE { set; get; }
    }
}
