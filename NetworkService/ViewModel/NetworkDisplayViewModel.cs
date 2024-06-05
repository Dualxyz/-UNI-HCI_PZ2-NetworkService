using NetworkService.Common;
using NetworkService.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NetworkService.ViewModel
{
    public class NetworkDisplayViewModel : BindableBase
    {
        private static NetworkDisplayViewModel _instance;
        public static NetworkDisplayViewModel Instance { get { return _instance; } }

        private ObservableCollection<string> _entityTypeList;
        private ObservableCollection<T4_Entity> _tableEntityList = new ObservableCollection<T4_Entity>();
        private ObservableCollection<T4_Entity> _entityList = new ObservableCollection<T4_Entity>();
        private ObservableCollection<T4_Entity> _displayEntityList = new ObservableCollection<T4_Entity>();

        private bool _IsSelectedDeleteButton;
        private bool _IsSelectedAddButton;
        private T4_Entity _selectedEntityFromEntityList;
        public ObservableCollection<T4_Entity> SavedList = new ObservableCollection<T4_Entity>();

        private BindableBase currentViewModel;
        private readonly NetworkEntitiesViewModel _networkEntitiesViewModel = new NetworkEntitiesViewModel();
        private readonly MeasurementGraphViewModel _measurementGraphViewModel = new MeasurementGraphViewModel();

        private ProfileSaver _profileSaverSelection;
        public ProfileSaver ProfileSaverSelection
        {
            get { return _profileSaverSelection; }
            set
            {
                _profileSaverSelection = value;
                Database.Database.ProfileSaver = _profileSaverSelection;
                OnPropertyChanged(nameof(ProfileSaverSelection));
                NetworkEntitiesViewModel.Instance.ItemIdTextBox = _profileSaverSelection.ItemId.ToString();
                NetworkEntitiesViewModel.Instance.SelectedTypeForFiltering = _profileSaverSelection.EntityType.ToString();
                switch (_profileSaverSelection.FilterRadio)
                {
                    case FilterRadioEnum.EQUALS:
                        NetworkEntitiesViewModel.Instance.IsOption1Checked = true;
                        NetworkEntitiesViewModel.Instance.IsOption2Checked = false;
                        NetworkEntitiesViewModel.Instance.IsOption3Checked = false;
                        NetworkEntitiesViewModel.Instance.RadioSelectedValue = 1;
                        break;
                    case FilterRadioEnum.LESS:
                        NetworkEntitiesViewModel.Instance.IsOption1Checked = false;
                        NetworkEntitiesViewModel.Instance.IsOption2Checked = true;
                        NetworkEntitiesViewModel.Instance.IsOption3Checked = false;
                        NetworkEntitiesViewModel.Instance.RadioSelectedValue = 2;
                        break;
                    case FilterRadioEnum.GREATER:
                        NetworkEntitiesViewModel.Instance.IsOption1Checked = false;
                        NetworkEntitiesViewModel.Instance.IsOption2Checked = false;
                        NetworkEntitiesViewModel.Instance.IsOption3Checked = true;
                        NetworkEntitiesViewModel.Instance.RadioSelectedValue = 3;
                        break;
                }
                //NetworkEntitiesViewModel.Instance.IsOption1Checked = _profileSaverSelection.EntityType.ToString();
            }
        }

        //Dragdrop
        public ObservableCollection<Canvas> CanvasCollection { get; set; }
        //public ObservableCollection<MyLine> LineCollection { get; set; } //Not implemented
        private T4_Entity selectedEntity;
        public T4_Entity SelectedEntity
        {
            get { return selectedEntity; }
            set
            {
                selectedEntity = value;
                OnPropertyChanged("SelectedEntity");
            }
        }
        private T4_Entity draggedItem = null;
        private bool dragging = false;
        public int draggingSourceIndex = -1;
        public MyICommand<object> DropEntityOnCanvas { get; set; }
        public MyICommand<object> LeftMouseButtonDownOnCanvas { get; set; }
        public MyICommand MouseLeftButtonUp { get; set; }
        public MyICommand<object> SelectionChanged { get; set; }
        public MyICommand<object> CanvasRelease { get; set; }
        public MyICommand<object> RightMouseButtonDownOnCanvas { get; set; }

        private bool isLineSourceSelected = false;
        private int sourceCanvasIndex = -1;
        private int destinationCanvasIndex = -1;
        private MyLine currentLine = new MyLine();
        private Point linePoint1 = new Point();
        private Point linePoint2 = new Point();

        public BindableBase CurrentViewModel
        {
            get { return currentViewModel; }
            set
            {
                SetProperty(ref currentViewModel, value);
            }
        }

        Stack StackOfOperations { get; set; } = new Stack();
        Stack AddedEntities { get; set; } = new Stack();
        Stack DeletedEntities { get; set; } = new Stack();

        public MyICommand<string> NavCommand { get; set; }

        public MyICommand<ListView> SelectionChangedCommand { get; set; } // Komanda koja reaguje na izmenu
        public MyICommand MouseLeftButtonUpCommand { get; set; } // Komanda koja reaguje na levi klik
        public MyICommand AutoFillButton { get; set; }
        public MyICommand UndoDPButton { get; set; }


        #region Getters and Setters

        public T4_Entity SelectedEntityFromEntityList
        {
            get => _selectedEntityFromEntityList;
            set
            {
                _selectedEntityFromEntityList = value;
                OnPropertyChanged(nameof(SelectedEntityFromEntityList));
                //MessageBox.Show(_selectedEntityFromEntityList.ToString());
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

        #endregion

        private void OnAutoFill()
        {
            //MessageBox.Show(Database.Database.DraggableEntities.Count.ToString());
            int canvasSlot = 0;

            List<T4_Entity> tempList = new List<T4_Entity>();

            foreach (var entity in Database.Database.DraggableEntities)
            {
                if(Database.Database.PannelEntities.Count < 12)
                {
                    while (CanvasCollection[canvasSlot].Resources["taken"] != null)
                    {
                        canvasSlot++;
                    }

                    BitmapImage logo = new BitmapImage();
                    logo.BeginInit();
                    if (entity.EntityType == EntityType.SolarPanel)
                    {
                        logo.UriSource = new Uri("pack://application:,,,/NetworkService;component/Images/EntityImages/SolarPanel.png", UriKind.RelativeOrAbsolute);
                    }
                    else if (entity.EntityType == EntityType.WindGenerator)
                    {
                        logo.UriSource = new Uri("pack://application:,,,/NetworkService;component/Images/EntityImages/Windgenerator.jpg", UriKind.RelativeOrAbsolute);
                    }
                    else
                    {
                        logo.UriSource = new Uri("", UriKind.RelativeOrAbsolute);
                    }
                    logo.EndInit();

                    CanvasCollection[canvasSlot].Background = new ImageBrush(logo);
                    CanvasCollection[canvasSlot].Resources.Add("taken", true);
                    CanvasCollection[canvasSlot].Resources.Add("data", entity);

                    Database.Database.PannelEntities.Add(entity);
                    StackOfOperations.Push(99);
                    AddedEntities.Push(canvasSlot);
                    tempList.Add(entity);
                    //dragging = false;
                }

                canvasSlot++;
            }

            foreach(var e in tempList)
            {
                Database.Database.DraggableEntities.Remove(e);
            }
        }

        private bool CheckOnAutoFill()
        {
            return true;
        }

        public void OnUndo()
        {
            if (StackOfOperations.Count > 0)
            {
                if ((int)StackOfOperations.Peek() == 99)
                {
                    StackOfOperations.Pop();
                    //(T4_Entity)AddedEntities.Pop();
                    var index = (int)AddedEntities.Pop();
                    //T4_Entity removedAddedEntity = new T4_Entity();

                    var smt = (T4_Entity)CanvasCollection[index].Resources["data"];

                    CanvasCollection[index].Background = Brushes.Black;
                    CanvasCollection[index].Resources.Remove("taken");
                    CanvasCollection[index].Resources.Remove("data");

                    //Database.Database.Entities.Remove(removedAddedEntity);
                    Database.Database.DraggableEntities.Add(smt);
                    Database.Database.PannelEntities.Remove(smt);
                }
                else
                {
                    int index = (int)StackOfOperations.Pop();
                    T4_Entity removedDeletedEntity = (T4_Entity)DeletedEntities.Pop();

                    var smt = (T4_Entity)CanvasCollection[removedDeletedEntity.Id].Resources["data"];
                    BitmapImage logo = new BitmapImage();
                    logo.BeginInit();

                    switch (removedDeletedEntity.EntityType)
                    {
                        case EntityType.SolarPanel:
                            logo.UriSource = new Uri("pack://application:,,,/NetworkService;component/Images/EntityImages/SolarPanel.png", UriKind.RelativeOrAbsolute);
                            logo.EndInit();
                            CanvasCollection[index].Background = new ImageBrush(logo);
                            break;
                        case EntityType.WindGenerator:
                            logo.UriSource = new Uri("pack://application:,,,/NetworkService;component/Images/EntityImages/Windgenerator.jpg", UriKind.RelativeOrAbsolute);
                            logo.EndInit();
                            CanvasCollection[index].Background = new ImageBrush(logo);
                            break;
                        default:
                            CanvasCollection[index].Background = Brushes.Red;
                            break;
                    }

                    CanvasCollection[index].Resources.Add("taken", true);
                    CanvasCollection[index].Resources.Add("data", removedDeletedEntity);

                    //Database.Database.Entities.Add(removedDeletedEntity);
                    Database.Database.DraggableEntities.Remove(removedDeletedEntity);
                    Database.Database.PannelEntities.Add(removedDeletedEntity);
                }
            }
        }
        public NetworkDisplayViewModel()
        {
            EntityTypeList = new ObservableCollection<string>() { nameof(EntityType.WindGenerator), nameof(EntityType.SolarPanel) };
            TableEntityList = new ObservableCollection<T4_Entity>();


            SelectionChangedCommand = new MyICommand<ListView>(OnSectionChanged);
            NavCommand = new MyICommand<string>(OnNav);
            AutoFillButton = new MyICommand(OnAutoFill, CheckOnAutoFill);
            UndoDPButton = new MyICommand(OnUndo);

            _instance = this;

            CurrentViewModel = _networkEntitiesViewModel;

            //Drag&Drop
            CanvasCollection = new ObservableCollection<Canvas>();
            for (int i = 0; i < 12; i++)
            {
                CanvasCollection.Add(new Canvas()
                {
                    Background = Brushes.Black,
                    AllowDrop = true
                });
            }

            //BorderBrushCollection = new ObservableCollection<Brush>();
            //for (int i = 0; i < 12; i++)
            //{
            //    BorderBrushCollection.Add(Brushes.DarkGray);
            //}
            //LineCollection = new ObservableCollection<MyLine>();

            DropEntityOnCanvas = new MyICommand<object>(OnDrop);
            LeftMouseButtonDownOnCanvas = new MyICommand<object>(OnLeftMouseButtonDown);
            MouseLeftButtonUp = new MyICommand(OnMouseLeftButtonUp);
            SelectionChanged = new MyICommand<object>(OnSelectionChanged);
            CanvasRelease = new MyICommand<object>(OnRelease);
            //RightMouseButtonDownOnCanvas = new MyICommand<object>(OnRightMouseButtonDown);
        }

        private void OnRelease(object parameter)
        {
            int index = Convert.ToInt32(parameter);

            if (CanvasCollection[index].Resources["taken"] != null)
            {
                if (sourceCanvasIndex != -1)
                {
                    isLineSourceSelected = false;
                    sourceCanvasIndex = -1;
                    linePoint1 = new Point();
                    linePoint2 = new Point();
                    //currentLine = new MyLine();
                }

                //DeleteLinesForCanvas(index);

                Database.Database.PannelEntities.Remove((T4_Entity)CanvasCollection[index].Resources["data"]);
                Database.Database.DraggableEntities.Add((T4_Entity)CanvasCollection[index].Resources["data"]);
                StackOfOperations.Push(index);
                DeletedEntities.Push((T4_Entity)CanvasCollection[index].Resources["data"]);

                CanvasCollection[index].Background = Brushes.Black;
                CanvasCollection[index].Resources.Remove("taken");
                CanvasCollection[index].Resources.Remove("data");
                //BorderBrushCollection[index] = Brushes.DarkGray;
            }
        }

        private void OnLeftMouseButtonDown(object parameter)
        {
            if (!dragging)
            {
                int index = Convert.ToInt32(parameter);

                if (CanvasCollection[index].Resources["taken"] != null)
                {
                    dragging = true;
                    draggedItem = (T4_Entity)(CanvasCollection[index].Resources["data"]);
                    draggingSourceIndex = index;
                    DragDrop.DoDragDrop(CanvasCollection[index], draggedItem, DragDropEffects.Move);
                }
            }
        }

        private void OnMouseLeftButtonUp()
        {
            draggedItem = null;
            SelectedEntity = null;
            dragging = false;
            draggingSourceIndex = -1;
        }
        private void OnSelectionChanged(object parameter)
        {
            if (!dragging)
            {
                dragging = true;
                draggedItem = SelectedEntity;
                DragDrop.DoDragDrop((ListView)parameter, draggedItem, DragDropEffects.Move);
            }
        }
        private void OnDrop(object parameter)
        {
            if (draggedItem != null)
            {
                int index = Convert.ToInt32(parameter);

                if (CanvasCollection[index].Resources["taken"] == null)
                {
                    BitmapImage logo = new BitmapImage();
                    logo.BeginInit();
                    if(draggedItem.EntityType == EntityType.SolarPanel)
                    {
                        //"pack://application:,,,/Images/Icons/view.png"
                        logo.UriSource = new Uri("pack://application:,,,/NetworkService;component/Images/EntityImages/SolarPanel.png", UriKind.RelativeOrAbsolute);
                        //logo.UriSource = new Uri("", UriKind.RelativeOrAbsolute);
                    }
                    else if (draggedItem.EntityType == EntityType.WindGenerator)
                    {
                        logo.UriSource = new Uri("pack://application:,,,/NetworkService;component/Images/EntityImages/Windgenerator.jpg", UriKind.RelativeOrAbsolute);
                        //logo.UriSource = new Uri("", UriKind.RelativeOrAbsolute);
                    }
                    else
                    {
                        logo.UriSource = new Uri("", UriKind.RelativeOrAbsolute);
                    }

                    logo.EndInit();

                    CanvasCollection[index].Background = new ImageBrush(logo);
                    CanvasCollection[index].Resources.Add("taken", true);
                    CanvasCollection[index].Resources.Add("data", draggedItem);
                    //BorderBrushCollection[index] = (draggedItem.IsValueValidForType()) ? Brushes.Green : Brushes.Red;

                    // PREVLACENJE IZ DRUGOG CANVASA
                    if (draggingSourceIndex != -1)
                    {
                        CanvasCollection[draggingSourceIndex].Background = Brushes.Black;
                        CanvasCollection[draggingSourceIndex].Resources.Remove("taken");
                        CanvasCollection[draggingSourceIndex].Resources.Remove("data");
                        //BorderBrushCollection[draggingSourceIndex] = Brushes.DarkGray;

                        //UpdateLinesForCanvas(draggingSourceIndex, index);

                        if (sourceCanvasIndex != -1)
                        {
                            isLineSourceSelected = false;
                            sourceCanvasIndex = -1;
                            linePoint1 = new Point();
                            linePoint2 = new Point();
                            //currentLine = new MyLine();
                        }

                        draggingSourceIndex = -1;
                    }

                    Database.Database.PannelEntities.Add(draggedItem);
                    AddedEntities.Push(index);
                    StackOfOperations.Push(99);
                    // PREVLACENJE IZ LISTE
                    if (Database.Database.DraggableEntities.Contains(draggedItem))
                    {
                        Database.Database.DraggableEntities.Remove(draggedItem);
                    }
                }
            }
        }

        private void OnNav(string view)
        {
            switch (view)
            {
                case "NetworkEntity":
                    CurrentViewModel = _networkEntitiesViewModel;
                    break;
                case "MeasurementGraph":
                    CurrentViewModel = _measurementGraphViewModel;
                    break;
                default:
                    MessageBox.Show($"{view}");
                    break;
            }
        }
        private void OnSectionChanged(ListView obj)
        {
            if (!dragging)
            {
                dragging = true;
                DragDrop.DoDragDrop(obj, SelectedEntityFromEntityList, DragDropEffects.Copy | DragDropEffects.Move);
            }
        }
    }
}
