using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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

        public class MatchView
        {
            public int Index { get; set; }
            public string Value { get; set; }
        }

        public void GetMatches()
        {
            Regex aRegex = new Regex(Pattern);
            MatchCollection aMatches = aRegex.Matches(TargetText);
            List<MatchView> aMatchViews = new List<MatchView>();
            for (int i = 0; i < aMatches.Count; i++)
            {
                aMatchViews.Add(new MatchView { Index = i, Value = aMatches[i].Value });
            }
            Matches = aMatchViews;
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

        public List<MatchView> Matches { get { return _Matches; } private set { if (_Matches == value) return; _Matches = value; OnPropertyChanged(nameof(Matches)); } }
        private List<MatchView> _Matches;

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
                if (_SelectedIndex == value) return;
                _SelectedIndex = value;
                if(value == -1)
                {
                    SeletedDiary = new Diary { Id = -1, Content = "请在左侧选择一个日记 或 点击新建以添加日记.", Timestamp = DateTime.Now };
                }
                else
                {
                    SeletedDiary = Diaries.ElementAtOrDefault(value);
                }
                TargetText = SeletedDiary.Content;
                OnPropertyChanged(nameof(SelectedIndex));
            }
        }
        private int _SelectedIndex = -1;

        public Diary SeletedDiary { get { return _SeletedDiary; } set { if (_SeletedDiary == value) return; _SeletedDiary = value; OnPropertyChanged(nameof(SeletedDiary)); } }
        private Diary _SeletedDiary = new Diary { Id = -1, Content = "请在左侧选择一个日记 或 点击新建以添加日记.", Timestamp = DateTime.Now };

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
