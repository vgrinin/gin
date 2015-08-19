using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.IO;
using System.Configuration;
using Gin.Logging;
using Gin;
using System.Threading;


namespace Gin.WCF
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, IncludeExceptionDetailInFaults = true)]
    public class RemotePackageService: IRemotePackageService
    {

        private Dictionary<string, RemotePackage> _packages;
        private object _locker = new object();

        public const string COOKIES_PATH = @"cookies";

        private string _ginRoot;

        //private Timer _timer;
        private const int AUTOROLLBACK_CHECK_PERIOD = 30;

        public RemotePackageService(string ginRoot)
        {
            _ginRoot = ginRoot;
            InitMetadata();
            LoadCookies();
           // _timer = new Timer(checkAutoRollback, null, AUTOROLLBACK_CHECK_PERIOD, AUTOROLLBACK_CHECK_PERIOD);
        }

        private void checkAutoRollback(object state)
        {
            DoAutoRollback();
        }

        private void LoadCookies()
        {
            string cookiesPath = Path.Combine(_ginRoot, COOKIES_PATH);
            string[] cookiePaths = Directory.GetDirectories(cookiesPath);
            _packages = new Dictionary<string, RemotePackage>();
            foreach (string cookiePath in cookiePaths)
            {
                string cookie = cookiePath.Split('\\').Last();
                RemotePackage rp = new RemotePackage(_ginRoot, cookie, null);
                lock (_locker)
                {
                    _packages.Add(cookie, rp);
                }
            }
        }

        private void DoAutoRollback()
        {
            lock (_locker)
            {
                foreach (var pair in _packages)
                {
                    RemotePackage package = pair.Value;
                    package.AutoRollback();
                }
            }
        }

        public string Invoke(string filePath)
        {
            string cookie = Guid.NewGuid().ToString("N");
            RemotePackage package = new RemotePackage(_ginRoot, cookie, filePath);
            lock (_locker)
            {
                _packages.Add(cookie, package);
            }
            package.Invoke();
            return cookie;
        }

        private void InitMetadata()
        {
            GinMetaData metadata = GinMetaData.GetInstance();
            string pluginsPath = Path.Combine(_ginRoot, "Plugins");
            metadata.Plugin(pluginsPath);
            GinSerializer.IncludeTypes(metadata.IncludedTypes);
        }

        public RemotePackageState GetState(string cookie)
        {
            RemotePackage package = GetPackage(cookie);
            return package.GetState();
        }

        public void Abort(string cookie)
        {
            RemotePackage package = GetPackage(cookie);
            package.Abort();
        }

        public void Commit(string cookie)
        {
            RemotePackage package = GetPackage(cookie);
            package.Commit();
        }

        public void Rollback(string cookie)
        {
            RemotePackage package = GetPackage(cookie);
            package.Rollback();
        }

        private RemotePackage GetPackage(string cookie)
        {
            lock (_locker)
            {
                if (!_packages.ContainsKey(cookie))
                {
                    throw new ArgumentOutOfRangeException("Пакет с cookie = " + cookie + " не существует");
                }
                return _packages[cookie];
            }
        }
    }
}
