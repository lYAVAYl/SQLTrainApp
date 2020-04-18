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
            private set => SetProperty(ref _error, value);
        }

        public string this[string columnName] => Error = Validate(columnName);


        public abstract string Validate(string columnName);

    }
}
