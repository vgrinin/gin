using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gin.Install.Installer.Properties;
using Gin.Install.Installer.Properties;
using System.Reflection;
using System.IO;
using System.Diagnostics;

namespace Gin.Install.Installer
{
    class Program
    {
        static void Main(string[] args)
        {
            string exeFilePath = Assembly.GetExecutingAssembly().Location;
            string exeDirPath = Path.GetDirectoryName(exeFilePath);
            if (!exeDirPath.EndsWith(@"\"))
            {
                exeDirPath += @"\";
            }
            string installerExePath = Path.Combine(exeDirPath, "Gin.Installer.exe");
            ResourceSaver.SaveResource(Resources.Interop_DTS, Path.Combine(exeDirPath, "Interop.DTS.dll"));
            ResourceSaver.SaveResource(Resources.Interop_IWshRuntimeLibrary, Path.Combine(exeDirPath, "Interop.IWshRuntimeLibrary.dll"));
            ResourceSaver.SaveResource(Resources.Microsoft_Office_Interop_Excel, Path.Combine(exeDirPath, "Microsoft.Office.Interop.Excel.dll"));
            ResourceSaver.SaveResource(Resources.Microsoft_Vbe_Interop, Path.Combine(exeDirPath, "Microsoft.Vbe.Interop.dll"));
            ResourceSaver.SaveResource(Resources.FlexCelWinforms, Path.Combine(exeDirPath, "FlexCelWinforms.dll"));
            ResourceSaver.SaveResource(Resources.Avicomp_Common, Path.Combine(exeDirPath, "Avicomp.Common.dll"));
            ResourceSaver.SaveResource(Resources.Avicomp_Installer, Path.Combine(exeDirPath, "Avicomp.Installer.dll"));
            ResourceSaver.SaveResource(Resources.FlexCel, Path.Combine(exeDirPath, "FlexCel.dll"));
            ResourceSaver.SaveResource(Resources.Gin, Path.Combine(exeDirPath, "Gin.dll"));
            ResourceSaver.SaveResource(Resources.Gin_Installer, installerExePath);
            ResourceSaver.SaveResource(Resources.Gin_Builder, Path.Combine(exeDirPath, "Gin.Builder.exe"));
            ResourceSaver.SaveResource(Resources.Gin_Installer_exe_config, Path.Combine(exeDirPath, "Gin.Installer.exe.config.tpl"));
            ResourceSaver.SaveResource(Resources.package, Path.Combine(exeDirPath, "package.gin"));
            string configText = Resources.Gin_Installer_exe;
            configText = configText.Replace("{ROOT_PATH}", exeDirPath);
            ResourceSaver.SaveResource(configText, Path.Combine(exeDirPath, "Gin.Installer.exe.config"));
            ResourceSaver.SaveResource(Resources.Gin_Builder_exe_config, Path.Combine(exeDirPath, "Gin.Builder.exe.config.tpl"));
            ResourceSaver.CreateDirectory(Path.Combine(exeDirPath, "templates"));
            ResourceSaver.CreateDirectory(Path.Combine(exeDirPath, @"templates\Синхронизация"));
            ResourceSaver.CreateDirectory(Path.Combine(exeDirPath, @"templates\Синхронизация\Полная"));
            ResourceSaver.SaveResource(Resources.package3, Path.Combine(exeDirPath, @"templates\Синхронизация\Полная\package.gin"));
            ResourceSaver.CreateDirectory(Path.Combine(exeDirPath, @"templates\Синхронизация\Полная с обновлением DTS"));
            ResourceSaver.SaveResource(Resources.package2, Path.Combine(exeDirPath, @"templates\Синхронизация\Полная с обновлением DTS\package.gin"));
            ResourceSaver.CreateDirectory(Path.Combine(exeDirPath, @"templates\Синхронизация\Простая 29 и 30"));
            ResourceSaver.SaveResource(Resources.package1, Path.Combine(exeDirPath, @"templates\Синхронизация\Простая 29 и 30\package.gin"));
            Process process = new Process();
            process.StartInfo.FileName = installerExePath;
            process.Start();
            process.WaitForExit();
            ResourceSaver.Clean();
            ResourceSaver.TryDeleteDirectory(Path.Combine(exeDirPath, "packages"));
            ResourceSaver.TryDeleteDirectory(Path.Combine(exeDirPath, "temp"));
            ResourceSaver.TryDeleteFile(Path.Combine(exeDirPath, "Gin.dll"));
        }
    }
}
