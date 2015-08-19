using System.Diagnostics;
using System.ServiceProcess;
using Gin.WCF;
using System.ServiceModel;


namespace Gin.Service
{
    public partial class MainService : ServiceBase
    {
        private const string EVENTLOG_SOURCE_NAME = "Gin.Service";
        private const string EVENTLOG_NAME = "Gin.Service.Log";

        private ServiceHost host;
        private RemotePackageService remote;

        public MainService()
        {
            InitializeComponent();
            if (!EventLog.SourceExists(EVENTLOG_SOURCE_NAME))
            {
                EventLog.CreateEventSource(
                   EVENTLOG_SOURCE_NAME, EVENTLOG_NAME);
            }
            eventLog1.Source = EVENTLOG_SOURCE_NAME;
            eventLog1.Log = EVENTLOG_NAME;
            remote = new RemotePackageService("");
        }

        protected override void OnStart(string[] args)
        {
            host = new ServiceHost(remote);
            host.Open();

            eventLog1.WriteEntry(EVENTLOG_SOURCE_NAME + " Stopped");
        }

        protected override void OnStop()
        {
            if (host != null)
            {
                host.Close();
                host = null;
            }

            eventLog1.WriteEntry(EVENTLOG_SOURCE_NAME + " Stopped");
        }


    }
}
