using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmarterWebDeploy
{
	class FileCopy : PendingCopy
	{
		public string DestinationFile { get; set; }
		public string SourceFile { get; set; }

		public FileCopy(string fromFile, string toFile)
		{
			this.SourceFile = fromFile;
			this.DestinationFile = toFile;
		}

	}
}
