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
        private void button7_Click(object sender, EventArgs e) => Main.Button7Click();


        // --- --- IDK WHAT IS THIS :/ --- ---
        private void button5_Click(object sender, EventArgs e)
        {
        }

        private void button6_Click(object sender, EventArgs e)
        {
        }

        private void label4_Click(object sender, EventArgs e) { }

        private void label1_Click(object sender, EventArgs e) { }
    }
}