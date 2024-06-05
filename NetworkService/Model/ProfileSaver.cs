using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace NetworkService.Model
{
    public class ProfileSaver : INotifyPropertyChanged
    {
        private static int _profileId;
        private string _profileName;
        private FilterRadioEnum _filterRadio;
        private EntityType _entityType;
        private int _itemId;

        public ProfileSaver(FilterRadioEnum filterRadio, EntityType entityType, int itemId)
        {
            _profileId++;
            _profileName = "Profile_"+_profileId;
            _filterRadio = filterRadio;
            _entityType = entityType;
            _itemId = itemId;
        }

        public string ProfileName
        {
            get => _profileName;
        }
        public EntityType EntityType
        {
            get => _entityType;
            set
            {
                _entityType = value;
                RaisePropertyChanged(nameof(EntityType));
            }
        }

        public int ItemId {
            get => _itemId;
            set
            {
                _itemId = value;
                RaisePropertyChanged(nameof(ItemId));
            }
        }

        public FilterRadioEnum FilterRadio
        {
            get => _filterRadio;
            set
            {
                _filterRadio = value;
                RaisePropertyChanged(nameof(FilterRadio));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        public override string ToString()
        {
            return _profileName;
        }
    }
}
