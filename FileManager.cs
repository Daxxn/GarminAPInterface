using System;
using System.Collections.Generic;
using System.IO;

namespace TBMAutopilotDashboard
{
   public static class FileManager
   {
      #region - Methods
      public static List<Variable> ReadVariables()
      {
         try
         {
            var output = new List<Variable>();
            using (StreamReader reader = new StreamReader(Path.Combine(Environment.CurrentDirectory, Properties.Settings.Default.VariablesFilePath)))
            {
               var lines = new List<string>();
               while (!reader.EndOfStream)
               {
                  lines.Add(reader.ReadLine());
               }

               //foreach (var line in lines)
               //{
               //   if (!String.IsNullOrWhiteSpace(line))
               //   {
               //      var split = line.Split(',');
               //      output.Add(new Variable
               //      {
               //         Definition = (Definition)lines.IndexOf(line),
               //         Name = split[0],
               //         Units = split.Length > 1 ? split[1] : "position"
               //      });
               //   }
               //}

               for (int i = 0; i < lines.Count; i++)
               {
                  if (!String.IsNullOrWhiteSpace(lines[i]))
                  {
                     var split = lines[i].Split(',');
                     output.Add(new Variable
                     {
                        Definition = (Definition)i,
                        Request = (Request)i,
                        Name = split[0],
                        Units = split.Length > 1 ? split[1] : "position"
                     });
                  }
               }
            }
            return output;
         }
         catch (Exception)
         {
            throw;
         }
      }
      #endregion

      #region - Full Properties

      #endregion
   }
}
