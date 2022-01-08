using Rocket.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace EnvyLogging.Configuration
{
    public class EnvyLoggingConfiguration : IRocketPluginConfiguration
    {


        public string WallpaperImage { get; set; }
        [XmlArray("Reglas")]
        public List<string> Reglas { get; set; }
        public string DiscordLink { get; set; }
        public int MinimaEdad { get; set; }
        public int MaximaEdad { get; set; }
        public bool Music { get; set; }
        public void LoadDefaults()
        {
            WallpaperImage = "Url Here";
            Reglas = new List<string>
            {
                "1# Ser un niño muy geimer",
                "2# Marga paso por aca",
                "3# ;v?"
            };

            Music = true;
            MinimaEdad = 18;
            MaximaEdad = 50;

            DiscordLink = "https://discord.gg/Dj2KZ5S9HX";


        }
    }
}
