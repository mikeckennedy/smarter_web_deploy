using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmarterWebDeploy
{
	class DirectoryCreate : PendingCopy
	{
		public DirectoryCreate(string destDirName)
		{
			this.DestDirName = destDirName;
		}

		public string DestDirName { get; private set; }
	}
}
