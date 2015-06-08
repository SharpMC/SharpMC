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

using System.Collections.Generic;
using System.IO;
using SharpMC.Core.Items;

namespace SharpMC.Core.Utils
{
	public class CraftingRecipe
	{
		private readonly string[] _recipe = new string[3];
		private readonly Dictionary<char, ItemStack> recipeDictionary = new Dictionary<char, ItemStack>();
		private readonly ItemStack resultItem;

		internal CraftingRecipe(Item[] items)
		{
			foreach (var item in items)
			{
				ItemStack i = new ItemStack(item, 1);
				recipeDictionary.Add(' ', i);
			}
		}

		public CraftingRecipe(ItemStack result, object[] recipe)
		{
			resultItem = result;
			for (var i = 0; i < 3; i++)
			{
				if (recipe[i] is string) _recipe[i] = (string) recipe[i];
				else throw new InvalidDataException("recipe invalid");
			}

			var prevchar = ' ';
			for (var i = 4; i < recipe.Length; i++) //Load dictionairy
			{
				var val = recipe[i];
				if (val is char)
				{
					var d = (char) val;
					prevchar = d;
				}

				if (val is ItemStack)
				{
					var d = (ItemStack) val;
					if (prevchar != ' ')
					{
						recipeDictionary.Add(prevchar, d);
						prevchar = ' ';
					}
				}
			}

			ConsoleFunctions.WriteDebugLine("Added crafting recipe for item id: " + result.ItemId);
		}

		public ItemStack GetResult()
		{
			return resultItem;
		}

		public override bool Equals(object obj)
		{
			var recipe = obj as CraftingRecipe;
			if (recipe != null)
			{
				if (recipe.recipeDictionary.Values == this.recipeDictionary.Values) return true;
			}
			return false;
		}
	}
}