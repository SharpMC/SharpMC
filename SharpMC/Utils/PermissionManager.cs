// Distrubuted under the MIT license
// ===================================================
// SharpMC uses the permissive MIT license.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the “Software”), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software
// 
// THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// 
// ©Copyright Kenny van Vulpen - 2015
using System;
using System.Collections.Generic;
using SharpMC.Entity;

namespace SharpMC.Utils
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