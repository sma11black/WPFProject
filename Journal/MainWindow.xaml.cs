using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Collections.Generic;
using System.Windows.Media;

namespace Journal
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            try
            {
                _Model = new MainModel();
                this.DataContext = _Model;
                
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private MainModel _Model;

        private void OnCreate_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                _Model.Insert();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void OnCreate_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _Model != null;
        }

        private void OnSave_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                _Model.Save();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void OnSave_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _Model != null && _Model.CanSave;
        }

        private void OnStartSearch_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                _Model.GetMatches();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void OnStartSearch_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _Model != null && _Model.CanStartSearch;
        }

        private void OnDelete_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                _Model.Delete();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void OnDelete_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _Model != null && _Model.CanDelete;
        }
    }
}
