using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Netcode;

namespace Assets.Scripts.UI.Leaderboard 
{
    // custome types need to be structs

    // INetwork Serializable lets me say how NGO should serialize things
    // Iequatable allows validation of another or same instance of the state type checking for 
    // in this case leaderboardEntityState
    public struct LeaderboardEntityState : INetworkSerializable, IEquatable<LeaderboardEntityState>
    {
        public ulong ClientId;
        public FixedString32Bytes playerName;
        public int Coins;

       

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            // serializer is just how you take data and convert it into bytes over network in this case
            serializer.SerializeValue(ref ClientId);
            serializer.SerializeValue(ref playerName);
            serializer.SerializeValue(ref Coins);
        }
        public bool Equals(LeaderboardEntityState other)
        {
            // if all these values match then the instance matches
            return ClientId == other.ClientId &&
                playerName.Equals(other.playerName) && // instead of == due to fixed string 
                Coins == other.Coins;
        }
    }
    
}
