using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace httpRequest
{
    public partial class Form1 : Form
    {
        Exporter exporter;

        public Form1()
        {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog1.ShowDialog().Equals(DialogResult.OK))
            {
                this.txtFileSrc.Text = openFileDialog1.FileName;
            }
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            if(null==exporter)
                exporter = new Exporter();

            exporter.upload(txtURL.Text.ToString(), 
                            txtFileSrc.Text.ToString(), 
                            txtTitle.Text.ToString(), 
                            txtDesc.Text.ToString(), 
                            txtToken.Text.ToString());
            MessageBox.Show(exporter.response);
        }
    }
}
