using System;
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
    ObjectPlacer objectPlacer;

    public PlacementState(int iD, Grid grid, PreviewSystem previewSystem, ObjectsDatabaseSo database, GridData buildingData, ObjectPlacer objectPlacer)
    {
        ID = iD;
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.database = database;
  
        this.buildingData = buildingData;
        this.objectPlacer = objectPlacer;

        selectedObjectIndex = database.objectsData.FindIndex(data => data.ID == ID);
        if (selectedObjectIndex > -1 && (PlayerPrefsManager.GetInt(PlayerPrefsKeys.Resource, GameManager.InitialResources)) - 100 >= 0)
        {

            previewSystem.StartShowingPlacementPreview(database.objectsData[selectedObjectIndex].Prefab, database.objectsData[selectedObjectIndex].Size);
        }
        

    }

    public void EndState()
    {
        previewSystem.StopShowingPreview();
    }

    public void OnAction(Vector3Int gridPosition)
    {
        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex, true);
        bool resourcesQty = CheckResources(selectedObjectIndex);
        if (placementValidity == false || resourcesQty == false)
            return;

        int index = objectPlacer.PlaceObject(database, selectedObjectIndex, database.objectsData[selectedObjectIndex].Prefab, grid, gridPosition);



        GridData selectedData = buildingData;

        Vector2Int buildingSize = new Vector2Int(database.objectsData[selectedObjectIndex].Size.x, database.objectsData[selectedObjectIndex].Size.y);
        selectedData.AddObjectAt(gridPosition, buildingSize, database.objectsData[selectedObjectIndex].ID, index);
        PlayerPrefsManager.SaveBuildings(selectedData);
        
        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), false);
    }

    private bool CheckResources(int selectedObjectIndex)
    {
        if(PlayerPrefsManager.GetInt(PlayerPrefsKeys.Resource, GameManager.InitialResources) >= 0)
        {
            return true;
        }
        return false;
    }

    public bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex, bool building)
    {
        GridData selectedData = buildingData;
        return selectedData.CanPlaceObjectAt(gridPosition, database.objectsData[selectedObjectIndex].Size, building);
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex, true);

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
