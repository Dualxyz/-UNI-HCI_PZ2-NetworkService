using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Xml.Linq;

namespace NetworkService.Model
{
    public class T4_Entity : INotifyPropertyChanged
    {
        private static readonly Random random = new Random();
        public event PropertyChangedEventHandler PropertyChanged;

        private int _id;
        private string _name;
        private EntityType _entityType;
        private double _value;

        public T4_Entity(int id, EntityType entityType)
        {
            Id = id;
            EntityType = entityType;
            Name = $"ID{_id}_TYPE{entityType}";
            Value = random.NextDouble() * 5.0;
        }

        public int Id
        {
            get => _id;
            set
            {
                _id = value;
                RaisePropertyChanged(nameof(Id));
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                RaisePropertyChanged(nameof(Name));
            }
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

        public double Value
        {
            get => _value;
            set
            {
                _value = value;
                RaisePropertyChanged(nameof(Value));
            }
        }

        private void RaisePropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        public override string ToString()
        {
            Console.WriteLine("hello");
            return "Id: " + Id + ", name: " + Name + ", entityType: " + EntityType;
        }
    }
}
