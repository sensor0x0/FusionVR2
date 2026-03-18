using UnityEngine;
using Fusion;

namespace Fusion.VR.Player
{
    public struct FusionVRNetworkedPlayerData : INetworkInput
    {
        public Vector3 headPosition;
        public Quaternion headRotation;
        public Vector3 leftHandPosition;
        public Quaternion leftHandRotation;
        public Vector3 rightHandPosition;
        public Quaternion rightHandRotation;

        public override string ToString()
        {
            return JsonUtility.ToJson(this);
        }

        public static bool operator ==(FusionVRNetworkedPlayerData lhs, FusionVRNetworkedPlayerData rhs)
        {
            return lhs.headPosition == rhs.headPosition &&
                   lhs.headRotation == rhs.headRotation &&
                   lhs.leftHandPosition == rhs.leftHandPosition &&
                   lhs.leftHandRotation == rhs.leftHandRotation &&
                   lhs.rightHandPosition == rhs.rightHandPosition &&
                   lhs.rightHandRotation == rhs.rightHandRotation;
        }

        public static bool operator !=(FusionVRNetworkedPlayerData lhs, FusionVRNetworkedPlayerData rhs)
        {
            return !(lhs == rhs);
        }

        public override bool Equals(object obj) => obj is FusionVRNetworkedPlayerData data && this == data;
        
        public override int GetHashCode()
        {
            return headPosition.GetHashCode() ^ headRotation.GetHashCode();
        }
    }
}