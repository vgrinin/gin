using System;
using System.Windows.Forms;

namespace Gin.Controls
{
    public static class UserControlUtil
    {
        public static void ExecuteOrInvoke(Action action, Control control)
        {
            if (control.InvokeRequired)
            {
                control.Invoke(action);
            }
            else
            {
                action();
            }
        }
    }
}
