namespace ProjectCarrot
{
    partial class Form1
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
            button1 = new Button();
            label1 = new Label();
            button2 = new Button();
            button3 = new Button();
            button4 = new Button();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            label6 = new Label();
            label7 = new Label();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new Point(12, 12);
            button1.Name = "button1";
            button1.Size = new Size(180, 112);
            button1.TabIndex = 0;
            button1.Text = "Create videos and upload";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(244, 12);
            label1.Name = "label1";
            label1.Size = new Size(38, 15);
            label1.TabIndex = 1;
            label1.Text = "label1";
            // 
            // button2
            // 
            button2.Location = new Point(12, 149);
            button2.Name = "button2";
            button2.Size = new Size(152, 59);
            button2.TabIndex = 2;
            button2.Text = "Create videos";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // button3
            // 
            button3.Location = new Point(12, 214);
            button3.Name = "button3";
            button3.Size = new Size(152, 59);
            button3.TabIndex = 3;
            button3.Text = "Upload videos";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // button4
            // 
            button4.Location = new Point(12, 392);
            button4.Name = "button4";
            button4.Size = new Size(180, 46);
            button4.TabIndex = 4;
            button4.Text = "Test";
            button4.UseVisualStyleBackColor = true;
            button4.Click += button4_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(354, 130);
            label2.Name = "label2";
            label2.Size = new Size(38, 15);
            label2.TabIndex = 5;
            label2.Text = "label2";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(170, 171);
            label3.Name = "label3";
            label3.Size = new Size(38, 15);
            label3.TabIndex = 6;
            label3.Text = "label3";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(170, 236);
            label4.Name = "label4";
            label4.Size = new Size(38, 15);
            label4.TabIndex = 7;
            label4.Text = "label4";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(558, 12);
            label5.Name = "label5";
            label5.Size = new Size(38, 15);
            label5.TabIndex = 8;
            label5.Text = "label5";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(354, 149);
            label6.Name = "label6";
            label6.Size = new Size(38, 15);
            label6.TabIndex = 9;
            label6.Text = "label6";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(354, 171);
            label7.Name = "label7";
            label7.Size = new Size(38, 15);
            label7.TabIndex = 10;
            label7.Text = "label7";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(label7);
            Controls.Add(label6);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(button4);
            Controls.Add(button3);
            Controls.Add(button2);
            Controls.Add(label1);
            Controls.Add(button1);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        public Button button1;
        public Label label1;
        public Button button2;
        public Button button3;
        public Button button4;
        public Label label2;
        public Label label3; // create time
        public Label label4; // upload time
        public Label label5; // custom folder
        public Label label6;
        public Label label7;
    }
}