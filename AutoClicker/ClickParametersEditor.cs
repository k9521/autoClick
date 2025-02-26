using autoClicker;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
using Label = System.Windows.Forms.Label;


namespace autoClicker
{
    public class ClickParametersEditor
    {
        private MainForm targetForm;
        //private List<ClickParameters> clickerParameterList;
        private ClickerList clickerList;
        private TextBox keyTextBox;
        private Panel itemsPanel;
        private List<Control> itemControls = new List<Control>();
        private string AppName;
        private Timer ShowPointTimer;
        private Mouse mouse;
        private bool wasPressed = false, clickToAdd = false;
        private ClickParameters clickRecord;
        public ClickParametersEditor(MainForm form, string appName)
        {
            this.targetForm = form;
            this.AppName = appName;
            ShowPointTimer = new Timer();
            ShowPointTimer.Tick += ShowPointTimer_Tick;
            mouse = new Mouse(appName);
            InitializeComponents();
        }

        private void ShowPointTimer_Tick(object sender, EventArgs e)
        {

            if (WindowFinder.GetActiveWindowTitle() == AppName && mouse.IsLpmPressed() && !wasPressed)
            {
                if (clickToAdd)
                {
                    clickToAdd = false;
                    clickRecord = new ClickParameters();
                }
                wasPressed = true;
                clickRecord.Start = mouse.GetMousePosition();
            }
            else if (WindowFinder.GetActiveWindowTitle() == AppName && !mouse.IsLpmPressed() && wasPressed)
            {
                wasPressed = false;
                clickRecord.End = mouse.GetMousePosition();
                clickToAdd = true;
                Control item = (Control)ShowPointTimer.Tag;
                ShowPointTimer.Enabled = false;
                updatePoint(item, clickRecord);
            }
        }

        private void updatePoint(Control item, ClickParameters clickRecord)
        {
            var cordsSize = WindowFinder.GetWindowPosition(AppName);
            if((clickRecord.Start.Value.X < 0 || clickRecord.Start.Value.Y < 0) ||
                (clickRecord.End.Value.X < 0 || clickRecord.End.Value.Y < 0) ||
                (clickRecord.Start.Value.X > cordsSize.Value.Right - cordsSize.Value.Left || clickRecord.Start.Value.Y > cordsSize.Value.Bottom - cordsSize.Value.Top) ||
                (clickRecord.End.Value.X > cordsSize.Value.Right - cordsSize.Value.Left || clickRecord.End.Value.Y > cordsSize.Value.Bottom - cordsSize.Value.Top))
            {
                clickRecord.Start = new Point(0, 0);
                clickRecord.End = new Point(0, 0);
            }
            foreach (NumericUpDown control in item.Controls.OfType<NumericUpDown>())
            {
                string tag = control?.Tag?.ToString();
                if (!string.IsNullOrEmpty(tag))
                {
                    switch (tag)
                    {
                        case "StartX":
                            {
                                control.Value = clickRecord.Start.Value.X;
                                break;
                            };
                        case "StartY":
                            {
                                control.Value = clickRecord.Start.Value.Y;
                                break;
                            };
                        case "EndX":
                            {
                                control.Value = clickRecord.End.Value.X;
                                break;
                            };
                        case "EndY":
                            {
                                control.Value = clickRecord.End.Value.Y;
                                break;
                            };
                    }
                }

            }
            targetForm.Enabled = true;
        }

        public void UpdateComponent(List<ClickParameters> clickerParameterList, ClickerList clickerList)
        {
            //this.clickerParameterList = clickerParameterList;
            this.clickerList = clickerList;
            RemoveAllItem();
            foreach (ClickParameters cp in clickerParameterList)
            {
                AddNewItem(cp);
            }
            RefreshItemsLayout();
        }

        private void InitializeComponents()
        {
            var scrollContainer = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true
            };

            var keySection = new GroupBox
            {
                Text = "Key",
                Location = new Point(10, 10),
                Size = new Size(600, 60)
            };

            keyTextBox = new TextBox
            {
                Location = new Point(10, 20),
                Width = 580
            };
            keySection.Controls.Add(keyTextBox);

            var buttonPanel = new Panel
            {
                Location = new Point(10, 80),
                Size = new Size(600, 40)
            };

            var addButton = new Button
            {
                Text = "Add Item",
                Location = new Point(0, 0),
                Size = new Size(80, 30)
            };
            addButton.Click += (s, e) => AddNewItem();

