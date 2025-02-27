using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace autoClicker
{
    public class AutoClicker
    {
        [DllImport("user32.dll", SetLastError = true)]

        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        Random random = new Random();
        public async Task ExecuteAsync(CancellationToken cancellationToken, ExecutionSchedule executionSchedule, List<ClickParameters> clickSequence, string appName)
        {
            int executionCount = 0;
            Mouse mouse = new Mouse(appName);
            IntPtr hWnd = FindWindow(null, appName);
            int delay = 0;
            while (!cancellationToken.IsCancellationRequested && (stillExecute(executionSchedule, executionCount)))
            {
                foreach (var parameters in clickSequence)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    mouse.SimulateClickDown(GetRandomPointInCircle(parameters.Start.Value, parameters.RNDPoint), hWnd);
                    delay = (int)parameters.WaitDurationTime + random.Next(0, (int)parameters.WaitRNDDurationTime);
                    await Task.Delay(delay, cancellationToken);

                    mouse.SimulateClickUp(GetRandomPointInCircle(parameters.End.Value, parameters.RNDPoint), hWnd);
                    delay = (int)parameters.WaitAfterTime + random.Next(0, (int)parameters.WaitRNDAfterTime);
                    await Task.Delay(delay, cancellationToken);
                }
                executionCount++;
            }
        }
        private bool stillExecute(ExecutionSchedule executionSchedule, int executionCount)
        {
            if (executionSchedule.NumberOfExecution == null && executionSchedule.ExecutionTime == null) return true;
            else if (executionSchedule.NumberOfExecution != null && executionSchedule.NumberOfExecution > executionCount) return true;
            else if (executionSchedule.ExecutionTime != null && executionSchedule.ExecutionTime > DateTime.Now) return true;
            else return false;
        }

        public Point GetRandomPointInCircle(Point center, int radius)
        {
            double theta = random.NextDouble() * 2 * Math.PI;
            double r = Math.Sqrt(random.NextDouble()) * radius;
            return new Point((int)(center.X + r * Math.Cos(theta)), (int)(center.Y + r * Math.Sin(theta)));
        }

    }
}
