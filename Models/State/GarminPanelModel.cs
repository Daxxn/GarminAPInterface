using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.FlightSimulator.SimConnect;

using MVVMLibrary;

using TBMAutopilotDashboard.Models.Enums;

namespace TBMAutopilotDashboard.Models.State
{
   public class GarminPanelModel : Model
   {
      #region Local Props
      private GarminButtons _buttons = new GarminButtons();
      private GarminIndicators _indicators = new GarminIndicators();
      #endregion

      #region Constructors
      public GarminPanelModel() { }
      #endregion

      #region Methods
      public void SendButtonStates(SimConnect simConnect)
      {
         if (Buttons.XFR)
         {
            Indicators.UpdateXFRIndicators();
            Buttons.XFR = false;
         }
         Buttons.SendInputsToSim(simConnect);
      }

      public void RegisterSimData(SimConnect simConnect)
      {
         if (simConnect is null) return;
         Buttons.InitSimInputs(simConnect);
         Indicators.RegisterSimData(simConnect);
      }
      #endregion

      #region Full Props
      public GarminButtons Buttons
      {
         get => _buttons;
         set
         {
            _buttons = value;
            OnPropertyChanged();
         }
      }

      public GarminIndicators Indicators
      {
         get => _indicators;
         set
         {
            _indicators = value;
            OnPropertyChanged();
         }
      }
      #endregion
   }
}
