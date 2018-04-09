using GML_EC.Properties;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace GML_EC
{
    public partial class FrmAbout : Form
    {
        public FrmAbout()
        {
            InitializeComponent();
        }

        private void frmAbout_Load(object sender, EventArgs e)
        {
            Text = @"O programie";
            btnOK.Text = @"OK";

            lblLicenseInfo.Text = @"Program do generowania raportów w formacie XLSX wykorzystuje bibiotekę EPPlus w oparciu o licencję GNU Library General Public License (LGPL)";
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void lblWWW_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://www.gisnet.com.pl");
        }

        private void FrmAbout_Shown(object sender, EventArgs e)
        {
            // ====================================================================================
            // zabezpiczenie przez XP, który nie potrafi podłączyć ikony
            try
            {
                Icon = Resources.gml;
            }
            catch (Exception)
            {
                // ignored
            }
            // ====================================================================================
        }
    }
}
