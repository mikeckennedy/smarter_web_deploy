using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security;
using System.Text;
using System.Threading;
using System.Web;

namespace SmarterWebDeploy
{
	public static class DeployManager
	{
		private static string fromBaseFolder;
		private static string toBaseFolder;
		private static string appOfflineSourceFile;
		private const string appOfflineDestFile = "app_offline.htm";
		private static string redirectUrl;
		private static string verificationKey;
		public static bool HasInitialized { get; private set; }

		public static void Init()
		{
			if (HasInitialized)
			{
				return;
			}

			dynamic config = DeployDynamicConfig.Current;

			fromBaseFolder = GetAndVerifyFolder("fromBaseFolder", config.fromBaseFolder);
			toBaseFolder = GetAndVerifyFolder("toBaseFolder", config.toBaseFolder);

			if (fromBaseFolder == toBaseFolder)
			{
				throw new ConfigurationErrorsException("Smart deploy cannot initialize, the from and to folders are the same.");
			}

			appOfflineSourceFile = GetAndVerifyFile("appOfflineSourceFile", config.appOfflineSourceFile);
			verificationKey = config.verificationKey;
			redirectUrl = config.redirectUrl;
			if (string.IsNullOrEmpty(redirectUrl))
			{
				throw new ConfigurationErrorsException("Smart deploy cannot initialize, the setting redirectUrl is missing.");
			}
			HasInitialized = true;
		}

		private static readonly Stopwatch stopwatch = new Stopwatch();

		public static string Deploy(string userVerificaioinCode, Stream responseStream)
		{
			StreamWriter output = new StreamWriter(responseStream);
			StringWriter logStream = new StringWriter();

			stopwatch.Restart();
			Write(output, logStream, "Smart deploy starting at " + DateTime.Now);

			if (!HasInitialized)
			{
				string msg = "Smart deploy cannot run, you must call DeployManager.Init() first.";
				Write(output, logStream, msg);
				throw new InvalidOperationException(msg);
			}

			// 1. Verify access code (if set).
			if (!string.IsNullOrWhiteSpace(verificationKey))
			{
				if (verificationKey != userVerificaioinCode)
				{
					string msg = "Smart deploy cannot run. The supplied verification code of '" + userVerificaioinCode +
					             "' does not match the configuration value.";
					Write(output, logStream, msg);
					throw new SecurityException(msg);
				}
			}

			// 2. Find files which have changed to copy and deploy.
			Write(output, logStream, "Compare source and destination files (application is still online during this step) ...");
			List<PendingCopy> list = FileUtilities.BuildCopySet(fromBaseFolder, toBaseFolder, true, true);

			if (list.Count == 0)
			{
				Write(output, logStream,
					"There are no file changes to deploy. Deploy cancelled and application is still online (unchanged).");
				SaveDeployLogFile(logStream.ToString());
				return redirectUrl;
			}

			RemoveOldDeployLogFile();

			// TODO: if only cshtml, png, jpg, gif, keep online

			// 3. Copy app offline to disable new incoming requests temporarily.
			Write(output, logStream,
				"Setting application to offline state (<a target='_blank' href='" + redirectUrl + "'>" + redirectUrl + "</a>)");

			string finalAppOfflineDestFile = Path.Combine(toBaseFolder, appOfflineDestFile);
			string finalAppOfflineSourceFile = Path.Combine(fromBaseFolder, appOfflineSourceFile);

			File.Copy(finalAppOfflineSourceFile, finalAppOfflineDestFile, true);

			// 4. Wait a few seconds for any pending requests to complete before splashing the app domain.
			Write(output, logStream, "Waiting 2 seconds for pending requests (if any) to complete ...");
			Thread.Sleep(1980);

			try
			{
				// 5. Copy all files (consider deleting dest folder first?)
				Write(output, logStream, "Copying files to destination site ...");
				int copiedCount = 0;
				FileUtilities.FileCopied += file => copiedCount++;
				FileUtilities.FileCopied += file => Write(output, logStream, "File copied: " + file);

				Write(output, logStream, "Running " + list.Count + " file operations...");
				FileUtilities.FinalizeCopy(list);

				Write(output, logStream, "Copied " + copiedCount + " files successfully.");
			}
			finally
			{
				FileUtilities.ClearEvents();
			}

			// 6. Wait 1 second for all file modifications to be detected.
			Write(output, logStream, "Waiting for final changes to be detected ...");
			Thread.Sleep(500);

			// 7. Delete app offline
			Write(output, logStream, "Removing application offline status ...");
			File.Delete(finalAppOfflineDestFile);
			Thread.Sleep(100);

			bool? verified = null;
			if (!string.IsNullOrWhiteSpace(redirectUrl))
			{
				verified = VerifySiteDeploy(output, logStream);
			}

			if (verified == null || verified == true)
			{
				// 8. Back online.
				Write(output, logStream,
					string.Format("Smart deploy successful. Visit this link to view the page: <a target='_blank' href='" + redirectUrl +
					              "'>" + redirectUrl + "</a>"));
			}

			SaveDeployLogFile(logStream.ToString());

			return redirectUrl;
		}

