using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace services.Utilities
{
    public static class LocationFinder
    {
        public static string GetLocationByCommandLine(string commandLine)
        {
            if (string.IsNullOrWhiteSpace(commandLine))
            {
                return string.Empty;
            }

            string location = ExtractLocation(commandLine);
            return Path.GetFullPath(location);
        }

        private static string ExtractLocation(string commandLine)
        {
            if (commandLine.StartsWith("\""))
            {
                var index = commandLine.IndexOf('\"', 1);
                if (index < 0)
                {
                    return commandLine;
                }

                return commandLine.Substring(1, index - 1);
            }

            var indexOfSpace = commandLine.IndexOf(' ', 0);
            if (indexOfSpace < 0)
            {
                return commandLine;
            }

            return commandLine.Substring(0, indexOfSpace);
        }
    }
}
