using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TBMAutopilotDashboard.Models.Enums;

namespace TBMAutopilotDashboard
{
   /// <summary>
   /// Simple message data type.
   /// <para/>
   /// 0  - INFO  - N/A
   /// <para/>
   /// 1  - ERROR - RED
   /// <para/>
   /// 2+ - WARN  - Orange
   /// </summary>
   public struct Message
   {
      public int Type { get; set; }
      public string Text { get; set; }

      /// <summary>
      /// New simple message data type.
      /// </summary>
      /// <param name="message">Message string.</param>
      /// <param name="type">Message type:
      /// <para/>
      /// 0-INFO
      /// 1-ERROR
      /// 2+-WARN
      /// </param>
      public Message(string message, int type = 0)
      {
         Text = message;
         Type = type;
      }

      public Message(string message, Messagetype type)
      {
         Text = message;
         Type = (int)type;
      }
   }
}
