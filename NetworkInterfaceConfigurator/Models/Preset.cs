using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkInterfaceConfigurator.Models
{
    class Preset : ProperyChanged
    {
        // Constructor.
        public Preset()
        {

        }

        #region Variables, Constants & Properties.
        //Variables.
        private ushort id;

        //Properties.
        /// <summary>
        /// Get or Set id for preset.
        /// ID sets only from constructor.
        /// </summary>
        public ushort ID
        {
            get { return id; }
            set
            {
                id = value;
                OnPropertyChanged("ID");
            }
        }
        #endregion

        #region GetMethods
        #endregion

        #region SetMethods
        #endregion
    }
}
