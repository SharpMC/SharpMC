using System.Collections.Generic;
using System.IO;

namespace SharpMC
{
	internal static class OperatorLoader
	{
		private static List<string> _ops = new List<string>();
		public static void LoadOperators()
		{
			if (File.Exists("operators.txt"))
			{
				string[] contents = File.ReadAllLines("operators.txt");
				foreach (string line in contents)
				{
					ConsoleFunctions.WriteInfoLine("Line: \"" + line + "\"");
					_ops.Add(line.ToLower());
				}
			}
			else
			{
				File.WriteAllLines("operators.txt", new string[] { "#Notice: If your server is in online mode the values have to be a users UUID!" });
			}
		}

		public static bool IsOperator(string uuid)
		{
			return _ops.Contains(uuid.ToLower());
		}

		public static bool Toggle(string uuid)
		{
			uuid = uuid.ToLower();
			if (_ops.Contains(uuid))
			{
				_ops.Remove(uuid);
				return false;
			}

			_ops.Add(uuid);
			return true;
		}

		public static void SaveOperators()
		{
			File.WriteAllLines("operators.txt", _ops.ToArray());
		}
	}
}
