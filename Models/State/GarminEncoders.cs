using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MVVMLibrary;

using TBMAutopilotDashboard.Models.Enums.Constants;
using TBMAutopilotDashboard.Models.Enums;
using Microsoft.FlightSimulator.SimConnect;
using TBMAutopilotDashboard.Models.AttributeModels;

namespace TBMAutopilotDashboard.Models.State
{
   public class GarminEncoders : Model
   {
      #region Local Props
      public int this[PanelEncoder btn] { get => States[btn]; set => States[btn] = value; }
      public int this[string btn] { get => States[PanelEncodernames.ToEnum[btn]]; set => States[PanelEncodernames.ToEnum[btn]] = value; }
      private Dictionary<PanelEncoder, int> States { get; set; } = new Dictionary<PanelEncoder, int>();
      #endregion

      #region Constructors
      public GarminEncoders()
      {
         foreach (PanelEncoder pe in Enum.GetValues(typeof(PanelEncoder)))
         {
            States.Add(pe, 0);
         }
      }
      #endregion

      #region Methods
      public void RegisterSimData(SimConnect simConnect)
      {

      }
      #endregion

      #region Full Props
      [SimEvent("AP_HEADING_BUG_SET")]
      public int HDG
      {
         get => States[PanelEncoder.HDG];
         set
         {
            States[PanelEncoder.HDG] = value;
            OnPropertyChanged();
         }
      }

      [SimEvent("AP_ALT_VAR_SET_ENGLISH")]
      public int ALT
      {
         get => States[PanelEncoder.ALT];
         set
         {
            States[PanelEncoder.ALT] = value;
            OnPropertyChanged();
         }
      }

      [SimEvent("AP_N1_REF_SET")]
      public int CRS1
      {
         get => States[PanelEncoder.CRS1];
         set
         {
            States[PanelEncoder.CRS1] = value;
            OnPropertyChanged();
         }
      }

      public int CRS2
      {
         get => States[PanelEncoder.CRS2];
         set
         {
            States[PanelEncoder.CRS2] = value;
            OnPropertyChanged();
         }
      }

      [SimEvent("AP_VS_VAR_SET_ENGLISH")]
      public int WHEEL
      {
         get => States[PanelEncoder.WHEEL];
         set
         {
            States[PanelEncoder.WHEEL] = value;
            OnPropertyChanged();
         }
      }
      #endregion
   }
}
