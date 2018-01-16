using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Linq;
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
        public Table<Diary> Diary
        {
            get { return DataContext.Diary; }
        }

        public DiaryDataContext DataContext { get; }

        public FontFamily CurrentFont { get { return _CurrentFont; } set { if (_CurrentFont == value) return; _CurrentFont = value; OnPropertyChanged(nameof(CurrentFont)); } }
        private FontFamily _CurrentFont = new FontFamily("Comic Sans MS");

        public Brush CurrentColor { get { return _CurrentColor; } set { if (_CurrentColor == value) return; _CurrentColor = value; OnPropertyChanged(nameof(CurrentColor)); } }
        private Brush _CurrentColor = Brushes.Red;

        private void OnPropertyChanged(string aPropertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(aPropertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
