namespace Server
{
    partial class Server
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
            this.lsvLog = new System.Windows.Forms.ListView();
            this.btStart = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lsvLog
            // 
            this.lsvLog.Location = new System.Drawing.Point(12, 65);
            this.lsvLog.Name = "lsvLog";
            this.lsvLog.Size = new System.Drawing.Size(776, 373);
            this.lsvLog.TabIndex = 0;
            this.lsvLog.UseCompatibleStateImageBehavior = false;
            this.lsvLog.View = System.Windows.Forms.View.List;
            // 
            // btStart
            // 
            this.btStart.Location = new System.Drawing.Point(682, 12);
            this.btStart.Name = "btStart";
            this.btStart.Size = new System.Drawing.Size(106, 47);
            this.btStart.TabIndex = 1;
            this.btStart.Text = "Start Server";
            this.btStart.UseVisualStyleBackColor = true;
            this.btStart.Click += new System.EventHandler(this.btStart_Click_1);
            // 
            // Server
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btStart);
            this.Controls.Add(this.lsvLog);
            this.Name = "Server";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView lsvLog;
        private System.Windows.Forms.Button btStart;
    }
}

