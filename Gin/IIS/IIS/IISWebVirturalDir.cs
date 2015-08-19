using System;
using System.Collections.Generic;
using System.Text;
using System.DirectoryServices;
using IISManagement.Exceptions;


namespace IISManagement
{
    public class IISWebVirturalDir
    {
        private DirectoryEntry _entry = null;
        internal const string IIsVirtualDir = "IIsWebVirtualDir";

        internal IISWebVirturalDir(DirectoryEntry entry)
        {
            this._entry = entry;
        }

        #region Properties

        public string Path
        {
            get
            {
                return this._entry.Properties["path"][0].ToString();
            }
            set
            {
                this._entry.Properties["path"][0] = value;
                this._entry.CommitChanges();
            }
        }

        public string Name
        {
            get
            {
                return this._entry.Name;
            }

        }

        #endregion Properties

        #region Operations

        public bool IsExist(string name)
        {
            return this.FindSubEntry(name) != null;
        }

        public IISWebVirturalDir CreateSubVirtualDir(string name, string path, string appPool)
        {
            if (this.IsExist(name))
            {
                throw new VirtualDirAlreadyExistException(this._entry, path);
            }

            if (global::System.IO.Directory.Exists(path) == false)
            {
                throw new DirNotFoundException(path);
            }

            
            DirectoryEntry entry = this._entry.Children.Add(name, IIsVirtualDir);
            entry.Properties["path"].Clear();
            entry.Properties["path"].Add(path);

            if (string.IsNullOrEmpty(appPool))
            {
                entry.Invoke("appCreate", 0);
            }
            else
            {
                entry.Invoke("appCreate3", 0, appPool, true);
            }

            entry.Properties["AppFriendlyName"].Clear();
            entry.Properties["AppIsolated"].Clear();
            entry.Properties["AccessFlags"].Clear();
            entry.Properties["FrontPageWeb"].Clear();
            entry.Properties["AppFriendlyName"].Add(this._entry.Name);
            entry.Properties["AppIsolated"].Add(2);
            entry.Properties["AccessFlags"].Add(513);
            entry.Properties["FrontPageWeb"].Add(1);

            entry.CommitChanges();
            return new IISWebVirturalDir(entry);
        }

        public IISWebVirturalDir CreateSubVirtualDir(string name, string path)
        {
            return CreateSubVirtualDir(name, path, null);
        }

        public IISWebVirturalDir OpenSubVirtualDir(string name)
        {
            DirectoryEntry entry = this.FindSubEntry(name);

            if (entry == null)
            {
                return null;
            }

            return new IISWebVirturalDir(entry);
        }

        public string[] EnumSubVirtualDirs()
        {
            List<string> ret = new List<string>();
            foreach (DirectoryEntry entry in this._entry.Children)
            {
                if (entry.SchemaClassName == IIsVirtualDir)
                {
                    ret.Add(entry.Name);
                }
            }

            return ret.ToArray();
        }

        public bool DeleteSubVirtualDir(string name)
        {
            DirectoryEntry entry = this.FindSubEntry(name);

            if (entry == null)
            {
                return false;
            }

            entry.DeleteTree();

            return true;

        }

        #region Script Map

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
                throw new FileNotFoundException(exefile);
            }

            if (name.IndexOf(".") != 0)
            {
                name = "." + name;
            }
            PropertyValueCollection oldMap = this._entry.Properties["ScriptMaps"];

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
            this._entry.Properties["ScriptMaps"].Add(newMap);
            this._entry.CommitChanges();
            return true;
        }

        #endregion Script Map

        #endregion Operations

        #region internal utils

        protected DirectoryEntry FindSubEntry(string name)
        {
            DirectoryEntry ret = null;
            foreach (DirectoryEntry entry in this._entry.Children)
            {
                if (entry.SchemaClassName == IIsVirtualDir && entry.Name.ToLower() == name.ToLower())
                {
                    ret = entry;
                }
            }

            return ret;
        }

        #endregion internal utils

    }
}
