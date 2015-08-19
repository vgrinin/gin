using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gin.Editors
{
    public interface IEditor
    {
        object Value { get; set; }
    }

    public interface ITemplatedEditor
    {
        void InsertAtCurrent(string value);
    }
}
