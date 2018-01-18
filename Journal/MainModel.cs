using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Journal
{
    class MainModel : INotifyPropertyChanged
    {
        public const string ConnectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=journal;Integrated Security=True;";
        public MainModel()
        {
            // InitDB
            DataContext = new DiaryDataContext(ConnectionString);
            if (!DataContext.DatabaseExists())
            {
                DataContext.CreateDatabase();
            }
            // Check
            if (!DataContext.DatabaseExists()) throw new ApplicationException("数据库不存在！");
            DataContext.Diary.FirstOrDefault();
        }

        public IEnumerable<TextRange> GetAllWordRanges(FlowDocument document)
        {
            // string pattern = @"[^\W\d](\w|[-']{1,2}(?=\w))*";
            Regex aRegex = new Regex(Pattern);
            TextPointer pointer = document.ContentStart;
            while (pointer != null)
            {
                if (pointer.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
                {
                    string textRun = pointer.GetTextInRun(LogicalDirection.Forward);
                    //MatchCollection matches = Regex.Matches(textRun, pattern);
                    MatchCollection aMatches = aRegex.Matches(textRun);
                    foreach (Match aMatch in aMatches)
                    {
                        int startIndex = aMatch.Index;
                        int length = aMatch.Length;
                        TextPointer start = pointer.GetPositionAtOffset(startIndex);
                        TextPointer end = start.GetPositionAtOffset(length);
                        // 验证选中的文本匹配
                        //TextRange aTextRange = new TextRange(start, end);
                        //if (aRegex.IsMatch(aTextRange.Text)) { yield return new TextRange(start, end); }
                        yield return new TextRange(start, end);
                    }
                }

                pointer = pointer.GetNextContextPosition(LogicalDirection.Forward);
            }
        }

        public void GetMatches()
        {
            Regex aRegex = new Regex(Pattern);
            //Matches = aRegex.Matches(TargetText);
            MatchCollection aMatches = aRegex.Matches(TargetText);

        }

        public void Insert()
        {
            DateTime Now = DateTime.Now;
            Diary aNewDiary = new Diary { Content = "", Timestamp = Now };
            DataContext.Diary.InsertOnSubmit(aNewDiary);
            Submit();
            // 默认应该选中第一个
            SelectedIndex = 0;
        }

        public void Save()
        {
            Diary aDiary = (from r in DataContext.Diary where r.Id == SeletedDiary.Id select r).FirstOrDefault();
            if (aDiary != null)
            {
                aDiary.Content = TargetText;
            }
            Submit();
        }


        internal void Delete()
        {
            Diary aDiary = (from r in DataContext.Diary where r.Id == SeletedDiary.Id select r).FirstOrDefault();
            if (aDiary != null)
            {
                DataContext.Diary.DeleteOnSubmit(aDiary);
            }
            Submit();
        }

        public void Submit()
        {
            DataContext.SubmitChanges();
            OnPropertyChanged(nameof(Diaries));
        }

        public bool CanStartSearch
        {
            get
            {
                return !string.IsNullOrWhiteSpace(Pattern) && !string.IsNullOrWhiteSpace(TargetText);
            }
        }

        public bool CanSave
        {
            get
            {
                return TargetText != null && SelectedIndex!=-1 && SeletedDiary != null && !TargetText.Equals(SeletedDiary.Content);
            }
        }

        public bool CanDelete
        {
            get
            {
                return SelectedIndex != -1 && SeletedDiary != null;
            }
        }

        public string Pattern
        {
            get { return _Pattern; }
            set
            {
                if (_Pattern == value) return; _Pattern = value; OnPropertyChanged(nameof(Pattern));
            }
        }
        private string _Pattern;

        public string TargetText { get { return _TargetText; } set { if (_TargetText == value) return; _TargetText = value; OnPropertyChanged(nameof(TargetText)); } }
        private string _TargetText = "请在左侧选择一个日记 或 点击新建以添加日记.";

        public int SelectedIndex
        {
            get
            {
                return _SelectedIndex;
            }
            set
            {
                //if (value < 0 || value >= Diaries.Count) return;
                if (_SelectedIndex == value) return;
                _SelectedIndex = value;
                SeletedDiary = Diaries.ElementAtOrDefault(value);
                TargetText = SeletedDiary.Content;
                OnPropertyChanged(nameof(SelectedIndex));
            }
        }
        private int _SelectedIndex = -1;

        public Diary SeletedDiary { get { return _SeletedDiary; } set { if (_SeletedDiary == value) return; _SeletedDiary = value; OnPropertyChanged(nameof(SeletedDiary)); } }
        private Diary _SeletedDiary = new Diary { Id = -1, Content = "", Timestamp = DateTime.Now };

        public List<Diary> Diaries => DataContext.Diary.OrderByDescending(e => e.Timestamp).ToList();

        public DiaryDataContext DataContext { get; }

        // Font
        public string CurrentFont { get { return _CurrentFont; } set { if (_CurrentFont == value) return; _CurrentFont = value; OnPropertyChanged(nameof(CurrentFont)); } }
        private string _CurrentFont = "Comic Sans MS";

        // Color
        public string CurrentColor { get { return _CurrentColor; } set { if (_CurrentColor == value) return; _CurrentColor = value; OnPropertyChanged(nameof(CurrentColor)); } }
        private string _CurrentColor = "Black";

        private void OnPropertyChanged(string aPropertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(aPropertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
