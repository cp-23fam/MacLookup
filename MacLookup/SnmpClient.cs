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
        IAuthenticationProvider authProvider;
        string username;

        public SnmpClient(string ip, string username, string pwd, AuthProvider.Types type)
        {
            IPAddress.TryParse(ip, out ipAddress);
            this.username = username;
            authProvider = AuthProvider.Get(type, pwd);

            getPortFromMac("10:0D:7F:6D:0E:B5");
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

            string path = "1.0.8802.1.1.2.1.4.1.1";

            var result = Walk(path);
            int index = ObjectIdentifier.Convert(path).Length;

            foreach (var r in result)
            {
                var oid = r.Id.ToNumerical();
                var id = (int)oid[index + 2];

                int integerValue = 0;
                string? stringValue = r.Data.ToString();
                OctetString? octetValue = null;
                switch (r.Data.TypeCode)
                {
                    case SnmpType.Integer32:
                        integerValue = (r.Data as Integer32).ToInt32();
                        break;
                    case SnmpType.OctetString:
                        octetValue = r.Data as OctetString;
                        break;
                }

                if (oid[index] == 5)
                {
                    var b = octetValue.GetRaw();

                    if ($"{b[0]:X2}:{b[1]:X2}:{b[2]:X2}:{b[3]:X2}:{b[4]:X2}:{b[5]:X2}" == mac)
                    {
                        Console.WriteLine($"Same mac! : {id}");
                        return id;
                    }
                }
            }

            return null;
        }
    }
}
