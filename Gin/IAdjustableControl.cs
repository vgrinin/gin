using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gin
{
    public interface IAdjustableControl
    {
        int Width { get; set; }
        int Height { get; set; }
        string Caption { get; set; }
    }
}
