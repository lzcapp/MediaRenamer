namespace ExportGPS {
    partial class FrmMian {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMian));
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.btnFolder = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(169, 12);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(530, 35);
            this.progressBar.TabIndex = 0;
            // 
            // btnFolder
            // 
            this.btnFolder.Location = new System.Drawing.Point(12, 12);
            this.btnFolder.Name = "btnFolder";
            this.btnFolder.Size = new System.Drawing.Size(135, 35);
            this.btnFolder.TabIndex = 2;
            this.btnFolder.Text = "Folder ...";
            this.btnFolder.UseVisualStyleBackColor = true;
            this.btnFolder.Click += new System.EventHandler(this.Button1_Click);
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.SystemColors.Control;
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label1.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.label1.DetectUrls = false;
            this.label1.HideSelection = false;
            this.label1.Location = new System.Drawing.Point(12, 64);
            this.label1.Name = "label1";
            this.label1.ReadOnly = true;
            this.label1.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.label1.Size = new System.Drawing.Size(687, 199);
            this.label1.TabIndex = 3;
            this.label1.Text = "";
            // 
            // FrmMian
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(711, 275);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnFolder);
            this.Controls.Add(this.progressBar);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "FrmMian";
            this.Text = "PhotoToolbox - ExportGPS";
            this.Load += new System.EventHandler(this.FrmMian_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Button btnFolder;
        private System.Windows.Forms.RichTextBox label1;
    }
}

