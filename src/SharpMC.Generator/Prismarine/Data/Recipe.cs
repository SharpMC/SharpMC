namespace SharpMC.Generator.Prismarine.Data
{
    internal class Recipe
    {
        public int[] Ingredients { get; set; }

        public int?[][] InShape { get; set; }

        public int?[][] OutShape { get; set; }

        public RecipeResult Result { get; set; }
    }
}