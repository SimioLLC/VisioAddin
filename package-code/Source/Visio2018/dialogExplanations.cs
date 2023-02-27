using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Visio2018
{
    public partial class dialogExplanations : Form
    {
        /// <summary>
        /// The explanation list to display
        /// </summary>
        public List<String> ExplanationList { get; set; }

        /// <summary>
        /// Message to the user
        /// </summary>
        public string Message { set { labelMessage.Text = value; } }

        public dialogExplanations()
        {
            InitializeComponent();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void dialogExplanations_Load(object sender, EventArgs e)
        {
            textExplanations.Text = "";
            if (ExplanationList == null)
                return;

            StringBuilder sb = new StringBuilder();
            foreach ( String line in ExplanationList)
            {
                sb.AppendLine(line);
            }

            textExplanations.Text = sb.ToString();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;

        }
    }
}
