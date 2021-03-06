﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

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

        private const string ConfigFileName = "Diary.cfg";
        public void SavaConfig()
        {
            // 使用Xml的linq库
            XDocument aXDocument = new XDocument(
                new XElement("Config",
                    new XElement("Color", CurrentColor),
                    new XElement("Font", CurrentFont)
                )
            );
            aXDocument.Save(ConfigFileName);
        }
        public void LoadConfig()
        {
            XDocument aXDocument = XDocument.Load(ConfigFileName);
            CurrentColor = aXDocument.Root.Element("Color").Value;
            CurrentFont = aXDocument.Root.Element("Font").Value;
            ConfigXml = aXDocument;
        }

        public void ReadXmlFile()
        {
            // 使用xml库
            //XmlDocument aXml = new XmlDocument();
            //aXml.Load(ConfigFileName);

            // 使用Xml的linq库
            if (ConfigXml != null)
            {
                IEnumerable<XElement> childList = from el in ConfigXml.Elements() select el;
                ConfigString = "";
                foreach (XElement e in childList)
                    ConfigString += e;
            }
            Console.WriteLine(ConfigString);
        }

        // 匹配
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

        // 数据库操作
        internal void Insert()
        {
            DateTime Now = DateTime.Now;
            Diary aNewDiary = new Diary { Content = "", Timestamp = Now };
            DataContext.Diary.InsertOnSubmit(aNewDiary);
            Submit();
            // 默认应该选中第一个
            SelectedIndex = 0;
        }

        internal void Save()
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

        internal void Submit()
        {
            DataContext.SubmitChanges();
            OnPropertyChanged(nameof(Diaries));
        }

        // 可执行逻辑判断
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

        public bool CanOpen
        {
            get
            {
                return File.Exists(ConfigFileName);
            }
        }

        // 存储字体与颜色配置
        public XDocument ConfigXml { get { return _ConfigXml; } set { if (_ConfigXml == value) return; _ConfigXml = value; OnPropertyChanged(nameof(ConfigXml)); } }
        private XDocument _ConfigXml;

        public string ConfigString { get { return _ConfigString; } set { if (_ConfigString == value) return; _ConfigString = value; OnPropertyChanged(nameof(ConfigString)); } }
        private string _ConfigString;

        // 匹配模式串
        public string Pattern
        {
            get { return _Pattern; }
            set
            {
                if (_Pattern == value) return; _Pattern = value; OnPropertyChanged(nameof(Pattern));
            }
        }
        private string _Pattern;
        
        // 匹配结果
        public class MatchView
        {
            public int Index { get; set; }
            public string Value { get; set; }
        }
        public List<MatchView> Matches { get { return _Matches; } private set { if (_Matches == value) return; _Matches = value; OnPropertyChanged(nameof(Matches)); } }
        private List<MatchView> _Matches;

        // 匹配目标文本
        public string TargetText { get { return _TargetText; } set { if (_TargetText == value) return; _TargetText = value; OnPropertyChanged(nameof(TargetText)); } }
        private string _TargetText = "请在左侧选择一个日记 或 点击新建以添加日记.";

        // 选中的日记Index
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

        // 选中的日记
        public Diary SeletedDiary { get { return _SeletedDiary; } set { if (_SeletedDiary == value) return; _SeletedDiary = value; OnPropertyChanged(nameof(SeletedDiary)); } }
        private Diary _SeletedDiary = new Diary { Id = -1, Content = "请在左侧选择一个日记 或 点击新建以添加日记.", Timestamp = DateTime.Now };

        // 所有日记
        public List<Diary> Diaries => DataContext.Diary.OrderByDescending(e => e.Timestamp).ToList();

        // 数据库Model对象
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
