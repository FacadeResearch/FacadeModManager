using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacadeModManager
{
    public class Mod
    {
        public required string Name { get; set; }

        public required string Description { get; set; }

        public bool Installed { get; set; }

        public DateTime LastPushedDate { get; set; }
        public bool NeedsUpdate { get; set; }
    }
}
