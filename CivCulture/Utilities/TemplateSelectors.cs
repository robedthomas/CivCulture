using CivCulture_ViewModel.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CivCulture.Utilities
{
    public class GridMapDataTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;
            if (element != null && item != null)
            {
                if (item.GetType() == typeof(MapSpaceViewModel))
                {
                    return element.FindResource("mapSpaceViewModelTemplate") as DataTemplate;
                }
                else if (item.GetType() == typeof(PopViewModel))
                {
                    return element.FindResource("popViewModelTemplate") as DataTemplate;
                }
            }
            return null;
        }
    }
}
