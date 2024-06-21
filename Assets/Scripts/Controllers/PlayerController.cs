using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour, IParent<PickableObject>
{
    [SerializeField] private float rotateSpeed = 360;
    public ulong playerId;
    public float speed = 2;
    public float runSpeed = 3;
    public float strength = 1;
    public KeyCode interactKey = KeyCode.F;
    public KeyCode pushKey = KeyCode.J;
    public KeyCode pullKey = KeyCode.K;
    [SerializeField] private Transform holdingPoint;
    [SerializeField] private GameObject playerArrow;
    [SerializeField] private GameObject playerTeamIndicator;
    [SerializeField] private PlayerVisual playerVisual;
    [SerializeField] private ParticleSystem footprintVfx;
    private Vector3 _input;
    private Rigidbody _rb;
    private Animator _animator;
    private static readonly int Speed = Animator.StringToHash("Speed");
    private ClashArenaController _clashArenaController;
    private GameManager _gameManager;
    private PickableObject _pickableObject;
    private string _teamName;
    private Color _teamColor;
    private int _teamId;
    private TeamCharacteristicsScriptableObject _teamCharacteristics;
    private bool _canMove;
    private float _currentEmissionRate = 0;
    private float stamina = 100;
    private float maxStamina = 100;
    private float staminaRegenRate = 20;
    private float staminaDepletionRate = 30;
    private bool isRunning = false;



    private static PlayerController _instance;
    public static PlayerController Instance => _instance;


    private void Start()
    {
        _canMove = false;
        PlayerData playerData = MultiplayerController.Instance.GetPlayerDataFromClientId(OwnerClientId);
        playerVisual.SetPlayerMesh(MultiplayerController.Instance.GetPlayerHeadMesh(playerData.headMeshId),
            MultiplayerController.Instance.GetPlayerBodyMesh(playerData.bodyMeshId));
        ClashArenaController.Instance.OnStateChanged += ClashArenaController_OnStateChanged;
        ClashSceneUI.Instance.SetStaminaSliderValue(stamina);
    }

    private void ClashArenaController_OnStateChanged(object sender, System.EventArgs e)
    {
        if (ClashArenaController.Instance.IsGamePlaying())
        {
            _canMove = true;
        }
        else
        {
            _canMove = false;
        }
    }

    public override void OnNetworkSpawn()
    {
        SetTeam();
        if (!IsOwner) return;
        
        base.OnNetworkSpawn();
        if (_instance == null)
        {
            _instance = this;
        }
        _clashArenaController = ClashArenaController.Instance;
        _rb = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        AssignPlayerId(GetComponent<NetworkObject>().OwnerClientId);
        playerArrow.SetActive(true);
        transform.position = _clashArenaController.spawnLocations[MultiplayerController.Instance.GetPlayerDataIndexFromClientId(OwnerClientId )].position;
        if (IsServer) {
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
        }
        Debug.Log(NetworkManager.LocalClientId);
        Debug.Log(NetworkManager.ServerClientId);
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong clientId) {
        if (clientId == OwnerClientId && HasChild()) {
            PickableObject.DestroyObject(GetChild());
        }
    }

    private void AssignPlayerId(ulong clientId)
    {
        playerId = clientId; 
        Debug.Log("Player with Client ID " + clientId + " assigned ID: " + playerId);
    }

    private void Update() {
        if (!IsOwner) return;
        if(!_canMove) return;
        
        GatherInput();
        Look();
        
        if (Input.GetKeyDown(KeyCode.LeftShift) && stamina > 0)
        {
            _gameManager.AudioManager.Instantplay(SoundName.Sprint, transform.position);
            isRunning = true;
            StartCoroutine(ClashSceneUI.Instance.FadeStaminaBar(1));
        }
        if (Input.GetKeyUp(KeyCode.LeftShift) || stamina <= 0)
        {
            isRunning = false;
        }
        
        RegenerateStamina();
        RotatePlayerArrowTowardsCamera();
        ClashSceneUI.Instance.SetStaminaSliderValue(stamina);
        if (!isRunning && stamina >= maxStamina && ClashSceneUI.Instance.staminaCanvasGroup.alpha > 0)
        {
            StartCoroutine(ClashSceneUI.Instance.FadeStaminaBar(0));
        }
    }
    
    private void RegenerateStamina()
    {
        if (!isRunning && stamina < maxStamina)
        {
            stamina += staminaRegenRate * Time.deltaTime;
            stamina = Mathf.Clamp(stamina, 0, maxStamina);
        }
    }
    
    private void RotatePlayerArrowTowardsCamera()
    {
        Quaternion desiredWorldRotation = Quaternion.Euler(new Vector3(0, 135, 270));
    
        Quaternion parentRotationInverse = Quaternion.Inverse(playerArrow.transform.parent.rotation);
        Quaternion localRotation = parentRotationInverse * desiredWorldRotation;
    
        playerArrow.transform.localRotation = localRotation;
    }
    

    private void FixedUpdate() {
        Move();
    }

    public Transform GetHoldingPointTransform()
    {
        return holdingPoint;
    }

    private void GatherInput() {
        _input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        SetAnimations();
    }

    private void SetAnimations()
    {
        // ANIMATIONS
        // idle
        if (_input == Vector3.zero)
        {
            _animator.SetFloat(Speed, 0);
        }
        // Walk
        else if (!isRunning)
        {
            _animator.SetFloat(Speed, 0.5f);
        }
        // run
        else
        {
            _animator.SetFloat(Speed, 1);
        }
    }

    private void Look() {
        if (_input == Vector3.zero) return;

        var rot = Quaternion.LookRotation(Utilities.ToIso(_input), Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, rotateSpeed * Time.deltaTime);
    }

    private void Move()
    {
        float desiredEmissionRate = 0;
        if (_input != Vector3.zero)
        {
            float actualSpeed = speed;
            
            if (isRunning && stamina > 0)
            {
                actualSpeed = runSpeed;
                stamina -= staminaDepletionRate * Time.deltaTime;
                stamina = Mathf.Clamp(stamina, 0, maxStamina);
            }
            desiredEmissionRate = 15;
            _rb.MovePosition(transform.position + transform.forward * (_input.normalized.magnitude * actualSpeed * Time.deltaTime));
        }

        if (_currentEmissionRate != desiredEmissionRate)
        {
            _currentEmissionRate = desiredEmissionRate;
            SetEmissionRateServerRpc(_currentEmissionRate);
        }
    }
    
    
    [ServerRpc(RequireOwnership = false)]
    private void SetEmissionRateServerRpc(float rate)
    {
        SetEmissionRateClientRpc(rate);
    }

    [ClientRpc]
    private void SetEmissionRateClientRpc(float rate)
    {
        var emission = footprintVfx.emission;
        emission.rateOverTime = rate;
    }

    private void SetTeam()
    {
        if(_gameManager == null)
            _gameManager = GameManager.Instance;
        if (MultiplayerController.Instance.GetPlayerDataFromClientId(OwnerClientId).teamId == 1)
        {
            _teamId = 1;
            _teamColor = _gameManager.team1.color;
            _teamName = _gameManager.team1.name;
            _teamCharacteristics = _gameManager.team1;
        }
        else
        {
            _teamId = 2;
            _teamColor = _gameManager.team2.color;
            _teamName = _gameManager.team2.name;
            _teamCharacteristics = _gameManager.team2;
        }
        playerTeamIndicator.GetComponent<Renderer>().material.color = _teamColor; // Set the color of boxes with respect to their team
    }


    public Transform GetChildFollowTransform()
    {
        return holdingPoint;
    }

    public void SetChild(PickableObject child)
    {
        _pickableObject = child;
    }

    public PickableObject GetChild()
    {
        return _pickableObject;
    }

    public void ClearChild()
    {
        _pickableObject = null;
    }

    public bool HasChild()
    {
        return _pickableObject != null;
    }

    public NetworkObject GetNetworkObject()
    {
        return GetComponent<NetworkObject>();
    }

    public Color GetTeamColor()
    {
        return _teamColor;
    }
    
    public int GetTeamId()
    {
        return _teamId;
    }

    public TeamCharacteristicsScriptableObject GetTeamCharacteristics()
    {
        return _teamCharacteristics;
    }

    public void SetPlayerAbleToMove(bool canMove)
    {
        _canMove = canMove;
    }
}