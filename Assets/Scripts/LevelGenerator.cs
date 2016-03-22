using UnityEngine;
using DragTank.MapGenerator;

public class LevelGenerator : MonoBehaviour
{
	public Rect Dimensions;
	public float TileSize;
	public GameObject Wall;
	public GameObject WallParent;

	private Map map;
	

	void Awake ()
	{
        map = new Map(
            Mathf.FloorToInt(Dimensions.width / TileSize), 
            Mathf.FloorToInt(Dimensions.height / TileSize), 
            false
        );

        //Add some rooms.
        for (int i = 0; i < 20; ++i)
        {
            Room room = Room.GenerateForMap(map, 5, map.Width / 4);
            room.writeToMap(map);
        }

		buildMap ();

		AstarPath.active.Scan ();
	}

    /// <summary>
    /// Turns the data in the map_data array into actual world objects.
    /// </summary>
	private void buildMap ()
	{
		for (int x = 0; x < map.Width; ++x) {
			for (int y = 0; y < map.Height; ++y) {
				if (map.GetTile(x, y)) {
					Vector3 new_wall_pos = new Vector3 (Dimensions.x + TileSize * (x + 0.5f), 0, Dimensions.y + TileSize * (y + 0.5f));
					GameObject new_wall = Instantiate (Wall, new_wall_pos, Quaternion.identity) as GameObject;
					new_wall.transform.parent = WallParent.transform;
				}
			}
		}
	}
}
