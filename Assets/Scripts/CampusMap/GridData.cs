using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

//where we store the data about the buildings that are already placed
//TODO: insert the data of the positions of the streets when we have them
public class GridData
{
    [SerializeField] Dictionary<Vector3Int, PlacementData> placedObjects = new();
    List<Vector3Int> roadPositions = new();
    public GridData()
    {
       
    }

   public int length()
    {
        return placedObjects.Count;
    }


    public Dictionary<Vector3Int, PlacementData> getPlacedObjects()
    {
        return placedObjects;
    }

    public void setPlacedObjects(Dictionary<Vector3Int, PlacementData> input)
    {
        placedObjects = input;
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
        Debug.Log(gridPosition);
        foreach (var pos in placedObjects[gridPosition].occupiedPositions)
        {
            placedObjects.Remove(pos);
        }
        
    }

    public bool CanPlaceObjectAt(Vector3Int gridPosition, Vector2Int objectSize, Boolean building)
    {
        if (roadPositions.Count == 0)
        {
            InstantiateRoadPositions();
        }
        if(building)
        {
            List<Vector3Int> positionToOccupy = CalculatePositions(gridPosition, objectSize);
            int[] index = { -1, -1, 0, -1, +1, 0, 0, +1, +1, -1 };
            foreach (var pos in positionToOccupy)
            {
                for (int i = 0; i < index.Length - 1; i++)
                {
                    Vector3Int position = new Vector3Int(pos.x + index[i], pos.y + index[i + 1]);
                    if (placedObjects.ContainsKey(position))
                    {
                        return false;
                    }
                }
                if (roadPositions.Contains(pos))
                {

                    return false;
                }

            }
        }
        else
        {
            List<Vector3Int> positionToOccupy = CalculatePositions(gridPosition, objectSize);
            if (roadPositions.Contains(positionToOccupy[0]))
            {

                return true;
            }
            if (placedObjects.ContainsKey(positionToOccupy[0]))
            {
                return false;
            }
        }
        
        
        return true;
    }

    private void InstantiateRoadPositions()
    {

        for (int i = -3; i < 5; i++)
        {
            roadPositions.Add(new Vector3Int(i, 3, 0));
        }
        for (int i = -8; i < 6; i++)
        {
            roadPositions.Add(new Vector3Int(i, 11, 0));
        }
        for (int i = -12; i < 10; i++)
        {
            roadPositions.Add(new Vector3Int(i, -7, 0));
        }
        for (int i = -12; i < 10; i++)
        {
            roadPositions.Add(new Vector3Int(i, -9, 0));
        }
        for (int i = -6; i < -3; i++)
        {
            roadPositions.Add(new Vector3Int(i, -11, 0));
        }
        for (int i = 6; i < 9; i++)
        {
            roadPositions.Add(new Vector3Int(i, -3, 0));
        }
        for (int i = -12; i < -7; i++)
        {
            roadPositions.Add(new Vector3Int(i, 8, 0));
        }
        for (int i = -12; i < -7; i++)
        {
            roadPositions.Add(new Vector3Int(i, 4, 0));
        }
        for (int i = -10; i < -4; i++)
        {
            roadPositions.Add(new Vector3Int(i, 1, 0));
        }
        for (int i = -12; i < -9; i++)
        {
            roadPositions.Add(new Vector3Int(i, -1, 0));
        }

        for (int i = -11; i<11; i++)
        {
            roadPositions.Add(new Vector3Int(-4, i, 0));
        }
        for (int i = -11; i < 11; i++)
        {
            roadPositions.Add(new Vector3Int(5, i, 0));
        }
        for (int i = 0; i < 8; i++)
        {
            roadPositions.Add(new Vector3Int(-12, i, 0));
        }
        for (int i = 9; i < 11; i++)
        {
            roadPositions.Add(new Vector3Int(-8, i, 0));
        }
        for (int i = 2; i < 4; i++)
        {
            roadPositions.Add(new Vector3Int(-8, i, 0));
        }
        for (int i = -10; i < -5; i++)
        {
            roadPositions.Add(new Vector3Int(4, i, 0));
        }
        for (int i = -10; i < -5; i++)
        {
            roadPositions.Add(new Vector3Int(6, i, 0));
        }
        for (int i = -9; i < 1; i++)
        {
            roadPositions.Add(new Vector3Int(-10, i, 0));
        }
        

       

        roadPositions.Add(new Vector3Int(-8, -8, 0));
        roadPositions.Add(new Vector3Int(-6, -8, 0));
        roadPositions.Add(new Vector3Int(-2, -8, 0));
        roadPositions.Add(new Vector3Int(1, -8, 0));
        roadPositions.Add(new Vector3Int(-6, -10, 0));
        roadPositions.Add(new Vector3Int(-2, -10, 0));

    }
}
public class PlacementData
{
    [SerializeField] public List<Vector3Int> occupiedPositions;

    [SerializeField] public int ID { get; private set; }

    [SerializeField] public int PlacedObjectIndex { get; private set; }

    public PlacementData(List<Vector3Int> occupiedPositions, int iD, int placedObjectIndex)
    {
        this.occupiedPositions = occupiedPositions;
        ID = iD;
        PlacedObjectIndex = placedObjectIndex;
    }
}
