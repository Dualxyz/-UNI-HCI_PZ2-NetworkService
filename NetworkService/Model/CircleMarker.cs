using NetworkService.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace NetworkService.Model
{
    public class CircleMarker : BindableBase
    {
        private string cmType;
        private int cmValue;
        private string cmDate;
        private string cmTime;
        private Thickness cmMargin;
        private Brush cmColor;

        public CircleMarker()
        {

        }

        public CircleMarker(string cmType, int cmValue, string cmDate, string cmTime)
        {
            CmType = cmType;
            CmValue = cmValue;
            CmDate = cmDate;
            CmTime = cmTime;
            CmColor = Brushes.Red;
        }

        public string CmType
        {
            get { return cmType; }
            set
            {
                cmType = value;
                OnPropertyChanged("CmType");
            }
        }

        public int CmValue
        {
            get { return cmValue; }
            set
            {
                cmValue = value;
                switch (cmValue)
                {
                    case 0:
                        CmMargin = new Thickness(0, 0, 0, cmValue);
                        break;
                    case 1:
                        CmMargin = new Thickness(0, 0, 0, 20);
                        break;
                    case 2:
                        CmMargin = new Thickness(0, 0, 0, 60);
                        break;
                    case 3:
                        CmMargin = new Thickness(0, 0, 0, 100);
                        break;
                    case 4:
                        CmMargin = new Thickness(0, 0, 0, 140);
                        break;
                    case 5:
                        CmMargin = new Thickness(0, 0, 0, 180);
                        break;
                    default:
                        CmMargin = new Thickness(0, 0, 0, 0);
                        break;
                }
                OnPropertyChanged("CmValue");
            }
        }

        public string CmDate
        {
            get { return cmDate; }
            set
            {
                cmDate = value;
                OnPropertyChanged("CmDate");
            }
        }

        public string CmTime
        {
            get { return cmTime; }
            set
            {
                cmTime = value;
                OnPropertyChanged("CmTime");
            }
        }

        public Thickness CmMargin
        {
            get { return cmMargin; }
            set
            {
                cmMargin = value;
                OnPropertyChanged("CmMargin");
            }
        }

        public Brush CmColor
        {
            get { return cmColor; }
            set
            {
                cmColor = value;
                OnPropertyChanged("CmColor");
            }
        }
    }
}
