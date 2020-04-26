using SQLTrainApp.Model.Commands;
using SQLTrainApp.Model.Logic;
using System;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Input;
using TrainSQL_DAL;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows.Media;
using System.Windows.Controls;
using System.Drawing;

using wForms = System.Windows.Forms;

namespace SQLTrainApp.ViewModel
{
    class EditThemePageViewModel:ValidateBaseViewModel, IPageViewModel
    {
        private string _themeIndex;
        public string ThemeID {
            get => _themeIndex;
            set
            {
                if (value.Length > 0)
                {
                    if (char.IsDigit(value.Last())) _themeIndex = value;
                }
                else _themeIndex = "";
            }
        }
        public string ThemeName { get; set; }
        public string ThemeInfo { get; set; }

        Theme _thisTheme = new Theme();

        public EditThemePageViewModel(int themeID=0)
        {

            if (themeID < 1)
            {
                _thisTheme = new Theme()
                {
                    ThemeID = 0,
                    ThemeName="",
                    Theory=""
                };
            }
            else
            {
                _thisTheme = TrainSQL_Commands.GetThemeByThemeID(themeID);
            }
            
            ThemeID = _thisTheme.ThemeID.ToString();
            ThemeName = _thisTheme.ThemeName;
            ThemeInfo = _thisTheme.Theory;            
        }

        private ICommand _saveChanges;
        public ICommand SaveChanges
        {
            get
            {
                return _saveChanges ?? (_saveChanges = new RelayCommand(x =>
                {
                    Save();
                }));
            }
        }

        private ICommand _cancelChanges;
        public ICommand CancelChanges
        {
            get
            {
                return _cancelChanges ?? (_cancelChanges = new RelayCommand(x =>
                {
                    Cancel();
                }));
            }
        }

        
        public override string Validate(string columnName)
        {
            string error = null;
            switch (columnName)
            {
                case nameof(ThemeID):
                    if (string.IsNullOrEmpty(ThemeID) || ThemeID == "0"
                        || (TrainSQL_Commands.IsThemeExists(Convert.ToInt32(ThemeID)) 
                            && ThemeID != _thisTheme?.ThemeID.ToString()))
                    {
                        error = "Недоступно";
                    }
                    break;
                case nameof(ThemeName):
                    if (string.IsNullOrEmpty(ThemeName))
                    {
                        error = "Отсутствует Тема";
                    }
                    break;
                case nameof(ThemeInfo):
                    if (string.IsNullOrEmpty(ThemeInfo))
                    {
                        error = "Отсутствует Теория";
                    }
                    break;
            }


            if (ErrorCollection.ContainsKey(columnName))
                ErrorCollection[columnName] = error;
            else ErrorCollection.Add(columnName, error);


            IsEnableBtn = ErrorCollection[nameof(ThemeID)] == null
                          && ErrorCollection[nameof(ThemeName)] == null
                          && ErrorCollection[nameof(ThemeInfo)] == null;

            OnPropertyChanged(nameof(ErrorCollection));

            return error;
        }

        private void Save()
        {
            wForms.DialogResult result = wForms.MessageBox.Show("Вы действительно хотите сохранить изменения?", "Подтверждение действия", wForms.MessageBoxButtons.YesNo, wForms.MessageBoxIcon.Question);

            if(result == wForms.DialogResult.Yes)
            {
                _thisTheme.ThemeID = Convert.ToInt32(ThemeID);
                _thisTheme.ThemeName = ThemeName;
                _thisTheme.Theory = ThemeInfo;

                if (TrainSQL_Commands.EditORAddTheme(_thisTheme)==null)
                {
                    MessageBox.Show("Изменения сохранены!");
                    Mediator.Notify("LoadTableOfContentsPage", "");
                }
                else
                {
                    MessageBox.Show("Что-то пошло не так");
                }
            }

        }


        private void Cancel()
        {
            wForms.DialogResult result = wForms.MessageBox.Show("Вы действительно хотите отменить изменения?", "Подтверждение действия", wForms.MessageBoxButtons.YesNo, wForms.MessageBoxIcon.Question);

            if (result == wForms.DialogResult.Yes)
            {
                Mediator.Notify("LoadTableOfContentsPage", "");
            }

        }        
    }
}
