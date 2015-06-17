using UnityEngine;
using System.Collections;

public class LevelGenerator : MonoBehaviour {

    public Vector2 TopLeftPosition;
    public Vector2 Dimensions;
    public float TileSize;
    public GameObject Wall;

    private bool[,] map_data;
    private int map_width;
    private int map_height;

	void Awake () 
    {
        map_width = (int) Mathf.Floor(Dimensions.x / TileSize);
        map_height = (int) Mathf.Floor(Dimensions.y / TileSize);
        map_data = new bool[map_width , map_height];

        DrunkardWalk(0.4f);

        placeWalls();
    }

    private void setAllMapData (bool value) 
    {
        for (int x = 0; x < map_width; ++x)
        {
            for (int y = 0; y < map_height; ++y)
            {
                map_data[x, y] = value;
            }
        }
    }

    private void DrunkardWalk(float emptyFraction)
    {
        setAllMapData(true);

        IntVector2 position = new IntVector2((int) Mathf.Floor(map_width / 2), (int) Mathf.Floor(map_height / 2));
        map_data [position.x, position.y] = false;

        int empty_tiles = 1;
        int total_tiles = map_height * map_width;

        IntVector2[] directions = {
            new IntVector2(1,0),
            new IntVector2(0,1),
            new IntVector2(-1,0),
            new IntVector2(0,-1),
        };

        while (empty_tiles < emptyFraction * total_tiles) {
            position += directions[(int) Random.Range(0, 4)];

            position.x = Mathf.Clamp(position.x, 0, map_width - 1);
            position.y = Mathf.Clamp(position.y, 0, map_height - 1);

            Debug.Log("Walked to pos " + position.x + ", " + position.y);

            if (map_data [position.x, position.y]) {
                map_data [position.x, position.y] = false;
                ++empty_tiles;
            }
        }
    }

    private void placeWalls()
    {
        for (int x = 0; x < map_width; ++x)
        {
            for (int y = 0; y < map_height; ++y)
            {
                if (map_data[x, y]) {
                    Instantiate(Wall, new Vector3(TopLeftPosition.x + TileSize * x, 0, TopLeftPosition.y + TileSize * y), Quaternion.Euler(new Vector3(90, 0, 0)));
                }
            }
        }
    }
}
