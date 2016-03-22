using UnityEngine;
using System.Collections;

public class LevelGenerator : MonoBehaviour
{

	public Vector2 TopLeftPosition;
	public Vector2 Dimensions;
	public float TileSize;
	public GameObject Wall;
	public GameObject WallParent;

	private bool[,] map_data;
	private int map_width;
	private int map_height;

	void Awake ()
	{
		map_width = (int)Mathf.Floor (Dimensions.x / TileSize);
		map_height = (int)Mathf.Floor (Dimensions.y / TileSize);
		map_data = new bool[map_width, map_height];

        //Set map to be open space.
        placeRoom(new IntVector2(0, 0), new IntVector2(map_width, map_height), new IntVector2[] { });

        //Add some rooms.
        for (int i = 0; i < 40; ++i)
        {
            IntVector2 room_center = new IntVector2(Random.Range(0, map_width), Random.Range(0, map_height));
            IntVector2 room_size = new IntVector2(Random.Range(4, map_width / 4), Random.Range(4, map_height / 4));

            IntVector2[] doorways = new IntVector2[]
            {
                new IntVector2(room_size.x / 2, 0),
                new IntVector2(0, room_size.y / 2),
                new IntVector2(room_size.x / 2, room_size.y - 1),
                new IntVector2(room_size.x - 1, room_size.y / 2),
            };

            placeRoom(room_center - (room_size / 2), room_size, doorways);
        }

		placeWalls ();

		AstarPath.active.Scan ();
	}

	private void setMapRegion (IntVector2 top_left, IntVector2 dimensions, bool value)
	{
		for (int x = top_left.x; x < top_left.x + dimensions.x; ++x) {
			for (int y = top_left.y; y < top_left.y + dimensions.y; ++y) {
				map_data [x, y] = value;
			}
		}
	}

    private void placeRoom (IntVector2 top_left, IntVector2 dimensions, IntVector2[] doorways)
    {
        for (int x = top_left.x; x < top_left.x + dimensions.x; ++x)
        {
            for (int y = top_left.y; y < top_left.y + dimensions.y; ++y)
            {
                //Ignore any out-of-bounds parts of the room.
                if (x < 0 || y < 0 || x >= map_width || y >= map_height)
                {
                    continue;
                }

                if (x == top_left.x || x == top_left.x + dimensions.x - 1 || y == top_left.y || y == top_left.y + dimensions.y - 1)
                {
                    //Border
                    map_data[x, y] = true;
                }
                else
                {
                    //Room
                    map_data[x, y] = false;
                }
            }
        }

        //Add doorways
        foreach (IntVector2 doorway in doorways)
        {
            //Ignore any out-of-bounds parts of the room.
            int x = top_left.x + doorway.x;
            int y = top_left.y + doorway.y;
            if (x < 0 || y < 0 || x >= map_width || y >= map_height)
            {
                continue;
            }

            map_data[x, y] = false;
        }
    }

    private void setAllMapData (bool value)
	{
		setMapRegion (new IntVector2 (0, 0), new IntVector2 (map_width, map_height), value);
	}

	private void randomiseAllMapData (float p_wall)
	{
		for (int x = 0; x < map_width; ++x) {
			for (int y = 0; y < map_height; ++y) {
				map_data [x, y] = Random.value < p_wall;
			}
		}
	}

	private void drunkardWalk (float emptyFraction)
	{
		setAllMapData (true);

		IntVector2 position = new IntVector2 ((int)Mathf.Floor (map_width / 2), (int)Mathf.Floor (map_height / 2));
		map_data [position.x, position.y] = false;

		int empty_tiles = 1;
		int total_tiles = map_height * map_width;

		IntVector2[] directions = {
            new IntVector2 (1, 0),
            new IntVector2 (0, 1),
            new IntVector2 (-1, 0),
            new IntVector2 (0, -1),
        };

		while (empty_tiles < emptyFraction * total_tiles) {
			position += directions [(int)Random.Range (0, 4)];

			position.x = Mathf.Clamp (position.x, 0, map_width - 1);
			position.y = Mathf.Clamp (position.y, 0, map_height - 1);

			if (map_data [position.x, position.y]) {
				map_data [position.x, position.y] = false;
				++empty_tiles;
			}
		}
	}

	private void CaveCA (int iterations)
	{
		randomiseAllMapData (0.5f);

		IntVector2[] neighbours = {
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

		for (int i = 0; i < iterations; ++i) {
			bool[,] old_map = (bool[,])map_data.Clone ();
			IntVector2 current_position = new IntVector2 (0, 0);

			for (current_position.x = 0; current_position.x < map_width; ++current_position.x) {
				for (current_position.y = 0; current_position.y < map_height; ++current_position.y) {
					int neighbouring_walls = 0;
					foreach (IntVector2 neighbour_vector in neighbours) {
						IntVector2 neighbour = current_position + neighbour_vector;

						if (neighbour.x < 0 || neighbour.x >= map_width || neighbour.y < 0 || neighbour.y >= map_height || old_map [neighbour.x, neighbour.y]) {
							++neighbouring_walls;
						}
					}

					map_data [current_position.x, current_position.y] = neighbouring_walls >= 5;
				}
			}
		}
	}

	private void placeWalls ()
	{
		for (int x = 0; x < map_width; ++x) {
			for (int y = 0; y < map_height; ++y) {
				if (map_data [x, y]) {
					Vector3 new_wall_pos = new Vector3 (TopLeftPosition.x + TileSize * (x + 0.5f), 0, TopLeftPosition.y + TileSize * (y + 0.5f));
					Quaternion new_wall_rot = Quaternion.Euler (new Vector3 (90, 0, 0));
					GameObject new_wall = Instantiate (Wall, new_wall_pos, new_wall_rot) as GameObject;
					new_wall.transform.parent = WallParent.transform;
				}
			}
		}
	}
}
