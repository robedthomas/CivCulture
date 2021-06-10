using CivCulture_Model.ViewModels;
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

namespace CivCulture
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Fields
        #endregion

        #region Events
        #endregion

        #region Properties
        public MainViewModel VM { get; private set; }
        #endregion

        #region Constructors
        public MainWindow()
        {
            VM = new MainViewModel();
            DataContext = VM;
            InitializeComponent();
        }
        #endregion

        #region Methods
        #endregion
    }
}
