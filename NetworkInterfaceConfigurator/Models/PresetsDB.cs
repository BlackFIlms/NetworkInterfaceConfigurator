using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NetworkInterfaceConfigurator.Models
{
    class PresetsDB
    {
        // Variables, Constants & Properties.
        private SQLiteConnection dbConn;
        private SQLiteCommand sqlCmd = new SQLiteCommand();

        #region Methods
        /// <summary>
        /// Creates DB file and connect to it.
        /// Return true if create and connect to DB.
        /// </summary>
        /// <param name="dbFileName">Full path to DataBase file.</param>
        /// <returns>True if create and connect to DB.</returns>
        public bool CreateAndConnect(string dbFileName)
        {
            bool result = false;

            SQLiteConnection.CreateFile(dbFileName);

            try
            {
                dbConn = new SQLiteConnection("Data Source=" + dbFileName + ";Version=3;");
                dbConn.Open();
                sqlCmd.Connection = dbConn; // Sets connection for commands.

                sqlCmd.CommandText = "CREATE TABLE IF NOT EXISTS Presets (ID INTEGER NOT NULL PRIMARY KEY, IP TEXT, Subnet TEXT, Gateway TEXT, DNS1 TEXT, DNS2 TEXT, MAC TEXT, MACR TEXT)"; // Sets command property for create table in DB.
                sqlCmd.ExecuteNonQuery(); // Create table in DB.

                result = true;
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

            return result;
        }
        /// <summary>
        /// If you have DB with table, you should use this method fot connect to DB.
        /// Return true if connect to DB.
        /// </summary>
        /// <param name="dbFileName">Full path to DataBase file.</param>
        /// <returns>True if connect to DB.</returns>
        public bool Connect(string dbFileName)
        {
            bool result = false;

            try
            {
                dbConn = new SQLiteConnection("Data Source=" + dbFileName + ";Version=3;");
                dbConn.Open();
                sqlCmd.Connection = dbConn; // Sets connection for commands.

                result = true;
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            return result;
        }
        /// <summary>
        /// Disconnects from DB.
        /// Return true if disconnected from DB.
        /// </summary>
        /// <returns>True if disconnected from DB.</returns>
        public bool Disconnect()
        {
            bool result = false;

            try
            {
                sqlCmd.Dispose();
                dbConn.Close();
                dbConn.Dispose();

                result = true;
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            return result;
        }
        /// <summary>
        /// Loading all data from DB.
        /// Return List with arrays which contain rows.
        /// </summary>
        public List<object[]> Load()
        {
            List<object[]> listObj = new List<object[]>();

            if (dbConn.State == ConnectionState.Open) // Check connection to DB.
            {
                try
                {
                    string sqlQuery = "SELECT * FROM Presets"; // Define query.
                    sqlCmd.CommandText = sqlQuery; // Set query to command.
                    SQLiteDataReader sqlReader = sqlCmd.ExecuteReader(); // Start sqlReader with query to DB.
                    
                    // Start read data from DB, and write it to List.
                    while (sqlReader.Read())
                    {
                        object[] objArr = new object[sqlReader.FieldCount]; // Array, for write each row from DB.
                        sqlReader.GetValues(objArr); // Write each row, from DB to array.
                        listObj.Add(objArr); // Write array to List.
                    }

                    sqlReader.Close(); // Close sqlReader session.
                    return listObj;
                }
                catch (SQLiteException ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }

            return listObj;
        }
        /// <summary>
        /// Add preset to db with current adapter settings.
        /// Return true if preset added to db.
        /// </summary>
        /// <param name="adapter">Array of adapter settings.</param>
        /// <returns>True if preset added to db.</returns>
        public bool AddPreset(string[] adapter)
        {
            bool result = false;

            if (dbConn.State == ConnectionState.Open) // Check connection to DB.
            {
                try
                {
                    // Sets adapter settings from array to command property.
                    sqlCmd.CommandText = "INSERT INTO Presets ('IP', 'Subnet', 'Gateway', 'DNS1', 'DNS2', 'MAC', 'MACR') values ('" +
                        adapter[0] + "' , '" +
                        adapter[1] + "' , '" +
                        adapter[2] + "' , '" +
                        adapter[3] + "' , '" +
                        adapter[4] + "' , '" +
                        adapter[5] + "' , '" +
                        adapter[6] + "')";

                    sqlCmd.ExecuteNonQuery(); // Add preset to DB.

                    result = true;
                }
                catch (SQLiteException ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }

            return result;
        }
        /// <summary>
        /// Edit preset in db with settings from edit preset window.
        /// Return true if preset edited in db.
        /// </summary>
        /// <param name="id">Index of updating adapter.</param>
        /// <param name="adapter">Array of adapter settings.</param>
        /// <returns>True if preset edited in db.</returns>
        public bool EditPreset(int id, string[] adapter)
        {
            bool result = false;

            if (dbConn.State == ConnectionState.Open) // Check connection to DB.
            {
                try
                {
                    // Sets adapter settings from array to command property.
                    sqlCmd.CommandText = "UPDATE Presets SET ('IP', 'Subnet', 'Gateway', 'DNS1', 'DNS2', 'MAC', 'MACR') = ('" +
                        adapter[0] + "' , '" +
                        adapter[1] + "' , '" +
                        adapter[2] + "' , '" +
                        adapter[3] + "' , '" +
                        adapter[4] + "' , '" +
                        adapter[5] + "' , '" +
                        adapter[6] + "') WHERE ID = " + id;

                    sqlCmd.ExecuteNonQuery(); // Edit preset in DB.

                    result = true;
                }
                catch (SQLiteException ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }

            return result;
        }
        /// <summary>
        /// Delete preset from db.
        /// Return true if preset deleted from db.
        /// </summary>
        /// <param name="id">Index of deleting adapter.</param>
        /// <param name="presetCount">Amount of presets.</param>
        /// <returns>True if preset deleted from db.</returns>
        public bool DeletePreset(int id, int presetCount)
        {
            bool result = false;

            if (dbConn.State == ConnectionState.Open) // Check connection to DB.
            {
                try
                {
                    // Sets id for delete adapter.
                    sqlCmd.CommandText = "DELETE FROM Presets WHERE ID = " + id;

                    sqlCmd.ExecuteNonQuery(); // Delete preset from DB.

                    for (int i = ++id; i <= (presetCount); i++)
                    {
                        sqlCmd.CommandText = "UPDATE Presets SET ID = " + (i - 1) + " WHERE ID = " + i;

                        sqlCmd.ExecuteNonQuery(); // Edit preset from DB.
                    }

                    result = true;
                }
                catch (SQLiteException ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }

            return result;
        }
        #endregion
    }
}
