using LoggertonHelpers;
using SdxHelpers;
using SdxVisio;
using SimioAPI.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Visio2018;
using static System.Environment;

namespace Visio2018
{
    public partial class FormVisio : Form
    {

        /// <summary>
        /// Selected Visio App
        /// </summary>
        SdxVisioApplication SelectedVisioApp = new SdxVisioApplication();

        /// <summary>
        /// The current SDX context
        /// </summary>
        public SdxDataSetContext SelectedSdxContext { get; set; }

        /// <summary>
        /// The transform for visio->simio
        /// </summary>
        public SimioTransform Transform { get; set; }

        /// <summary>
        /// Reference the Simio Design context (set before call to this form)
        /// </summary>
        public IDesignContext DesignContext { get; set; }

        /// <summary>
        /// Context for drawing the picturebox (preview diagram)
        /// </summary>
        PicContext picContext { get; set; }

        /// <summary>
        /// The returned SDX dataset.
        /// </summary>
        ////public DataSet SelectedSdxDataSet { get; set; } = null;

        private PageInfo SelectedPage = null;

        public FormVisio()
        {
            InitializeComponent();
        }

        private void FormVisio_Load(object sender, EventArgs e)
        {
            picContext = new PicContext(picPreviewDiagram, SelectedSdxContext);

            timerLogs.Interval = 1000;
            timerLogs.Enabled = true;

        }

        private void importToSimioToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ImportVisioToFacility(cbAutoScaleObjects.Checked);
        }

        private void buttonVisioToFacility_Click(object sender, EventArgs e)
        {

        }

        private void buttonFetchDataSet_Click(object sender, EventArgs e)
        {

        }

        private void buttonVisioToFacilityx_Click(object sender, EventArgs e)
        {

        }

        private void CreateAndDisplayNewDataSet(int pageNumber)
        {
            if ( SelectedVisioApp == null )
            {
                alert($"No Visio app is selected.");
                return;
            }

            // Process it into a Sdx DataSet
            string explanation = "";

            DataSet ds = SelectedVisioApp.CreateSdxDataSet("Visio", SelectedVisioApp.FilePath, pageNumber, out explanation);
            if (ds != null)
            {
                if (!OnNewSdxDataSetSelected("Visio", "Visio", SelectedVisioApp.FilePath, ds, out explanation))
                {
                    alert(explanation);
                    return;
                }
                SelectedSdxContext = new SdxDataSetContext("Visio", "Visio", SelectedVisioApp.FilePath, ds);
            }
            return;
        }

        private SdxVisioApplication SelectAndOpenVisioFile(List<string> explanationList)
        {
            explanationList.Clear();

            if ( !SelectAndOpenVisioFile(out SelectedVisioApp, explanationList))
            {
                explanationList.Add($"Problems processing the Visio File. Please check the log. (if it is open in Visio, please close the file).");
                return null;
            }
            else
            {
                if (explanationList.Any())
                    ShowExplanations(explanationList);
            }

            int pageNumber = 1;
            CreateAndDisplayNewDataSet(pageNumber);

            return SelectedVisioApp;
        }

        /// <summary>
        /// Display the explanation list.
        /// </summary>
        /// <param name="explanationList"></param>
        private void ShowExplanations(List<string> explanationList)
        { 
            logit(EnumLogFlags.Information, $"There were {explanationList.Count} Warnings/Errors");

            dialogExplanations dialog = new dialogExplanations();
            dialog.ExplanationList = explanationList;
            dialog.Message = "Processing completed, but with {explanationList.Count} Warnings/Errors.";

            dialog.ShowDialog();
        }

        /// <summary>
        /// A new SDX DataSet has been selected, so we must:
        /// * refresh the preview
        /// * refresh the DataSet display
        /// using the selected page, and then call OnNewSdxDataSet to display the necessary part.
        /// </summary>
        /// <param name="ds">The new DataSet</param>
        private bool OnNewSdxDataSetSelected(string name, string sourceType, string sourcePath, DataSet ds, out string explanation)
        {
            explanation = "";
            if (ds == null)
            {
                explanation = "DataSet is null.";
                return false;
            }

            try
            {
                SelectedSdxContext = new SdxDataSetContext(name, sourceType, sourcePath, ds);

                ClearDisplayPreview();

                DisplaySdxPreview();

                return true;
            }
            catch (Exception ex)
            {
                alert($"Err={ex}");
                return false;
            }
        }

