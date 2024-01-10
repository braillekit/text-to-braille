using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example2.Config.Sections
{
    public record GeneralSection
    {
        public const string Name = "General";

        public bool AutoUpdate { get; set; } = true;
    }
}
