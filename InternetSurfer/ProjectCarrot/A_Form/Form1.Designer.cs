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
            button2 = new Button();
            button3 = new Button();
            button4 = new Button();
            label2 = new Label();
            label4 = new Label();
            label5 = new Label();
            label6 = new Label();
            label7 = new Label();
            label8 = new Label();
            label1 = new Label();
            button7 = new Button();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new Point(40, 40);
            button1.Name = "button1";
            button1.Size = new Size(180, 112);
            button1.TabIndex = 0;
            button1.Text = "Create videos and upload";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.Location = new Point(40, 169);
            button2.Name = "button2";
            button2.Size = new Size(152, 59);
            button2.TabIndex = 2;
            button2.Text = "Create videos";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // button3
            // 
            button3.Location = new Point(40, 244);
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
            label2.Location = new Point(511, 149);
            label2.Name = "label2";
            label2.Size = new Size(38, 15);
            label2.TabIndex = 5;
            label2.Text = "label2";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(40, 12);
            label4.Name = "label4";
            label4.Size = new Size(61, 15);
            label4.TabIndex = 7;
            label4.Text = "Settings01";
            label4.Click += label4_Click;
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
            label6.Location = new Point(511, 169);
            label6.Name = "label6";
            label6.Size = new Size(38, 15);
            label6.TabIndex = 9;
            label6.Text = "label6";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(511, 189);
            label7.Name = "label7";
            label7.Size = new Size(38, 15);
            label7.TabIndex = 10;
            label7.Text = "label7";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(511, 209);
            label8.Name = "label8";
            label8.Size = new Size(38, 15);
            label8.TabIndex = 11;
            label8.Text = "label8";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(283, 12);
            label1.Name = "label1";
            label1.Size = new Size(61, 15);
            label1.TabIndex = 15;
            label1.Text = "Settings02";
            label1.Click += label1_Click;
            // 
            // button7
            // 
            button7.Location = new Point(283, 40);
            button7.Name = "button7";
            button7.Size = new Size(180, 112);
            button7.TabIndex = 12;
            button7.Text = "Create videos and upload";
            button7.UseVisualStyleBackColor = true;
            button7.Click += button7_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(label1);
            Controls.Add(button7);
            Controls.Add(label8);
            Controls.Add(label7);
            Controls.Add(label6);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label2);
            Controls.Add(button4);
            Controls.Add(button3);
            Controls.Add(button2);
            Controls.Add(button1);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        public Button button1;
        public Button button2;
        public Button button3;
        public Button button4;
        public Label label2;
        public Label label4; // upload time
        public Label label5; // custom folder
        public Label label6;
        public Label label7;
        public Label label8; // pyttsx reader warning
        public Label label1;
        public Button button7;
    }
}