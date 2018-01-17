using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq;
using System.Linq;
using System.Windows;
using System.Windows.Media;

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

        public void Insert(string DiaryContent)
        {
            DateTime Now = DateTime.Now;
            Diary aNewDiary = new Diary { Content = DiaryContent, Timestamp = Now };
            DataContext.Diary.InsertOnSubmit(aNewDiary);
        }

        public void GetDiaryList()
        {
            var aDiaries = from r in DataContext.Diary select r;
            foreach (Diary aDiary in aDiaries)
            {
                ;
            }
        }

        public void Submit()
        {
            DataContext.SubmitChanges();
        }

        public int SelectedIndex {
            get
            {
                return _SelectedIndex;
            }
            set
            {
                if (_SelectedIndex == value) return;
                _SelectedIndex = value;
                SeletedDiary = Diaries.ElementAtOrDefault(value);
                OnPropertyChanged(nameof(SelectedIndex));
            }
        }
        private int _SelectedIndex = 0;

        public Diary SeletedDiary { get { return _SeletedDiary; } set { if (_SeletedDiary == value) return; _SeletedDiary = value; OnPropertyChanged(nameof(SeletedDiary)); } }
        private Diary _SeletedDiary = new Diary { Id = 0, Content = "Please Selete A Diary Or Create A New Diary.", Timestamp = DateTime.Now };

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
