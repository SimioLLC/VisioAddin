
using SdxVisio;
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
using static System.Environment;
using System.Xml.Linq;
using SdxHelpers;
using LoggertonHelpers;

namespace SdxHarness
{
    public partial class FormSdx : Form
    {

        /// <summary>
        /// Selected Excell App
        /// </summary>
        //SdxExcelApplication SelectedExcelApp = new SdxExcelApplication();

        /// <summary>
        /// Selected Visio App
        /// </summary>
        SdxVisioApplication SelectedVisioApp = new SdxVisioApplication();

        PageInfo CurrentVisioPageInfo { get; set; } = null;

        /// <summary>
        /// The file and type we are currently working on.
        /// This is displayed at the top of the form(s)
        /// </summary>
        String CurrentFileContext { get; set; }

        /// <summary>
        /// The current SDX context
        /// </summary>
        SdxDataSetContext SelectedSdxContext { get; set; }

        /// <summary>
        /// Context for drawing the picturebox (preview diagram)
        /// </summary>
        PicContext picContext;

        public FormSdx()
        {
            InitializeComponent();
        }

        private void FormSdx_Load(object sender, EventArgs e)
        {
            picContext = new PicContext(picPreviewDiagram, SelectedSdxContext);

            propertyGrid1.SelectedObject = Properties.Settings.Default;

            ClearDisplays();

            timerLogs.Interval = 1000;
            timerLogs.Enabled = true;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
            Application.Exit();
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

                string dir = Properties.Settings.Default.VisioFileFolder;
                if ( string.IsNullOrEmpty(dir))
                {
                    dir = SpecialFolder.MyDocuments.ToString();
                }
                dialog.InitialDirectory = dir;

                dialog.CheckFileExists = true;
                dialog.CheckPathExists = true;
                dialog.Filter = "Visio Files|*.vsdx;*.vssx";

                DialogResult result = dialog.ShowDialog();
                if (result != DialogResult.OK)
                    return false;

                filepath = dialog.FileName;
                Properties.Settings.Default.VisioFileFolder = Path.GetDirectoryName(filepath);
                Properties.Settings.Default.Save();
                return true;
            }
            catch (Exception ex)
            {
                explanation = $"Err={ex}";
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
        private bool PromptForExcelFilepath(out string filepath, out string explanation)
        {
            explanation = "";
            filepath = "";
            try
            {
                OpenFileDialog dialog = new OpenFileDialog();
                string dir = Properties.Settings.Default.ExcelFileFolder;
                if (string.IsNullOrEmpty(dir))
                {
                    dir = SpecialFolder.MyDocuments.ToString();
                }
                dialog.InitialDirectory = dir;

                dialog.CheckFileExists = true;
                dialog.CheckPathExists = true;
                dialog.Filter = "Excel Files|*.xslx";

                DialogResult result = dialog.ShowDialog();
                if (result != DialogResult.OK)
                    return false;

                filepath = dialog.FileName;
                Properties.Settings.Default.ExcelFileFolder = Path.GetDirectoryName(filepath);
                Properties.Settings.Default.Save();

                return true;
            }
            catch (Exception ex)
            {
                explanation = $"Err={ex}";
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
        private bool PromptForDataSetFilepath(out string filepath, out string explanation)
        {
            explanation = "";
            filepath = "";
            try
            {
                OpenFileDialog dialog = new OpenFileDialog();

                string dir = Properties.Settings.Default.DataSetFolder;
                if (string.IsNullOrEmpty(dir))
                {
                    dir = SpecialFolder.MyDocuments.ToString();
                }
                dialog.InitialDirectory = dir;

                dialog.CheckFileExists = true;
                dialog.CheckPathExists = true;
                dialog.Filter = "DataSet Files (xml)|*.xml";

                DialogResult result = dialog.ShowDialog();
                if (result != DialogResult.OK)
                    return false;

                filepath = dialog.FileName;
                Properties.Settings.Default.DataSetFolder = Path.GetDirectoryName(filepath);
                Properties.Settings.Default.Save();

                return true;
            }
            catch (Exception ex)
            {
                explanation = $"Err={ex}";
                return false;
            }
        }

        /// <summary>
        /// Get a path we can save to.
        /// It is checked for existence.
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        private bool GetSaveFilepath(out string filepath, out string explanation)
        {
            explanation = "";
            filepath = string.Empty;

            try
            {
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.CheckPathExists = true;
                dialog.InitialDirectory = Application.StartupPath;

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

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {

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

        private void writeDataSetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string filepath = "";
            string explanation = "";
            if (!GetSaveFilepath(out filepath, out explanation))
            {
                alert(explanation);
                return;
            }

            ////if (!SelectedExcelApp.WriteInternalDataSet(filepath, true, out explanation))
            ////    alert(explanation);
        }

        private void readDataSetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string filepath = "";
            string explanation = "";
            if (!PromptForDataSetFilepath(out filepath, out explanation))
            {
                alert(explanation);
                return;
            }

            ////if (!SelectedExcelApp.ReadInternalDataSet(filepath, out explanation))
            ////    alert(explanation);

        }

        private void openExcelFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string filepath = "";
                string explanation = "";
                ////if (!PromptForExcelFilepath(out filepath, out explanation))
                ////{
                ////    alert(explanation);
                ////    return;
                ////}

                ////if (!SelectedExcelApp.OpenAndReadPackage(filepath, out explanation))
                ////{
                ////    alert(explanation);
                ////    return;
                ////}

            }
            catch (Exception ex)
            {

                alert($"Err={ex}");
            }

        }


        /// <summary>
        /// Prompt for, and read a visio file.
        /// A Visio app is created and returned.
        /// </summary>
        /// <param name="explanation"></param>
        /// <returns></returns>
        public static bool SelectAndOpenVisioFile(out SdxVisioApplication visioApp, List<string> explanationList)
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

                visioApp = new SdxVisioApplication();


                return visioApp.OpenAndReadPackage(filepath, explanationList);

            }
            catch (Exception ex)
            {
                explanationList.Add( $"Err={ex}");
                return false;
            }

        }

