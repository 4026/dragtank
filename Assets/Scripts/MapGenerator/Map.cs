using UnityEngine;

namespace DragTank.MapGenerator
{
    public class Map
    {
        public readonly int Width;
        public readonly int Height;

        private bool[,] m_data;

        public Map(int width, int height, bool starting_value)
        {
            Width = width;
            Height = height;

            m_data = new bool[Width, Height];
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


        public bool GetTile(int x, int y)
        {
            if (!IsInBounds(x, y)) { return false; }
            return m_data[x, y];
        }

        public bool GetTile(IntVector2 location)
        {
            return GetTile(location.x, location.y);
        }


        public void SetTile(int x, int y, bool contains_wall)
        {
            if (!IsInBounds(x, y)) { return; }
            m_data[x, y] = contains_wall;
        }

        public void SetTile (IntVector2 location, bool contains_wall)
        {
            SetTile(location.x, location.y, contains_wall);
        }
        
        public void SetAll (bool value)
        {
            SetRegion(new IntVector2(0, 0), new IntVector2(Width, Height), value);
        }

        public void SetRegion(IntVector2 top_left, IntVector2 dimensions, bool value)
        {
            for (int x = top_left.x; x < top_left.x + dimensions.x; ++x)
            {
                for (int y = top_left.y; y < top_left.y + dimensions.y; ++y)
                {
                    SetTile(x, y, value);
                }
            }
        }

        public void RandomiseAll(float p_wall)
        {
            for (int x = 0; x < Width; ++x)
            {
                for (int y = 0; y < Height; ++y)
                {
                    SetTile(x, y, Random.value < p_wall);
                }
            }
        }

        public bool[,] GetDataClone()
        {
            return (bool[,]) m_data.Clone();
        }
    }
}
