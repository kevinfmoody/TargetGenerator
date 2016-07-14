namespace TargetGenerator
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
            this.label1 = new System.Windows.Forms.Label();
            this.NetworkStatusLabel = new System.Windows.Forms.Label();
            this.SituationFileTextBox = new System.Windows.Forms.TextBox();
            this.SituationFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.BrowseFilesButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(141, 154);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 25);
            this.label1.TabIndex = 0;
            this.label1.Text = "Status:";
            // 
            // NetworkStatusLabel
            // 
            this.NetworkStatusLabel.AutoSize = true;
            this.NetworkStatusLabel.Location = new System.Drawing.Point(238, 153);
            this.NetworkStatusLabel.Name = "NetworkStatusLabel";
            this.NetworkStatusLabel.Size = new System.Drawing.Size(143, 25);
            this.NetworkStatusLabel.TabIndex = 1;
            this.NetworkStatusLabel.Text = "Disconnected";
            // 
            // SituationFileTextBox
            // 
            this.SituationFileTextBox.Location = new System.Drawing.Point(127, 249);
            this.SituationFileTextBox.Name = "SituationFileTextBox";
            this.SituationFileTextBox.Size = new System.Drawing.Size(100, 31);
            this.SituationFileTextBox.TabIndex = 2;
            this.SituationFileTextBox.Click += new System.EventHandler(this.SituationFileTextBox_Enter);
            this.SituationFileTextBox.Enter += new System.EventHandler(this.SituationFileTextBox_Enter);
            // 
            // SituationFileDialog
            // 
            this.SituationFileDialog.Filter = "ACSim Aircraft Files (*.acs)|*.acs|TWRTrainer Aircraft Files (*.air)|*.air";
            this.SituationFileDialog.Multiselect = true;
            this.SituationFileDialog.Title = "Load Situation Files";
            // 
            // BrowseFilesButton
            // 
            this.BrowseFilesButton.Location = new System.Drawing.Point(234, 256);
            this.BrowseFilesButton.Name = "BrowseFilesButton";
            this.BrowseFilesButton.Size = new System.Drawing.Size(228, 79);
            this.BrowseFilesButton.TabIndex = 3;
            this.BrowseFilesButton.Text = "Browse";
            this.BrowseFilesButton.UseVisualStyleBackColor = true;
            this.BrowseFilesButton.Click += new System.EventHandler(this.BrowseFilesButton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1015, 596);
            this.Controls.Add(this.BrowseFilesButton);
            this.Controls.Add(this.SituationFileTextBox);
            this.Controls.Add(this.NetworkStatusLabel);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label NetworkStatusLabel;
        private System.Windows.Forms.TextBox SituationFileTextBox;
        private System.Windows.Forms.OpenFileDialog SituationFileDialog;
        private System.Windows.Forms.Button BrowseFilesButton;
    }
}