        private string ToStringExplanationList(List<string> explanationList)
        {
            if (explanationList == null)
                return "";
            StringBuilder sb = new StringBuilder();
            foreach (string ss in explanationList)
            {
                sb.AppendLine(ss);
            }
            return sb.ToString();

        }

        /// <summary>
        /// Prompt for, and read a visio file.
        /// An initial (page 1) dataset is created from the visio file.
        /// </summary>
        /// <param name="explanation"></param>
        /// <returns></returns>
        public SdxVisioApplication SelectAndOpenVisioFile(List<string> explanationList)
        {
            explanationList.Clear();

            SelectAndOpenVisioFile(out SelectedVisioApp, explanationList);

            if (SelectedVisioApp == null)
            {
                alert($"Could not process file. Reason={ToStringExplanationList(explanationList)}");
                return null;
            }

            if ( explanationList.Any() )
            {
                alert($"File processed ok, but with Warnings={ToStringExplanationList(explanationList)}");
            }

            // Process it into a Sdx DataSet for the arbitrarily selected first page.
            int pageNumber = 1;
            string explanation = "";
            logit( EnumLogFlags.Information, $"Info: Select Page={pageNumber} of file={SelectedVisioApp.FilePath}");
            DataSet ds = SelectedVisioApp.CreateSdxDataSet("Visio", SelectedVisioApp.FilePath, pageNumber, out explanation);
            if (ds != null)
            {
                if ( !OnNewSdxDataSetSelected("Visio", "Visio", SelectedVisioApp.FilePath, ds, out explanation))
                {
                    alert(explanation);
                    return null;
                }
                SelectedSdxContext = new SdxDataSetContext("Visio", "Visio", SelectedVisioApp.FilePath, ds);
                if (!DisplayVisioApp(SelectedVisioApp, out explanation))
                    return null;
            }

            return SelectedVisioApp;
        }

        private void openVisioFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string explanation = "";
            ClearDisplays();

            List<string> explanationList = new List<string>();
            SelectedVisioApp = SelectAndOpenVisioFile(explanationList);

            if (SelectedVisioApp == null)
                return;

        }


