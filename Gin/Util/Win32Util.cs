using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Gin.Util
{
    public static class Win32Util
    {
        public static void ExecuteOrInvoke(Control control, Action action)
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

        public static void ShowError(Form form, string message)
        {
            Win32Util.ExecuteOrInvoke(form, () =>
            {
                MessageBox.Show(form, message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            });
        }

    }
}
