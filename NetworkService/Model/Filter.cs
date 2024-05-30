using NetworkService.Common;
using System.ComponentModel;

namespace NetworkService.Model
{
    public class Filter : INotifyPropertyChanged
    {
        private EntityType _selectedType;
        private int _id; //string because it's from the textbox; might change to int...
        private FilterRadioEnum _radioEnum;

        public Filter(EntityType selectedType, int id, FilterRadioEnum radioEnum)
        {
            _selectedType = selectedType;
            _id = id;
            _radioEnum = radioEnum;
        }

        public EntityType SelectedType
        {
            get => _selectedType;
            set
            {
                _selectedType = value;
                RaisePropertyChanged(nameof(SelectedType));
            }
        }
        public int Id { 
            get => _id;
            set { 
                _id = value; 
                RaisePropertyChanged(nameof(Id));
            }
        }

        public FilterRadioEnum RadioEnum { 
            get => _radioEnum; 
            set {
                _radioEnum = value;
                RaisePropertyChanged(nameof(RadioEnum));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
