using System;
using log4net;

using Plus.Communication.Packets.Outgoing.Moderation;

namespace Plus.Core
{
    public static class ConsoleCommands
    {
        private static readonly ILog log = LogManager.GetLogger("Plus.Core.ConsoleCommands");

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
                            log.Warn("The server is saving users furniture, rooms, etc. WAIT FOR THE SERVER TO CLOSE, DO NOT EXIT THE PROCESS IN TASK MANAGER!!");
                            PlusEnvironment.PerformShutDown();
                            break;
                        }
                    #endregion

                    #region alert
                    case "alert":
                        {
                            string Notice = inputData.Substring(6);

                            PlusEnvironment.GetGame().GetClientManager().SendPacket(new BroadcastMessageAlertComposer(PlusEnvironment.GetLanguageManager().TryGetValue("server.console.alert") + "\n\n" + Notice));

                            log.Info("Alert successfully sent.");
                            break;
                        }
                    #endregion

                    default:
                        {
                            log.Error(parameters[0].ToLower() + " is an unknown or unsupported command. Type help for more information");
                            break;
                        }
                }
            }
            catch (Exception e)
            {
                log.Error("Error in command [" + inputData + "]: " + e);
            }
        }
    }
}