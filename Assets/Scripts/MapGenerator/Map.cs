using UnityEngine;

namespace DragTank.MapGenerator
{
    public class Map
    {
        public enum Tile
        {
            Empty = '.',
            Wall = '#',
            Objective = '*',
            Exit = '>',
            Entrance = '<'
        }

        public readonly int Width;
        public readonly int Height;

        private Tile[,] m_data;

        public Map(int width, int height, Tile starting_value)
        {
            Width = width;
            Height = height;

            m_data = new Tile[Width, Height];
            SetAll(starting_value);
        }


        public bool IsInBounds(int x, int y)
        {
            return (x >= 0 && y >= 0 && x < Width && y < Height);
        }

        public bool IsInBounds(IntVector2 location)
        {
            return IsInBounds(location.x, location.y);
        }


        public Tile GetTile(int x, int y)
        {
            if (!IsInBounds(x, y)) { return Tile.Empty; }
            return m_data[x, y];
        }

        public Tile GetTile(IntVector2 location)
        {
            return GetTile(location.x, location.y);
        }


        public void SetTile(int x, int y, Tile value)
        {
            if (!IsInBounds(x, y)) { return; }
            m_data[x, y] = value;
        }

        public void SetTile (IntVector2 location, Tile value)
        {
            SetTile(location.x, location.y, value);
        }
        
        public void SetAll (Tile value)
        {
            SetRegion(new IntVector2(0, 0), new IntVector2(Width, Height), value);
        }

        public void SetRegion(IntVector2 top_left, IntVector2 dimensions, Tile value)
        {
            for (int x = top_left.x; x < top_left.x + dimensions.x; ++x)
            {
                for (int y = top_left.y; y < top_left.y + dimensions.y; ++y)
                {
                    SetTile(x, y, value);
                }
            }
        }

        /// <summary>
        /// Randomly set every tile in the map to be either a wall or empty space, with the specified probability.
        /// </summary>
        /// <param name="p_wall">The probability of a single tile being set to contain a wall.</param>
        public void PlaceRandomWalls(float p_wall)
        {
            for (int x = 0; x < Width; ++x)
            {
                for (int y = 0; y < Height; ++y)
                {
                    SetTile(x, y, (Random.value < p_wall) ? Tile.Wall : Tile.Empty);
                }
            }
        }

        /// <summary>
        /// Get the location of a randomly-selected empty tile in the map. 
        /// NB: currently super-inefficient if the map is mostly walls, and loops infinitely if there are no empty tiles.
        /// </summary>
        /// <returns></returns>
        public IntVector2 GetRandomEmptyLocation()
        {
            IntVector2 location;
            do {
                location = new IntVector2(Random.Range(0, Width), Random.Range(0, Height));
            } while (GetTile(location) != Tile.Empty);

            return location;
        }

        public Tile[,] GetDataClone()
        {
            return (Tile[,]) m_data.Clone();
        }
    }
}
