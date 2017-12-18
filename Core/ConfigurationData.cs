using System;
using System.Collections.Generic;
using System.IO;

namespace Plus.Core
{
    public class ConfigurationData
    {
        public Dictionary<string, string> data;

        public ConfigurationData(string filePath, bool maynotexist = false)
        {
            data = new Dictionary<string, string>();

            if (!File.Exists(filePath))
            {
                if (!maynotexist)
                    throw new ArgumentException("Unable to locate configuration file at '" + filePath + "'.");
                else
                    return;
            }

            try
            {
                using (var stream = new StreamReader(filePath))
                {
                    string line = "";

                    while ((line = stream.ReadLine()) != null)
                    {
                        if (line.Length < 1 || line.StartsWith("#"))
                        {
                            continue;
                        }

                        int delimiterIndex = line.IndexOf('=');

                        if (delimiterIndex != -1)
                        {
                            string key = line.Substring(0, delimiterIndex);
                            string val = line.Substring(delimiterIndex + 1);

                            data.Add(key, val);
                        }
                    }
                }
            }

            catch (Exception e)
            {
                throw new ArgumentException("Could not process configuration file: " + e.Message);
            }
        }
    }
}