using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example2.Config.Sections
{
    public record PrintingSection
    {
        public const string Name = "Printing";

        public bool PrintPageFoot { get; set; } = true;

        public double PrintTextFontSize { get; set; } = 12;
    }

}
