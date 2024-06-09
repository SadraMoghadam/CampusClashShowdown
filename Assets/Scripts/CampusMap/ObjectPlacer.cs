using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
    [SerializeField] private List<GameObject> placedGameObject;
    private Dictionary<Vector3Int, int> gridPositionToObjectIndexMap;

    private void Start()
    {
        placedGameObject = new List<GameObject>();
        gridPositionToObjectIndexMap = new Dictionary<Vector3Int, int>();
    }

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
        int objectIndex = placedGameObject.Count - 1;
        gridPositionToObjectIndexMap[gridPosition] = objectIndex;

        Debug.Log($"Placed object at {gridPosition} with index {objectIndex}");

        return objectIndex;
    }

    public void RemoveObjectAt(Vector3Int gridPosition)
    {
        if (!gridPositionToObjectIndexMap.ContainsKey(gridPosition))
        {
            if (gridPositionToObjectIndexMap.ContainsKey(new Vector3Int(gridPosition.x - 1, gridPosition.y, gridPosition.z)))
            {
                RemoveObjectAt(new Vector3Int(gridPosition.x - 1, gridPosition.y, gridPosition.z));
                return;
            }
            else
            {
                Debug.Log($"No object at {gridPosition} to remove");
                return;
            }
        }

        int gameObjectIndex = gridPositionToObjectIndexMap[gridPosition];
        if (placedGameObject.Count <= gameObjectIndex || placedGameObject[gameObjectIndex] == null)
        {
            Debug.Log($"Object at index {gameObjectIndex} is already null");
            return;
        }

        Destroy(placedGameObject[gameObjectIndex]);
        placedGameObject[gameObjectIndex] = null;
        gridPositionToObjectIndexMap.Remove(gridPosition);

        Debug.Log($"Removed object at {gridPosition} with index {gameObjectIndex}");
    }

    internal void RemoveObjectAt(int gameObjectIndex)
    {
        if (placedGameObject.Count <= gameObjectIndex || placedGameObject[gameObjectIndex] == null)
        {
            Debug.Log($"Object at index {gameObjectIndex} is already null or out of bounds");
            return;
        }

        Destroy(placedGameObject[gameObjectIndex]);
        placedGameObject[gameObjectIndex] = null;

        // Rimuovi la posizione della griglia dalla mappa
        foreach (var kvp in gridPositionToObjectIndexMap)
        {
            if (kvp.Value == gameObjectIndex)
            {
                gridPositionToObjectIndexMap.Remove(kvp.Key);
                Debug.Log($"Removed mapping for object at index {gameObjectIndex} from grid position {kvp.Key}");
                break;
            }
        }

        Debug.Log($"Removed object at index {gameObjectIndex}");
    }
}
