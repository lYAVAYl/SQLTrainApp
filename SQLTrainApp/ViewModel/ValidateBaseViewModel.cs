using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLTrainApp.ViewModel
{
    public abstract class ValidateBaseViewModel:BaseViewModel, IDataErrorInfo
    {
        private string _error;

        public string Error
        {
            get => _error;
            set => _error = value;
        }

        public bool IsEnableBtn { get; set; } = false;

        public Dictionary<string, string> ErrorCollection { get; set; } = new Dictionary<string, string>();
        public string this[string columnName] => Validate(columnName);


        public abstract string Validate(string columnName);

    }
}
