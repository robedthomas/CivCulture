using CivCulture_Model.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivCulture.Utilities.Converters
{
    public class TechResearchStateToPerimeterStrokeConverter : ValueConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ResearchState state)
            {
                switch (state)
                {
                    case ResearchState.UnavailableForResearch:
                        return System.Windows.Media.Brushes.Gray;
                    case ResearchState.AvailableForResearch:
                        return System.Windows.Media.Brushes.SteelBlue;
                    case ResearchState.QueuedForResearch:
                        return System.Windows.Media.Brushes.LightSteelBlue;
                    case ResearchState.BeingResearched:
                        return System.Windows.Media.Brushes.PaleGoldenrod;
                    case ResearchState.Researched:
                        return System.Windows.Media.Brushes.PaleGreen;
                }
            }
            return null;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class TechResearchStateToFillConverter : ValueConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ResearchState state)
            {
                switch (state)
                {
                    case ResearchState.UnavailableForResearch:
                        return System.Windows.Media.Brushes.DarkGray;
                    case ResearchState.AvailableForResearch:
                        return System.Windows.Media.Brushes.DarkSlateBlue;
                    case ResearchState.QueuedForResearch:
                        return System.Windows.Media.Brushes.SteelBlue;
                    case ResearchState.BeingResearched:
                        return System.Windows.Media.Brushes.Goldenrod;
                    case ResearchState.Researched:
                        return System.Windows.Media.Brushes.DarkOliveGreen;
                }
            }
            return null;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class TechCategoryToFillConverter : ValueConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TechnologyCategory category)
            {
                switch (category)
                {
                    case TechnologyCategory.Agricultural:
                        return System.Windows.Media.Brushes.Green;
                    case TechnologyCategory.Cultural:
                        return System.Windows.Media.Brushes.Magenta;
                    case TechnologyCategory.Infrastructure:
                        return System.Windows.Media.Brushes.SaddleBrown;
                    case TechnologyCategory.Mercantile:
                        return System.Windows.Media.Brushes.Yellow;
                    case TechnologyCategory.Military:
                        return System.Windows.Media.Brushes.Red;
                    case TechnologyCategory.Scientific:
                        return System.Windows.Media.Brushes.Blue;
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