        private void DisplaySdxPreview()
        {
            picPreviewDiagram.Refresh();
        }


        private void ClearDisplayPreview()
        {
            try
            {
                textPreviewMouse.Clear();
                picPreviewDiagram.Refresh();
            }
            catch (Exception ex)
            {
                logit(EnumLogFlags.Error, $"Err={ex}");
            }
        }

        /// <summary>
        /// Calls the picturebox context's 'Draw' method to display
        /// (preview) each of the shapes
        /// </summary>
        /// <param name="explanation"></param>
        /// <returns></returns>
        public bool DisplayPreview(SdxDataSetContext sdxContext, PicContext pContext, Graphics gg, out string explanation)
        {
            explanation = "";

            if (SelectedSdxContext == null)
            {
                alert("No SDX file selected.");
                return false;
            }

            string marker = "Begin";
            try
            {
                if (!pContext.Draw(gg, sdxContext, out explanation))
                    return false;

                return true;
            }
            catch (Exception ex)
            {
                explanation = $"Marker={marker} Err={ex}";
                return false;
            }
        }


        /// <summary>
        /// Prompt for, and read a visio file.
        /// </summary>
        /// <param name="explanation"></param>
        /// <returns></returns>
        public bool SelectAndOpenVisioFile(out SdxVisioApplication visioApp, List<string> explanationList)
        {
            explanationList.Clear();
            visioApp = null;

            string filepath = "";
            try
            {
                string explanation = "";
                if (!PromptForVisioFilepath(out filepath, out explanation))
                {
                    explanationList.Add(explanation);
                    return false;
                }

                textVisioFilePath.Text = filepath;

                visioApp = new SdxVisioApplication();

                return visioApp.OpenAndReadPackage(filepath, explanationList);

            }
            catch (Exception ex)
            {
                explanationList.Add( $"Exception={ex}");
                return false;
            }

        }
        /// <summary>
        /// Prompt for and return the filepath.
        /// Return true if the user selects and enters OK.
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="explanation"></param>
        /// <returns></returns>
        public static bool PromptForVisioFilepath(out string filepath, out string explanation)
        {
            explanation = "";
            filepath = "";
            try
            {
                OpenFileDialog dialog = new OpenFileDialog();

                string dir = SpecialFolder.MyDocuments.ToString();  
                dialog.InitialDirectory = dir;

                dialog.CheckFileExists = true;
                dialog.CheckPathExists = true;
                dialog.Filter = "Visio Files|*.vsdx";

                DialogResult result = dialog.ShowDialog();
                if (result != DialogResult.OK)
                    return false;

                filepath = dialog.FileName;
                return true;
            }
            catch (Exception ex)
            {
                explanation = $"Err={ex}";
                return false;
            }
        }


        private void buttonVisioToDsGetVisioFile_Click(object sender, EventArgs e)
        {
            List<String> explanationList = new List<string>();
            SelectedVisioApp = SelectAndOpenVisioFile(explanationList);
            if (SelectedVisioApp == null)
            {
                dialogExplanations dialog = new dialogExplanations();
                dialog.ExplanationList = explanationList;
                dialog.ShowDialog();
                EnableImporting(true);
            }
            else
            {
                comboVisioPages.DataSource = SelectedVisioApp.PageDict.Values.ToList();
                comboVisioPages.DisplayMember = "PageName";
                EnableImporting(true);
            }
        }

        private void picDiagram_MouseMove(object sender, MouseEventArgs e)
        {
            Point p1 = e.Location;
            PointF ptF = picContext.ConvertToPointF(p1);
            textPreviewMouse.Text = $"Point=({e.Location.X},{e.Location.Y}) PointF=({ptF.X.ToString("0.00")},{ptF.Y.ToString("0.00")}";

            Point p2 = picContext.ConvertFromPointF(ptF);

        }

        private void alert(string msg)
        {
            MessageBox.Show(msg);
            logit(EnumLogFlags.Warning, msg);
        }

        private void logit(EnumLogFlags flags, string msg)
        {
            Loggerton.Instance.LogIt(flags, msg);
        }

        private void comboVisioPages_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Select a page
                PageInfo pi = (PageInfo)comboVisioPages.SelectedItem;
                if (pi != null)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine($"Page={pi.PageName}");
                    sb.AppendLine($"Objects={pi.ObjectDict.Count} Links={pi.LinkDict.Count}");

                    textVisioData.Text = sb.ToString();
                    SelectedPage = pi;

