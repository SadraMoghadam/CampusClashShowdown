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
        int prec = -1;
        foreach (var i in buildingsData.getPlacedObjects())
        {
            if (prec != i.Value.ID)
            {
                objectPlacer.PlaceObject(database, i.Value.ID, database.objectsData[i.Value.ID].Prefab, grid, i.Key);
            }
            prec = i.Value.ID;
        }
    }

    public void StartPlacement(int ID)
    {
        StopPlacement();
        if (PlayerPrefsManager.GetInt(PlayerPrefsKeys.Resource, 200) - 100 >= 0)
        {
            gridVisualization.SetActive(true);
        }

        buildingState = new PlacementState(ID, grid, preview, database, PlayerPrefsManager.LoadBuildings(), objectPlacer);
        campusUI.updateResources(100, ID, true);

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

        buildingState.OnAction(gridPosition);
        buildingsData = PlayerPrefsManager.LoadBuildings();

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
