
namespace File_Downloader
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.Button1 = new System.Windows.Forms.Button();
            this.submit = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.button5 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.redownloadFiles = new System.Windows.Forms.CheckBox();
            this.stopOnErrorBox = new System.Windows.Forms.CheckBox();
            this.button6 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.webView21 = new Microsoft.Web.WebView2.WinForms.WebView2();
            this.button8 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.webView21)).BeginInit();
            this.SuspendLayout();
            // 
            // Button1
            // 
            this.Button1.Location = new System.Drawing.Point(22, 63);
            this.Button1.Name = "Button1";
            this.Button1.Size = new System.Drawing.Size(134, 41);
            this.Button1.TabIndex = 0;
            this.Button1.Text = "Login";
            this.Button1.UseVisualStyleBackColor = true;
            this.Button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // submit
            // 
            this.submit.Location = new System.Drawing.Point(22, 376);
            this.submit.Name = "submit";
            this.submit.Size = new System.Drawing.Size(134, 41);
            this.submit.TabIndex = 1;
            this.submit.Text = "Submit";
            this.submit.UseVisualStyleBackColor = true;
            this.submit.Click += new System.EventHandler(this.submit_click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(22, 183);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(134, 41);
            this.button3.TabIndex = 2;
            this.button3.Text = "Select Destination Folder";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(22, 120);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(134, 41);
            this.button4.TabIndex = 5;
            this.button4.Text = "Select Spreadsheet";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // richTextBox1
            // 
            this.richTextBox1.BackColor = System.Drawing.SystemColors.WindowText;
            this.richTextBox1.DetectUrls = false;
            this.richTextBox1.ForeColor = System.Drawing.SystemColors.Window;
            this.richTextBox1.Location = new System.Drawing.Point(338, 63);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(302, 292);
            this.richTextBox1.TabIndex = 6;
            this.richTextBox1.Text = "";
            this.richTextBox1.WordWrap = false;
            this.richTextBox1.TextChanged += new System.EventHandler(this.richTextBox1_TextChanged);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(731, 370);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(91, 52);
            this.button2.TabIndex = 7;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(177, 89);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(135, 20);
            this.label2.TabIndex = 8;
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(828, 370);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(84, 52);
            this.button5.TabIndex = 9;
            this.button5.Text = "Exit";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(168, 197);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(161, 41);
            this.label3.TabIndex = 10;
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(168, 134);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(161, 41);
            this.label5.TabIndex = 14;
            this.label5.Click += new System.EventHandler(this.label5_Click);
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(162, 74);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(167, 30);
            this.label6.TabIndex = 16;
            this.label6.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.label6.Click += new System.EventHandler(this.label6_Click);
            // 
            // redownloadFiles
            // 
            this.redownloadFiles.AutoSize = true;
            this.redownloadFiles.Location = new System.Drawing.Point(22, 338);
            this.redownloadFiles.Name = "redownloadFiles";
            this.redownloadFiles.Size = new System.Drawing.Size(134, 17);
            this.redownloadFiles.TabIndex = 17;
            this.redownloadFiles.Text = "Download Existing files";
            this.redownloadFiles.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.redownloadFiles.UseVisualStyleBackColor = true;
            this.redownloadFiles.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // stopOnErrorBox
            // 
            this.stopOnErrorBox.AutoSize = true;
            this.stopOnErrorBox.Checked = true;
            this.stopOnErrorBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.stopOnErrorBox.Location = new System.Drawing.Point(22, 315);
            this.stopOnErrorBox.Name = "stopOnErrorBox";
            this.stopOnErrorBox.Size = new System.Drawing.Size(90, 17);
            this.stopOnErrorBox.TabIndex = 18;
            this.stopOnErrorBox.Text = "Stop On Error";
            this.stopOnErrorBox.UseVisualStyleBackColor = true;
            this.stopOnErrorBox.CheckedChanged += new System.EventHandler(this.checkBox2_CheckedChanged);
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(722, 12);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(92, 48);
            this.button6.TabIndex = 20;
            this.button6.Text = "List Empty Folders";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click_1);
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(820, 12);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(92, 48);
            this.button7.TabIndex = 21;
            this.button7.Text = "List Files";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "GET",
            "POST"});
            this.comboBox1.Location = new System.Drawing.Point(22, 239);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 21);
            this.comboBox1.TabIndex = 22;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged_1);
            // 
            // webView21
            // 
            this.webView21.AllowExternalDrop = true;
            this.webView21.CreationProperties = null;
            this.webView21.DefaultBackgroundColor = System.Drawing.Color.White;
            this.webView21.Location = new System.Drawing.Point(646, 63);
            this.webView21.Name = "webView21";
            this.webView21.Size = new System.Drawing.Size(266, 292);
            this.webView21.TabIndex = 23;
            this.webView21.ZoomFactor = 1D;
            this.webView21.Click += new System.EventHandler(this.webView21_Click);
            // 
            // button8
            // 
            this.button8.Location = new System.Drawing.Point(626, 12);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(90, 45);
            this.button8.TabIndex = 24;
            this.button8.Text = "Bulk Upload";
            this.button8.UseVisualStyleBackColor = true;
            this.button8.Click += new System.EventHandler(this.button8_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(935, 450);
            this.Controls.Add(this.button8);
            this.Controls.Add(this.webView21);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.stopOnErrorBox);
            this.Controls.Add(this.redownloadFiles);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.submit);
            this.Controls.Add(this.Button1);
            this.Controls.Add(this.label2);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "File Downloader";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.webView21)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Button1;
        private System.Windows.Forms.Button submit;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox redownloadFiles;
        private System.Windows.Forms.CheckBox stopOnErrorBox;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.ComboBox comboBox1;
        private Microsoft.Web.WebView2.WinForms.WebView2 webView21;
        private System.Windows.Forms.Button button8;
    }
}

