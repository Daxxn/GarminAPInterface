using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace TBMAutopilotDashboard.Models.Utils
{
   public class BoolColorConverter : IValueConverter
   {
      private static readonly SolidColorBrush TrueColor  = new SolidColorBrush(Color.FromArgb(255, 0, 255, 0));
      private static readonly SolidColorBrush FalseColor = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
      public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
      {
         if (value is bool b)
         {
            return b ? TrueColor : FalseColor;
         }
         return FalseColor;
      }

      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
      {
         throw new NotImplementedException();
      }
   }
}
