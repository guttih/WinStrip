using System;
using System.Drawing;
using System.Windows.Forms;
namespace WinStrip
{
    public partial class FormSplash : Form
    {
        private const int CP_NOCLOSE_BUTTON = 0x200;
        
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams myCp = base.CreateParams;
                myCp.ClassStyle = myCp.ClassStyle | CP_NOCLOSE_BUTTON;
                return myCp;
            }
        }

        public string Heading
        {
            get { return labelHeading.Text; }
            set { labelHeading.Text = value; }
        }

        public string Message
        {
            get { return labelMessage.Text; }
            set { labelMessage.Text = value;
            }
        }
        private void InitForm(string Message)
        {
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            InitializeComponent();
            Heading = "";
            this.Message = Message;
        }

        private void DrawSurroundingLines(PaintEventArgs e)
        {
            int penWidth = 2;
           Pen pen = new Pen(Color.FromArgb(255, 0, 0, 0),penWidth);
           e.Graphics.DrawLine(pen,      0,      0,     0, Height);
           e.Graphics.DrawLine(pen,      0, Height, Width, Height);
           e.Graphics.DrawLine(pen,  Width, Height, Width,      0);
            e.Graphics.DrawLine(pen, Width,      0,     0,      0);
        }

        public FormSplash(string Message)
        {
            InitForm(Message);
        }

        public FormSplash(string Heading, string Message)
        {
            InitForm(Message);
            this.Heading = Heading;
        }

        public void Set(string Heading, string Message)
        {
            this.Heading = Heading;
            this.Message = Message;
            Application.DoEvents();
        }

        private void FormSplash_Load(object sender, System.EventArgs e)
        {

        }

        private void FormHeading_Paint(object sender, PaintEventArgs e)
        {
            DrawSurroundingLines(e);
        }
    }
}
