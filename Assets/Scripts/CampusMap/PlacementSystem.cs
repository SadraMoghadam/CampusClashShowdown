using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField] CampusUI campusUI;
    [SerializeField] InputManager inputManager;
    [SerializeField] private ObjectsDatabaseSo database;
    [SerializeField] private GameObject gridVisualization;
    [SerializeField] Grid grid;
    private GridData buildingsData;
    [SerializeField] private PreviewSystem preview;
    [SerializeField] private ObjectPlacer objectPlacer;
    private Vector3Int lastDetectedPosition = Vector3Int.zero;
    IBuildingState buildingState;
    private int _currentID;

    private void Update()
    {
        if (buildingState == null)
        {
            return;
        }
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        if (lastDetectedPosition != gridPosition)
        {
            buildingState.UpdateState(gridPosition);
            lastDetectedPosition = gridPosition;
        }
    }

    private void Start()
    {
        StopPlacement();
        buildingsData = PlayerPrefsManager.LoadBuildings();
        Vector3Int prec = new Vector3Int();
        foreach (var i in buildingsData.getPlacedObjects())
        {
            
            if (i.Key != new Vector3Int(prec.x + 1, prec.y, prec.z))
            {
                objectPlacer.PlaceObject(database, i.Value.ID, database.objectsData[i.Value.ID].Prefab, grid, i.Key);
            }
            prec = i.Key;
        }
    }

    public void StartPlacement(int ID)
    {
        StopPlacement();
        if (PlayerPrefsManager.GetInt(PlayerPrefsKeys.Resource, GameManager.InitialResources) - database.objectsData[ID].Price >= 0)
        {
            gridVisualization.SetActive(true);
        }
        else
        {
            return;
        }

        buildingState = new PlacementState(ID, grid, preview, database, PlayerPrefsManager.LoadBuildings(), objectPlacer);

        _currentID = ID;
        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }

    public void StartRemoving()
    {
        StopPlacement();
        gridVisualization.SetActive(true);
        buildingState = new RemovingState(database, grid, preview, PlayerPrefsManager.LoadBuildings(), objectPlacer, campusUI);

        inputManager.OnClicked += RemoveStructure;
        inputManager.OnExit += StopPlacement;
    }

    private void PlaceStructure()
    {
        GameManager.Instance.AudioManager.Instantplay(SoundName.BuildingPlacement, Vector3.zero);
        if (inputManager.IsPointerOverUI())
        {
            return;
        }
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        if (buildingState.CanPlace(gridPosition))
        {
            campusUI.updateResources(database.objectsData[_currentID].Price, _currentID, true);   
        }
        buildingState.OnAction(gridPosition);
        buildingsData = PlayerPrefsManager.LoadBuildings();

        if (!PlayerPrefsManager.GetBool(PlayerPrefsKeys.FirstBuildingPlaced, false))
        {
            CampusController.Instance.DialogueController.Show(6);
            PlayerPrefsManager.SetBool(PlayerPrefsKeys.FirstBuildingPlaced, true);
        }

        StopPlacement();
    }

    private void RemoveStructure()
    {
        GameManager.Instance.AudioManager.Instantplay(SoundName.BuildingRemove, Vector3.zero);
        if (inputManager.IsPointerOverUI())
        {
            return;
        }
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        buildingState.OnAction(gridPosition);
        buildingsData = PlayerPrefsManager.LoadBuildings();

        StopPlacement();
    }

    private void StopPlacement()
    {
        if (buildingState == null)
            return;
        buildingsData = PlayerPrefsManager.LoadBuildings();
        gridVisualization.SetActive(false);
        buildingState.EndState();
        inputManager.OnClicked -= PlaceStructure;
        inputManager.OnClicked -= RemoveStructure; // Assicurati di rimuovere anche questo handler
        inputManager.OnExit -= StopPlacement;
        lastDetectedPosition = Vector3Int.zero;
        buildingState = null;
    }

}
