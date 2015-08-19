using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Gin.Installer
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            ProgramStarter starter = new ProgramStarter();
            starter.Start(args);
        }
    }
}
