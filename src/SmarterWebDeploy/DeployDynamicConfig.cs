using System.Configuration;
using System.Dynamic;
using System.Linq;

namespace SmarterWebDeploy
{
	internal class DeployDynamicConfig : DynamicObject
	{
		private static DeployDynamicConfig current = new DeployDynamicConfig();

		public static dynamic Current
		{
			get { return current; }
		}

		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			var keyLookup = ConfigurationManager.AppSettings.AllKeys.ToDictionary(n => n.ToLower());
			string name = binder.Name.ToLower();

			if (!keyLookup.ContainsKey(name))
			{
				result = null;
				return true;
			}

			var key = keyLookup[name];

			result = ConfigurationManager.AppSettings[key];
			if (result != null)
			{
				return true;
			}

			return true;
		}
	}
}
