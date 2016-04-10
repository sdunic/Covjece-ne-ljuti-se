namespace _2dGameLanguage
{
    partial class BGL
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BGL));
            this.syncRate = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.lblPlayerInfo = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblNumOfMoves = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // syncRate
            // 
            this.syncRate.AutoSize = true;
            this.syncRate.BackColor = System.Drawing.Color.Transparent;
            this.syncRate.Location = new System.Drawing.Point(3, 60);
            this.syncRate.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.syncRate.Name = "syncRate";
            this.syncRate.Size = new System.Drawing.Size(55, 39);
            this.syncRate.TabIndex = 0;
            this.syncRate.Text = "60";
            this.syncRate.Visible = false;
            // 
            // timer1
            // 
            this.timer1.Interval = 17;
            this.timer1.Tick += new System.EventHandler(this.Update);
            // 
            // timer2
            // 
            this.timer2.Interval = 250;
            this.timer2.Tick += new System.EventHandler(this.updateFrameRate);
            // 
            // lblPlayerInfo
            // 
            this.lblPlayerInfo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(182)))), ((int)(((byte)(137)))));
            this.lblPlayerInfo.Location = new System.Drawing.Point(818, 32);
            this.lblPlayerInfo.Name = "lblPlayerInfo";
            this.lblPlayerInfo.Size = new System.Drawing.Size(230, 50);
            this.lblPlayerInfo.TabIndex = 1;
            this.lblPlayerInfo.Text = "Player";
            this.lblPlayerInfo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblStatus
            // 
            this.lblStatus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(182)))), ((int)(((byte)(137)))));
            this.lblStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblStatus.Location = new System.Drawing.Point(818, 82);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(230, 50);
            this.lblStatus.TabIndex = 2;
            this.lblStatus.Text = "Status";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblNumOfMoves
            // 
            this.lblNumOfMoves.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(182)))), ((int)(((byte)(137)))));
            this.lblNumOfMoves.Location = new System.Drawing.Point(818, 132);
            this.lblNumOfMoves.Name = "lblNumOfMoves";
            this.lblNumOfMoves.Size = new System.Drawing.Size(230, 50);
            this.lblNumOfMoves.TabIndex = 3;
            this.lblNumOfMoves.Text = "Bacanja";
            this.lblNumOfMoves.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // BGL
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(20F, 39F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(1060, 701);
            this.Controls.Add(this.lblNumOfMoves);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.lblPlayerInfo);
            this.Controls.Add(this.syncRate);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(8, 9, 8, 9);
            this.MaximizeBox = false;
            this.Name = "BGL";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "2DGL";
            this.Load += new System.EventHandler(this.startTimer);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Draw);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.keyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.keyUp);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.mouseClicked);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.mouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.mouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.mouseUp);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label syncRate;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Timer timer2;
        private System.Windows.Forms.Label lblPlayerInfo;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblNumOfMoves;
    }
}

