using System.Windows.Forms;

namespace EasyBrailleEdit
{
    public partial class BusyForm : Form
    {
        public BusyForm()
        {
            InitializeComponent();
        }

        public string Message
        {
            set
            {
                lblMsg.Text = value;
                Application.DoEvents();
            }
        }
    }
}