﻿using System.Windows;
using System.Windows.Controls;
using GenShin_Launcher_Plus.ViewModels;

namespace GenShin_Launcher_Plus.Views
{
    /// <summary>
    /// AddUsersPage.xaml 的交互逻辑
    /// </summary>
    public partial class UsersPage : UserControl
    {
        public UsersPage()
        {
            InitializeComponent();
            DataContext = new UsersPageViewModel();
        }
    }
}
