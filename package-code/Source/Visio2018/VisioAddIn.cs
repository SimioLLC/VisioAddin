using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using SimioAPI;
using SimioAPI.Extensions;
using SdxHelpers;
using Visio2018;
using LoggertonHelpers;

namespace Visio2018
{
    class VisioAddIn : IDesignAddIn
    {
        #region IDesignAddIn Members

        /// <summary>
        /// Property returning the name of the add-in. This name may contain any characters and is used as the display name for the add-in in the UI.
        /// </summary>
        public string Name
        {
            get { return "VisioAddIn2018"; }
        }

        /// <summary>
        /// Property returning a short description of what the add-in does.
        /// </summary>
        public string Description
        {
            get { return "The Simio Add-In for Visio (2018)"; }
        }

        /// <summary>
        /// Property returning an icon to display for the add-in in the UI.
        /// </summary>
        public System.Drawing.Image Icon
        {
            get { return Properties.Resources.SDX; }
        }

        /// <summary>
        /// Method called when the add-in is run.
        /// </summary>
        public void Execute(IDesignContext context)
        {

            string marker = "Begin";
            try
            {

                // This example code places some new objects from the Standard Library into the active model of the project.
                if (context.ActiveModel != null)
                {

                    // Launch the form to select a Visio file.
                    FormVisio dialog = new FormVisio();
                    dialog.DesignContext = context;

                    dialog.Show();

                    DataSet ds = dialog.SelectedSdxContext?.SdxDataSet;

                    SimioTransform transform = dialog.Transform; 
                }
            }
            catch (Exception ex)
            {
                alert($"Marker={marker} Err={ex}");
            }
        }

        private void alert(string msg)
        {
            MessageBox.Show($"{msg}");
            logit(msg);
        }


        private void logit(string msg)
        {
            Loggerton.Instance.LogIt(EnumLogFlags.Error, msg);
            // Your logger here.
        }
        #endregion
    }

}
