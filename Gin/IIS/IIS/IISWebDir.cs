using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.DirectoryServices;


namespace IISManagement
{
    public enum IISWebDirType
    {
        IIsWebVirtualDir = 1,
        IIsWebDirectory = 2,
        IISWebDirUnknow = 3
    }

    public class IISWebDir
    {
        private DirectoryEntry server = null;

        private const string IIsVirtualDir = "IIsWebVirtualDir";
        private const string IIsWebDirectory = "IIsWebDirectory";
        private const string DefaultScriptExeFile = @"c:\windows\microsoft.NET\framework\v2.0.50727\aspnet_isapi.dll";

        internal IISWebDir(DirectoryEntry server)
        {
            this.server = server;
        }

        public string Path
        {
            get
            {
                return this.server.Properties["path"][0].ToString();
            }
            set
            {
                this.server.Properties["path"][0] = value;
            }
        }

        public string Name
        {
            get
            {
                return this.server.Name;
            }
        }

        public string SchemaClassName
        {
            get
            {
                return this.server.SchemaClassName;
            }
        }

        public IISWebDirType Type
        {
            get
            {
                if (this.SchemaClassName == IIsVirtualDir)
                {
                    return IISWebDirType.IIsWebVirtualDir;
                }
                else if (this.SchemaClassName == IIsWebDirectory)
                {
                    return IISWebDirType.IIsWebDirectory;
                }
                else
                {
                    return IISWebDirType.IISWebDirUnknow;
                }
            }
        }


        /// <summary>
        /// create application from this dir
        /// </summary>
        /// <param name="appPool">app pool to use, null means default</param>
        public void CreateApplication(IISAppPool appPool)
        {
            if (appPool == null)
            {
                this.server.Invoke("AppCreate", true);
            }
            else
            {
                this.server.Invoke("AppCreate3", new object[] { 0, appPool.Name, true });
            }

            this.server.Properties["AppFriendlyName"].Clear();
            this.server.Properties["AppIsolated"].Clear();
            this.server.Properties["AccessFlags"].Clear();
            this.server.Properties["FrontPageWeb"].Clear();
            this.server.Properties["AppFriendlyName"].Add(this.server.Name);
            this.server.Properties["AppIsolated"].Add(2);
            this.server.Properties["AccessFlags"].Add(513);
            this.server.Properties["FrontPageWeb"].Add(1);
            //siteVDir.Invoke("AppCreate3",   new   object[]   {2,   "DefaultAppPool",   true});     

            this.server.CommitChanges();
        }

        public void CreateApplication(string appPool)
        {
            if (string.IsNullOrEmpty(appPool) == true)
            {
                this.server.Invoke("AppCreate", true);
            }
            else
            {
                this.server.Invoke("AppCreate3", new object[] { 0, appPool, true });
            }

            this.server.Properties["AppFriendlyName"].Clear();
            this.server.Properties["AppIsolated"].Clear();
            this.server.Properties["AccessFlags"].Clear();
            this.server.Properties["FrontPageWeb"].Clear();
            this.server.Properties["AppFriendlyName"].Add(this.server.Name);
            this.server.Properties["AppIsolated"].Add(2);
            this.server.Properties["AccessFlags"].Add(513);
            this.server.Properties["FrontPageWeb"].Add(1);
            //siteVDir.Invoke("AppCreate3",   new   object[]   {2,   "DefaultAppPool",   true});     

            this.server.CommitChanges();
        }

        public bool DeleteApplication()
        {
            try
            {
                DirectoryEntry parent = this.server.Parent;
                object[] param = new object[2];
                param[0] = "IIsWebVirtualDir"; // indicate that the operation target is a virtual directory
                param[1] = this.server.Name; 
                parent.Invoke("Delete", param);
             //   this.server.Invoke("AppDelete", true);
             //   this.server.CommitChanges();
            }
            catch (Exception e)
            {
                string msg = e.Message;
            }
            return true;
        }

        public static bool DeleteVirtualDir(IISWebDir virtualDir)
        {
            if (virtualDir.Type == IISWebDirType.IIsWebVirtualDir)
            {
                virtualDir.DeleteApplication();
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool AddScriptMap(string name)
        {
            return this.AddScriptMap(name, IISWebDir.DefaultScriptExeFile);
        }

        public bool AddScriptMap(string name, string exefile)
        {
            return this.AddScriptMap(name, exefile, 1, "");
        }

        /// <summary>
        /// add script map for application
        /// </summary>
        /// <param name="name">".do" or something like this</param>
        /// <param name="exefile">dll to be loaded</param>
        /// <param name="mask">1 means check "script engine", 4 means check "check file exsit", can be added together</param>
        /// <param name="limitString">limit string</param>
        /// <returns></returns>
        public bool AddScriptMap(string name, string exefile, int mask, string limitString)  
        {
            if (global::System.IO.File.Exists(exefile) == false)
            {
                throw new Exception("file not exist:" + exefile);
            }

            if (name.IndexOf(".") != 0)
            {
                name = "." + name;
            }
            PropertyValueCollection oldMap = this.server.Properties["ScriptMaps"];

            for (int i = 0; i < oldMap.Count; i++)
            {
                string mapFile = oldMap[i].ToString();
                if (mapFile.IndexOf(name) == 0)
                {
                    return false;
                }
            }

            string newMap = name + "," + exefile;
            newMap += "," + mask + ",";   // 1 & 4 means the two options
            newMap += limitString;
            this.server.Properties["ScriptMaps"].Add(newMap);
            this.server.CommitChanges();
            return true;
        }

        public IISWebDir CreateVirtualDir(string name, string path)
        {
            // validate path
            if (global::System.IO.Directory.Exists(path) == false)
            {
                throw new Exception("path:" + path + " not exsit!");
            }

            DirectoryEntry entry = this.server.Children.Add(name, IIsVirtualDir);
            entry.Properties["path"].Clear();
            entry.Properties["path"].Add(path);
            entry.CommitChanges();
            return new IISWebDir(entry);
        }

        public IISWebDir OpenDir(string name)
        {
            foreach (DirectoryEntry entry in this.server.Children)
            {
                if (entry.Name.ToLower() == name.Trim().ToLower())
                {
                    return new IISWebDir(entry);
                }
            }
            throw new Exception("the website directory:" + name + " not found!");
        }
    }
}
