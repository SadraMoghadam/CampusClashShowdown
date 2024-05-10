using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
    [SerializeField] private List<GameObject> placedGameObject;
 


    public int PlaceObject(ObjectsDatabaseSo database, int selectedObjectIndex, GameObject prefab, Grid grid, Vector3Int gridPosition)
    {
        GameObject newObject = Instantiate(prefab);
        if (database.objectsData[selectedObjectIndex].Size.x == 1)
        {
            newObject.transform.position = grid.GetCellCenterWorld(gridPosition) - new Vector3(0, 0.5f, 0);
        }
        else
        {
            newObject.transform.position = grid.GetCellCenterWorld(gridPosition) - new Vector3(-0.5f, 0.5f, 0);
        }

        placedGameObject.Add(newObject);
        return placedGameObject.Count - 1;
    }

    internal void RemoveObjectAt(int gameObjectIndex)
    {
        if(placedGameObject.Count <= gameObjectIndex || placedGameObject[gameObjectIndex] == null)
        {
            return;
        }
        Destroy(placedGameObject[gameObjectIndex]);
        placedGameObject[gameObjectIndex] = null;
    }
}
