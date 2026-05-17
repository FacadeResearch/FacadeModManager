using System.Collections.Generic;
using System.Windows.Documents;

namespace FacadeModManager
{
    public class ModManagerSettings
    {
        public string FacadePath { get; set; }

        public List<Mod> InstalledMods = new List<Mod>();
    }
}