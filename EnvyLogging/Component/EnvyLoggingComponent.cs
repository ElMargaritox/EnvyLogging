using Rocket.Unturned.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnvyLogging.Component
{
    public class EnvyLoggingComponent : UnturnedPlayerComponent
    {
        public string Name { get; set; }
        public string Lastname { get; set; }
        public string Sexo { get; set; }
        public int Year { get; set; }
        
    }
}
