using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gin
{

    public class CustomInstallationParameter
    {
        public string Name { get; set; }
        public object Value { get; set; }
    }

    public class PackageData
    {
        public DateTime InstallationDate { get; set; }
        public string InstallationUserName { get; set; }
        public List<CustomInstallationParameter> InstallationParameters { get; set; }

        public PackageData()
        {
            InstallationParameters = new List<CustomInstallationParameter>();
        }
    }

    public class SoftwareData
    {
        public string SoftwareName { get; set; }
        public List<CustomInstallationParameter> InstallationParameters { get; set; }

        public SoftwareData()
        {
            InstallationParameters = new List<CustomInstallationParameter>();
        }
    }
}
