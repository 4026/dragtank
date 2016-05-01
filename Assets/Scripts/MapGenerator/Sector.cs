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
        /// of the sector, rotated by the provided rotation.
        /// </summary>
        public void WriteToMap(Map map, IntVector2 top_left, IntVector2.Rotation rotation)
        {
            IntVector2 read_position = new IntVector2(0, 0);

            for (read_position.y = 0; read_position.y < Height; ++read_position.y)
            {
                for (read_position.x = 0; read_position.x < Width; ++read_position.x)
                {
                    Map.Tile tile = m_tiles[read_position.x, read_position.y];
                    IntVector2 write_position = read_position.Rotated(rotation) + getRotatedWriteTranslation(rotation);
                    map.SetTile(write_position.x + top_left.x, write_position.y + top_left.y, tile);
                }
            }
        }

        /// <summary>
        /// Get the translation to be applied to rotated sectors to ensure that they are written in the correct location.
        /// </summary>
        /// <param name="rotation"></param>
        /// <returns></returns>
        private static IntVector2 getRotatedWriteTranslation(IntVector2.Rotation rotation)
        {
            switch (rotation)
            {
                case IntVector2.Rotation.deg0:
                    return new IntVector2(0, 0);
                case IntVector2.Rotation.deg90:
                    return new IntVector2(Width-1, 0);
                case IntVector2.Rotation.deg180:
                    return new IntVector2(Width-1, Height-1);
                case IntVector2.Rotation.deg270:
                    return new IntVector2(0, Height-1);
                default:
                    throw new ArgumentOutOfRangeException("Unrecognised IntVector2 rotation " + rotation);
            }
        }
    }
}
