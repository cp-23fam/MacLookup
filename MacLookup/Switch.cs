using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MacLookup
{
    public class Switch
    {
        public string Ip;
        public int[] ConnexionPorts;
        public AuthProvider.Types ConnexionType;
        public string Name;

        public Switch(string ip, int[] connexionPorts, AuthProvider.Types type, string name)
        {
            Ip = ip;
            ConnexionPorts = connexionPorts;
            ConnexionType = type;
            Name = name;
        }
    };

}
