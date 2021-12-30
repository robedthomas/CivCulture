using CivCulture_Model.Models;
using CivCulture_Model.Models.Modifiers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;

namespace CivCulture.Utilities.Converters
{
    public abstract class ValueConverter : MarkupExtension, IValueConverter
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        public abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);

        public abstract object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture);
    }

    public class ObjectToVisibilityConverter : ValueConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter != null && ((string)parameter).ToLower() == "true")
            {
                return value == null ? Visibility.Collapsed : Visibility.Visible;
            }
            else
            {
                return value == null ? Visibility.Hidden : Visibility.Visible;
            }
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class PopTemplateToTemplateNameConverter : ValueConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
            {
                return "NULL";
            }
            return (value as PopTemplate).Name;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class DecimalToDoubleConverter : ValueConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Convert.ToDouble((decimal)value);
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Convert.ToDecimal((double)value);
        }
    }

    public class MapSpaceToTerrainBrushConverter : ValueConverter
    {
        private static Uri mapResourcesUri = new Uri("Resources/MapResources.xaml", UriKind.RelativeOrAbsolute);
        private static string invalidTerrainBrushName = "InvalidTerrainBrush";
        private static string terrainBrushNameSuffix = "TerrainBrush";

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is MapSpace space)
            {
                ResourceDictionary mapResources = new ResourceDictionary() { Source = mapResourcesUri };
                if (space.Terrain == null || !mapResources.Contains(space.Terrain.Name + terrainBrushNameSuffix))
                {
                    return mapResources[invalidTerrainBrushName] as System.Windows.Media.Brush;
                }
                return mapResources[space.Terrain.Name + terrainBrushNameSuffix] as System.Windows.Media.Brush;
            }
            throw new ArgumentException();
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ModifiableToStringConverter : ValueConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            decimal modification;
            if (value is DecimalModifiable decMod)
            {
                modification = decMod.Value;
            }
            else if (value is decimal mod)
            {
                modification = mod;
            }
            else
            {
                throw new ArgumentException();
            }
            if (modification >= 0)
            {
                return $"(+{modification})";
            }
            return $"({modification})";
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ModifiableToTooltipConverter : ValueConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DecimalModifiable decMod)
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendJoin("\n", decMod.Modifiers);
                return builder.ToString();
            }
            else
            {
                throw new ArgumentException();
            }
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ResourceToIconConverter : ValueConverter
    {
        private static Uri iconResourcesUri = new Uri("Resources/Icons/IconsDictionary.xaml", UriKind.RelativeOrAbsolute);
        private static ResourceDictionary iconsDictionary = new ResourceDictionary() { Source = iconResourcesUri };

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Fundamental f)
            {
                if (f == Fundamental.Food)
                {
                    return (iconsDictionary["FoodIcon"] as Image).Source;
                }
                else if (f == Fundamental.Money)
                {
                    return (iconsDictionary["MoneyIcon"] as Image).Source;
                }
            }
            else if (value is Resource r)
            {
                if (r == Resource.Wheat)
                {
                    return (iconsDictionary["WheatIcon"] as Image).Source;
                }
            }
            return null;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
