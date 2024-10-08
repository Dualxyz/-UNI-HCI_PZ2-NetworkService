﻿using NetworkService.Common;
using NetworkService.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace NetworkService.ViewModel
{
    public class MeasurementGraphViewModel : BindableBase
    {
        private static MeasurementGraphViewModel _instance;
        public static MeasurementGraphViewModel Instance { get { return _instance; } }
        private DispatcherTimer _timer;
        public ObservableCollection<CircleMarker> CircleMarkers { get; set; } = new ObservableCollection<CircleMarker>();
        public MyICommand RefreshCommand { get; set; }


        private ObservableCollection<T4_Entity> _comboBoxEntity;
        public ObservableCollection<T4_Entity> ComboboxEntity { 
            get { return _comboBoxEntity; }
            set { 
                _comboBoxEntity = value;
                OnPropertyChanged(nameof(ComboboxEntity));
            }
        }

        private T4_Entity selectedEntity;
        public T4_Entity SelectedEntity
        {
            get { return selectedEntity; }
            set
            {
                selectedEntity = value;
                OnPropertyChanged(nameof(SelectedEntity));
            }
        }
        public MeasurementGraphViewModel()
        {
            _instance = this;
            ComboboxEntity = Database.Database.Entities;
            RefreshCommand = new MyICommand(OnRefresh);
            SetUpTimer();

        }

        private void SetUpTimer()
        {
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(0.5)
            };
            _timer.Tick += (sender, args) => OnRefresh();
            _timer.Start();
        }

        private void OnRefresh()
        {
            try
            {
                //SetValuesToCircleMarkers(LoadLastFiveUpdates("Object_0"));
                SetValuesToCircleMarkers(LoadLastFiveUpdates(SelectedEntity));
            }
            catch
            {

            }
        }

        private List<CircleMarker> LoadLastFiveUpdates(T4_Entity selectedType)
        {
            string objName = String.Empty;
            try
            {
                objName = selectedEntity.Id.ToString();
                objName = "Object_" + objName;

                if (!File.Exists("LogFile.txt"))
                {
                    return null;
                }

                string[] lines = File.ReadAllLines("LogFile.txt");
                string pattern = @"Date Time:\s+(?<date>\d{2}/\d{2}/\d{4})\s+(?<time>\d{1,2}:\d{2}:\d{2}\s+[APM]{2})\s+(?<type>\w+)\s+Value:\s+(?<value>\d+)";
                Regex regex = new Regex(pattern, RegexOptions.Compiled);

                List<CircleMarker> lastFiveUpdates = new List<CircleMarker>();

                for (int i = lines.Count() - 1; i >= 0; i--)
                {
                    string line = lines[i];
                    Match match = regex.Match(line);

                    string dateStr = match.Groups["date"].Value;
                    string timeStr = match.Groups["time"].Value;
                    string type = match.Groups["type"].Value;
                    string valueStr = match.Groups["value"].Value;

                    if ((type == objName) && (lastFiveUpdates.Count < 5))
                    {
                        lastFiveUpdates.Add(new CircleMarker(type, int.Parse(valueStr), dateStr, timeStr));
                    }
                }

                return (lastFiveUpdates.Count == 5) ? lastFiveUpdates : null;
            }
            catch
            {
                return null;
            }
            
        }

        private void SetDefaultValuesToCircleMarkers()
        {
            for (int i = 0; i <= 4; i++)
            {
                CircleMarkers[i].CmType = "Type";
                CircleMarkers[i].CmValue = 0;
                CircleMarkers[i].CmDate = "Date";
                CircleMarkers[i].CmTime = "Time";
            }
        }

        private void SetValuesToCircleMarkers(List<CircleMarker> markers)
        {
            if (markers != null)
            {
                CircleMarkers.Clear();
                foreach (CircleMarker marker in markers)
                {
                    CircleMarkers.Add(marker);
                }
            }
            else
            {
                SetDefaultValuesToCircleMarkers();
            }
        }
    }
}
