using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Gin;
using Gin.PackageContent;

namespace Gin.Builder
{
    public partial class ChooseExportTypeForm : Form
    {
        public ChooseExportTypeForm()
        {
            InitializeComponent();
        }

        public PackageContentType GetResultContentType()
        {
            if (radioGZipAll.Checked)
            {
                return PackageContentType.Packed;
            }
            else if (radioDirect.Checked)
            {
                return PackageContentType.Direct;
            }
            else
            {
                return PackageContentType.Empty;
            }
        }
    }
}
