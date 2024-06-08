using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{

    [SerializeField] CampusUI campusUI;

    [SerializeField] InputManager inputManager;

    [SerializeField]
    private ObjectsDatabaseSo database;

    [SerializeField]
    private GameObject gridVisualization;

    [SerializeField] Grid grid;

    private GridData buildingsData;

    [SerializeField] private PreviewSystem preview;

    [SerializeField] private ObjectPlacer objectPlacer;

    private Vector3Int lastDetectedPosition = Vector3Int.zero;

    IBuildingState buildingState;

    private void Update()
    {
        if (buildingState == null)
        {
            return;
        }
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);


        if(lastDetectedPosition != gridPosition)
        {
            buildingState.UpdateState(gridPosition);
            
            
            lastDetectedPosition = gridPosition; 
        }

        
    }

    private void Start()
    {
        StopPlacement();
        buildingsData = PlayerPrefsManager.LoadBuildings();
        Debug.Log(buildingsData);
        foreach (var i in buildingsData.getPlacedObjects())
        {
            objectPlacer.PlaceObject(database, i.Value.ID, database.objectsData[i.Value.ID].Prefab, grid, i.Key);
        }



    }

    public void StartPlacement(int ID)
    {
        StopPlacement();
        if (PlayerPrefsManager.GetInt(PlayerPrefsKeys.Resource, 200) - 100 >= 0)
        {
            gridVisualization.SetActive(true);
        }

        buildingState = new PlacementState(ID, grid, preview, database, buildingsData, objectPlacer);
        campusUI.updateResources(100, ID, true);

        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }

    public void StartRemoving()
    {
        StopPlacement();
        gridVisualization.SetActive(true);
        buildingState = new RemovingState(database, grid, preview, buildingsData, objectPlacer, campusUI);
        
        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
       
    } 

    private void PlaceStructure()
    {
        if(inputManager.IsPointerOverUI())
        {
            return;
        }
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        buildingState.OnAction(gridPosition);
        
        StopPlacement();

    }

    

    private void StopPlacement()
    {
        if (buildingState == null)
            return;
        gridVisualization.SetActive(false);
        buildingState.EndState();
        inputManager.OnClicked -= PlaceStructure;
        inputManager.OnExit -= StopPlacement;
        lastDetectedPosition = Vector3Int.zero;
        buildingState = null;


    }
}
