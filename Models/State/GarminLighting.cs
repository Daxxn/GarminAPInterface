using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MVVMLibrary;

using TBMAutopilotDashboard.Models.AttributeModels;

namespace TBMAutopilotDashboard.Models.State
{
   public class GarminLighting : Model
   {
      #region Local Props
      private uint _backlight = 0;
      private uint _indicatorBright = 0;
      #endregion

      #region Constructors
      public GarminLighting() { }
      #endregion

      #region Methods

      #endregion

      #region Full Props
      [SimEvent("LIGHT_POTENTIOMETER_SET")]
      public uint Backlight
      {
         get => _backlight;
         set
         {
            _backlight = value;
            OnPropertyChanged();
         }
      }

      public uint IndicatorBrightness
      {
         get => _indicatorBright;
         set
         {
            _indicatorBright = value;
            OnPropertyChanged();
         }
      }
      #endregion
   }
}
