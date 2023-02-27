using SdxVisio;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SdxHarness
{
    public partial class DialogVisioPrompt : Form
    {
        /// <summary>
        /// Either set by caller, or selected by user.
        /// </summary>
        public SdxVisioApplication visioApp { get; set; }

        /// <summary>
        /// Which page number is desired? Values 1..N, where N is the number of pages in the visio doc.
        /// </summary>
        public int PageNumber = 1;

        public DialogVisioPrompt()
        {
            InitializeComponent();
        }

        private void DialogVisioPrompt_Load(object sender, EventArgs e)
        {
            if ( visioApp == null )
            {
                DialogResult result = MessageBox.Show("No Visio File selected. Select one now?", "", MessageBoxButtons.OKCancel);
                if ( result != DialogResult.OK )
                {
                    this.Close();
                }
                buttonSelectVisioFile.Enabled = true;
            }
            else
            {
                loadPages();
            }
        }

        private void loadPages()
        {
            comboSelectPage.DataSource = visioApp.PageDict.Values.ToList();
            comboSelectPage.DisplayMember = "PageName";

        }

        private void alert(string msg)
        {
            MessageBox.Show(msg);
        }

        private void buttonSelectVisioFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.InitialDirectory = Environment.SpecialFolder.MyDocuments.ToString();

            DialogResult result = dialog.ShowDialog();
            if ( result == DialogResult.OK )
            {

            }
        }

        private void comboSelectPage_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboSelectPage.SelectedItem == null)
                return;

            this.PageNumber = (comboSelectPage.SelectedItem as PageInfo).PageNumber;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void groupPageInfo_Enter(object sender, EventArgs e)
        {

        }
    }
}