                    CreateAndDisplayNewDataSet(pi.PageNumber);

                    EnableImporting(true);
                }

            }
            catch (Exception ex)
            {
                alert($"Err={ex}");
            }
        }


        /// <summary>
        /// Perform the import
        /// </summary>
        private void ImportVisioToFacility(bool autoScaleObjects)
        {
            List<string> explanationList = new List<string>();

            try
            {
                if (SelectedVisioApp == null)
                {
                    alert("Please select a Visio file.");
                    return;
                }

                int pageNbr = 1;
                if (SelectedPage != null)
                {
                    pageNbr = SelectedPage.PageNumber;
                }

                string explanation = "";
                logit(EnumLogFlags.Information, $"Info: Select Page={pageNbr} of file={SelectedVisioApp.FilePath}");
                DataSet ds = SelectedVisioApp.CreateSdxDataSet("Visio", textVisioFilePath.Text, pageNbr, out explanation);
                if (ds == null)
                {
                    explanationList.Add($"Errors creating the Simio SDX DataSet={explanation} ");
                    return;
                }

                SelectedSdxContext = new SdxDataSetContext("Visio", "Visio", SelectedVisioApp.FilePath, ds);

                SdxVector vOriginTranslate = new SdxVector((double)updnSimioXOrigin.Value, (double)updnSimioYOrigin.Value, (double)updnSimioZOrigin.Value);
                SdxVector vDrawingScale = new SdxVector((double)updnScaleDrawingX.Value, (double)updnScaleDrawingY.Value, (double)updnScaleDrawingZ.Value);
                SdxVector vObjectsScale = new SdxVector((double)updnScaleObjectsX.Value, (double)updnScaleObjectsY.Value, (double)updnScaleObjectsZ.Value);

                EnumTransformOptions transformOptions = 0;

                if ( autoScaleObjects )
                    transformOptions |= EnumTransformOptions.AutoScaleObjects;

                Transform = new SimioTransform(vDrawingScale, vObjectsScale, vOriginTranslate, transformOptions);

                if (SdxFacilityHelpers.CreateFacilityFromDataSet(DesignContext, ds, Transform, explanationList))
                    EnableImporting(false);

            }
            catch (Exception ex)
            {
                alert($"Err={ex}");

            }
            finally
            {
                if (explanationList.Any())
                    ShowExplanations(explanationList);
            }

        }

        private void EnableImporting(bool enable)
        {
            buttonImportVisioToFacility.Enabled = enable;
            importToSimioToolStripMenuItem.Enabled = enable;
        }

        private void buttonImportVisioToFacility_Click(object sender, EventArgs e)
        {
            ImportVisioToFacility(cbAutoScaleObjects.Checked);
        }

        private void picPreview_Paint(object sender, PaintEventArgs e)
        {
            string explanation = "";

            if (!DisplayPreview(SelectedSdxContext, picContext, e.Graphics, out explanation))
                logit(EnumLogFlags.Error, explanation);

        }

        private void picPreviewDiagram_MouseMove(object sender, MouseEventArgs e)
        {
            Point p1 = e.Location;
            PointF ptF = picContext.ConvertToPointF(p1);
            textPreviewMouse.Text = $"Point=({e.Location.X},{e.Location.Y}) PointF=({ptF.X.ToString("0.00")},{ptF.Y.ToString("0.00")}";

            Point p2 = picContext.ConvertFromPointF(ptF);

        }

        private void picPreviewDiagram_Resize(object sender, EventArgs e)
        {
            picPreviewDiagram.Refresh();
        }

        private void timerLogs_Tick(object sender, EventArgs e)
        {
            textLogs.Text = Loggerton.Instance.GetLogs(EnumLogFlags.All);
            Application.DoEvents();
        }

        private void textLogs_TextChanged(object sender, EventArgs e)
        {
            Loggerton.Instance.ClearLogs();
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
            Application.Exit();
        }

        private void actionsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void updnScaleObjectsX_ValueChanged(object sender, EventArgs e)
        {

        }

        private void updnSimioXOrigin_ValueChanged(object sender, EventArgs e)
        {
            EnableImporting(true);
        }

        private void updnSimioYOrigin_ValueChanged(object sender, EventArgs e)
        {
            EnableImporting(true);
        }

        private void updnSimioZOrigin_ValueChanged(object sender, EventArgs e)
        {
            EnableImporting(true);
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label17_Click(object sender, EventArgs e)
        {

        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void cbAutoScaleObjects_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
