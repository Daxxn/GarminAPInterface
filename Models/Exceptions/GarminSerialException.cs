using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TBMAutopilotDashboard.Models.Exceptions
{
   public class GarminSerialException : Exception
   {
      public GarminSerialException() { }
      public GarminSerialException(string message) : base(message) { }
      public GarminSerialException(string message, Exception innerException) : base(message, innerException) { }
      protected GarminSerialException(SerializationInfo info, StreamingContext context) : base(info, context) { }
   }
}
