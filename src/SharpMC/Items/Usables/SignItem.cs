using System.Collections;
using System.Numerics;
using SharpMC.API.Enums;
using SharpMC.Blocks;

namespace SharpMC.Items.Usables
{
    internal class SignItem : UsableItem
    {
        internal SignItem()
        {
            IsUsable = true;
        }

        public override void UseItem(IItemLevel world, ILevelPlayer player,
            Vector3 coordinates, BlockFace face)
        {
            coordinates = GetNewCoordinatesFromFace(coordinates, face);
            if (face == BlockFace.PositiveY)
            {
                var bss = KnownBlocks.BirchSign;

                var rawbytes = new BitArray(new[] {(byte) bss.DefaultState});

                var direction = player.Direction;
                switch (direction)
                {
                    case 0:
                        // South
                        rawbytes[2] = true;
                        break;
                    case 1:
                        // West
                        rawbytes[3] = true;
                        break;
                    case 2:
                        // North DONE
                        rawbytes[2] = true;
                        rawbytes[3] = true;
                        break;
                    case 3:
                        // East
                        break;
                }

                world.SetBlock(bss, coordinates);

                // TODO
                /* new SignEditorOpen(player.Wrapper)
                {
                    Coordinates = blockCoordinates
                }.Write(); */
            }
            else
            {
                //TODO: implement wall signs
            }
        }
    }
}