using OpenQA.Selenium;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace ProjectCarrot
{
    public partial class Form1 : Form
    {
        public static Form1 form;

        public Form1()
        {
            InitializeComponent();
            form = this;

            Main.Inialize();
        }

        private void button1_Click(object sender, EventArgs e) => Main.Button1Click();
        private void button2_Click(object sender, EventArgs e) => Main.Button2Click();
        private void button3_Click(object sender, EventArgs e) => Main.Button3Click();
        private void button4_Click(object sender, EventArgs e) => Main.Button4Click();
    }
}