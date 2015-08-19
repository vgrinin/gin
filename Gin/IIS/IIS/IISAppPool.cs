using System.DirectoryServices;

namespace IISManagement
{
    public class IISAppPool
    {
        private DirectoryEntry _entry = null;

        protected IISAppPool(DirectoryEntry entry)
        {
            this._entry = entry;
        }

        public string Name
        {
            get
            {
                return _entry.Name;
            }
        }

        public void Start()
        {
            this._entry.Invoke("Start");
        }

        public void Stop()
        {
            this._entry.Invoke("Stop");
        }

        public static IISAppPool OpenAppPool(string name)
        {
            string connectStr = "IIS://localhost/W3SVC/AppPools/";
            connectStr += name;

            if (IISAppPool.IsExist(name) == false)
            {
                return null;
            }

            DirectoryEntry entry = new DirectoryEntry(connectStr); 
            return new IISAppPool(entry);
        }

        public static IISAppPool CreateAppPool(string name)
        {
            DirectoryEntry Service = new DirectoryEntry("IIS://localhost/W3SVC/AppPools");
            foreach (DirectoryEntry entry in Service.Children)
            {
                if (entry.Name.Trim().ToLower() == name.Trim().ToLower())
                {
                    return IISAppPool.OpenAppPool(name.Trim());
                }
            }

            DirectoryEntry appPool = Service.Children.Add(name, "IIsApplicationPool");
            appPool.CommitChanges();
            Service.CommitChanges();
            
            return new IISAppPool(appPool);
        }

        public static bool IsExist(string name)
        {
            DirectoryEntry Service = new DirectoryEntry("IIS://localhost/W3SVC/AppPools");
            foreach (DirectoryEntry entry in Service.Children)
            {
             
                if (entry.Name.Trim().ToLower() == name.Trim().ToLower())
                {
                    return true;
                }
               
            }
            return false;
        }

        public static bool DeleteAppPool(string name)
        {
            if (IISAppPool.IsExist(name) == false)
            {
                return false;
            }

            IISAppPool appPool = IISAppPool.OpenAppPool(name);
            appPool._entry.DeleteTree();
            return true;
        }     
    }
}
