using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MVVMLibrary;
using TBMAutopilotDashboard.Models.Enums;

namespace TBMAutopilotDashboard.Models.State
{
   public class GarminButtonModel : Model
   {
      #region Local Props
      private PanelButton? _name = null;
      private bool _state = false;
      private bool _prevState = false;
      private ulong _hash = 0;
      #endregion

      #region Constructors
      public GarminButtonModel(PanelButton name) => Name = name;
      #endregion

      #region Methods
      public void UpdateState() => PrevState = State;
      #endregion

      #region Full Props
      public PanelButton? Name
      {
         get => _name;
         set
         {
            _name = value;
            OnPropertyChanged();
         }
      }

      public bool State
      {
         get => _state;
         set
         {
            _state = value;
            OnPropertyChanged();
         }
      }

      public bool PrevState
      {
         get => _prevState;
         set
         {
            _prevState = value;
            OnPropertyChanged();
         }
      }

      public ulong Hash
      {
         get => _hash;
         set
         {
            _hash = value;
            OnPropertyChanged();
         }
      }

      public bool StateChanged => _state != _prevState;
      #endregion
   }
}
