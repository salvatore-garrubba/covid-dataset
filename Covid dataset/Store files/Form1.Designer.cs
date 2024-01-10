namespace Store_files
{
    partial class StoreFilesForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            selectedFileTextbox = new TextBox();
            browseBtn = new Button();
            importBtn = new Button();
            resultTextbox = new TextBox();
            filedocBindingSource1 = new BindingSource(components);
            filedocBindingSource = new BindingSource(components);
            ((System.ComponentModel.ISupportInitialize)filedocBindingSource1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)filedocBindingSource).BeginInit();
            SuspendLayout();
            // 
            // selectedFileTextbox
            // 
            selectedFileTextbox.Location = new Point(44, 12);
            selectedFileTextbox.Name = "selectedFileTextbox";
            selectedFileTextbox.Size = new Size(952, 27);
            selectedFileTextbox.TabIndex = 0;
            // 
            // browseBtn
            // 
            browseBtn.Location = new Point(1016, 12);
            browseBtn.Name = "browseBtn";
            browseBtn.Size = new Size(94, 29);
            browseBtn.TabIndex = 1;
            browseBtn.Text = "Browse";
            browseBtn.UseVisualStyleBackColor = true;
            browseBtn.Click += browse_Click;
            // 
            // importBtn
            // 
            importBtn.Location = new Point(328, 45);
            importBtn.Name = "importBtn";
            importBtn.Size = new Size(499, 29);
            importBtn.TabIndex = 2;
            importBtn.Text = "Import";
            importBtn.UseVisualStyleBackColor = true;
            importBtn.Click += import_Click;
            // 
            // resultTextbox
            // 
            resultTextbox.Location = new Point(12, 94);
            resultTextbox.Multiline = true;
            resultTextbox.Name = "resultTextbox";
            resultTextbox.ScrollBars = ScrollBars.Both;
            resultTextbox.Size = new Size(1144, 437);
            resultTextbox.TabIndex = 3;
            resultTextbox.WordWrap = false;
            // 
            // filedocBindingSource1
            // 
            filedocBindingSource1.DataSource = typeof(Data.Models.Filedoc);
            // 
            // filedocBindingSource
            // 
            filedocBindingSource.DataSource = typeof(Data.Models.Filedoc);
            // 
            // StoreFilesForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1168, 543);
            Controls.Add(resultTextbox);
            Controls.Add(importBtn);
            Controls.Add(browseBtn);
            Controls.Add(selectedFileTextbox);
            Name = "StoreFilesForm";
            Text = "Covid dataset: store files";
            ((System.ComponentModel.ISupportInitialize)filedocBindingSource1).EndInit();
            ((System.ComponentModel.ISupportInitialize)filedocBindingSource).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private TextBox selectedFileTextbox;
        private Button browseBtn;
        private Button importBtn;
        private TextBox resultTextbox;
        private BindingSource filedocBindingSource;
        private BindingSource filedocBindingSource1;
    }
}