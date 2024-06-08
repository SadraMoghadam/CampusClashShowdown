using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public enum RewardType
{
    BoxDelivery,
    BoxDestruction,
    BeltMovement,
    BoxConveyorPlacement,
}

public class ClashRewardCalculator : NetworkBehaviour
{
    private Dictionary<Team, int> _totalBoxesDelivered;
    private Dictionary<Team, int> _totalBoxesDestroyed;
    private Dictionary<Team, int> _totalBeltsMoved;
    private Dictionary<Team, int> _totalBoxesPlacedOnConveyor;

    private int _boxDeliveryRewardCoefficient = 100;
    private int _boxDestructionRewardCoefficient = 50;
    private int _beltMovementRewardCoefficient = 5;
    private int _boxConveyorPlacementRewardCoefficient = 10;

    private static ClashRewardCalculator _instance;
    public static ClashRewardCalculator Instance => _instance;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            InitializeDictionaries();
        }
    }

    private void InitializeDictionaries()
    {
        _totalBoxesDelivered = new Dictionary<Team, int>
        {
            { Team.Team1, 0 },
            { Team.Team2, 0 }
        };

        _totalBoxesDestroyed = new Dictionary<Team, int>
        {
            { Team.Team1, 0 },
            { Team.Team2, 0 }
        };

        _totalBeltsMoved = new Dictionary<Team, int>
        {
            { Team.Team1, 0 },
            { Team.Team2, 0 }
        };

        _totalBoxesPlacedOnConveyor = new Dictionary<Team, int>
        {
            { Team.Team1, 0 },
            { Team.Team2, 0 }
        };
    }

    public void AddRewardByRewardType(Team team, RewardType type)
    {
        switch (type)
        {
            case RewardType.BoxDelivery:
                _totalBoxesDelivered[team]++;
                break;
            case RewardType.BoxDestruction:
                _totalBoxesDestroyed[team]++;
                break;
            case RewardType.BoxConveyorPlacement:
                _totalBoxesPlacedOnConveyor[team]++;
                break;
            case RewardType.BeltMovement:
                _totalBeltsMoved[team]++;
                break;
            default:
                break;
        }
    }

    public int GetTotalByRewardType(Team team, RewardType type)
    {
        int total = 0;
        switch (type)
        {
            case RewardType.BoxDelivery:
                total = _totalBoxesDelivered[team];
                break;
            case RewardType.BoxDestruction:
                total = _totalBoxesDestroyed[team];
                break;
            case RewardType.BoxConveyorPlacement:
                total = _totalBoxesPlacedOnConveyor[team];
                break;
            case RewardType.BeltMovement:
                total = _totalBeltsMoved[team];
                break;
            default:
                break;
        }

        return total;
    }
    
    public int GetTotalByRewardType(RewardType type)
    {
        PlayerData playerData = MultiplayerController.Instance.GetPlayerDataFromClientId(OwnerClientId);
        
        Team team = playerData.teamId == 1 ? Team.Team1 : Team.Team2;
        int total = 0;
        switch (type)
        {
            case RewardType.BoxDelivery:
                total = _totalBoxesDelivered[team];
                break;
            case RewardType.BoxDestruction:
                total = _totalBoxesDestroyed[team];
                break;
            case RewardType.BoxConveyorPlacement:
                total = _totalBoxesPlacedOnConveyor[team];
                break;
            case RewardType.BeltMovement:
                total = _totalBeltsMoved[team];
                break;
            default:
                break;
        }

        return total;
    }

    public int CalculateRewards(Team team)
    {
        int totalReward = _totalBoxesDelivered[team] * _boxDeliveryRewardCoefficient +
                          _totalBoxesDestroyed[team] * _boxDestructionRewardCoefficient +
                          _totalBoxesPlacedOnConveyor[team] * _boxConveyorPlacementRewardCoefficient +
                          _totalBeltsMoved[team] * _beltMovementRewardCoefficient;
        int currentResources = PlayerPrefsManager.GetInt(PlayerPrefsKeys.Resource, 0);
        PlayerPrefsManager.SetInt(PlayerPrefsKeys.Resource, currentResources + totalReward);
        return totalReward;
    }
    
    public int CalculateRewards()
    {
        PlayerData playerData = MultiplayerController.Instance.GetPlayerDataFromClientId(OwnerClientId);
        
        Team team = playerData.teamId == 1 ? Team.Team1 : Team.Team2;
        int totalReward = _totalBoxesDelivered[team] * _boxDeliveryRewardCoefficient +
                          _totalBoxesDestroyed[team] * _boxDestructionRewardCoefficient +
                          _totalBoxesPlacedOnConveyor[team] * _boxConveyorPlacementRewardCoefficient +
                          _totalBeltsMoved[team] * _beltMovementRewardCoefficient;
        int currentResources = PlayerPrefsManager.GetInt(PlayerPrefsKeys.Resource, 0);
        PlayerPrefsManager.SetInt(PlayerPrefsKeys.Resource, currentResources + totalReward);
        return totalReward;
    }
}