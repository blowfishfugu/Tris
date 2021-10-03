namespace tris
{
    partial class TrisForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TrisForm));
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.chkSurprises = new System.Windows.Forms.CheckBox();
            this.btnHelp = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btnMusic = new System.Windows.Forms.Button();
            this.chkOff = new System.Windows.Forms.CheckBox();
            this.previewCanvas = new tris.BlockCanvas();
            this.gameField = new tris.BlockCanvas();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Location = new System.Drawing.Point(548, 0);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(0);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(34, 31);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.TabStop = false;
            this.btnCancel.Text = "X";
            this.toolTip1.SetToolTip(this.btnCancel, "Bye");
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            this.btnCancel.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TrisForm_KeyDown);
            this.btnCancel.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TrisForm_KeyUp);
            this.btnCancel.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.lblStatus_PreviewKeyDown);
            // 
            // btnStart
            // 
            this.btnStart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStart.Location = new System.Drawing.Point(0, 0);
            this.btnStart.Margin = new System.Windows.Forms.Padding(0);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(45, 31);
            this.btnStart.TabIndex = 1;
            this.btnStart.TabStop = false;
            this.btnStart.Text = ">";
            this.toolTip1.SetToolTip(this.btnStart, "Start/Pause\r\nwhen GameOver Restart");
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            this.btnStart.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TrisForm_KeyDown);
            this.btnStart.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TrisForm_KeyUp);
            this.btnStart.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.lblStatus_PreviewKeyDown);
            // 
            // lblStatus
            // 
            this.lblStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblStatus.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblStatus.Location = new System.Drawing.Point(45, 0);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(470, 31);
            this.lblStatus.TabIndex = 2;
            this.lblStatus.Text = "Tris";
            this.lblStatus.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lblStatus_MouseDown);
            this.lblStatus.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.lblStatus_PreviewKeyDown);
            // 
            // chkSurprises
            // 
            this.chkSurprises.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkSurprises.AutoSize = true;
            this.chkSurprises.Location = new System.Drawing.Point(414, 205);
            this.chkSurprises.Name = "chkSurprises";
            this.chkSurprises.Size = new System.Drawing.Size(120, 19);
            this.chkSurprises.TabIndex = 5;
            this.chkSurprises.TabStop = false;
            this.chkSurprises.Text = "extended mode";
            this.toolTip1.SetToolTip(this.chkSurprises, "Disable for classic gameplay (no surprises)\r\nIf enabled, longplay-mode\r\nwith some" +
        " distractions\r\nto keep you busy :)");
            this.chkSurprises.UseVisualStyleBackColor = true;
            // 
            // btnHelp
            // 
            this.btnHelp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnHelp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnHelp.Location = new System.Drawing.Point(515, 0);
            this.btnHelp.Margin = new System.Windows.Forms.Padding(0);
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.Size = new System.Drawing.Size(34, 31);
            this.btnHelp.TabIndex = 6;
            this.btnHelp.TabStop = false;
            this.btnHelp.Text = "?";
            this.toolTip1.SetToolTip(this.btnHelp, "Move / Rotate:\r\nCursorkeys \r\nNumpad 4,6 / 8,2\r\na,d / w,x,q,e\r\n\r\nspeedup:\r\nS-Key\r\n" +
        "Numpad 5\r\n(numlock must be disabled to work properly)");
            this.btnHelp.UseVisualStyleBackColor = true;
            // 
            // toolTip1
            // 
            this.toolTip1.BackColor = System.Drawing.Color.Black;
            this.toolTip1.ForeColor = System.Drawing.Color.White;
            // 
            // btnMusic
            // 
            this.btnMusic.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMusic.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMusic.Location = new System.Drawing.Point(414, 227);
            this.btnMusic.Margin = new System.Windows.Forms.Padding(0);
            this.btnMusic.Name = "btnMusic";
            this.btnMusic.Size = new System.Drawing.Size(159, 31);
            this.btnMusic.TabIndex = 7;
            this.btnMusic.TabStop = false;
            this.btnMusic.Text = "next musictrack";
            this.toolTip1.SetToolTip(this.btnMusic, "Hotkeys:\r\nM or Numpad 9");
            this.btnMusic.UseVisualStyleBackColor = true;
            this.btnMusic.Click += new System.EventHandler(this.btnMusic_Click);
            // 
            // chkOff
            // 
            this.chkOff.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkOff.AutoSize = true;
            this.chkOff.BackColor = System.Drawing.Color.Black;
            this.chkOff.ForeColor = System.Drawing.Color.White;
            this.chkOff.Location = new System.Drawing.Point(413, 261);
            this.chkOff.Name = "chkOff";
            this.chkOff.Size = new System.Drawing.Size(92, 19);
            this.chkOff.TabIndex = 8;
            this.chkOff.TabStop = false;
            this.chkOff.Text = "music off";
            this.chkOff.UseVisualStyleBackColor = false;
            this.chkOff.CheckedChanged += new System.EventHandler(this.chkOff_CheckedChanged);
            // 
            // previewCanvas
            // 
            this.previewCanvas.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.previewCanvas.BackColor = System.Drawing.Color.Black;
            this.previewCanvas.Location = new System.Drawing.Point(413, 37);
            this.previewCanvas.Name = "previewCanvas";
            this.previewCanvas.Size = new System.Drawing.Size(160, 160);
            this.previewCanvas.TabIndex = 4;
            this.previewCanvas.Text = "blockCanvas2";
            // 
            // gameField
            // 
            this.gameField.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gameField.BackColor = System.Drawing.Color.Black;
            this.gameField.Location = new System.Drawing.Point(46, 35);
            this.gameField.Name = "gameField";
            this.gameField.Size = new System.Drawing.Size(320, 640);
            this.gameField.TabIndex = 3;
            this.gameField.Text = "blockCanvas1";
            this.gameField.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TrisForm_KeyDown);
            this.gameField.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TrisForm_KeyUp);
            this.gameField.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.lblStatus_PreviewKeyDown);
            // 
            // TrisForm
            // 
            this.AcceptButton = this.btnStart;
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.ControlText;
            this.ClientSize = new System.Drawing.Size(582, 682);
            this.ControlBox = false;
            this.Controls.Add(this.chkOff);
            this.Controls.Add(this.btnMusic);
            this.Controls.Add(this.btnHelp);
            this.Controls.Add(this.chkSurprises);
            this.Controls.Add(this.previewCanvas);
            this.Controls.Add(this.gameField);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.btnCancel);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.ForeColor = System.Drawing.SystemColors.Control;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "TrisForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TrisForm_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TrisForm_KeyUp);
            this.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.lblStatus_PreviewKeyDown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Label lblStatus;
        private BlockCanvas gameField;
        private BlockCanvas previewCanvas;
        private System.Windows.Forms.CheckBox chkSurprises;
        private System.Windows.Forms.Button btnHelp;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button btnMusic;
        private System.Windows.Forms.CheckBox chkOff;
    }
}

