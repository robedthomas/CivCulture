using CivCulture.Views;
using CivCulture_Model.ViewModels;
using CivCulture_ViewModel.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CivCulture.Controls
{
    /// <summary>
    /// Interaction logic for GridMap.xaml
    /// </summary>
    public partial class GridMap : UserControl
    {
        #region Fields
        #region Dependency Properties
        public static readonly DependencyProperty SpacesProperty = DependencyProperty.Register(
            "Spaces",
            typeof(ObservableCollection<MapSpaceViewModel>),
            typeof(GridMap),
            new PropertyMetadata(new ObservableCollection<MapSpaceViewModel>())
            );

        public static readonly DependencyProperty SelectedSpaceProperty = DependencyProperty.Register(
            "SelectedSpace",
            typeof(MapSpaceViewModel),
            typeof(GridMap),
            new PropertyMetadata(null)
            );

        public static readonly DependencyProperty RowsProperty = DependencyProperty.Register(
            "Rows",
            typeof(int),
            typeof(GridMap),
            new PropertyMetadata(1)
            );

        public static readonly DependencyProperty ColumnsProperty = DependencyProperty.Register(
            "Columns",
            typeof(int),
            typeof(GridMap),
            new PropertyMetadata(1)
            );
        #endregion

        private bool isMouseClickStarted = false;
        private object mouseClickLock = new object();
        private MapSpaceView lastSelectedSpaceView = null;
        #endregion

        #region Events
        #endregion

        #region Properties
        public ObservableCollection<MapSpaceViewModel> Spaces
        {
            get { return (ObservableCollection<MapSpaceViewModel>)GetValue(SpacesProperty); }
            set 
            {
                SetValue(SpacesProperty, value);
            }
        }

        public MapSpaceViewModel SelectedSpace
        {
            get => (MapSpaceViewModel)GetValue(SelectedSpaceProperty);
            set
            {
                SetValue(SelectedSpaceProperty, value);
            }
        }

        public int Rows
        {
            get { return (int)GetValue(RowsProperty); }
            set 
            { 
                SetValue(RowsProperty, value);
            }
        }

        public int Columns
        {
            get { return (int)GetValue(ColumnsProperty); }
            set 
            { 
                SetValue(ColumnsProperty, value);
            }
        }
        #endregion

        #region Constructors
        public GridMap()
        {
            InitializeComponent();
        }
        #endregion

        #region Methods
        #endregion

        private void MapSpaceView_MouseUp(object sender, MouseButtonEventArgs e)
        {
            lock(mouseClickLock)
            {
                if (isMouseClickStarted)
                {
                    isMouseClickStarted = false;
                    if (sender is MapSpaceView)
                    {
                        if (lastSelectedSpaceView != null)
                        {
                            lastSelectedSpaceView.Opacity = 1;
                            if (lastSelectedSpaceView == sender as MapSpaceView)
                            {
                                lastSelectedSpaceView = null;
                                SelectedSpace = null;
                                return;
                            }
                        }
                        lastSelectedSpaceView = sender as MapSpaceView;
                        SelectedSpace = lastSelectedSpaceView.DataContext as MapSpaceViewModel;
                        lastSelectedSpaceView.Opacity = 0.5;
                    }
                }
            }
        }

        private void MapSpaceView_MouseDown(object sender, MouseButtonEventArgs e)
        {
            lock (mouseClickLock)
            {
                if (!isMouseClickStarted)
                {
                    isMouseClickStarted = true;
                }
            }
        }
    }
}
