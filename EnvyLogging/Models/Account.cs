using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnvyLogging.Models
{
    public class Account
    {
        public CSteamID Id { get; set; }
        public string Name { get; set; }
        public string Lastname { get; set; }
        public string Sexo { get; set; }
        public int Age { get; set; }

    }
}
