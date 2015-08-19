using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;


namespace Gin.Installer
{

    public class MainInstallerPanel: Panel, IAdjustableControl
    {

        #region IAdjustableControl Members

        int IAdjustableControl.Width
        {
            get
            {
                return Width;
            }
            set
            {
                SetFormFixedSize(value, -1, null);
            }
        }

        int IAdjustableControl.Height
        {
            get
            {
                return Height;
            }
            set
            {
                SetFormFixedSize(-1, value, null);
            }
        }

        private void SetFormFixedSize(int width, int height, string caption)
        {
            Form mainForm = FindForm();
            if (mainForm == null || !(mainForm is IAdjustableControl))
            {
                return;
            }

            IAdjustableControl iAdjust = (IAdjustableControl)mainForm;
            if (width > -1)
            {
                iAdjust.Width = width;
            }
            if (height > -1)
            {
                iAdjust.Height = height;
            }
            if (caption != null)
            {
                iAdjust.Caption = caption;
            }
        }

        public string Caption
        {
            get
            {
                Form mainForm = FindForm();
                if (mainForm == null)
                {
                    return null;
                } 
                return mainForm.Text;
            }
            set
            {
                Form mainForm = FindForm();
                if (mainForm == null)
                {
                    return;
                } 
                mainForm.Text = value;
            }
        }

        #endregion
    }
}
