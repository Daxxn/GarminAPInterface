using Microsoft.FlightSimulator.SimConnect;
using MVVMLibraryFW;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBMAutopilotDashboard
{
   public enum Request
   {
      Default = 0
   };
   public enum Definition
   {
      Default = 0
   };

   public class Variable : BaseModel
   {
      #region - Fields & Properties
      private Definition _definition;
      private Request _request;
      public SIMCONNECT_DATATYPE DataType { get; private set; }

      private string _name;
      private bool _isPending;

      private double _value;
      private string _units;
      #endregion

      #region - Constructors
      public Variable() { }
      #endregion

      #region - Methods
      public override string ToString()
      {
         return $"{Name} : {Value} , {Request}  {Definition} ({IsPending})";
      }
      #endregion

      #region - Full Properties
      public Request Request
      {
         get { return _request; }
         set
         {
            _request = value;
            OnPropertyChanged();
         }
      }

      public Definition Definition
      {
         get { return _definition; }
         set
         {
            _definition = value;
            OnPropertyChanged();
         }
      }

      public string Name
      {
         get { return _name; }
         set
         {
            _name = value;
            OnPropertyChanged();
         }
      }


      public bool IsPending
      {
         get { return _isPending; }
         set
         {
            _isPending = value;
            OnPropertyChanged();
         }
      }

      public double Value
      {
         get { return _value; }
         set
         {
            _value = value;
            OnPropertyChanged();
         }
      }

      public string Units
      {
         get { return _units; }
         set
         {
            _units = value;
            OnPropertyChanged();
         }
      }
      #endregion
   }
}