            var saveButton = new Button
            {
                Text = "Save All",
                Location = new Point(90, 0),
                Size = new Size(80, 30)
            };
            saveButton.Click += (s, e) => SaveAll();

            var showPoints = new CheckBox
            {
                Text = "Show points",
                Checked = true,
                Location = new Point(180, 0)
            };
            showPoints.CheckedChanged += (s, e) => targetForm.ShowPoints(showPoints.Checked);
            
            buttonPanel.Controls.AddRange(new Control[] { addButton, saveButton, showPoints });

            itemsPanel = new Panel
            {
                Location = new Point(10, 130),
                Size = new Size(600, 400),
                AutoSize = true
            };

            scrollContainer.Controls.AddRange(new Control[] { keySection, buttonPanel, itemsPanel });
            targetForm.Controls.Add(scrollContainer);
        }

        public void RemoveAllItem()
        {
            while (itemsPanel.Controls.Count > 0)
            {
                Control item = itemsPanel.Controls[0];
                itemsPanel.Controls.Remove(item);
                itemControls.Remove(item);
            }
            RefreshItemsLayout();
        }


        public void AddNewItem(ClickParameters parameters = null)
        {
            var itemControl = CreateItemControl(parameters ?? new ClickParameters());
            itemsPanel.Controls.Add(itemControl);
            itemControls.Add(itemControl);
            RefreshItemsLayout();
            UpdateGroupNameComponent();
        }

        private Control CreateItemControl(ClickParameters parameters)
        {
            if(parameters.Start == null)
            {
                parameters.Start = new Point(0, 0);
            }
            if (parameters.End == null)
            {
                parameters.End = new Point(0, 0);
            }
            var group = new GroupBox
            {
                Text = "Click Parameters",
                Size = new Size(580, 240),
                Tag = parameters
            };
            var yPos = 20;


            AddXYLabels(group, ref yPos);
            AddPointControl(group, "Start", parameters.Start, (p) => { parameters.Start = p; targetForm.UpdatedPoint(CreateList());}, ref yPos);
            AddPointControl(group, "End", parameters.End, (p) => { parameters.End = p; targetForm.UpdatedPoint(CreateList());}, ref yPos);
            AddNumericField(group, "RND Point", parameters.RNDPoint, v => { parameters.RNDPoint = v; targetForm.UpdatedPoint(CreateList());}, ref yPos);

            var slideCheck = new CheckBox
            {
                Text = "Slide",
                Checked = parameters.Slide,
                Location = new Point(10, yPos)
            };
            slideCheck.CheckedChanged += (s, e) => parameters.Slide = slideCheck.Checked;
            group.Controls.Add(slideCheck);
            yPos += 30;

            AddNumericField(group, "Wait Duration", parameters.WaitDurationTime, v => parameters.WaitDurationTime = v, ref yPos);
            yPos -= 30;
            AddNumericField(group, "RND Duration", parameters.WaitRNDDurationTime, v => parameters.WaitRNDDurationTime = v, ref yPos , 240);
            AddNumericField(group, "Wait After", parameters.WaitAfterTime, v => parameters.WaitAfterTime = v, ref yPos);
            yPos -= 30;
            AddNumericField(group, "RND After Time", parameters.WaitRNDAfterTime, v => parameters.WaitRNDAfterTime = v, ref yPos , 240);


            var buttonPanel = new Panel
            {
                Location = new Point(460, yPos - 120),
                Size = new Size(90, 120)
            };

            var showPoint = new Button
            {
                Text = "Show Point",
                Size = new Size(90, 25),
                Location = new Point(0, 0),
            };
            showPoint.Click += (s, e) => ShowPointItem(group);

            var deleteButton = new Button
            {
                Text = "Delete",
                Size = new Size(90, 25),
                Location = new Point(0, 30),
            };
            deleteButton.Click += (s, e) => RemoveItem(group);

            var addBeforeBtn = new Button
            {
                Text = "Add before",
                Size = new Size(90, 25),
                Location = new Point(0, 60)
            };
            addBeforeBtn.Click += (s, e) => AddItemRelative(group, true);

            var addAfterBtn = new Button
            {
                Text = "Add after",
                Size = new Size(90, 25),
                Location = new Point(0, 90)
            };
            
            addAfterBtn.Click += (s, e) => AddItemRelative(group, false);

            buttonPanel.Controls.AddRange(new[] { showPoint, deleteButton, addBeforeBtn, addAfterBtn });
            group.Controls.Add(buttonPanel);

            return group;
        }

