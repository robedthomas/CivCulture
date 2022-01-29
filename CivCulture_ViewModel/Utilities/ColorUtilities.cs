using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace CivCulture_ViewModel.Utilities
{
    public static class ColorUtilities
    {
        public static Color GetComplimentaryColor(Color initialColor)
        {
            return Color.FromArgb(initialColor.A, (byte)(byte.MaxValue - initialColor.R), (byte)(byte.MaxValue - initialColor.G), (byte)(byte.MaxValue - initialColor.B));
        }
    }
}
