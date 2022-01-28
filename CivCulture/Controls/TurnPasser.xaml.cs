using CivCulture_ViewModel.Utilities;
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

namespace CivCulture.Controls
{
    /// <summary>
    /// Interaction logic for TurnPasser.xaml
    /// </summary>
    public partial class TurnPasser : UserControl
    {
        #region Fields
        #region Dependency Properties
        public static readonly DependencyProperty EndTurnCommandProperty = DependencyProperty.Register(
            "EndTurnCommand",
            typeof(RelayCommand),
            typeof(TurnPasser),
            new PropertyMetadata()
            );
        #endregion
        #endregion

        #region Events
        #endregion

        #region Properties
        public RelayCommand EndTurnCommand
        {
            get => (RelayCommand)GetValue(EndTurnCommandProperty);
            set => SetValue(EndTurnCommandProperty, value);
        }
        #endregion

        #region Constructors
        public TurnPasser()
        {
            InitializeComponent();
        }
        #endregion

        #region Methods
        #endregion
    }
}
