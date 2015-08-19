using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;


namespace Gin.Util
{
    public class CancellingExecutor
    {

        private const int LOOP_PERIOD = 500;

        private Func<bool> _checkPendingCancel;
        private Thread _worker;
        private bool _actionComplete = false;
        private bool _pendingCancel = false;

        public CancellingExecutor(Func<bool> checkPendingCancel)
        {
            _checkPendingCancel = checkPendingCancel;
        }

        public void Execute(Action executingAction)
        {
            _worker = new Thread(DoWorker);
            _worker.Start(executingAction);
            MainLoop();
            if (_pendingCancel)
            {
                _worker.Abort();
            }
        }

        private void DoWorker(object arg)
        {
            Action action = (Action)arg;
            action();
            _actionComplete = true;
        }

        private void MainLoop()
        {
            while (!_actionComplete && !_pendingCancel)
            {
                Thread.Sleep(LOOP_PERIOD);
                _pendingCancel = _checkPendingCancel();
            }
        }
    }
}
