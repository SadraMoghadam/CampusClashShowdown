using System.Collections.Generic;
using UnityEngine;

public class RemovingState : IBuildingState
{
    private int gameObjectIndex = -1;
    private Grid grid;
    private PreviewSystem previewSystem;
    private GridData buildingData;
    private ObjectPlacer objectPlacer;
    private ObjectsDatabaseSo database;
    private CampusUI campusUI;

    public RemovingState(ObjectsDatabaseSo database, Grid grid, PreviewSystem previewSystem, GridData buildingData, ObjectPlacer objectPlacer, CampusUI campusUI)
    {
        this.database = database;
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.buildingData = buildingData;
        this.objectPlacer = objectPlacer;
        this.campusUI = campusUI;

        previewSystem.StartShowingRemovePreview();
    }

    public void EndState()
    {
        previewSystem.StopShowingPreview();
    }

    public void OnAction(Vector3Int gridPosition)
    {
        Debug.Log($"Attempting to remove object at {gridPosition}");

        if (!buildingData.CanPlaceObjectAt(gridPosition, Vector2Int.one, false))
        {
            gameObjectIndex = buildingData.GetRepresentationIndex(gridPosition);
            if (gameObjectIndex == -1)
            {
                Debug.Log($"No valid object found at {gridPosition} to remove");
                return;
            }
            
            PlacementData data = buildingData.RemoveObjectAt(gridPosition);
            objectPlacer.RemoveObjectAt(gridPosition);
            
            campusUI.updateResources(database.objectsData[data.ID].Price, gameObjectIndex, false);

            // campusUI.updateResources(100, gameObjectIndex, false);
            PlayerPrefsManager.SaveBuildings(buildingData);

            Debug.Log($"Object at {gridPosition} removed successfully");
        }
        else
        {
            Debug.Log($"Cannot remove object at {gridPosition} as it's valid for placement");
        }

        Vector3 cellPosition = grid.CellToWorld(gridPosition);
        previewSystem.UpdatePosition(cellPosition, CheckIfSelectionIsValid(gridPosition, false));
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        bool validity = CheckIfSelectionIsValid(gridPosition, false);
        previewSystem.UpdatePosition(grid.GetCellCenterWorld(gridPosition) - new Vector3(-0.1f, 0.4f, 0.1f), validity);
    }

    public bool CanPlace(Vector3Int gridPosition)
    {
        return true;
    }

    private bool CheckIfSelectionIsValid(Vector3Int gridPosition, bool building)
    {
        return !buildingData.CanPlaceObjectAt(gridPosition, Vector2Int.one, building);
    }
}
