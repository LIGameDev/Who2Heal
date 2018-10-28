using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Tile3D
{
    public GameObject gameObject;

    public int prefabId; 
    public int row;
    public int col; 

    public Tile3D(int prefabId, int row, int col)
    {
        this.prefabId = prefabId;
        this.row = row;
        this.col = col;

        gameObject = null; 
    }
}

public class Tilemap3D : MonoBehaviour {

    public List<GameObject> tilePrefabs;

    public Vector3 mapOffset; 
    public float tileWidth = 1f;
    public Vector3 tileRotation; 

    Tile3D[,] tiles;

	void Start () {
        // Default: 10x10
        InitMap(10, 10);
        //FillArea(5, 0, 0, 1, 1); 
        FillArea(5, 1, 5, 4, 8); 
        GenerateTileObjects(); 
	}
	
	void Update () {
		
	}

    public void InitMap(int rows, int cols)
    {
        if (rows <= 0 || cols <= 0)
            return;

        tiles = new Tile3D[rows, cols];
        for (int i = 0; i < tiles.GetLength(0); i++)
        {
            for (int j = 0; j < tiles.GetLength(1); j++)
            {
                tiles[i, j] = new Tile3D(0, i, j); 
            }
        }
    }

    public void GenerateTileObjects()
    {
        if (tilePrefabs.Count == 0)
        {
            Debug.LogWarning("[Tilemap3D] No tile prefabs assigned; cannot generate tile objects.");
            return; 
        }

        for (int i = 0; i < tiles.GetLength(0); i++)
        {
            for (int j = 0; j < tiles.GetLength(1); j++)
            {
                Tile3D currentTile = tiles[i, j];

                if (currentTile.prefabId >= 0 && currentTile.prefabId < tilePrefabs.Count)
                {
                    Vector3 tilePos = GetTilePositionInWorldSpace(currentTile);
                    Quaternion tileRot = Quaternion.Euler(tileRotation);
                    //tileRot = tileRot * Quaternion.Euler(0f, 180f, 0f); //very temp until rotation properly implemented
                    currentTile.gameObject = Instantiate(tilePrefabs[currentTile.prefabId], tilePos, tileRot, transform); //tiles[i, j]
                }
            }
        }
    }

    public void ClearTileObjects()
    {
        // TODO
    }

    public void FillArea(int tileId, int corner1x, int corner1y, int corner2x, int corner2y)
    {
        //if (!IsCoordinateInBounds(corner1x, corner1y) || !IsCoordinateInBounds(corner2x, corner2y))
        //    return;

        for (int i = corner1x; i < corner2x; i++)
        {
            for (int j = corner1y; j < corner2y; j++)
            {
                SetTile(i, j, tileId); 
            }
        }
    }

    public bool IsCoordinateInBounds(int row, int col)
    {
        return row >= 0 && col >= 0 && row < tiles.GetLength(0) && col < tiles.GetLength(1); 
    }

    public Vector3 GetTilePositionInWorldSpace(Tile3D tile)
    {
        Vector3 pos = new Vector3(mapOffset.x + (tile.row * tileWidth), mapOffset.y, mapOffset.z + (tile.col * tileWidth));
        return pos; 
    }

    public void SetTile(int row, int col, int newId)
    {
        if (newId < 0 || newId >= tilePrefabs.Count)
            return;

        if (!IsCoordinateInBounds(row, col))
            return;

        //Debug.Log(string.Format("[Tilemap3D] Setting tile ({0}, {1}) to {2}", row, col, newId)); 
        tiles[row, col].prefabId = newId; 
    }
}
