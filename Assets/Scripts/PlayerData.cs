using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public struct PlayerData : IEquatable<PlayerData>, INetworkSerializable {


    public ulong clientId;
    public int bodyMeshId;
    public int headMeshId;
    public FixedString64Bytes playerName;
    public FixedString64Bytes playerId;
    public int teamId;


    public bool Equals(PlayerData other)
    {
        return
            clientId == other.clientId &&
            bodyMeshId == other.bodyMeshId &&
            headMeshId == other.headMeshId &&
            playerName == other.playerName &&
            playerId == other.playerId &&
            teamId == other.teamId;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
        serializer.SerializeValue(ref clientId);
        serializer.SerializeValue(ref bodyMeshId);
        serializer.SerializeValue(ref headMeshId);
        serializer.SerializeValue(ref playerName);
        serializer.SerializeValue(ref playerId);
        serializer.SerializeValue(ref teamId);
    }

}