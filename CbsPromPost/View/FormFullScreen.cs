using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CbsPromPost.View
{
    public partial class FormFullScreen : Form
    {
        public FormFullScreen()
        {
            InitializeComponent();

            var screen = Screen.PrimaryScreen.Bounds;
            Bounds = new Rectangle(0, 0, screen.Width, screen.Height);
        }
    }
}
