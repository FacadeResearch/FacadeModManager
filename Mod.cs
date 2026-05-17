using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacadeModManager
{
    public class Mod
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string Version { get; set; }

        public bool Installed { get; set; } = false;
    }
}
