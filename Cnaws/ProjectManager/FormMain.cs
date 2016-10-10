using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjectManager
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
            menuStrip.Renderer = DefaultRenderer.Instance;
            toolStrip.Renderer = DefaultRenderer.Instance;
            statusStrip.Renderer = DefaultRenderer.Instance;
        }

        private void Exit(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void Open(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                foreach (string file in openFileDialog.FileNames)
                {
                    if (file.EndsWith(".dll", StringComparison.InvariantCultureIgnoreCase))
                        (new ChildForm(this, file)).Show();
                }
            }
        }
    }
}
