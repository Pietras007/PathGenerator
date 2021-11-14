using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Geometric2
{
    public partial class RenameElement : Form
    {
        private string[] name;
        public RenameElement(string[] newName)
        {
            InitializeComponent();
            name = newName;
        }

        private void renameButton_Click(object sender, EventArgs e)
        {
            name[0] = nameTextBox.Text;
            this.Close();
        }
    }
}
