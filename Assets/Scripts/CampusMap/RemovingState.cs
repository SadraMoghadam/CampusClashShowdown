using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemovingState : IBuildingState
{
    private int gameObjectIndex = -1;
    Grid grid;
    PreviewSystem previewSystem;
    GridData buildingData;
    ObjectPlacer objectPlacer;
    ObjectsDatabaseSo database;

    public RemovingState(ObjectsDatabaseSo database, Grid grid, PreviewSystem previewSystem, GridData buildingData, ObjectPlacer objectPlacer)
    {
        this.database = database;
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.buildingData = buildingData;
        this.objectPlacer = objectPlacer;

        previewSystem.StartShowingRemovePreview();
    }

    public void EndState()
    {
        previewSystem.StopShowingPreview();
    }

    public void OnAction(Vector3Int gridPosition)
    {
        GridData selectedData = null;
        if (buildingData.CanPlaceObjectAt(gridPosition, Vector2Int.one, false) == false)
        {
            selectedData = buildingData;
        }
        

        if(selectedData != null)
        {
            gameObjectIndex = selectedData.GetRepresentationIndex(gridPosition);
            if(gameObjectIndex == -1)
            {
                return;
            }
            selectedData.RemoveObjectAt(gridPosition);
            objectPlacer.RemoveObjectAt(gameObjectIndex);
        }
        Vector3 cellPosition = grid.CellToWorld(gridPosition);
        previewSystem.UpdatePosition(cellPosition, CheckIfSelectionIsValid(gridPosition, false));
    }

    private bool CheckIfSelectionIsValid(Vector3Int gridPosition, Boolean building)
    {
        return !(buildingData.CanPlaceObjectAt(gridPosition, Vector2Int.one, building));
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        bool validity = CheckIfSelectionIsValid(gridPosition, false);
        previewSystem.UpdatePosition(grid.GetCellCenterWorld(gridPosition) - new Vector3(-0.1f, 0.4f, 0.1f), validity);
        /*if (database.objectsData[gameObjectIndex].Size.x == 1)
        {
            previewSystem.UpdatePosition(grid.GetCellCenterWorld(gridPosition) - new Vector3(0, 0.5f, 0), validity);
        }
        else
        {
            previewSystem.UpdatePosition(grid.GetCellCenterWorld(gridPosition) - new Vector3(-0.5f, 0.5f, 0), validity);
        }*/
    }
}
