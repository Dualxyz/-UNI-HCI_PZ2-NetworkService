using NetworkService.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;

namespace NetworkService.Database
{
    public class Database
    {
        public static ObservableCollection<T4_Entity> Entities { get; set; } =  new ObservableCollection<T4_Entity>();
        public static ObservableCollection<T4_Entity> DraggableEntities { get; set; } =  new ObservableCollection<T4_Entity>();
        public static ObservableCollection<T4_Entity> PannelEntities { get; set; } =  new ObservableCollection<T4_Entity>();
        public static ObservableCollection<Canvas> CanvasCollection { get; set; } = new ObservableCollection<Canvas>();
        public static ObservableCollection<ProfileSaver> ProfileSaverList { get; set; } = new ObservableCollection<ProfileSaver>();

        public event PropertyChangedEventHandler PropertyChanged;
        private static ProfileSaver _profileSaver;

        public static ProfileSaver ProfileSaver 
        {
            get => _profileSaver;
            set
            {
                _profileSaver = value;
            } 
        }


    }
}
