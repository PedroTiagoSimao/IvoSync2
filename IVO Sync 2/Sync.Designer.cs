namespace IVO_Sync_2
{
    partial class Sync
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Sync));
            this.btnSync = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.worker = new System.ComponentModel.BackgroundWorker();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblLinhas = new System.Windows.Forms.Label();
            this.lblCabecs = new System.Windows.Forms.Label();
            this._backgroundProgressBar = new System.Windows.Forms.ProgressBar();
            this.btnClose = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnSync
            // 
            this.btnSync.Image = global::IVO_Sync_2.Properties.Resources.Synchronize_48x48;
            this.btnSync.Location = new System.Drawing.Point(124, 12);
            this.btnSync.Name = "btnSync";
            this.btnSync.Size = new System.Drawing.Size(114, 74);
            this.btnSync.TabIndex = 0;
            this.btnSync.UseVisualStyleBackColor = true;
            this.btnSync.Click += new System.EventHandler(this.btnSync_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(14, 135);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(139, 15);
            this.lblStatus.TabIndex = 1;
            this.lblStatus.Text = "Pronto para sincronizar";
            // 
            // lblLinhas
            // 
            this.lblLinhas.AutoSize = true;
            this.lblLinhas.Location = new System.Drawing.Point(14, 165);
            this.lblLinhas.Name = "lblLinhas";
            this.lblLinhas.Size = new System.Drawing.Size(42, 15);
            this.lblLinhas.TabIndex = 2;
            this.lblLinhas.Text = "label1";
            // 
            // lblCabecs
            // 
            this.lblCabecs.AutoSize = true;
            this.lblCabecs.Location = new System.Drawing.Point(14, 150);
            this.lblCabecs.Name = "lblCabecs";
            this.lblCabecs.Size = new System.Drawing.Size(42, 15);
            this.lblCabecs.TabIndex = 3;
            this.lblCabecs.Text = "label1";
            // 
            // _backgroundProgressBar
            // 
            this._backgroundProgressBar.Location = new System.Drawing.Point(17, 105);
            this._backgroundProgressBar.Name = "_backgroundProgressBar";
            this._backgroundProgressBar.Size = new System.Drawing.Size(330, 27);
            this._backgroundProgressBar.TabIndex = 4;
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(272, 189);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 5;
            this.btnClose.Text = "Fechar";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // Sync
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(359, 217);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this._backgroundProgressBar);
            this.Controls.Add(this.lblCabecs);
            this.Controls.Add(this.lblLinhas);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.btnSync);
            this.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(367, 251);
            this.MinimumSize = new System.Drawing.Size(367, 251);
            this.Name = "Sync";
            this.Text = "Ivo Sync 2013";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSync;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.ComponentModel.BackgroundWorker worker;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblLinhas;
        private System.Windows.Forms.Label lblCabecs;
        private System.Windows.Forms.ProgressBar _backgroundProgressBar;
        private System.Windows.Forms.Button btnClose;
    }
}

