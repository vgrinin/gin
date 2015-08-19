using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gin.Commands;

namespace Gin.Logging
{
    public class Logging
    {

        #region Интерфейс логгеров

        public bool AutoFlushLoggers { get; set; }

        private List<ExecutionLogger> _loggers = new List<ExecutionLogger>();

        public void AddLogger(ExecutionLogger logger)
        {
            _loggers.Add(logger);
        }

        public void RemoveLogger(ExecutionLogger logger)
        {
            _loggers.Remove(logger);
        }

        public void AddLogEvent(ExecutionLoggerEventType type, string message)
        {
            IterateLoggers((logger)=>{
                logger.Event(type, message);
            });
        }

        public void AddLogException(Exception exception)
        {
            IterateLoggers((logger) =>
            {
                logger.Exception(exception);
            });
        }

        public void AddLogException(string message)
        {
            IterateLoggers((logger) =>
            {
                logger.Error(message);
            });
        }

        public void AddLogInformation(string message)
        {
            IterateLoggers((logger) =>
            {
                logger.Information(message);
            });
        }

        public void AddLogInformation(string message, LogMessageFilter filter)
        {
            IterateLoggers((logger) =>
            {
                logger.Information(filter.Filter(message));
            });
        }

        public void AddLogWarning(string message)
        {
            IterateLoggers((logger) =>
            {
                logger.Warning(message);
            });
        }

        public void AddLogError(string message)
        {
            IterateLoggers((logger) =>
            {
                logger.Error(message);
            });
        }

        public void AddUserInfo(UserInfoData info)
        {
            IterateLoggers((logger) =>
            {
                logger.UserInfo(info);
            });
        }

        public void FlushLogger(bool releaseResource)
        {
            IterateLoggers((logger) =>
            {
                logger.Flush(releaseResource);
            });
        }

        private void IterateLoggers(Action<ExecutionLogger> action)
        {
            _loggers.ForEach((logger) =>
            {
                if (logger != null)
                {
                    action(logger);
                    if (AutoFlushLoggers)
                    {
                        logger.Flush(false);
                    }
                }
            });
        }

        #endregion

        #region Интерфейс прогрессбара

        private ExecutionProgress _progress = null;
        
        public void SetProgress(ExecutionProgress progress)
        {
            _progress = progress;
        }

        public void SendProgress(ExecutionProgressInfo info)
        {
            if (_progress != null)
            {
                _progress.Progress(info);
            }
        }

        public int ProgressTotalCost
        {
            get
            {
                if (_progress != null)
                {
                    return _progress.TotalCost;
                }
                return 0;
            }
            set
            {
                if (_progress != null)
                {
                    _progress.TotalCost = value;
                }
            }
        }

        public int CurrentProgress
        {
            get
            {
                if (_progress != null)
                {
                    return _progress.Current;
                }
                return 0;
            }
            set
            {
                if (_progress != null)
                {
                    _progress.Current = value;
                }
            }
        }

        #endregion


        #region Cancel members

        private bool _pendingCancel = false;

        public void GetPendingCancel(QueryCancelEventArgs args)
        {
            args.Cancel = _pendingCancel;
        }

        public void SetPendingCancel()
        {
            _pendingCancel = true;
        }

        #endregion
    }
}
