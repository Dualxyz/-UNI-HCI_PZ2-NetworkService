using NetworkService.Common;
using NetworkService.Database;
using NetworkService.Model;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace NetworkService.ViewModel
{
    public class NetworkEntitiesViewModel : BindableBase
    {
        public static int EntityId1 = 0;
        private bool dragging = false;
        #region List of Entity Types (Used to fill the comboboxes)
        private ObservableCollection<string> _entityTypeList;
        public ObservableCollection<string> EntityTypeList
        {
            get { return _entityTypeList; }
            set
            {
                _entityTypeList = value;
                OnPropertyChanged(nameof(EntityTypeList));
            }
        }
        #endregion

        #region List of entities in the table
        private ObservableCollection<T4_Entity> _tableEntityList = new ObservableCollection<T4_Entity>(); 
        public ObservableCollection<T4_Entity> TableEntityList
        {
            get { return _tableEntityList; }
            set
            {
                _tableEntityList = value;
                OnPropertyChanged(nameof(TableEntityList));
            }
        }
        #endregion

        #region List of All the Entities (Most likely will be moved to the static db...)
        private ObservableCollection<T4_Entity> _entityList = new ObservableCollection<T4_Entity>();
        public ObservableCollection<T4_Entity> EntityList
        {
            get { return _entityList; }
            set
            {
                _entityList = value;
                OnPropertyChanged(nameof(EntityList));
            }
        }
        #endregion

        #region Selected Entity in the table
        private T4_Entity _selectedEntity;
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
        #endregion

        #region Checks if Delete button is clickable, used for settings the color of the delete button
        private bool _IsSelectedDeleteButton;
        public bool IsSelectedDeleteButton
        {
            get { return _IsSelectedDeleteButton; }
            set
            {
                _IsSelectedDeleteButton = value;
                OnPropertyChanged(nameof(IsSelectedDeleteButton));
            }
        }
        #endregion

        #region Contains the selected entity in the Entity List (this is wrong and doesn't need to be handled here)
        private T4_Entity _selectedEntityFromEntityList;
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
        #endregion

        #region Checks if Add button is clickable, used for settings the color of the add button
        private bool _IsSelectedAddButton;
        public bool IsSelectedAddButton
        {
            get { return _IsSelectedAddButton; }
            set
            {
                _IsSelectedAddButton = value;
                OnPropertyChanged(nameof(IsSelectedAddButton));
            }
        }
        #endregion

        #region Property that checks if an item in the combobox is selected (only for adding, the filtering one will have a different check!)
        private string _selectedItemForAdding;
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
        #endregion

        #region Property that checks if an item in the combobox is selected (only for filtering, the filtering one will have a different check!)
        private string _selectedItemForFiltering;
        public string SelectedTypeForFiltering
        {
            get => _selectedItemForFiltering;
            set
            {
                _selectedItemForFiltering = value;
                OnPropertyChanged(nameof(SelectedTypeForFiltering));
                if (ItemIdTextBox != String.Empty)
                {
                    FilterCommand.RaiseCanExecuteChanged();
                }
            }
        }
        #endregion

        #region Property that is bound on the Id TextBox. 
        private string _itemIdTextBox;
        public string ItemIdTextBox
        {
            get { return _itemIdTextBox; }
            set
            {
                _itemIdTextBox = value;
                OnPropertyChanged(nameof(ItemIdTextBox));
                if (SelectedTypeForFiltering != null)
                {
                    FilterCommand.RaiseCanExecuteChanged();
                }
            }
        }
        #endregion

        #region Radio Button selected Value
        private int _radioSelectedValue = -1;
        public int RadioSelectedValue { get => _radioSelectedValue; set => _radioSelectedValue = value; }
        #endregion

        public ObservableCollection<T4_Entity> SavedList = new ObservableCollection<T4_Entity>();
        public MyICommand AddCommand { get; set; }
        public MyICommand DeleteCommand { get; set; }
        public MyICommand FilterCommand { get; set; }
        public MyICommand UndoCommand { get; set; }
        public MyICommand EqualsRadioButtonCommand { get; set; }
        public MyICommand LessRadioButtonCommand { get; set; }
        public MyICommand GreaterRadioButtonCommand { get; set; }
        public MyICommand<ListView> SelectionChangedCommand { get; set; } // Command that responds to a change in the Entity List (this is wrong and doesn't need to be handled here)
        public MyICommand MouseLeftButtonUpCommand { get; set; } // Command that responds to a left click in the Entity List (this is wrong and doesn't need to be handled here)

        Stack StackOfOperations { get; set; } = new Stack();
        Stack AddedEntities { get; set; } = new Stack();
        Stack DeletedEntities { get; set; } = new Stack();

        public NetworkEntitiesViewModel() 
        {
            EntityTypeList = new ObservableCollection<string>() { nameof(EntityType.WindGenerator), nameof(EntityType.SolarPanel) };
            AddCommand = new MyICommand(OnAdd, OnAddCheck);
            DeleteCommand = new MyICommand(OnDelete, OnDeleteCheck);
            FilterCommand = new MyICommand(OnFilter, OnFilterCheck);
            UndoCommand = new MyICommand(OnUndo);
            //SelectionChangedCommand = new MyICommand<ListView>(OnSectionChanged);


            EqualsRadioButtonCommand = new MyICommand(OnEqualsSelect);
            LessRadioButtonCommand = new MyICommand(OnLessRadioButtonSelect);
            GreaterRadioButtonCommand = new MyICommand(OnGreaterRadioButtonSelected);
        }

        //private void OnSectionChanged(ListView obj)
        //{
        //    if (!dragging)
        //    {
        //        dragging = true;
        //        DragDrop.DoDragDrop(obj, SelectedEntityFromEntityList, DragDropEffects.Copy | DragDropEffects.Move);
        //    }
        //}
        private bool OnDeleteCheck()
        {
            var test = IsSelectedDeleteButton = SelectedEntity != null;
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

        private bool OnAddCheck()
        {
            return IsSelectedAddButton = SelectedTypeForAdding != null;
        }
        private void OnAdd()
        {
            //MessageBox.Show($"{SelectedTypeForAdding} je selektovan");
            var tempEntity = new T4_Entity(EntityId1++, (EntityType)Enum.Parse(typeof(EntityType), SelectedTypeForAdding));
            EntityList.Add(tempEntity);
            TableEntityList.Add(tempEntity);
            AddedEntities.Push(tempEntity);
            StackOfOperations.Push(true);
            Database.Database.Entities.Add(tempEntity);

            Database.Database.DraggableEntities.Add(tempEntity);

            //StringBuilder sb = new StringBuilder();
            //foreach (var smt in Database.Database.Entities)
            //{
            //    sb.Append(smt.Name.ToString() + "\n");
            //}
            //MessageBox.Show(sb.ToString());

            SavedList.Clear();
            foreach (var entity in EntityList)
            {
                SavedList.Add(entity);
            }
        }

        private void OnDelete()
        {
            var itemToRemove = EntityList.Where(item => item.Id == SelectedEntity.Id).FirstOrDefault();
            EntityList.Remove(itemToRemove);
            SavedList.Remove(itemToRemove);
            TableEntityList.Remove(itemToRemove);
            DeletedEntities.Push(itemToRemove);

            Database.Database.Entities.Remove(itemToRemove);
            Database.Database.DraggableEntities.Remove(itemToRemove);

            StackOfOperations.Push(false);
        }

        private void OnFilter()
        {
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
                    foreach (var item in SavedList)
                    {
                        if (item.Id == newFilter.Id && item.EntityType == newFilter.SelectedType)
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
                    foreach (var item in SavedList)
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
            if (StackOfOperations.Count > 0)
            {
                if ((bool)StackOfOperations.Peek())
                {
                    StackOfOperations.Pop();
                    T4_Entity removedAddedEntity = (T4_Entity)AddedEntities.Pop();
                    EntityList.Remove(removedAddedEntity);
                    TableEntityList.Remove(removedAddedEntity);

                    Database.Database.Entities.Remove(removedAddedEntity);
                    Database.Database.DraggableEntities.Remove(removedAddedEntity);
                }
                else
                {
                    StackOfOperations.Pop();
                    T4_Entity removedDeletedEntity = (T4_Entity)DeletedEntities.Pop();
                    EntityList.Add(removedDeletedEntity);
                    TableEntityList.Add(removedDeletedEntity);

                    Database.Database.Entities.Add(removedDeletedEntity);
                    Database.Database.DraggableEntities.Add(removedDeletedEntity);
                }
            }
        }

    }
}
