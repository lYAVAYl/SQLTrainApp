﻿using SQLTrainApp.Model.Commands;
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
    class TheoryPageViewModel:BaseViewModel, IPageViewModel
    {
        public string ChapterInfo { get; set; }

        public TheoryPageViewModel()
        {
            using(var context = new TrainSQL_Entities())
            {
                ChapterInfo = context.Themes.FirstOrDefault(x => x.ThemeID == 22).Theory;
            }
        }
    }
}