		private static bool VerifySiteDeploy(StreamWriter output, StringWriter logStream)
		{
			Write(output, logStream, "Requesting page at " + redirectUrl + ", starting site and verifying deploy ...");
			try
			{
				WebClient client = new WebClient();
				string data = client.DownloadString(redirectUrl);

				Write(output, logStream,
					"Site started <strong style='color: darkgreen'>SUCCESSFULLY, " + data.Length.ToString("N0") +
					" characters returned.</strong>");
				return true;
			}
			catch (WebException x)
			{
				Write(output, logStream,
					"<strong style='color: red'>ERROR: Site failed: Status=" + x.Status + ", Message=" + x.Message + "</strong>");
			}
			catch (Exception x)
			{
				Write(output, logStream,
					"<strong style='color: darkgreen'>ERROR: Unknown error requesting page: " + x.Message + "</strong>");
			}
			return false;
		}

		private static void SaveDeployLogFile(string logText)
		{
			string file = Path.Combine(toBaseFolder, "last-deploy.txt");
			File.WriteAllText(file, logText);
		}

		private static void RemoveOldDeployLogFile()
		{
			string file = Path.Combine(toBaseFolder, "last-deploy.txt");
			if (File.Exists(file))
				File.Delete(file);
		}

		private static void Write(StreamWriter output, StringWriter logStream, string text)
		{
			string time = (stopwatch.ElapsedMilliseconds/1000.0).ToString("N3") + " seconds: ";
			StringBuilder sb = new StringBuilder(1024);
			for (int i = 0; i < 1024; i++)
			{
				sb.Append(" ");
			}
			output.WriteLine(time + text + "<br />" + sb);
			output.Flush();
			output.BaseStream.Flush();

			logStream.WriteLine(text);
		}

		private static string GetAndVerifyFolder(string folderSettingName, string folderValue)
		{
			if (string.IsNullOrEmpty(folderValue))
			{
				throw new ConfigurationErrorsException("Smart deploy cannot initialize, the " + folderSettingName +
				                                       " app setting is missing.");
			}

			if (!Directory.Exists(folderValue))
			{
				throw new ConfigurationErrorsException(
					"Smart deploy cannot initialize, the " + folderSettingName +
					" specified folder does not exist or is inaccessible: folder = " +
					folderValue);
			}
			return folderValue;
		}

		private static string GetAndVerifyFile(string fileSettingName, string fileValue)
		{
			if (string.IsNullOrEmpty(fileValue))
			{
				throw new ConfigurationErrorsException("Smart deploy cannot initialize, the " + fileSettingName +
				                                       " app setting is missing.");
			}

			if (!Path.IsPathRooted(fileValue))
			{
				string myDir = HttpContext.Current.Server.MapPath("~/");
				fileValue = Path.Combine(myDir, fileValue);
			}

			if (!File.Exists(fileValue))
			{
				throw new ConfigurationErrorsException(
					"Smart deploy cannot initialize, the " + fileSettingName +
					" specified file does not exist or is inaccessible: file = " +
					fileValue);
			}
			return fileValue;
		}
	}
}