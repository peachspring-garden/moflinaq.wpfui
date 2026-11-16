using System;
using System.Linq;
using System.Windows;
using MoflinaQ.DocumentManagement.ViewModels;
using Wpf.Ui.Controls;

namespace MoflinaQ.DocumentManagement.Views
{
    public partial class MainWindow : FluentWindow
    {
        public MainWindow() : this(new MainWindowViewModel("山田 太郎", "システム管理部",
            Enum.GetValues(typeof(MenuPermission)).Cast<MenuPermission>()))
        {
        }

        public MainWindow(MainWindowViewModel viewModel)
        {
            ViewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
            InitializeComponent();
            DataContext = ViewModel;
        }

        public MainWindowViewModel ViewModel { get; }

        private void Navigation_Loaded(object sender, RoutedEventArgs e)
        {
            Navigation.Loaded -= Navigation_Loaded;
            Navigation.Navigate(typeof(HomePage));
        }

        private void Navigation_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (Navigation.SelectedItem?.Content is string title)
            {
                ViewModel.CurrentPageTitle = title;
            }
        }
    }
}
