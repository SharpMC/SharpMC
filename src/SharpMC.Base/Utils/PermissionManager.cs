using System;
using System.Collections.Generic;

namespace SharpMC.Core.Utils
{
	public class PermissionManager
	{
		internal static List<Tuple<string, string>> Permissions = new List<Tuple<string, string>>();

		public static bool HasPermission(Player player, string permission)
		{
			if (player.IsOperator) return true; //Operators (OP) have all permissions.
			if (permission == "") return true;

			foreach (var d in Permissions)
			{
				if (d.Item1 == player.Username)
				{
					if (d.Item2 == permission) return true;
				}
			}

			return false;
		}

		public static void AddPermission(Player player, string permission)
		{
			Permissions.Add(new Tuple<string, string>(player.Username, permission));
		}

		public static bool RemovePermission(Player player, string permission)
		{
			if (Permissions.Contains(new Tuple<string, string>(player.Username, permission)))
				return Permissions.Remove(new Tuple<string, string>(player.Username, permission));

			return false;
		}

		public static string[] GetPermissions(Player player)
		{
			var list = new List<string>();
			foreach (var val in Permissions)
			{
				if (val.Item1 == player.Username) list.Add(val.Item2);
			}
			return list.ToArray();
		}
	}
}