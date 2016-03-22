
namespace DragTank.MapGenerator
{
    public class CaveCellularAutomata
    {
        private static IntVector2[] neighbours = {
            new IntVector2 (1, 1),
            new IntVector2 (1, 0),
            new IntVector2 (1, -1),
            new IntVector2 (0, 1),
            new IntVector2 (0, 0),
            new IntVector2 (0, -1),
            new IntVector2 (-1, 1),
            new IntVector2 (-1, 0),
            new IntVector2 (-1, -1),
        };

        public static void runOnMap(Map map, int iterations)
        {
            map.RandomiseAll(0.5f);

            for (int i = 0; i < iterations; ++i)
            {
                bool[,] old_map = map.GetDataClone();
                IntVector2 current_position = new IntVector2(0, 0);

                for (current_position.x = 0; current_position.x < map.Width; ++current_position.x)
                {
                    for (current_position.y = 0; current_position.y < map.Height; ++current_position.y)
                    {
                        int neighbouring_walls = 0;
                        foreach (IntVector2 neighbour_vector in neighbours)
                        {
                            IntVector2 neighbour = current_position + neighbour_vector;

                            if (neighbour.x < 0 || neighbour.x >= map.Width || neighbour.y < 0 || neighbour.y >= map.Height || old_map[neighbour.x, neighbour.y])
                            {
                                ++neighbouring_walls;
                            }
                        }

                        map.SetTile(current_position, neighbouring_walls >= 5);
                    }
                }
            }
        }
    }
}
