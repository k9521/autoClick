namespace autoClicker
{
    partial class MainForm
    {
        /// <summary>
        /// Wymagana zmienna projektanta.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Wyczyść wszystkie używane zasoby.
        /// </summary>
        /// <param name="disposing">prawda, jeżeli zarządzane zasoby powinny zostać zlikwidowane; Fałsz w przeciwnym wypadku.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Kod generowany przez Projektanta formularzy systemu Windows

        /// <summary>
        /// Metoda wymagana do obsługi projektanta — nie należy modyfikować
        /// jej zawartości w edytorze kodu.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.buttonRecord = new System.Windows.Forms.Button();
            this.timerRecord = new System.Windows.Forms.Timer(this.components);
            this.clickListSaved = new System.Windows.Forms.ListBox();
            this.buttonRunInfinity = new System.Windows.Forms.Button();
            this.buttonRunX = new System.Windows.Forms.Button();
            this.buttonRunUntilTime = new System.Windows.Forms.Button();
            this.numberOfExecution = new System.Windows.Forms.NumericUpDown();
            this.timeOfExecutionExpired = new System.Windows.Forms.DateTimePicker();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.labelAppName = new System.Windows.Forms.Label();
            this.textBoxAppName = new System.Windows.Forms.TextBox();
            this.buttonRemoveClickList = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numberOfExecution)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonRecord
            // 
            this.buttonRecord.Location = new System.Drawing.Point(631, 216);
            this.buttonRecord.Name = "buttonRecord";
            this.buttonRecord.Size = new System.Drawing.Size(157, 34);
            this.buttonRecord.TabIndex = 3;
            this.buttonRecord.Text = "Record";
            this.buttonRecord.UseVisualStyleBackColor = true;
            this.buttonRecord.Click += new System.EventHandler(this.buttonRecord_Click);
            // 
            // timerRecord
            // 
            this.timerRecord.Interval = 10;
            this.timerRecord.Tick += new System.EventHandler(this.timerRecord_Tick);
            // 
            // clickListSaved
            // 
            this.clickListSaved.FormattingEnabled = true;
            this.clickListSaved.Location = new System.Drawing.Point(631, 79);
            this.clickListSaved.Name = "clickListSaved";
            this.clickListSaved.Size = new System.Drawing.Size(157, 134);
            this.clickListSaved.TabIndex = 4;
            this.clickListSaved.SelectedIndexChanged += new System.EventHandler(this.clickListSaved_SelectedIndexChanged);
            // 
            // buttonRunInfinity
            // 
            this.buttonRunInfinity.Location = new System.Drawing.Point(631, 256);
            this.buttonRunInfinity.Name = "buttonRunInfinity";
            this.buttonRunInfinity.Size = new System.Drawing.Size(157, 34);
            this.buttonRunInfinity.TabIndex = 5;
            this.buttonRunInfinity.Text = "Run until stop";
            this.buttonRunInfinity.UseVisualStyleBackColor = true;
            // 
            // buttonRunX
            // 
            this.buttonRunX.Location = new System.Drawing.Point(631, 296);
            this.buttonRunX.Name = "buttonRunX";
            this.buttonRunX.Size = new System.Drawing.Size(88, 34);
            this.buttonRunX.TabIndex = 6;
            this.buttonRunX.Text = "Run X times";
            this.buttonRunX.UseVisualStyleBackColor = true;
            // 
            // buttonRunUntilTime
            // 
            this.buttonRunUntilTime.Location = new System.Drawing.Point(631, 336);
            this.buttonRunUntilTime.Name = "buttonRunUntilTime";
            this.buttonRunUntilTime.Size = new System.Drawing.Size(88, 34);
            this.buttonRunUntilTime.TabIndex = 7;
            this.buttonRunUntilTime.Text = "Run until time:";
            this.buttonRunUntilTime.UseVisualStyleBackColor = true;
            // 
            // numberOfExecution
            // 
            this.numberOfExecution.Location = new System.Drawing.Point(725, 305);
            this.numberOfExecution.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numberOfExecution.Name = "numberOfExecution";
            this.numberOfExecution.Size = new System.Drawing.Size(63, 20);
            this.numberOfExecution.TabIndex = 8;
            // 
            // timeOfExecutionExpired
            // 
            this.timeOfExecutionExpired.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.timeOfExecutionExpired.Location = new System.Drawing.Point(725, 341);
            this.timeOfExecutionExpired.Name = "timeOfExecutionExpired";
            this.timeOfExecutionExpired.Size = new System.Drawing.Size(63, 20);
            this.timeOfExecutionExpired.TabIndex = 9;
            this.timeOfExecutionExpired.Value = new System.DateTime(2025, 2, 24, 2, 7, 0, 0);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Enabled = false;
            this.buttonCancel.Location = new System.Drawing.Point(631, 376);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(157, 34);
            this.buttonCancel.TabIndex = 11;
            this.buttonCancel.Text = "Cancel AutoClick";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // labelAppName
            // 
            this.labelAppName.AutoSize = true;
            this.labelAppName.Location = new System.Drawing.Point(628, 16);
            this.labelAppName.Name = "labelAppName";
            this.labelAppName.Size = new System.Drawing.Size(60, 13);
            this.labelAppName.TabIndex = 12;
            this.labelAppName.Text = "App Name:";
            // 
            // textBoxAppName
            // 
            this.textBoxAppName.Location = new System.Drawing.Point(688, 13);
            this.textBoxAppName.Name = "textBoxAppName";
            this.textBoxAppName.Size = new System.Drawing.Size(100, 20);
            this.textBoxAppName.TabIndex = 13;
            this.textBoxAppName.Leave += new System.EventHandler(this.textBoxAppName_Leave);
            // 
            // buttonRemoveClickList
            // 
            this.buttonRemoveClickList.Location = new System.Drawing.Point(631, 39);
            this.buttonRemoveClickList.Name = "buttonRemoveClickList";
            this.buttonRemoveClickList.Size = new System.Drawing.Size(157, 34);
            this.buttonRemoveClickList.TabIndex = 14;
            this.buttonRemoveClickList.Text = "Remove Selected";
            this.buttonRemoveClickList.UseVisualStyleBackColor = true;
            this.buttonRemoveClickList.Click += new System.EventHandler(this.buttonRemoveClickList_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(817, 450);
            this.Controls.Add(this.buttonRemoveClickList);
            this.Controls.Add(this.textBoxAppName);
            this.Controls.Add(this.labelAppName);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.timeOfExecutionExpired);
            this.Controls.Add(this.numberOfExecution);
            this.Controls.Add(this.buttonRunUntilTime);
            this.Controls.Add(this.buttonRunX);
            this.Controls.Add(this.buttonRunInfinity);
            this.Controls.Add(this.clickListSaved);
            this.Controls.Add(this.buttonRecord);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "AutoClicker";
            ((System.ComponentModel.ISupportInitialize)(this.numberOfExecution)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button buttonRecord;
        private System.Windows.Forms.Timer timerRecord;
        private System.Windows.Forms.ListBox clickListSaved;
        private System.Windows.Forms.Button buttonRunInfinity;
        private System.Windows.Forms.Button buttonRunX;
        private System.Windows.Forms.Button buttonRunUntilTime;
        private System.Windows.Forms.NumericUpDown numberOfExecution;
        private System.Windows.Forms.DateTimePicker timeOfExecutionExpired;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label labelAppName;
        private System.Windows.Forms.TextBox textBoxAppName;
        private System.Windows.Forms.Button buttonRemoveClickList;
    }
}

