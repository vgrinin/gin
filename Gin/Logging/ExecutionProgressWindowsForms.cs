using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;


namespace Gin.Logging
{

    public class ExecutionProgressWindowsForms : ExecutionProgress
    {

        public ExecutionProgressWindowsForms(ProgressBar progressBar, Label progressLabel)
        {
            _progressBar = progressBar;
            _progressLabel = progressLabel;
        }

        private ProgressBar _progressBar;
        private Label _progressLabel;

        protected override void VisualizeProgress(ExecutionProgressInfo progressInfo)
        {
            if (_progressBar != null)
            {
                if (_progressBar.InvokeRequired)
                {
                    _progressBar.Invoke(new Action(() =>
                    {
                        if (Current <= _totalCost)
                        {
                            _progressBar.Maximum = _totalCost;
                            _progressBar.Value = Current;
                        }
                    }), null);
                }
                else
                {
                    if (Current <= _totalCost)
                    {
                        _progressBar.Maximum = _totalCost;
                        _progressBar.Value = Current;
                    }
                }
            }

            if (_progressLabel != null)
            {
                _progressLabel.Invoke(new Action(() => { _progressLabel.Text = progressInfo.ModuleName + ": " + progressInfo.Message; }), null);
            }
        }
    }
}
