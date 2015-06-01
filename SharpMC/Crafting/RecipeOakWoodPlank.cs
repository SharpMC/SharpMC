using SharpMC.Utils;

namespace SharpMC.Crafting
{
	public class RecipeOakWoodPlank : CraftingRecipe
	{
		public RecipeOakWoodPlank() : base(new ItemStack(5, 1, 0), new object[] {"W  ", "   ", "   ", 'W', new ItemStack(17, 1, 0)})
		{
		}
	}
}
