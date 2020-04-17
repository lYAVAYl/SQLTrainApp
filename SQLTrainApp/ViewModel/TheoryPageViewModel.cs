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


namespace SQLTrainApp.ViewModel
{
    class TheoryPageViewModel : BaseViewModel, IPageViewModel
    {
        public int ThemeID { get; set; }
        public string ThemeName { get; set; }
        public string ThemeInfo { get; set; }
        Theme _currentTheme;

        public TheoryPageViewModel()
        {
            GetTheme();            
        }

        private ICommand _previousTheme;
        public ICommand PreviousTheme
        {
            get
            {
                return _previousTheme ?? (_previousTheme = new RelayCommand(x =>
                {
                    GetTheme(ThemeID-1);
                }));
            }
        }

        private ICommand _nextTheme;
        public ICommand NextTheme
        {
            get
            {
                return _nextTheme ?? (_nextTheme = new RelayCommand(x =>
                {
                    GetTheme(ThemeID+1);
                }));
            }
        }

        private void GetTheme(int themeID = 1)
        {

            using (var context = new TrainSQL_Entities())
            {
                if(themeID>0 && themeID < context.Themes.Count())
                {
                    _currentTheme = context.Themes.FirstOrDefault(x => x.ThemeID == themeID);
                    UpdateContent(_currentTheme.ThemeID, _currentTheme.ThemeName, _currentTheme.Theory);
                }                    
            }

            //MessageBox.Show("Обновление темы");
        }

        private void UpdateContent(int id, string name, string content)
        {
            ThemeID = id;
            ThemeName = name;
            ThemeInfo = content;
        }


        private ICommand _loadTableOfContentsPage;
        public ICommand LoadTableOfContentsPage
        {
            get
            {
                return _loadTableOfContentsPage ?? (_loadTableOfContentsPage = new RelayCommand(x =>
                {
                    Mediator.Notify("LoadTableOfContentsPage", "");
                }));
            }
        }


    }
}
