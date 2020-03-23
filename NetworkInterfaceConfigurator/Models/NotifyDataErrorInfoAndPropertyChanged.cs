using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace NetworkInterfaceConfigurator.Models
{
    class NotifyDataErrorInfoAndPropertyChanged : ProperyChanged, INotifyDataErrorInfo
    {
        #region Implementation basic validation.
        private readonly Dictionary<string, List<string>> _errorsByPropertyName = new Dictionary<string, List<string>>();

        public bool HasErrors => _errorsByPropertyName.Any();

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public IEnumerable GetErrors(string propertyName)
        {
            return _errorsByPropertyName.ContainsKey(propertyName) ?
                _errorsByPropertyName[propertyName] : null;
        }

        private void OnErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        protected void AddError(string propertyName, string error)
        {
            if (!_errorsByPropertyName.ContainsKey(propertyName))
                _errorsByPropertyName[propertyName] = new List<string>();

            if (!_errorsByPropertyName[propertyName].Contains(error))
            {
                _errorsByPropertyName[propertyName].Add(error);
                OnErrorsChanged(propertyName);
            }
        }

        protected void ClearErrors(string propertyName)
        {
            if (_errorsByPropertyName.ContainsKey(propertyName))
            {
                _errorsByPropertyName.Remove(propertyName);
                OnErrorsChanged(propertyName);
            }
        }
        #endregion

        #region Implementation additional validation.
        /// <summary>
        /// Compare all elements in bit array with reference value, and return true if all elements in array equal to reference value.
        /// </summary>
        protected bool EqualAll(BitArray bitarray, bool reference)
        {
            int count = 0;

            foreach (bool bit in bitarray)
            {
                if (bit != reference)
                    count++;
            }

            return count > 0 ? false : true;
        }
        /// <summary>
        /// Validate string on pattern.
        /// </summary>
        /// <param name="str">String for validate.</param>
        /// <param name="pattern">Pattern for validate.</param>
        /// <returns>Bool value. True if string format equal to pattern.</returns>
        protected bool ValidateStringFormat(string str, string pattern)
        {
            bool value = false;

            System.Text.RegularExpressions.MatchCollection matches;
            matches = System.Text.RegularExpressions.Regex.Matches(str, pattern);

            if (matches.Count > 0)
            {
                value = true;
            }
            return value;
        }
        /// <summary>
        /// Validate int values in string on pattern.
        /// </summary>
        /// <param name="str">String for validate.</param>
        /// <param name="pattern">Values pattern for validate.</param>
        /// <param name="valueGroups">Number of value groups in string.</param>
        /// <param name="maxValue">Max value. If one part of the string contain value bigger than this, returned - false.</param>
        /// <returns>Bool value. True if all values in the string less or equal maxValue.</returns>
        protected bool ValidateIntMaxValues(string str, string pattern, int valueGroups, int maxValue)
        {
            bool value = false;
            BitArray values = new BitArray(valueGroups);
            values.SetAll(false);

            System.Text.RegularExpressions.MatchCollection matches;
            matches = System.Text.RegularExpressions.Regex.Matches(str, pattern);

            if (matches.Count > 0)
            {
                for (int i = 0; i < matches.Count; i++)
                {
                    if (Convert.ToInt16(matches[i].ToString()) <= maxValue)
                        values[i] = true;
                }
            }
            value = EqualAll(values, true);
            return value;
        }
        /// <summary>
        /// Validate hex values in string on pattern.
        /// </summary>
        /// <param name="str">String for validate.</param>
        /// <param name="pattern">Values pattern for validate.</param>
        /// <param name="valueGroups">Number of value groups in string.</param>
        /// <param name="maxValue">Max value. If one part of the string contain value bigger than this, returned - false.</param>
        /// <returns>Bool value. True if all values in the string less or equal maxValue.</returns>
        protected bool ValidateHEXMaxValues(string str, string pattern, int valueGroups, int maxValue)
        {
            bool value = false;
            BitArray values = new BitArray(valueGroups);
            values.SetAll(false);

            System.Text.RegularExpressions.MatchCollection matches;
            matches = System.Text.RegularExpressions.Regex.Matches(str, pattern);

            if (matches.Count > 0)
            {
                for (int i = 0; i < matches.Count; i++)
                {
                    if (Convert.ToInt32(matches[i].ToString(), 16) <= maxValue)
                        values[i] = true;
                }
            }
            value = EqualAll(values, true);
            return value;
        }
        #endregion
    }
}
