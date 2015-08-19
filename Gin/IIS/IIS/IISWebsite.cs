using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.DirectoryServices;
using IISManagement.Exceptions;

namespace IISManagement
{
    public enum IISWebsiteStatus : int
    {
        Starting = 1,
        Started = 2,
        Stopping = 3,
        Stopped = 4,
        Pausing = 5,
        Paused = 6,
        Continuign = 7
    }

    public class IISWebsite
    {
        private DirectoryEntry websiteEntry = null;
        internal const string IIsWebServer = "IIsWebServer";

        protected IISWebsite(DirectoryEntry Server)
        {
            websiteEntry = Server;
        }


        #region Properties

        public string Name
        {
            get
            {
                return this.websiteEntry.Properties["ServerComment"][0].ToString();
            }
            set
            {
                this.websiteEntry.Properties["ServerComment"][0] = value;
                this.websiteEntry.CommitChanges();
            }
        }

        public int Port
        {
            get
            {
                string port = this.websiteEntry.Properties["Serverbindings"][0].ToString();
                port = port.Substring(1);
                port = port.Remove(port.Length - 1, 1);
                return Convert.ToInt32(port);
            }
            set
            {
                this.websiteEntry.Properties["Serverbindings"][0] = ":" + value + ":";
                this.websiteEntry.CommitChanges();
            }
        }

        public IISWebVirturalDir Root
        {
            get
            {
                foreach (DirectoryEntry entry in websiteEntry.Children)
                {
                    if (entry.SchemaClassName == IISWebVirturalDir.IIsVirtualDir)
                    {
                        return new IISWebVirturalDir(entry);
                    }
                }

                throw new WebsiteWithoutRootException(this.Name);
            }
        }

        public IISWebsiteStatus Status
        {
            get
            {
                object status = this.websiteEntry.InvokeGet("Status");
                return (IISWebsiteStatus)status;
            }
        }

        #endregion Properties

        #region Operations

        public void Start()
        {
            this.websiteEntry.Invoke("Start");
        }

        public void Stop()
        {
            this.websiteEntry.Invoke("Stop");
        }

        public void Pause()
        {
            this.websiteEntry.Invoke("Pause");
        }

        public void Continue()
        {
            this.websiteEntry.Invoke("Continue");
        }

        #endregion Operations

        #region Static Methods

        public static IISWebsite CreateWebsite(string name, int port, string rootPath)
        {
            return IISWebsite.CreateWebsite(name, port, rootPath, null);
        }

        public static IISWebsite CreateWebsite(string name, int port, string rootPath, string appPool)
        {
            if (global::System.IO.Directory.Exists(rootPath) == false)
            {
                throw new DirNotFoundException(rootPath);
            }

            DirectoryEntry Services = new DirectoryEntry("IIS://localhost/W3SVC");

            int index = 0;
            foreach (DirectoryEntry server in Services.Children)
            {
                if (server.SchemaClassName == "IIsWebServer")
                {
                    if (server.Properties["ServerComment"][0].ToString() == name)
                    {
                        throw new Exception("website:" + name + " already exsit.");
                    }

                    if (Convert.ToInt32(server.Name) > index)
                    {
                        index = Convert.ToInt32(server.Name);
                    }
                }
            }
            index++; // new index created

            DirectoryEntry Server = Services.Children.Add(index.ToString(), IIsWebServer);
            Server.Properties["ServerComment"].Clear();
            Server.Properties["ServerComment"].Add(name);
            Server.Properties["Serverbindings"].Clear();
            Server.Properties["Serverbindings"].Add(":" + port + ":");

            DirectoryEntry root = Server.Children.Add("ROOT", IISWebVirturalDir.IIsVirtualDir);
            root.Properties["path"].Clear();
            root.Properties["path"].Add(rootPath);

            if (string.IsNullOrEmpty(appPool))
            {
                root.Invoke("appCreate", 0);
            }
            else
            {
                root.Invoke("appCreate3", 0, appPool, true);
            }

            root.Properties["AppFriendlyName"].Clear();
            root.Properties["AppIsolated"].Clear();
            root.Properties["AccessFlags"].Clear();
            root.Properties["FrontPageWeb"].Clear();
            root.Properties["AppFriendlyName"].Add(root.Name);
            root.Properties["AppIsolated"].Add(2);
            root.Properties["AccessFlags"].Add(513);
            root.Properties["FrontPageWeb"].Add(1);

            root.CommitChanges();
            Server.CommitChanges();

            IISWebsite website = new IISWebsite(Server);
            return website;
        }

        public static IISWebsite OpenWebsite(string name)
        {
            DirectoryEntry Services = new DirectoryEntry("IIS://localhost/W3SVC");
            IEnumerator ie = Services.Children.GetEnumerator();
            DirectoryEntry Server = null;

            while (ie.MoveNext())
            {
                Server = (DirectoryEntry)ie.Current;
                if (Server.SchemaClassName == "IIsWebServer")
                {
                    // "ServerComment" means name
                    if (Server.Properties["ServerComment"][0].ToString() == name)
                    {
                        return new IISWebsite(Server);
                    }
                }
            }

            return null;
        }

        public static string[] ExistedWebsites
        {
            get
            {
                List<string> ret = new List<string>();

                DirectoryEntry Services = new DirectoryEntry("IIS://localhost/W3SVC");
                IEnumerator ie = Services.Children.GetEnumerator();
                DirectoryEntry Server = null;

                while (ie.MoveNext())
                {
                    Server = (DirectoryEntry)ie.Current;
                    if (Server.SchemaClassName == "IIsWebServer")
                    {
                        ret.Add(Server.Properties["ServerComment"][0].ToString());
                      
                    }
                }

                return ret.ToArray();
            }
        }

        public static bool DeleteWebsite(string name)
        {
            IISWebsite website = IISWebsite.OpenWebsite(name);
            if (website == null)
            {
                return false;
            }

            website.websiteEntry.DeleteTree();
            return true;
        }

        public static bool IsExist(string name)
        {
            DirectoryEntry Services = new DirectoryEntry("IIS://localhost/W3SVC");
            IEnumerator ie = Services.Children.GetEnumerator();
            DirectoryEntry Server = null;

            while (ie.MoveNext())
            {
                Server = (DirectoryEntry)ie.Current;
                if (Server.SchemaClassName == "IIsWebServer")
                {
                    if (Server.Properties["ServerComment"][0].ToString() == name)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        #endregion Static Methods
    }
}
