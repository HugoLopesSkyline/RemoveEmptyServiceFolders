using System;
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

		private void RunSafe(IEngine engine)
		{
			// Hardcoded list of folder paths
			string[] folderPaths = new string[]
			{
				@"C:\Temp\Folder1",
				@"C:\Temp\Folder2",
				@"C:\Temp\Folder3"
			};

			// Delete folders
			DeleteFolders(folderPaths, engine);

			// Perform SLNet call
			PerformSLNetCall(folderPaths, engine);
		}

		/// <summary>
		/// Deletes folders from the provided list of paths.
		/// </summary>
		/// <param name="folderPaths">Array of folder paths to delete.</param>
		/// <param name="engine">The automation engine for logging.</param>
		private void DeleteFolders(string[] folderPaths, IEngine engine)
		{
			foreach (var folderPath in folderPaths)
			{
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
			}
		}

		/// <summary>
		/// Performs an SLNet call.
		/// </summary>
		/// <param name="engine">The automation engine for logging.</param>
		private void PerformSLNetCall(string[] folderPaths, IEngine engine)
		{
			foreach (var folderPath in folderPaths)
			{
				try
				{
					// Example SLNet call
					var message = new SetDataMinerInfoMessage()
					{
						What = (int)NotifyType.RemoveFileChange,
						StrInfo1 = folderPath,
					};

					var response = engine.SendSLNetMessage(message);
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
