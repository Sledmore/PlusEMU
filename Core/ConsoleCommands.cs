using System;

using Plus.Communication.Packets.Outgoing.Moderation;
using Serilog;

namespace Plus.Core
{
    public static class ConsoleCommands
    {
        public static void InvokeCommand(string inputData)
        {
            if (string.IsNullOrEmpty(inputData))
                return;

            try
            {
                string[] parameters = inputData.Split(' ');

                switch (parameters[0].ToLower())
                {
                    #region stop
                    case "stop":
                    case "shutdown":
                        {
                            Log.Warning("The server is saving users furniture, rooms, etc. WAIT FOR THE SERVER TO CLOSE, DO NOT EXIT THE PROCESS IN TASK MANAGER!!");
                            PlusEnvironment.PerformShutDown();
                            break;
                        }
                    #endregion

                    #region alert
                    case "alert":
                        {
                            string notice = inputData.Substring(6);

                            PlusEnvironment.GetGame().GetClientManager().SendPacket(new BroadcastMessageAlertComposer(PlusEnvironment.GetLanguageManager().TryGetValue("server.console.alert") + "\n\n" + notice));

                            Log.Information("Alert successfully sent.");
                            break;
                        }
                    #endregion

                    default:
                        {
                            Log.Error(parameters[0].ToLower() + " is an unknown or unsupported command. Type help for more information");
                            break;
                        }
                }
            }
            catch (Exception e)
            {
                Log.Error("Error in command [" + inputData + "]: " + e);
            }
        }
    }
}