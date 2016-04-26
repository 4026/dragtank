using System;

namespace DragTank.MapGenerator
{
    public class Sector
    {
        public static readonly int Width = 8;
        public static readonly int Height = 8;

        public bool ContainsEntrance { get; private set; }
        public bool ContainsExit { get; private set; }
        public bool ContainsObjective { get; private set; }

        private Map.Tile[,] m_tiles = new Map.Tile[Width, Height];

        /// <summary>
        /// Construct a Sector instance from a serialised text representation.
        /// </summary>
        /// <param name="tile_string"></param>
        public Sector(string tile_string)
        {
            ContainsEntrance = false;
            ContainsExit = false;
            ContainsObjective = false;

            //Iterate through each row of the data...
            string[] tile_rows = tile_string.Split(new String[] { "\r\n" }, Height, StringSplitOptions.None);

            if (tile_rows.Length != Height)
            {
                throw new Exception(string.Format("Sector data contains {0} rows. Expected {1}", tile_rows.Length, Height));
            }

            for (int y = 0; y < Height; ++y)
            {
                string row = tile_rows[y];
                if (row.Length != Width)
                {
                    throw new Exception(
                        string.Format("Sector data contains {0} tiles on row {1}. Expected {2}", row.Length, y, Width)
                    );
                }
                //... and each character of each row.
                for (int x = 0; x < Width; ++x)
                {
                    Map.Tile tile = (Map.Tile)row[x];

                    //Update Contains booleans.
                    ContainsEntrance  |= tile == Map.Tile.Entrance;
                    ContainsExit      |= tile == Map.Tile.Exit;
                    ContainsObjective |= tile == Map.Tile.Objective;

                    m_tiles[x, y] = tile;
                }
            }
        }

        /// <summary>
        /// Write the sector data to the provided Map instance, using the provided co-ordinates on the map as the top left 
        /// of the sector.
        /// </summary>
        public void WriteToMap(Map map, IntVector2 top_left)
        {
            for (int y = 0; y < Height; ++y)
            {
                for (int x = 0; x < Width; ++x)
                {
                    map.SetTile(x + top_left.x, y + top_left.y, m_tiles[x,y]);
                }
            }
        }
    }
}
