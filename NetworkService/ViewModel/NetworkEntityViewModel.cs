using NetworkService.Common;
using NetworkService.Model;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace NetworkService.ViewModel
{
    public class NetworkDisplayViewModel : BindableBase
    {
        public static int EntityId = 0;
        private ObservableCollection<string> _entityTypeList;
        private ObservableCollection<T4_Entity> _tableEntityList = new ObservableCollection<T4_Entity>();
        private string _selectedItemForAdding;
        private string _selectedItemForFiltering;
        private string _itemIdTextBox;
        private int _radioSelectedValue = -1;
        private T4_Entity _selectedEntity;
        private ObservableCollection<T4_Entity> _entityList = new ObservableCollection<T4_Entity>();
        private ObservableCollection<T4_Entity> _displayEntityList = new ObservableCollection<T4_Entity>();
        private bool dragging = false;

        private bool _IsSelectedDeleteButton;
        private bool _IsSelectedAddButton;
        private T4_Entity _selectedEntityFromEntityList;
        public ObservableCollection<T4_Entity> SavedList = new ObservableCollection<T4_Entity>();

        private BindableBase currentViewModel;
        private readonly NetworkEntitiesViewModel _networkEntitiesViewModel = new NetworkEntitiesViewModel();

        public BindableBase CurrentViewModel
        {
            get { return currentViewModel; }
            set
            {
                SetProperty(ref currentViewModel, value);
            }
        }

        public MyICommand AddCommand { get; set; }
        public MyICommand DeleteCommand { get; set; }
        public MyICommand FilterCommand { get; set; }
        public MyICommand UndoCommand { get; set; }
        public MyICommand EqualsRadioButtonCommand { get; set; }
        public MyICommand LessRadioButtonCommand { get; set; }
        public MyICommand GreaterRadioButtonCommand { get; set; }

        public MyICommand<ListView> SelectionChangedCommand { get; set; } // Komanda koja reaguje na izmenu
        public MyICommand MouseLeftButtonUpCommand { get; set; } // Komanda koja reaguje na levi klik

        Stack StackOfOperations { get; set; } = new Stack();
        Stack AddedEntities { get; set; } = new Stack();
        Stack DeletedEntities { get; set; } = new Stack();

        #region Getters and Setters
        public int RadioSelectedValue { get => _radioSelectedValue; set => _radioSelectedValue = value; }
        public string ItemIdTextBox { 
            get { return _itemIdTextBox; }
            set {
                _itemIdTextBox = value;
                OnPropertyChanged(nameof(ItemIdTextBox));
                if (SelectedTypeForFiltering != null)
                {
                    FilterCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public T4_Entity SelectedEntityFromEntityList
        {
            get => _selectedEntityFromEntityList;
            set
            {
                _selectedEntityFromEntityList = value;
                OnPropertyChanged(nameof(SelectedEntityFromEntityList));
                MessageBox.Show(_selectedEntityFromEntityList.ToString());
            }
        }

        public ObservableCollection<T4_Entity> DisplayEntityList
        {
            get { return _displayEntityList; }
            set
            {
                _displayEntityList = value;
                OnPropertyChanged(nameof(DisplayEntityList));
            }
        }
        public bool IsSelectedDeleteButton
        {
            get { return _IsSelectedDeleteButton; }
            set
            {
                _IsSelectedDeleteButton = value;
                OnPropertyChanged(nameof(IsSelectedDeleteButton));
            }
        }

        public bool IsSelectedAddButton
        {
            get { return _IsSelectedAddButton; }
            set
            {
                _IsSelectedAddButton = value;
                OnPropertyChanged(nameof(IsSelectedAddButton));
            }
        }

        public T4_Entity SelectedEntity
        {
            get { return _selectedEntity; }
            set
            {
                _selectedEntity = value;
                OnPropertyChanged(nameof(SelectedEntity));
                DeleteCommand.RaiseCanExecuteChanged();
            }
        }
        public ObservableCollection<T4_Entity> TableEntityList
        {
            get { return _tableEntityList; } set
            {
                _tableEntityList = value;
                OnPropertyChanged(nameof(TableEntityList));
            }
        }

        public ObservableCollection<T4_Entity> EntityList
        {
            get { return _entityList; }
            set
            {
                _entityList = value;
                OnPropertyChanged(nameof(EntityList));
            }
        }

        public ObservableCollection<string> EntityTypeList
        {
            get { return _entityTypeList; }
            set
            {
                _entityTypeList = value;
                OnPropertyChanged(nameof(EntityTypeList));
            }
        }

        public string SelectedTypeForAdding
        {
            get => _selectedItemForAdding;
            set
            {
                _selectedItemForAdding = value;
                OnPropertyChanged(nameof(SelectedTypeForAdding));
                AddCommand.RaiseCanExecuteChanged();
            }
        }

        public string SelectedTypeForFiltering
        {
            get => _selectedItemForFiltering;
            set
            {
                _selectedItemForFiltering = value;
                OnPropertyChanged(nameof(SelectedTypeForFiltering));
                if(ItemIdTextBox != String.Empty)
                {
                    FilterCommand.RaiseCanExecuteChanged();
                }
            }
        }

        #endregion

        public NetworkDisplayViewModel()
        {
            EntityTypeList = new ObservableCollection<string>() { nameof(EntityType.WindGenerator), nameof(EntityType.SolarPanel) };
            TableEntityList = new ObservableCollection<T4_Entity>
            {
                //new T4_Entity (EntityId++, EntityType.WindGenerator),
                //new T4_Entity (EntityId++, EntityType.SolarPanel),
                //new T4_Entity (EntityId++, EntityType.SolarPanel),
                //new T4_Entity (EntityId++, EntityType.SolarPanel),
                //new T4_Entity (EntityId++, EntityType.SolarPanel),
                //new T4_Entity (EntityId++, EntityType.SolarPanel),
            };

            AddCommand = new MyICommand(OnAdd, OnAddCheck);
            DeleteCommand = new MyICommand(OnDelete, OnDeleteCheck);
            FilterCommand = new MyICommand(OnFilter, OnFilterCheck);
            UndoCommand = new MyICommand(OnUndo);
            SelectionChangedCommand = new MyICommand<ListView>(OnSectionChanged);


            EqualsRadioButtonCommand = new MyICommand(OnEqualsSelect);
            LessRadioButtonCommand = new MyICommand(OnLessRadioButtonSelect);
            GreaterRadioButtonCommand = new MyICommand(OnGreaterRadioButtonSelected);

            CurrentViewModel = _networkEntitiesViewModel;
        }

        private void OnSectionChanged(ListView obj)
        {
            if (!dragging)
            {
                dragging = true;
                DragDrop.DoDragDrop(obj, SelectedEntityFromEntityList, DragDropEffects.Copy | DragDropEffects.Move);
            }
        }
        private bool OnDeleteCheck()
        {
            var test = IsSelectedDeleteButton = SelectedEntity != null;
            //return IsSelectedDeleteButton = SelectedEntity != null;
            return test;
        }

        private bool OnFilterCheck()
        {
            //the reason why ItemIdTextBox can be null instead of the empty string is during the initialization of the viewmodel
            //If I added _itemIdTextbox = String.Empty there would be no need for that check
            if (SelectedTypeForFiltering != null && ItemIdTextBox != String.Empty && ItemIdTextBox != null)
                return true;
            return false;
        }

        private bool OnAddCheck() {
            return IsSelectedAddButton = SelectedTypeForAdding != null;
        }
        private void OnAdd()
        {
            MessageBox.Show($"{SelectedTypeForAdding} je selektovan");
            var tempEntity = new T4_Entity(EntityId++, (EntityType)Enum.Parse(typeof(EntityType), SelectedTypeForAdding));
            EntityList.Add(tempEntity);
            TableEntityList.Add(tempEntity);
            AddedEntities.Push(tempEntity);
            StackOfOperations.Push(true);

            SavedList.Clear();
            foreach (var entity in EntityList)
            {
                SavedList.Add(entity);
            }
        }

        private void OnDelete()
        {
            //MessageBox.Show("Hey I am here");
            var itemToRemove = EntityList.Where(item => item.Id == SelectedEntity.Id).FirstOrDefault();
            EntityList.Remove(itemToRemove);
            SavedList.Remove(itemToRemove);
            TableEntityList.Remove(itemToRemove);
            DeletedEntities.Push(itemToRemove);
            StackOfOperations.Push(false);
        }

        private void OnFilter()
        {
            //MessageBox.Show($"{SelectedTypeForFiltering} {ItemIdTextBox} {(FilterRadioEnum)RadioSelectedValue} je selektovan");
            var parsedTypeForFiltering = (EntityType)Enum.Parse(typeof(EntityType), SelectedTypeForFiltering);
            var radioSelectedValue = (FilterRadioEnum)RadioSelectedValue;
            Filter newFilter = new Filter(parsedTypeForFiltering, Convert.ToInt32(ItemIdTextBox), radioSelectedValue);
            MessageBox.Show($"Created a class FIlter with {newFilter.Id}, {newFilter.SelectedType}, {newFilter.RadioEnum}");
            ObservableCollection<T4_Entity> tempList = new ObservableCollection<T4_Entity>();
            foreach (var temp in TableEntityList)
            {
                tempList.Add(temp);
            }

            switch (newFilter.RadioEnum)
            {
                case FilterRadioEnum.EQUALS:
                    TableEntityList.Clear();
                    foreach(var item in SavedList)
                    {
                        if(item.Id == newFilter.Id && item.EntityType == newFilter.SelectedType)
                        {
                            TableEntityList.Add(item);
                        }
                    }
                    //tempList.Clear();
                    break;
                case FilterRadioEnum.GREATER:
                    TableEntityList.Clear();
                    foreach (var item in SavedList)
                    {
                        if (item.Id > newFilter.Id && item.EntityType == newFilter.SelectedType)
                        {
                            TableEntityList.Add(item);
                        }
                    }
                    break;
                case FilterRadioEnum.LESS:
                    TableEntityList.Clear();
                    foreach (var item in SavedList)
                    {
                        if (item.Id < newFilter.Id && item.EntityType == newFilter.SelectedType)
                        {
                            TableEntityList.Add(item);
                        }
                    }
                    break;
                default:
                    TableEntityList.Clear();
                    foreach(var item in SavedList)
                    {
                        TableEntityList.Add(item);
                    }
                    tempList.Clear();
                    break;
            }
        }

        private void OnEqualsSelect()
        {
            RadioSelectedValue = 1;
            FilterCommand.RaiseCanExecuteChanged();
        }

        private void OnLessRadioButtonSelect()
        {
            RadioSelectedValue = 2;
            FilterCommand.RaiseCanExecuteChanged();
        }

        private void OnGreaterRadioButtonSelected()
        {
            RadioSelectedValue = 3;
            FilterCommand.RaiseCanExecuteChanged();
        }

        public void OnUndo()
        {
            if(StackOfOperations.Count > 0)
            {
                if ((bool)StackOfOperations.Peek())
                {
                    StackOfOperations.Pop();
                    T4_Entity removedAddedEntity = (T4_Entity)AddedEntities.Pop();
                    EntityList.Remove(removedAddedEntity);
                    TableEntityList.Remove(removedAddedEntity);
                } else
                {
                    StackOfOperations.Pop();
                    T4_Entity removedDeletedEntity = (T4_Entity)DeletedEntities.Pop();
                    EntityList.Add(removedDeletedEntity);
                    TableEntityList.Add(removedDeletedEntity);
                }
            }
        }

    }
}
