using UnityEngine;
using DragTank.MapGenerator;
using System;
using System.Collections.Generic;

public class LevelGenerator : MonoBehaviour
{
    [Serializable]
    public struct TilePrefabMapping
    {
        public Map.Tile TileType;
        public GameObject Prefab;
    }

    public TilePrefabMapping[] TilePrefabs;
    public float TileSize;

    private Map m_map;
    private EnvironmentController m_environment;
    private Dictionary<Map.Tile, GameObject> m_tilePrefabs;

    void Start ()
	{
        //Build a dictionary from the TilePrefabs property set in the editor.
        m_tilePrefabs = new Dictionary<Map.Tile, GameObject>(TilePrefabs.Length);
        foreach (TilePrefabMapping mapping in TilePrefabs)
        {
            m_tilePrefabs.Add(mapping.TileType, mapping.Prefab);
        }

        //Find the environment parent object in the scene.
        m_environment = FindObjectOfType<EnvironmentController>();

        //Initialise an empty map.
        m_map = new Map(
            Mathf.FloorToInt(m_environment.Bounds.width / TileSize), 
            Mathf.FloorToInt(m_environment.Bounds.height / TileSize), 
            Map.Tile.Empty
        );

        //Add some rooms.
        for (int i = 0; i < 10; ++i)
        {
            Room room = Room.GenerateForMap(m_map, 5, m_map.Width / 4);
            room.writeToMap(m_map);
        }

        //Add an exit, and some objectives.
        m_map.SetTile(m_map.GetRandomEmptyLocation(), Map.Tile.Exit);

        int num_objectives = m_tilePrefabs[Map.Tile.Exit].GetComponent<ExitController>().ObjectivesRequired;
        for (int i = 0; i < num_objectives; ++i)
        {
            m_map.SetTile(m_map.GetRandomEmptyLocation(), Map.Tile.Objective);
        }

        //Instantiate world objects.
		writeMapToWorld ();

        //Rebuild pathfinding grid.
		AstarPath.active.Scan ();
	}

    /// <summary>
    /// Turns the data in the map_data array into actual world objects.
    /// </summary>
	private void writeMapToWorld ()
	{
		for (int x = 0; x < m_map.Width; ++x) {
			for (int y = 0; y < m_map.Height; ++y) {
                Map.Tile tile = m_map.GetTile(x, y);
                if (m_tilePrefabs.ContainsKey(tile))
                {
                    Vector3 new_tile_pos = new Vector3(
                        m_environment.Bounds.x + TileSize * (x + 0.5f), 
                        0, 
                        m_environment.Bounds.y + TileSize * (y + 0.5f)
                    );
                    GameObject new_tile = Instantiate(m_tilePrefabs[tile], new_tile_pos, m_tilePrefabs[tile].transform.rotation) as GameObject;
                    new_tile.transform.parent = m_environment.transform;
                }
			}
		}
	}
}