        private void AddItemRelative(Control referenceControl, bool insertBefore)
        {
            var index = itemControls.IndexOf(referenceControl);
            if (index == -1) return;

            index = insertBefore ? index : index + 1;

            var newParams = new ClickParameters();
            newParams.Start = new Point(0, 0);
            newParams.End = new Point(0, 0);
            var newControl = CreateItemControl(newParams);

            itemControls.Insert(index, newControl);
            itemsPanel.Controls.Add(newControl);
            RefreshItemsLayout();
            UpdateGroupNameComponent();
            targetForm.UpdatedPoint(CreateList());
        }

        private void UpdateGroupNameComponent()
        {
            foreach(var control in itemsPanel.Controls)
            {
                if(control.GetType() == typeof(GroupBox)){
                    (control as GroupBox).Text = "Click Parameters: #" + (((control as GroupBox).Top/210)+1);
                }
            }
        }

        private void AddXYLabels(Control parent, ref int yPos)
        {
            var lblX = new Label
            {
                Text = "X",
                Location = new Point(120, yPos),
                AutoSize = true
            };

            var lblY = new Label
            {
                Text = "Y",
                Location = new Point(240, yPos),
                AutoSize = true
            };
            parent.Controls.AddRange(new Control[] { lblX, lblY });
            yPos += 30;
        }

        private void AddPointControl(Control parent, string label, Point? value,
                                   Action<Point?> setter, ref int yPos)
        {
            var lbl = new Label
            {
                Text = label,
                Location = new Point(10, yPos),
                AutoSize = true
            };

            var xBox = new NumericUpDown
            {
                Location = new Point(120, yPos),
                Enabled = true,
                Minimum = int.MinValue,
                Maximum = int.MaxValue,
                Value = value?.X ?? 0,
                Tag = label + "X"
            };

            var yBox = new NumericUpDown
            {
                Location = new Point(240, yPos),
                Enabled = true,
                Minimum = 0,
                Maximum= int.MaxValue,
                Value = value?.Y ?? 0,
                Tag = label + "Y"
            };

            xBox.ValueChanged += (s, e) => setter(new Point((int)xBox.Value, (int)yBox.Value));
            yBox.ValueChanged += (s, e) => setter(new Point((int)xBox.Value, (int)yBox.Value));

            parent.Controls.AddRange(new Control[] { lbl, xBox, yBox });
            yPos += 30;
        }

        private void AddNumericField(Control parent, string label, int value,
                                   Action<int> setter, ref int yPos, int xPos = 10)
        {
            var lbl = new Label
            {
                Text = label,
                Location = new Point(xPos, yPos),
                AutoSize = true
            };

            var numBox = new NumericUpDown
            {
                Minimum = 0,
                Maximum = int.MaxValue,
                Width = 100,
                Value = value,
                Location = new Point(xPos + 110, yPos)
            };

            numBox.ValueChanged += (s, e) => setter((int)numBox.Value);
            parent.Controls.AddRange(new Control[] { lbl, numBox });
            yPos += 30;
        }

        private void RemoveItem(Control item)
        {
            itemsPanel.Controls.Remove(item);
            itemControls.Remove(item);
            RefreshItemsLayout();
            UpdateGroupNameComponent();
            targetForm.UpdatedPoint(CreateList());
        }

        private void ShowPointItem(Control item)
        {
            targetForm.Enabled = false;
            clickToAdd = true;
            wasPressed = false;
            ShowPointTimer.Tag = item;
            ShowPointTimer.Enabled = true;
        }

        private void RefreshItemsLayout()
        {
            var yPos = 0;
            foreach (var control in itemControls)
            {
                control.Location = new Point(0, yPos);
                yPos += control.Height + 10;
            }
            itemsPanel.Height = yPos;
            targetForm.Refresh();
        }

        private void SaveAll()
        {
            var key = keyTextBox.Text;
            if (string.IsNullOrWhiteSpace(key)) return;

            if(clickerList == null)
            {
                clickerList = new ClickerList("data");
            }
            clickerList.Items[key] = CreateList();
            clickerList.WindowParams[key] = WindowFinder.CreateWindowParam(AppName);
            clickerList.SaveToFile();
            RemoveAllItem();
            targetForm.UpdateSavedList();
        }

        private List<ClickParameters> CreateList()
        {
            var parameters = new List<ClickParameters>();
            foreach (GroupBox control in itemControls)
            {
                if (control.Tag is ClickParameters cp)
                    parameters.Add(cp);
            }
            return parameters;
        }
    }
}
