using UnityEngine;

namespace DragTank.MapGenerator
{
    public class Room
    {
        IntVector2 Center;
        IntVector2 Size;
        IntVector2[] Exits;

        IntVector2 TopLeft { get { return Center - (Size / 2); } }

        public Room(IntVector2 center, IntVector2 size)
        {
            Center = center;
            Size = size;
            Exits = new IntVector2[]
            {
                new IntVector2(Size.x / 2, 0),
                new IntVector2(0, Size.y / 2),
                new IntVector2(Size.x / 2, Size.y - 1),
                new IntVector2(Size.x - 1, Size.y / 2)
            };
        }

        public static Room GenerateForMap(Map map, int min_size, int max_size)
        {
            return new Room(
                new IntVector2(Random.Range(0, map.Width), Random.Range(0, map.Height)),
                new IntVector2(Random.Range(min_size, max_size), Random.Range(min_size, max_size))
            );
        }

        public void writeToMap(Map map)
        {
            IntVector2 write_location = new IntVector2(0,0);

            //Write the room, including walls and empty space.
            for (write_location.x = TopLeft.x; write_location.x < TopLeft.x + Size.x; ++write_location.x)
            {
                for (write_location.y = TopLeft.y; write_location.y < TopLeft.y + Size.y; ++write_location.y)
                {
                    if (
                        write_location.x == TopLeft.x 
                        || write_location.x == TopLeft.x + Size.x - 1 
                        || write_location.y == TopLeft.y 
                        || write_location.y == TopLeft.y + Size.y - 1
                    )
                    {
                        //Wall
                        map.SetTile(write_location, true);
                    }
                    else
                    {
                        //Room
                        map.SetTile(write_location, false);
                    }
                }
            }

            //Add exits
            foreach (IntVector2 exit in Exits)
            {
                map.SetTile(TopLeft + exit, false);
            }
        }
    }
}