        private void DisplaySdxDataSet(DataSet ds)
        {
            if (ds == null || ds.Tables.Count == 0)
                return;

            comboDataSetTable.DataSource = null;
            comboDataSetTable.Items.Clear();

            List<DataTable> tableList = new List<DataTable>();
            foreach (DataTable dt in ds.Tables)
                tableList.Add(dt);

            comboDataSetTable.DataSource = tableList;
            comboDataSetTable.DisplayMember = "TableName";
        }


        private void timerLogs_Tick(object sender, EventArgs e)
        {
            textLogs.Text = Loggerton.Instance.GetLogs(EnumLogFlags.All);
            Application.DoEvents();
        }

        /// <summary>
        /// Assumes that there is a currently selected Visio file, and 
        /// a currently selected page from that file.
        /// Writes the resulting dataset to a file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void writeVisioDataSetToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                if ( SelectedVisioApp == null || CurrentVisioPageInfo == null)
                {
                    alert("Select a Visio file and page first.");
                    return;
                }

                SaveFileDialog dialog = new SaveFileDialog();
                dialog.InitialDirectory = Environment.SpecialFolder.MyDocuments.ToString();
                dialog.Filter = "XML File|*.xml";

                DialogResult result = dialog.ShowDialog();
                if (result != DialogResult.OK)
                    return;

                int pageNbr = CurrentVisioPageInfo.PageNumber;

                string explanation = "";
                DataSet ds = SelectedVisioApp.CreateSdxDataSet("Visio", SelectedVisioApp.FilePath, pageNbr, out explanation);
                if ( ds == null)
                {
                    alert($"Could not Create DataSet. Err={explanation}");
                }

