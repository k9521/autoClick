using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace autoClicker
{
    public partial class MainForm : Form
    {
        private List<ClickParameters> recordList;
        Mouse mouse;
        bool wasPressed, clickToAdd;
        ClickParameters clickRecord;
        ClickerList clickerList;
        ClickParametersEditor editor;
        OverlayForm overlayForm;
        string AppName = "Whiteout Survival";
        AutoClicker autoClicker;
        private CancellationTokenSource cts;
        private bool isRunning = false;

        public MainForm()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            textBoxAppName.Text = AppName;
            mouse = new Mouse(AppName);
            editor = new ClickParametersEditor(this, AppName);
            overlayForm = new OverlayForm(AppName);
            overlayForm.Ycorrection = 30;
            overlayForm.Xcorrection = 10;
            clickerList = new ClickerList("data");
            UpdateSavedList();
            autoClicker = new AutoClicker();
            buttonRunInfinity.Click += ButtonRunAutoClicker;
            buttonRunX.Click += ButtonRunAutoClicker;
            buttonRunUntilTime.Click += ButtonRunAutoClicker;

        }

        private void buttonRecord_Click(object sender, EventArgs e)
        {
            if (!timerRecord.Enabled)
            {
                buttonRecord.Text = "Stop Record";
                recordList = new List<ClickParameters>();
                wasPressed = false;
                clickToAdd = false;
                clickRecord = new ClickParameters();
                editor.RemoveAllItem();
                overlayForm.Clean();
                timerRecord.Enabled = true;
            }
            else
            {
                buttonRecord.Text = "Record";
                timerRecord.Enabled = false;
                fixRecordedList();
                editor.UpdateComponent(recordList, clickerList);
                showPoint(recordList, null, WindowFinder.CreateWindowParam(AppName).Start);
            }
        }

        private void fixRecordedList()
        {
            List<ClickParameters> fixedRecordList = new List<ClickParameters>();
            ClickParameters windowParam = WindowFinder.CreateWindowParam(AppName);
            foreach (ClickParameters clickParameters in recordList)
            {
                if (clickerList.PointInScope(clickParameters.Start, windowParam) && clickerList.PointInScope(clickParameters.End, windowParam))
                {
                    fixedRecordList.Add(clickParameters);
                }
            }
            recordList = fixedRecordList;
        }

        private void showPoint(List<ClickParameters> recordList, string key = null, Point? sizeAfterRecord = null, bool? resize = false)
        {
            Point? size = key != null ? clickerList.WindowParams[key].Start : sizeAfterRecord;
            WindowFinder.ForceAppWindowSize(AppName, size);
            if (resize.Value)
            {
                overlayForm.resetComponent();
            }
            overlayForm.drawPoint(recordList);
        }

        private void clickListSaved_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedIndex = clickListSaved.SelectedIndex;
            if (selectedIndex < 0 || selectedIndex > clickListSaved.Items.Count) return;
            string key = clickListSaved.Items[selectedIndex].ToString();
            recordList = clickerList.Items[key];
            fixRecordedList();
            editor.UpdateComponent(recordList, clickerList);

            showPoint(recordList, key);
        }

        private void timerRecord_Tick(object sender, EventArgs e)
        {
            if (mouse.IsLpmPressed() && !wasPressed)
            {
                if (clickToAdd)
                {
                    clickRecord.WaitAfterTime = (int)(DateTime.Now.Ticks - clickRecord.WaitAfterTime) / 10000;
                    clickToAdd = false;
                    recordList.Add(clickRecord);
                    clickRecord = new ClickParameters();
                }
                wasPressed = true;
                clickRecord.Start = mouse.GetMousePosition();
                clickRecord.WaitDurationTime = (int)DateTime.Now.Ticks;
            }
            else if (!mouse.IsLpmPressed() && wasPressed)
            {
                wasPressed = false;
                clickRecord.End = mouse.GetMousePosition();
                clickRecord.WaitDurationTime = (int)(DateTime.Now.Ticks - clickRecord.WaitDurationTime) / 10000;
                clickRecord.Slide = false;
                clickRecord.WaitAfterTime = (int)DateTime.Now.Ticks;
                clickToAdd = true;
            }
        }

        public void UpdatedPoint(List<ClickParameters> recordList)
        {
            Point settingSize = WindowFinder.CreateWindowParam(AppName).Start.Value;
            this.recordList = recordList;
            showPoint(recordList, null, settingSize, true);

        }

        public void UpdateSavedList()
        {
            clickListSaved.Items.Clear();
            if (clickerList.AllKeys() == null)
            {
                return;
            }
            foreach (var key in clickerList.AllKeys())
            {
                clickListSaved.Items.Add(key);
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            if (isRunning)
            {
                cts?.Cancel();
                setupEnabledComponents(true);
                buttonCancel.Enabled = false;
                return;
            }
        }

        private void setupEnabledComponents(bool enabled)
        {
            buttonRecord.Enabled = enabled;
            clickListSaved.Enabled = enabled;
            buttonRunInfinity.Enabled = enabled;
            buttonRunX.Enabled = enabled;
            buttonRunUntilTime.Enabled = enabled;
            numberOfExecution.Enabled = enabled;
            timeOfExecutionExpired.Enabled = enabled;
            buttonRemoveClickList.Enabled = enabled;
            textBoxAppName.Enabled = enabled;

        }

        private void textBoxAppName_Leave(object sender, EventArgs e)
        {
            AppName = textBoxAppName.Text;
            mouse = new Mouse(AppName);
            editor = new ClickParametersEditor(this, AppName);
            overlayForm = new OverlayForm(AppName);

        }

        private void buttonRemoveClickList_Click(object sender, EventArgs e)
        {
            int selectedIndex = clickListSaved.SelectedIndex;
            if (selectedIndex < 0 || selectedIndex > clickListSaved.Items.Count) return;
            string key = clickListSaved.Items[selectedIndex].ToString();

            clickerList.Items.Remove(key);
            clickerList.WindowParams.Remove(key);
            clickerList.SaveToFile();
            UpdateSavedList();
        }

        private async void ButtonRunAutoClicker(object sender, EventArgs e)
        {
            ExecutionSchedule executionSchedule = new ExecutionSchedule();
            executionSchedule.ExecutionTime = null;
            executionSchedule.NumberOfExecution = null;
            if (sender == buttonRunX)
            {
                executionSchedule.NumberOfExecution = (int)numberOfExecution.Value;
            }
            else if (sender == buttonRunUntilTime)
            {
                executionSchedule.ExecutionTime = timeOfExecutionExpired.Value;
            }

            isRunning = true;
            setupEnabledComponents(false);

            cts = new CancellationTokenSource();
            try
            {
                overlayForm.changeTimer(false);
                buttonCancel.Enabled = true;
                await autoClicker.ExecuteAsync(cts.Token, executionSchedule, recordList, AppName);
            }
            catch (TaskCanceledException)
            {
                setupEnabledComponents(true);
                buttonCancel.Enabled = false;
                overlayForm.changeTimer(true);
            }
            finally
            {
                isRunning = false;
                cts.Dispose();
                cts = null;
                setupEnabledComponents(true);
                buttonCancel.Enabled = false;
                overlayForm.changeTimer(true);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var position = WindowPositionFinder.GetWindowPositionByTitle(AppName);
            if (position.HasValue)
            {
                Console.WriteLine($"Pozycja okna: {position.Value}");
                Mouse.MoveNonRelativeMouse(new Point(position.Value.Left, position.Value.Top));
            }
            else
            {
                Console.WriteLine("Aplikacja nie została znaleziona lub okno jest niewidoczne.");
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            overlayForm.Xcorrection = trackBar1.Value;//10
            overlayForm.drawPoint(recordList);
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            overlayForm.Ycorrection = trackBar2.Value;
            overlayForm.drawPoint(recordList);
        }

        public void ShowPoints(bool isShowPoints)
        {
            overlayForm.changeTimer(isShowPoints);
        }


    }
}
