using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace SmarterWebDeploy
{
	public static class HashUtility
	{
		public static string HashToString(FileInfo file)
		{
			if (file == null || !file.Exists)
			{
				return "0";
			}

			return HashToString(File.ReadAllBytes(file.FullName));
		}

		public static string HashToString(byte[] bytes)
		{
			if (bytes == null || bytes.Length == 0)
			{
				return "0";
			}

			using (var md5 = MD5.Create())
			{
				return string.Join(string.Empty, md5.ComputeHash(bytes).Select(c => c.ToString("x2")));
			}
		}
	}
}