                string path = dialog.FileName;
                ds.WriteXml(path);
            }
            catch (Exception ex)
            {
                alert($"Err={ex}");
            }
        }


        /// <summary>
        /// Prompt for SDX file (*.xml) and set the selected SDX DataSet
        /// file as SelectedSdxDataSet.
        /// </summary>
        /// <param name="explanation"></param>
        /// <returns></returns>
        private bool PromptAndGetDataSetPath(out string sdxDatasetPath, out string explanation)
        {
            explanation = "";
            sdxDatasetPath = "";

            try
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Multiselect = false;
                string dir = Properties.Settings.Default.DataSetFolder;
                if (string.IsNullOrEmpty(dir))
                    dir = Environment.GetFolderPath(SpecialFolder.MyDocuments);

                dialog.InitialDirectory = dir;

                //dialog.RootFolder = Environment.SpecialFolder.MyDocuments;
                dialog.Filter = "XML Files|*.xml";

                DialogResult result = dialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    string filename = dialog.FileName;

                    if ( !SdxDataSetContext.IsValidSdx(filename, out explanation))
                        return false;

                    string folder = Path.GetDirectoryName(filename);
                    Properties.Settings.Default.DataSetFolder = folder;
                    Properties.Settings.Default.Save();

                    SelectedSdxContext = new SdxDataSetContext("SDX", filename);
                    OnSdxDataSetSelected();

                    textDescSdxDataSetPath.Text = filename;
                }
                return true;
            }
            catch (Exception ex)
            {
                explanation = $"Err={ex}";
                return false;
            }
        }

        /// <summary>
        /// Actions that occure when a new SDX DataSet is selected.
        /// </summary>
        private void OnSdxDataSetSelected()
        {
            logit(EnumLogFlags.Information, $"Set New SDX DataSet Context={SelectedSdxContext.Name}");

            textDescSdxDataSetPath.Text = SelectedSdxContext.Name;

            //Test: Create a file now.
            string folder = Path.GetTempPath();
            if (Directory.Exists(folder))
            {
                string filename = "TestSdxDataSet.xml";
                string filepath = Path.Combine(folder, filename);
                SelectedSdxContext.SdxDataSet.WriteXml(filepath);
            }

        }




        private void buttonPreviewDraw_Click(object sender, EventArgs e)
        {
            if (SelectedSdxContext == null)
            {
                alert("Select an Application or Read in an SDX file directly.");
                return;
            };

            picPreviewDiagram.Refresh();


        }

        /// <summary>
        /// Grab a SDX DataSet.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReadSdxDataSetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string explanation = "";

            ClearDisplays();

            string path = "";
            if (!PromptAndGetDataSetPath(out path, out explanation))
            {
                alert(explanation);
                return;
            }
        }


        /// <summary>
        /// Clear all the displays.
        /// </summary>
        private void ClearDisplays()
        {
            ClearDataSetDisplay();
            ClearDisplayPreview();
            ClearDisplayVisioDetails();
        }

        private void ClearDataSetDisplay()
        {
            try
            {
                comboDataSetTable.DataSource = null;
                comboDataSetTable.Items.Clear();

                dgvDataTable.Rows.Clear();

            }
            catch (Exception ex)
            {
                logit(EnumLogFlags.Error, $"Err={ex}");
            }

        }

        private void ClearDisplayPreview()
        {
            try
            {
                textPreviewPicInfo.Clear();
                picPreviewDiagram.Refresh();
            }
            catch (Exception ex)
            {
                logit(EnumLogFlags.Error, $"Err={ex}");
            }
        }

        private void ClearVisioPage()
        {
            try
            {
                treeVisioArtifacts.Nodes.Clear();
                treeVisioConnects.Nodes.Clear();
                treeVisioShapes.Nodes.Clear();

                textVisioArtifactXml.Clear();
                textVisioConnectXml.Clear();
                textVisioShapeXml.Clear();

            }
            catch (Exception ex)
            {
                logit(EnumLogFlags.Error, $"Err={ex}");
            }
        }


        private void ClearDisplayVisioDetails()
        {
            try
            {
                treeVisioMasters.Nodes.Clear();
                textVisioMasterTemplateXml.Clear();
                textVisioMasterXml.Clear();

                ClearVisioPage();

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
        public bool DisplayPreview(SdxDataSetContext sdxContext, PicContext pContext, Graphics gg, out string explanation )
        {
            explanation = "";

            if ( SelectedSdxContext == null )
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

        private void picPreviewDiagram_Paint(object sender, PaintEventArgs e)
        {
            string explanation = "";

            if (!DisplayPreview( SelectedSdxContext, picContext, e.Graphics, out explanation))
                logit(EnumLogFlags.Error, explanation);
        }

        private void picDiagram_Resize(object sender, EventArgs e)
        {

            picPreviewDiagram.Refresh();
        }

        private void picDiagram_MouseMove(object sender, MouseEventArgs e)
        {
            Point p1 = e.Location;
            PointF ptF = picContext.ConvertToPointF(p1);
            textPreviewPicInfo.Text = $"Point=({e.Location.X},{e.Location.Y}) PointF=({ptF.X.ToString("0.00")},{ptF.Y.ToString("0.00")}";

            Point p2 = picContext.ConvertFromPointF(ptF);

        }



        private void comboPreviewDataSet_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void FormSdx_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.Save();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dialogAbout dialog = new dialogAbout();
            dialog.ShowDialog();
        }

        /// <summary>
        /// Display the entire VisioApp.
        /// This should be done when a new Visio file is selected.
        /// Puts the Master information in a treelist, and - if the combobox is empty -
        /// also populates the pages of the VisioApp into the combo.
        /// </summary>
        /// <param name="visioApp"></param>
        private bool DisplayVisioApp(SdxVisioApplication visioApp, out string explanation)
        {
            explanation = "";
            try
            {

                // Show the Masters in the tree
                ClearDisplayVisioDetails();

                foreach (MasterInfo mi in visioApp.MasterSummaryDict.Values.OrderBy(rr => rr.Id))
                {
                    string info = $"{mi.Id} ({mi.Name} Class={mi.SimioClass} MasterTemplate={mi.TemplateId}";
                    TreeNode tn = treeVisioMasters.Nodes.Add(info);

                    tn.Tag = mi;
                } // foreach master

                textDescApplicationPath.Text = visioApp.FilePath;

                // Put the pages in the combo
                if ( comboVisioPages.DataSource == null )
                {
                    comboVisioPages.DataSource = null;
                    comboVisioPages.Items.Clear();

                    comboVisioPages.DataSource = visioApp.PageDict.Values.ToList();
                    comboVisioPages.DisplayMember = "PageName";
                }

                comboVisioPages.SelectedItem = CurrentVisioPageInfo;
                if (!OnNewVisioPageSelected(CurrentVisioPageInfo, out explanation))
                    return false;

                return true;
            }
            catch (Exception ex)
            {
                explanation = $"Err={ex}";
                return false;
            }
        }

        private void DisplayVisioMaster(MasterInfo mi)
        {
            if (mi == null)
                return;

            try
            {
                textVisioMasterXml.Text = mi.XMaster.ToString();

                string ss = mi.TemplateId;

                int templateId = 0;
                templateId = int.Parse(mi.TemplateId);

                MasterInfo ti = null;
                if ( SelectedVisioApp.MasterTemplateDict.TryGetValue(templateId, out ti))
                {
                    textVisioMasterTemplateXml.Text = ti.XMaster.ToString();
                }

            }
            catch (Exception ex)
            {
                logit(EnumLogFlags.Error, $"Err={ex}");
            }
        }

        private void DisplayVisioPage(PageInfo pi)
        {
            try
            {

                foreach (ShapeInfo si in pi.ShapeDict.Values.OrderBy(rr => rr.Id))
                {
                    string info = $"{si.Id} ({si.ShapeName} Class={si.SimioClass} Master={si.MasterId}";
                    TreeNode tn = treeVisioShapes.Nodes.Add(info);

                    tn.Tag = si;
                }

                foreach (ConnectInfo ci in pi.ConnectDict.Values)
                {
                    string info = $"From={ci.FromCellName} To={ci.ToCellName}";
                    TreeNode tn = treeVisioConnects.Nodes.Add(info);

                    tn.Tag = ci;
                }

                foreach (ArtifactInfo ai in pi.ArtifactDict.Values)
                {
                    string info = $"{ai.ArtifactId} ({ai.Name} Master={ai.MasterId}";
                    TreeNode tn = treeVisioArtifacts.Nodes.Add(info);

                    tn.Tag = ai;
                }
            }
            catch (Exception ex)
            {
                logit(EnumLogFlags.Error, $"Err={ex}");
            }
        }

        private void DisplayVisioShape(ShapeInfo si)
        {
            try
            {
                XElement xe = si.XShape;
                textVisioShapeXml.Text = xe.ToString();
            }
            catch (Exception ex)
            {
                logit(EnumLogFlags.Error, $"Err={ex}");
            }
        }

        private void DisplayVisioConnect(ConnectInfo ci)
        {
            try
            {
                XElement xe = ci.XConnect;
                textVisioConnectXml.Text = xe.ToString();
            }
            catch (Exception ex)
            {
                logit(EnumLogFlags.Error, $"Err={ex}");
            }
        }

        private void DisplayVisioArtifact(ArtifactInfo ai)
        {
            try
            {
                XElement xe = ai.XShape;
                textVisioArtifactXml.Text = xe.ToString();
            }
            catch (Exception ex)
            {
                logit(EnumLogFlags.Error, $"Err={ex}");
            }
        }

        //private void buttonVisioRefresh_Click(object sender, EventArgs e)
        //{
        //    DisplayVisioApp(SelectedVisioApp);
        //}




            /// <summary>
            /// A new visio page is selected, which means we must create a new SDX DataSet 
            /// using the selected page, and then call OnNewSdxDataSet to display the necessary part.
            /// </summary>
            /// <param name="pi"></param>
        private bool OnNewVisioPageSelected(PageInfo pi, out string explanation)
        {
            explanation = "";
            if ( pi == null )
            {
                explanation = $"PageInfo is null";
                return false;
            }

            try
            {
                string info = $"Set New Visio Page={CurrentVisioPageInfo.PageName}";
                logit(EnumLogFlags.Information, info);

                DataSet ds = SelectedVisioApp.CreateSdxDataSet("Visio", SelectedVisioApp.FilePath, pi.PageNumber, out explanation);
                if (ds != null)
                {
                    SelectedSdxContext = new SdxDataSetContext("Visio", "Visio", SelectedVisioApp.FilePath, ds);
                    ClearVisioPage();

                    if ( !OnNewSdxDataSetSelected("Visio", "Visio", SelectedVisioApp.FilePath, ds, out explanation))
                    {
                        logit(EnumLogFlags.Error, $"On SDX Selected. Err={explanation}");
                        return false;
                    }
                }

                DisplayVisioPage(pi);
                return true;
            }
            catch (Exception ex)
            {
                explanation = $"PageInfo={pi} Err={ex}";
                return false;
            }
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
            if ( ds == null )
            {
                explanation = "DataSet is null.";
                return false;
            }

            try
            {
                SelectedSdxContext = new SdxDataSetContext(name, sourceType, sourcePath, ds);

                ClearDisplayPreview();
                ClearDataSetDisplay();

                DisplaySdxPreview();
                DisplaySdxDataSet(SelectedSdxContext.SdxDataSet);

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


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboVisioPages_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ClearVisioPage();

                if (comboVisioPages.SelectedItem == null)
                    return;

                this.CurrentVisioPageInfo = (PageInfo)comboVisioPages.SelectedItem;
                string explanation = "";
                if (!OnNewVisioPageSelected(this.CurrentVisioPageInfo, out explanation))
                    alert(explanation);

            }
            catch (Exception ex)
            {
                logit(EnumLogFlags.Error, $"Err={ex}");
            }
        }

        private void treeShapes_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            try
            {
                ShapeInfo si = (ShapeInfo)e.Node.Tag;
                DisplayVisioShape(si);
            }
            catch (Exception ex)
            {
                logit(EnumLogFlags.Error, $"Err={ex}");
            }
        }

        private void treeVisioMasters_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            try
            {
                MasterInfo mi = (MasterInfo)e.Node.Tag;
                DisplayVisioMaster(mi);
            }
            catch (Exception ex)
            {
                logit(EnumLogFlags.Error, $"Err={ex}");
            }
        }

        private void treeVisioMasters_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node == null)
                return;

            if ( e.Node.IsSelected )
            {
                MasterInfo mi = (MasterInfo)e.Node.Tag;
                DisplayVisioMaster(mi);
            }
        }

        private void treeVisioShapes_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node == null)
                return;

            if (e.Node.IsSelected)
            {
                ShapeInfo si = (ShapeInfo)e.Node.Tag;
                DisplayVisioShape(si);
            }

        }

        private void treeVisioArtifacts_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node == null)
                return;

            if (e.Node.IsSelected)
            {
                ArtifactInfo ai = (ArtifactInfo) e.Node.Tag;
                DisplayVisioArtifact(ai);
            }

        }

        private void treeVisioConnects_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node == null)
                return;

            if (e.Node.IsSelected)
            {
                ConnectInfo ci = (ConnectInfo)e.Node.Tag;
                DisplayVisioConnect(ci);
            }

        }

        private void treeVisioConnects_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {

        }

        private void treeVisioArtifacts_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {

        }


        /// <summary>
        /// Prompt the user for a dataset file (an XML file) and return the dataset.
        /// </summary>
        /// <param name="explanation"></param>
        /// <returns></returns>
        private DataSet GetDataSet(out string explanation)
        {
            explanation = "";
            DataSet ds = null;

            try
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "XML File|*.xml";

                DialogResult result = dialog.ShowDialog();
                if ( result == DialogResult.OK )
                {
                    string filepath = dialog.FileName;
                    if (SdxApplication.ReadDataSet(filepath, out ds, out explanation))
                        return null;
                }
                return ds;
            }
            catch (Exception ex)
            {
                explanation = $"Err={ex}";
                return null;
            }
        }

        /// <summary>
        /// A new datatable is selected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboDataSetTable_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ComboBox combo = sender as ComboBox;
                if (combo == null)
                    return;

                DataTable dt = combo.SelectedItem as DataTable;
                if (dt == null)
                    return;

                dgvDataTable.DataSource = null;
                dgvDataTable.DataSource = dt;

            }
            catch (Exception ex)
            {
                logit( EnumLogFlags.Error, $"Err={ex}");
            }
        }

        private void textLogs_TextChanged(object sender, EventArgs e)
        {
            Loggerton.Instance.ClearLogs();
        }
    }
}
