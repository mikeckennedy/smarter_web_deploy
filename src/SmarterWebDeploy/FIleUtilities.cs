using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SmarterWebDeploy
{
	internal class FileUtilities
	{
		public static List<PendingCopy> BuildCopySet(string sourceDirName, string destDirName, bool copySubDirs, bool overwrite, List<PendingCopy> list = null)
		{
			if (list == null)
			{
				list = new List<PendingCopy>();
			}

			// Get the subdirectories for the specified directory.
			DirectoryInfo dir = new DirectoryInfo(sourceDirName);
			DirectoryInfo[] dirs = dir.GetDirectories();

			if (!dir.Exists)
			{
				throw new DirectoryNotFoundException(
					"Source directory does not exist or could not be found: "
					+ sourceDirName);
			}

			// If the destination directory doesn't exist, create it. 
			if (!Directory.Exists(destDirName))
			{
				list.Add(new DirectoryCreate(destDirName));
			}

			// Get the files in the directory and copy them to the new location.
			FileInfo[] files = dir.GetFiles();
			foreach (FileInfo file in files)
			{
				string fromFile = file.FullName;
				string toFile = Path.Combine(destDirName, file.Name);

				if (!File.Exists(toFile) || !AreFilesIdentical(fromFile, toFile))
					list.Add(new FileCopy(fromFile, toFile));
			}

			// If copying subdirectories, copy them and their contents to new location. 
			if (copySubDirs)
			{
				foreach (DirectoryInfo subdir in dirs)
				{
					string temppath = Path.Combine(destDirName, subdir.Name);
					BuildCopySet(subdir.FullName, temppath, true, overwrite, list );
				}
			}

			return list;
		}

		public static void FinalizeCopy(List<PendingCopy> pendingCopies)
		{
			DirectoryCreate[] dirs = pendingCopies.OfType<DirectoryCreate>().ToArray();
			FileCopy[] files = pendingCopies.OfType<FileCopy>().ToArray();

			foreach (var dir in dirs)
			{
				Directory.CreateDirectory(dir.DestDirName);
			}

			foreach (var file in files)
			{
				File.Copy(file.SourceFile, file.DestinationFile, true);
				FileCopied(file.DestinationFile);
			}
		}

		private static bool AreFilesIdentical(string fromFile, string toFile)
		{
			string hashFrom = HashUtility.HashToString(new FileInfo(fromFile));
			string hashTo = HashUtility.HashToString(new FileInfo(toFile));

			return hashFrom == hashTo;
		}

		public static void ClearEvents()
		{
			FileCopied =  delegate { };
		}

		public delegate void FileCopiedHandler(string filePath);
		public static event FileCopiedHandler FileCopied = delegate { };
	}
}