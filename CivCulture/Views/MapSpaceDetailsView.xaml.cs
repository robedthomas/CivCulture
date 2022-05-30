using CivCulture_ViewModel.ViewModels;
using System;
using System.Collections.Generic;
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

namespace CivCulture.Views
{
    /// <summary>
    /// Interaction logic for MapSpaceDetailsView.xaml
    /// </summary>
    public partial class MapSpaceDetailsView : UserControl
    {
        public MapSpaceDetailsView()
        {
            InitializeComponent();
        }

        private void OpenDominantCulture_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            (DataContext as MapSpaceDetailsViewModel).OpenDominantCultureView();
        }
    }
}
