using NetworkService.Common;
using NetworkService.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Threading;

namespace NetworkService.ViewModel
{
    public class MeasurementGraphViewModel : BindableBase
    {
        private DispatcherTimer _timer;
        public ObservableCollection<CircleMarker> CircleMarkers { get; set; } = new ObservableCollection<CircleMarker>();
        public MyICommand DebugCommand { get; set; }
        public MeasurementGraphViewModel()
        {
            DebugCommand = new MyICommand(OnDebug);
            SetUpTimer();

        }

        private void SetUpTimer()
        {
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(0.5)
            };
            _timer.Tick += (sender, args) => OnDebug();
            _timer.Start();
        }

        private void OnDebug()
        {
            try
            {
                SetValuesToCircleMarkers(LoadLastFiveUpdates("Object_0"));
                //var resp = LoadLastFiveUpdates("Object_0");
                //StringBuilder sb = new StringBuilder();
                //foreach(var item in resp)
                //{
                //    //sb.AppendLine(item.CmType.ToString() + "" + Convert.ToString(item.CmValue) +"\n");
                //    sb.AppendLine($"{item.CmType} {item.CmValue}");
                //}
                //MessageBox.Show($"{sb}");
            }
            catch
            {

            }
            
            
        }

        private List<CircleMarker> LoadLastFiveUpdates(string selectedType)
        {
            if (!File.Exists("LogFile.txt"))
            {
                //ErrorMessage = "Log file doesn't exist.";
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

                if ((type == selectedType) && (lastFiveUpdates.Count < 5))
                {
                    lastFiveUpdates.Add(new CircleMarker(type, int.Parse(valueStr), dateStr, timeStr));
                }
            }

            return (lastFiveUpdates.Count == 5) ? lastFiveUpdates : null;
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
                //for (int i = 0; i <= 4; i++)
                //{
                //    CircleMarkers[i].CmType = markers[4 - i].CmType;
                //    CircleMarkers[i].CmValue = markers[4 - i].CmValue;
                //    CircleMarkers[i].CmDate = markers[4 - i].CmDate;
                //    CircleMarkers[i].CmTime = markers[4 - i].CmTime;
                //}
            }
            else
            {
                // Ako se u log fajlu nalazi manje od 5 promena vrednosti
                // Na kruzice se postavljaju default vrednosti
                SetDefaultValuesToCircleMarkers();
            }
        }
    }
}
