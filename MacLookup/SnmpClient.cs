using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Lextm.SharpSnmpLib.Messaging;
using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Security;
using Avalonia.Controls.Platform;

namespace MacLookup
{
    class SnmpClient
    {
        IPAddress ipAddress;
        IAuthenticationProvider? authProvider;
        string username;

        public SnmpClient(string ip, string username, string pwd, AuthProvider.Types type)
        {
            ipAddress = IPAddress.Parse(ip);
            this.username = username;
            authProvider = AuthProvider.Get(type, pwd);
        }

        private List<Variable> Walk(string path)
        {
            var ipEndPoint = new IPEndPoint(ipAddress, 161);
            Discovery discovery = Messenger.GetNextDiscovery(SnmpType.GetRequestPdu);
            ReportMessage report = discovery.GetResponse(60000, ipEndPoint);
            var result = new List<Variable>();
            Messenger.BulkWalk(VersionCode.V3,
                              ipEndPoint,
                              new OctetString(username),
                              OctetString.Empty, // context name
                              new ObjectIdentifier(path),
                              result,
                              60000,
                              10,
                              WalkMode.WithinSubtree,
                              new DefaultPrivacyProvider(authProvider),
                              report);

            return result;
        }

        public int? getPortFromMac(string macString)
        {
            var mac = macString.ToUpper();

            string path = "1.3.6.1.2.1.17.7.1.2.2.1.2.1";

            var result = Walk(path);
            int index = ObjectIdentifier.Convert(path).Length;

            foreach (var r in result)
            {
                var oid = r.Id.ToNumerical();

                string treatedMac = "";
                for (int i = 0; i < oid.Length - index; i++)
                {
                    string part = oid[index + i].ToString("X");
                    if (part.Length < 2)
                    {
                        part = "0" + part;
                    }

                    treatedMac += part + ":";
                }
                treatedMac = treatedMac.Substring(0, treatedMac.Length - 1);

                if (mac == treatedMac)
                {
                    return int.Parse(r.Data.ToString());
                }
            }

            return null;
        }
    }
}
