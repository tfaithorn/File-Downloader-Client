
namespace File_Downloader
{
    partial class ScreenshotOptions
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
            this.button1 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.optionsWebView = new Microsoft.Web.WebView2.WinForms.WebView2();
            ((System.ComponentModel.ISupportInitialize)(this.optionsWebView)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(678, 677);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(120, 33);
            this.button1.TabIndex = 1;
            this.button1.Text = "Apply";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(73, 6);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(579, 20);
            this.textBox1.TabIndex = 2;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // optionsWebView
            // 
            this.optionsWebView.AllowExternalDrop = true;
            this.optionsWebView.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.optionsWebView.CreationProperties = null;
            this.optionsWebView.Cursor = System.Windows.Forms.Cursors.Default;
            this.optionsWebView.DefaultBackgroundColor = System.Drawing.Color.White;
            this.optionsWebView.Location = new System.Drawing.Point(73, 54);
            this.optionsWebView.Name = "optionsWebView";
            this.optionsWebView.Size = new System.Drawing.Size(579, 656);
            this.optionsWebView.Source = new System.Uri("https://google.com", System.UriKind.Absolute);
            this.optionsWebView.TabIndex = 3;
            this.optionsWebView.ZoomFactor = 1D;
            this.optionsWebView.Click += new System.EventHandler(this.optionsWebView_Click_1);
            // 
            // ScreenshotOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(820, 776);
            this.Controls.Add(this.optionsWebView);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button1);
            this.Name = "ScreenshotOptions";
            this.Text = "ScreenshotOptions";
            this.TransparencyKey = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.Load += new System.EventHandler(this.ScreenshotOptions_Load);
            ((System.ComponentModel.ISupportInitialize)(this.optionsWebView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBox1;
        private Microsoft.Web.WebView2.WinForms.WebView2 optionsWebView;
    }
}