using UnityEngine;
using System.Collections;

public class LevelGenerator : MonoBehaviour
{

	public Vector2 TopLeftPosition;
	public Vector2 Dimensions;
	public float TileSize;
	public GameObject Wall;

	private bool[,] map_data;
	private int map_width;
	private int map_height;

	void Awake ()
	{
		map_width = (int)Mathf.Floor (Dimensions.x / TileSize);
		map_height = (int)Mathf.Floor (Dimensions.y / TileSize);
		map_data = new bool[map_width, map_height];

		CaveCA (3);

		//Clear a space in the middle of the map for the player to spawn in.
		setMapRegion (new IntVector2 ((int)((map_width / 2) - 1), (int)((map_height / 2) - 1)), new IntVector2 (3, 3), false);

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
					Instantiate (Wall, new Vector3 (TopLeftPosition.x + TileSize * x, 0, TopLeftPosition.y + TileSize * y), Quaternion.Euler (new Vector3 (90, 0, 0)));
				}
			}
		}
	}
}
