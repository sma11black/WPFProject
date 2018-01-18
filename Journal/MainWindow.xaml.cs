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

        TextPointer FindFirstRunInTextContainer(DependencyObject container)
        {
            TextPointer position = null;
            
            if (container != null)
            {
                if (container is FlowDocument)
                    position = ((FlowDocument)container).ContentStart;
                else if (container is TextBlock)
                    position = ((TextBlock)container).ContentStart;
                else
                    return position;
            }
            
            while (position != null)
            {
                if (position.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.ElementStart)
                {
                    if (position.Parent is Run)
                        break;
                }

                // Not what we're looking for; on to the next position.
                position = position.GetNextContextPosition(LogicalDirection.Forward);
            }

            // This will be either null if no Run is found, or a position just inside of the first Run element in the
            // specifed text container.  Because position is formed from ContentStart, it will have a logical direction
            // of Backward.
            return position;
        }

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
                //IEnumerable<TextRange> wordRanges = _Model.GetAllWordRanges(tbk);
                //// 着色渲染由前端负责
                //// 色彩还原
                //tbk.TextEffects.Clear();
                //TextRange aTextRange = new TextRange(tbk.ContentStart, tbk.ContentEnd);
                //aTextRange.ApplyPropertyValue(TextElement.ForegroundProperty, _Model.CurrentColor);
                //aTextRange.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.White);
                //// 匹配关键字着色
                //foreach (TextRange wordRange in wordRanges)
                //{
                //    //wordRange.Text == "keyword"
                //    wordRange.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.White);
                //    wordRange.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.Black);
                //}

                FlowDocument doc = rtb.Document;
                IEnumerable<TextRange> wordRanges = _Model.GetAllWordRanges(doc);
                // 着色渲染由前端负责
                // 色彩还原
                doc.TextEffects.Clear();
                TextRange aTextRange = new TextRange(doc.ContentStart, doc.ContentEnd);
                aTextRange.ApplyPropertyValue(TextElement.ForegroundProperty, _Model.CurrentColor);
                aTextRange.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.White);
                // 匹配关键字着色
                foreach (TextRange wordRange in wordRanges)
                {
                    //wordRange.Text == "keyword"
                    wordRange.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.White);
                    wordRange.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.Black);
                }
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

        //private void tbx_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    tbk.TextEffects.Clear();
        //    tbk.Text = tbx.Text;
        //}

        //private void tbx_SizeChanged(object sender, SizeChangedEventArgs e)
        //{
        //    tbx_TextChanged(sender, null);
        //}
    }
}
