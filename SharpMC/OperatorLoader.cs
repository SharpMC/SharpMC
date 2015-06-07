#region Header

// Distrubuted under the MIT license
// ===================================================
// SharpMC uses the permissive MIT license.
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the “Software”), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software
// THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// ©Copyright Kenny van Vulpen - 2015
#endregion

namespace SharpMC
{
	using System.Collections.Generic;
	using System.IO;

	internal static class OperatorLoader
	{
		private static readonly List<string> _ops = new List<string>();

		public static void LoadOperators()
		{
			if (File.Exists("operators.txt"))
			{
				var contents = File.ReadAllLines("operators.txt");
				foreach (var line in contents)
				{
					_ops.Add(line.ToLower());
				}
			}
			else
			{
				File.WriteAllLines(
					"operators.txt", 
					new[] { "#Notice: If your server is in online mode the values have to be a users UUID!" });
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