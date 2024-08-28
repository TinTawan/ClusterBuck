using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public struct PlayerData : IEquatable<PlayerData>, INetworkSerializable
{
    public ulong clientId;
    public FixedString32Bytes playerName;
    //public Color playerColour;
    public FixedString64Bytes playerId;
    public FixedString32Bytes playerHexColour;

    public bool Equals(PlayerData other)
    {
        return clientId == other.clientId && 
            playerName == other.playerName &&
            //playerColour == other.playerColour &&
            playerHexColour == other.playerHexColour && 
            playerId == other.playerId;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref clientId);
        serializer.SerializeValue(ref playerName);
        //serializer.SerializeValue(ref playerColour);
        serializer.SerializeValue(ref playerHexColour);
        serializer.SerializeValue(ref playerId);
    }
}
