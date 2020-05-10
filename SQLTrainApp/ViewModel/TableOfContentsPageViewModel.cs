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

            ContentList = new ContentsCollection(TrainSQL_Commands.GetAllThemes());
        }

        private ICommand _addItem;
        public ICommand AddItem => _addItem ?? (_addItem = new RelayCommand(parameter =>
        {
            Mediator.Inform("LoadEditThemePage", 0);
        }));
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
                if (MessageBox.Show($"Вы действительно хотите удалить тему #{item.ThemeID}?", "Удалить?", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
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
                Mediator.Inform("LoadEditThemePage", item.ThemeID);
            }
        }));

        public ICommand OpenItemCommand => _openItemCommand ?? (_openItemCommand = new RelayCommand(parameter =>
        {
            if (parameter is Theme item)
            {
                Mediator.Inform("LoadTheoryPage", item.ThemeID);
            }
        }));
    }
}
