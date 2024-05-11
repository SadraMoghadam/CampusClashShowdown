using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//where we store the data about the buildings that are already placed
//TODO: insert the data of the positions of the streets when we have them
public class GridData
{
    Dictionary<Vector3Int, PlacementData> placedObjects = new();

    public GridData()
    {
        
    }


    public void AddObjectAt(Vector3Int gridPosition, Vector2Int objectSize, int ID, int placedObjectIndex)
    {

        List<Vector3Int> positionToOccupy = CalculatePositions(gridPosition, objectSize);
        PlacementData data = new PlacementData(positionToOccupy, ID, placedObjectIndex);
        foreach(var pos in positionToOccupy)
        {
            if(placedObjects.ContainsKey(pos))
            {
                throw new Exception($"Dictionary already contains this cell position {pos}");

            }
            placedObjects[pos] = data;
        }

    }

    private List<Vector3Int> CalculatePositions(Vector3Int gridPosition, Vector2Int objectSize)
    {
        List<Vector3Int> positions = new List<Vector3Int>();

        int padding = 0;  // Una cella di distanza da ogni lato

        // Espandi la griglia per includere il padding
        for (int x = -padding; x < objectSize.x + padding; x++)
        {
            for (int y = -padding; y < objectSize.y + padding; y++)
            {
                Vector3Int position = new Vector3Int(gridPosition.x + x, gridPosition.y + y, gridPosition.z);
                positions.Add(position);
            }
        }

        return positions;
    }



    internal int GetRepresentationIndex(Vector3Int gridPosition)
    {
        if(placedObjects.ContainsKey(gridPosition) == false)
        {
            return -1;
        }
        return placedObjects[gridPosition].PlacedObjectIndex;

    }

    internal void RemoveObjectAt(Vector3Int gridPosition)
    {
        foreach (var pos in placedObjects[gridPosition].occupiedPositions)
        {
            placedObjects.Remove(pos);
        }
    }

    public bool CanPlaceObjectAt(Vector3Int gridPosition, Vector2Int objectSize)
    {
        List<Vector3Int> positionToOccupy = CalculatePositions(gridPosition, objectSize);
        int[] index = { -1, -1, 0, -1, +1, 0, 0, +1, +1, -1 };
        foreach (var pos in positionToOccupy)
        {
            for(int i = 0; i < index.Length - 1; i++)
            {
                Vector3Int position = new Vector3Int(pos.x + index[i], pos.y + index[i + 1]);
                if (placedObjects.ContainsKey(position))
                {
                    return false;
                }
            }
           
        }
        return true;
    }
}
public class PlacementData
{
    public List<Vector3Int> occupiedPositions;

    public int ID { get; private set; }

    public int PlacedObjectIndex { get; private set; }

    public PlacementData(List<Vector3Int> occupiedPositions, int iD, int placedObjectIndex)
    {
        this.occupiedPositions = occupiedPositions;
        ID = iD;
        PlacedObjectIndex = placedObjectIndex;
    }
}
