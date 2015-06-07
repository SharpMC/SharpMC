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

namespace SharpMC.Items
{
	internal class ItemFactory
	{
		public static Item GetItemById(short id)
		{
			return GetItemById(id, 0);
		}

		public static Item GetItemById(short id, byte metadata)
		{
			if (id == 259)
			{
				return new ItemFlintAndSteel();
			}

			if (id == 263)
			{
				return new ItemCoal();
			}

			if (id == 276)
			{
				return new ItemDiamondSword();
			}

			if (id == 310)
			{
				return new ItemDiamondHelmet();
			}

			if (id == 311)
			{
				return new ItemDiamondChestplate();
			}

			if (id == 312)
			{
				return new ItemDiamondLeggings();
			}

			if (id == 313)
			{
				return new ItemDiamondBoots();
			}

			if (id == 306)
			{
				return new ItemIronHelmet();
			}

			if (id == 307)
			{
				return new ItemIronChestplate();
			}

			if (id == 308)
			{
				return new ItemIronLeggings();
			}

			if (id == 309)
			{
				return new ItemIronBoots();
			}

			if (id == 267)
			{
				return new ItemIronSword();
			}

			if (id == 326)
			{
				return new ItemWaterBucket();
			}

			if (id == 327)
			{
				return new ItemLavaBucket();
			}

			if (id == 325)
			{
				return new ItemBucket();
			}

			if (id == 280)
			{
				return new ItemStick();
			}

			if (id == 332)
			{
				return new ItemSnowball();
			}

			return new Item((ushort)id, metadata);
		}
	}
}