using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementState : IBuildingState
{
    private int selectedObjectIndex = -1;
    int ID;
    Grid grid;
    PreviewSystem previewSystem;
    ObjectsDatabaseSo database;
    GridData buildingData;
    GridData floorData;
    ObjectPlacer objectPlacer;

    public PlacementState(int iD, Grid grid, PreviewSystem previewSystem, ObjectsDatabaseSo database, GridData floorData ,GridData buildingData, ObjectPlacer objectPlacer)
    {
        ID = iD;
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.database = database;
        this.floorData = floorData;
        this.buildingData = buildingData;
        this.objectPlacer = objectPlacer;

        selectedObjectIndex = database.objectsData.FindIndex(data => data.ID == ID);
        if (selectedObjectIndex > -1)
        {

            previewSystem.StartShowingPlacementPreview(database.objectsData[selectedObjectIndex].Prefab, database.objectsData[selectedObjectIndex].Size);
        }
        else
        {
            throw new System.Exception($"No objects with ID {iD}");
        }

    }

    public void EndState()
    {
        previewSystem.StopShowingPreview();
    }

    public void OnAction(Vector3Int gridPosition)
    {
        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
        if (placementValidity == false)
            return;

        int index = objectPlacer.PlaceObject(database, selectedObjectIndex, database.objectsData[selectedObjectIndex].Prefab, grid, gridPosition);



        GridData selectedData = buildingData;

        Vector2Int buildingSize = new Vector2Int(database.objectsData[selectedObjectIndex].Size.x + (int)1.5, database.objectsData[selectedObjectIndex].Size.y + (int)1.5);
        selectedData.AddObjectAt(gridPosition, buildingSize, database.objectsData[selectedObjectIndex].ID, index);
        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), false);
    }

    public bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex)
    {
        GridData selectedData = buildingData;
        return selectedData.CanPlaceObjectAt(gridPosition, database.objectsData[selectedObjectIndex].Size);
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);

        if (database.objectsData[selectedObjectIndex].Size.x == 1)
        {
            previewSystem.UpdatePosition(grid.GetCellCenterWorld(gridPosition) - new Vector3(0, 0.5f, 0), placementValidity);
        }
        else
        {
            previewSystem.UpdatePosition(grid.GetCellCenterWorld(gridPosition) - new Vector3(-0.6f, 0.5f, 0), placementValidity);
        }
    }
}
