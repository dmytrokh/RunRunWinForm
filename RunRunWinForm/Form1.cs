using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;

namespace RunRunWinForm
{
    public partial class Form1 : Form
    {
        private CancellationTokenSource _cancellationTokenSource1;
        private CancellationTokenSource _cancellationTokenSource2;

        private Task _run1;
        private Task _run2;

        private readonly Dispatcher dispatcherUI;

        public Form1()
        {
            InitializeComponent();
            dispatcherUI = Dispatcher.CurrentDispatcher;
        }

        private void btStart1_Click(object sender, EventArgs e)
        {
            if ((_run1 != null) && (_run1.IsCompleted == false ||
                           _run1.Status == TaskStatus.Running ||
                           _run1.Status == TaskStatus.WaitingToRun ||
                           _run1.Status == TaskStatus.WaitingForActivation))
            {
                return;
            }

            _cancellationTokenSource1 = new CancellationTokenSource();

            _run1 = Task.Run(async () =>
            {
                await DoWorkAsync(label1, _cancellationTokenSource1.Token);
            });
        }

        private void btStop1_Click(object sender, EventArgs e)
        {
            _cancellationTokenSource1.Cancel();
        }

        private void btStart2_Click(object sender, EventArgs e)
        {
            if ((_run2 != null) && (_run2.IsCompleted == false ||
                           _run2.Status == TaskStatus.Running ||
                           _run2.Status == TaskStatus.WaitingToRun ||
                           _run2.Status == TaskStatus.WaitingForActivation))
            {
                return;
            }

            _cancellationTokenSource2 = new CancellationTokenSource();

            _run2 = Task.Run(async () =>
            {
                await DoWorkAsync(label2, _cancellationTokenSource2.Token);
            });
        }

        private void btStop2_Click(object sender, EventArgs e)
        {
            _cancellationTokenSource2.Cancel();
        }

        private async Task DoWorkAsync(Label label, CancellationToken token)
        {
            int cnt = 0;

            DispatcherOperation disp = null;

            while (true)
            {
                //await Task.Delay(1, token);
                if (token.IsCancellationRequested)
                {
                    break;
                }

                cnt++;

                if (disp == null || disp.Status == DispatcherOperationStatus.Completed
                        || disp.Status == DispatcherOperationStatus.Aborted)
                {
                    disp = dispatcherUI.BeginInvoke(new Action(() =>
                        {
                            label.Text = cnt.ToString();
                        }), DispatcherPriority.Background);
                }
            }
        }
    }
}
