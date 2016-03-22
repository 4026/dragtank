using UnityEngine;

namespace DragTank.MapGenerator
{
    public class DrunkardWalk
    {
        private static IntVector2[] directions = {
            new IntVector2 (1, 0),
            new IntVector2 (0, 1),
            new IntVector2 (-1, 0),
            new IntVector2 (0, -1),
        };

        public static void runOnMap(Map map, float emptyFraction)
        {
            map.SetAll(true);

            IntVector2 position = new IntVector2(Mathf.FloorToInt(map.Width / 2), Mathf.FloorToInt(map.Height / 2));
            map.SetTile(position, false);

            int empty_tiles = 1;
            int total_tiles = map.Height * map.Width;

            while (empty_tiles < emptyFraction * total_tiles)
            {
                position += directions[(int)Random.Range(0, 4)];

                position.x = Mathf.Clamp(position.x, 0, map.Width - 1);
                position.y = Mathf.Clamp(position.y, 0, map.Height - 1);

                if (map.GetTile(position))
                {
                    map.SetTile(position, false);
                    ++empty_tiles;
                }
            }
        }
    }
}
