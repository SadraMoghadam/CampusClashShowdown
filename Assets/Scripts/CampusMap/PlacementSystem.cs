using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField] GameObject mouseIndicator, cellIndicator;

    [SerializeField] InputManager inputManager;

    [SerializeField]
    private ObjectsDatabaseSo database;
    private int selectedObjectIndex = -1;

    [SerializeField]
    private GameObject gridVisualization;

    [SerializeField] Grid grid;

    private GridData floorData, buildingsData;

    private Renderer previewRenderer;

    private List<GameObject> placedGameObject = new();

    private void Update()
    {
        if(selectedObjectIndex<0)
        {
            return;
        }
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
        previewRenderer.material.color = placementValidity ? Color.green : Color.red;


        mouseIndicator.transform.position = mousePosition;
        cellIndicator.transform.position = grid.CellToWorld(gridPosition);
    }

    private void Start()
    {
        StopPlacement();
        floorData = new();
        buildingsData = new();
        previewRenderer = cellIndicator.GetComponentInChildren<Renderer>();
        
    }

    public void StartPlacement(int ID)
    {
        StopPlacement();
        selectedObjectIndex = database.objectsData.FindIndex(data => data.ID == ID);
        if (selectedObjectIndex < 0)
        {
            Debug.LogError($"No ID found {ID}");
            return;
        }
        gridVisualization.SetActive(true);
        cellIndicator.SetActive(true);
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

        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
        if (placementValidity == false)
            return; 

        GameObject newObject = Instantiate(database.objectsData[selectedObjectIndex].Prefab);
        if(database.objectsData[selectedObjectIndex].Size.x == 1)
        {
            newObject.transform.position = grid.GetCellCenterWorld(gridPosition) - new Vector3(0, 0.5f, 0);
        }
        else
        {
            newObject.transform.position = grid.GetCellCenterWorld(gridPosition) - new Vector3(-0.5f, 0.5f, 0);
        }
        
        

        placedGameObject.Add(newObject);

        GridData selectedData = database.objectsData[selectedObjectIndex].ID == 0 ? floorData : buildingsData;
        selectedData.AddObjectAt(gridPosition, database.objectsData[selectedObjectIndex].Size, database.objectsData[selectedObjectIndex].ID, placedGameObject.Count - 1);

    }

    private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex)
    {
        GridData selectedData = database.objectsData[selectedObjectIndex].ID == 0 ? floorData : buildingsData;
        return selectedData.CanPlaceObjectAt(gridPosition, database.objectsData[selectedObjectIndex].Size);
    }

    private void StopPlacement()
    {

        selectedObjectIndex = -1;
        gridVisualization.SetActive(false);
        cellIndicator.SetActive(false);
        inputManager.OnClicked -= PlaceStructure;
        inputManager.OnExit -= StopPlacement;


    }
}
