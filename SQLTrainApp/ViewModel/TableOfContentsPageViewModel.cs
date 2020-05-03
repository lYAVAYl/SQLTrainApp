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
using System.Collections.ObjectModel;

using wForms = System.Windows.Forms;


namespace SQLTrainApp.ViewModel
{
    class TableOfContentsPageViewModel : BaseViewModel, IPageViewModel
    {
        private ContentsCollection _contentList;
        private bool _isAdmin;

        public ContentsCollection ContentList // здесь хранится ToC
        {
            get => _contentList;
            set
            {
                _contentList = value;
                OnPropertyChanged();
            }
        }

        public bool IsAdmin // права админа
        {
            get => _isAdmin;
            set
            {
                _isAdmin = value;
                OnPropertyChanged();
            }
        }
        
        public TableOfContentsPageViewModel()
        {
            IsAdmin = CurrentUser.Role == "Administrator"; // сделать админом

            // загрузка ToC должна производиться из класса, отвечающего за работу с данными
            // из конструктора это делать не рекомендуется
            ContentList = new ContentsCollection(TrainSQL_Commands.GetAllThemes());

            // и вообще лучше это делать асинхронно, чтобы не морозить интерфейс, пока идет загрузка
            //_ = LoadTableOfContentsAsync();
        }

        private ICommand _addItem;
        public ICommand AddItem => _addItem ?? (_addItem = new RelayCommand(parameter =>
        {
            Mediator.Notify("LoadEditThemePage", 0);
        }));

        #region ShitCode
        //    public string SearchedChapter { get; set; }
        //    public Visibility CanEdit { get; set; } = Visibility.Hidden;
        //    public Theme SelectedTheme { get; set; }

        //    public ObservableCollection<Theme> ContentList { get; set; }

        //    public TableOfContentsPageViewModel()
        //    {
        //        ContentList = new ObservableCollection<Theme>(TrainSQL_Commands.GetAllThemes());
        //        CanEdit = CurrentUser.Role == "Administrator" ? Visibility.Visible : Visibility.Hidden;
        //    }

        //    private ICommand _loadTheme;
        //    public ICommand LoadTheme
        //    {
        //        get
        //        {
        //            return _loadTheme ?? (_loadTheme = new RelayCommand(x =>
        //              {
        //                  ShowTheme(x as Theme);
        //              }));
        //        }
        //    }

        //    private ICommand _editTheme;
        //    public ICommand EditTheme
        //    {
        //        get
        //        {
        //            return _editTheme ?? (_editTheme = new RelayCommand(x =>
        //              {
        //                  if (x is Theme)
        //                      Mediator.Notify("LoadEditThemePage", TrainSQL_Commands.GetThemeIndex(x as Theme));
        //                  else
        //                      Mediator.Notify("LoadEditThemePage", 0);

        //              }));
        //        }
        //    }

        //    private ICommand _deleteTheory;
        //    public ICommand DeleteTheme
        //    {
        //        get
        //        {
        //            return _deleteTheory ?? (_deleteTheory = new RelayCommand(x =>
        //            {
        //                MessageBox.Show("Удаление");
        //            }));
        //        }
        //    }

        //    private void ShowTheme(Theme theme)
        //    {
        //        Mediator.Notify("LoadTheoryPage", TrainSQL_Commands.GetThemeIndex(theme));
        //    }
        #endregion

    }

    public class ContentsCollection : ObservableCollection<Theme>
    {
        public ContentsCollection(List<Theme> themes = null) : base(themes)
        {

        }
        

        private ICommand _deleteItemCommand;
        private ICommand _editItemCommand;
        private ICommand _openItemCommand;

        // эта команда реализована и работает, остальные команды приведены как демо   
        public ICommand DeleteItemCommand => _deleteItemCommand ?? (_deleteItemCommand = new RelayCommand(parameter =>
        {
            if (parameter is Theme item)
            {
                if (MessageBox.Show(item.ToString(), "Удалить?", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
                {
                    TrainSQL_Commands.DeleteTheme(item);
                    Remove(item);
                }
            }
        }));

        public ICommand EditItemCommand => _editItemCommand ?? (_editItemCommand = new RelayCommand(parameter =>
        {
            if (parameter is Theme item)
            {
                Mediator.Notify("LoadEditThemePage", item.ThemeID);
            }
        }));

        public ICommand OpenItemCommand => _openItemCommand ?? (_openItemCommand = new RelayCommand(parameter =>
        {
            if (parameter is Theme item)
            {
                Mediator.Notify("LoadTheoryPage", item.ThemeID);

            }
        }));
    }
}
