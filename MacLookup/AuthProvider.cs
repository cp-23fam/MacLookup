using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lextm.SharpSnmpLib.Security;
using Lextm.SharpSnmpLib;

namespace MacLookup
{
    public class AuthProvider
    {
        public enum Types { MD5, SHA1, SHA256, SHA384, SHA512 }
        static public IAuthenticationProvider? Get(Types authType, string pwd)
        {
            IAuthenticationProvider? auth = null;

            switch (authType)
            {
                case Types.MD5:
                    auth = new MD5AuthenticationProvider(new OctetString(pwd));
                    break;
                case Types.SHA1:
                    auth = new SHA1AuthenticationProvider(new OctetString(pwd));
                    break;
                case Types.SHA256:
                    auth = new SHA256AuthenticationProvider(new OctetString(pwd));
                    break;
                case Types.SHA384:
                    auth = new SHA384AuthenticationProvider(new OctetString(pwd));
                    break;
                case Types.SHA512:
                    auth = new SHA512AuthenticationProvider(new OctetString(pwd));
                    break;
            }
            return auth;
        }

    }
}
