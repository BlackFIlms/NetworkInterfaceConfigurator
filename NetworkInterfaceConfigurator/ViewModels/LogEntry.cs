using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkInterfaceConfigurator.Models;

namespace NetworkInterfaceConfigurator.ViewModels
{
    class LogEntry : PropChanged
    {
        // Constructor.
        public LogEntry()
        {
            IndexCount++;
        }

        // Variables, Constants & Properties.
        private static int indexCount = 0;
        public static int IndexCount
        {
            get { return indexCount; }
            set
            {
                indexCount = value;
            }
        }

        private DateTime dateTime;
        public DateTime DateTime
        {
            get { return dateTime; }
            set
            {
                dateTime = value;
                OnPropertyChanged("DateTime");
            }
        }

        private string time;
        public string Time
        {
            get { return time; }
            set
            {
                time = value;
                OnPropertyChanged("Time");
            }
        }
        
        private int index;
        public int Index
        {
            get { return index; }
            set
            {
                index = value;
                OnPropertyChanged("Index");
            }
        }

        private string message;
        public string Message
        {
            get { return message; }
            set
            {
                message = value;
                OnPropertyChanged("Message");
            }
        }
    }

    class LogEntryMessage : LogEntry
    {
        private readonly string type = "Message";
        public string Type
        {
            get { return type; }
        }
    }

    class LogEntryWarning : LogEntry
    {
        private readonly string type = "Warning";
        public string Type
        {
            get { return type; }
        }
    }

    class LogEntryError : LogEntry
    {
        private readonly string type = "Error";
        public string Type
        {
            get { return type; }
        }
    }
}
