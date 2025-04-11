using System;
using System.Collections.Generic;
using System.IO;
using Skyline.DataMiner.Automation;
using Skyline.DataMiner.Net.Messages;
using Skyline.DataMiner.Net.Messages.Advanced;

namespace RemoveEmptyServiceFolders
{
    /// <summary>
    /// Represents a DataMiner Automation script.
    /// </summary>
    public class Script
    {
        /// <summary>
        /// The script entry point.
        /// </summary>
        /// <param name="engine">Link with SLAutomation process.</param>
        public void Run(IEngine engine)
        {
			try
			{
				RunSafe(engine);
			}
			catch (ScriptAbortException)
			{
				throw; // Comment if it should be treated as a normal exit of the script.
			}
			catch (ScriptForceAbortException)
			{
				throw;
			}
			catch (ScriptTimeoutException)
			{
				throw;
			}
			catch (InteractiveUserDetachedException)
			{
				throw;
			}
			catch (Exception e)
			{
				engine.ExitFail("Run|Something went wrong: " + e);
			}
		}

        public struct ServiceData
        {
            public ServiceData(string dmaID, string serviceName)
            {
                DmaID = dmaID;
                ServiceName = serviceName;
            }

            public string DmaID { get; }
            public string ServiceName { get; }
        }

        private void RunSafe(IEngine engine)
        {
            // Hardcoded list of folder paths
            var datafolderPaths = new List<ServiceData>
                            {
                                new ServiceData("1008", "MUX A MAIN_RM-SM-GC1-ASI-1_DIST - RAI GULP to MM-HE-EPIC2-PiP 5"),
                            };

            // Delete folders and perform SLNet call
            DeleteAndPerformSLNetCall(datafolderPaths, engine);
        }

        /// <summary>
        /// Deletes folders and performs SLNet calls for the provided list of paths.
        /// </summary>
        /// <param name="folderPaths">List of folder paths to process.</param>
        /// <param name="engine">The automation engine for logging.</param>
        private void DeleteAndPerformSLNetCall(List<ServiceData> folderPaths, IEngine engine)
        {
            foreach (var datafolderPath in folderPaths)
            {
                string folderPath = Path.Combine(@"C:\Skyline DataMiner\RemoteServices", datafolderPath.DmaID, datafolderPath.ServiceName);

                // Delete folder
                try
                {
                    if (Directory.Exists(folderPath))
                    {
                        Directory.Delete(folderPath, true);
                        engine.Log($"Successfully deleted folder: {folderPath}");
                    }
                    else
                    {
                        engine.Log($"Folder does not exist: {folderPath}");
                    }
                }
                catch (Exception ex)
                {
                    engine.Log($"Error deleting folder {folderPath}: {ex.Message}");
                }

                // Perform SLNet call
                try
                {
                    var message = new SetDataMinerInfoMessage
                    {
                        What = (int)NotifyType.RemoveFileChange,
                        StrInfo1 = folderPath,
                    };

                    engine.SendSLNetMessage(message);
                    engine.Log("SLNet call executed successfully.");
                }
                catch (Exception ex)
                {
                    engine.Log($"Error during SLNet call: {ex.Message}");
                }
            }
        }
    }
}
