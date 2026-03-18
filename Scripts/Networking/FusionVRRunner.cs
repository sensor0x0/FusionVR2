using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

using UnityEngine;

using Fusion;
using Photon.Voice;
using Photon.Voice.Fusion;
using Fusion.Sockets;
using Fusion.Photon.Realtime;

using Fusion.VR;
using Fusion.VR.Player;
using Fusion.VR.Cosmetics;
using Fusion.VR.Saving;
using Photon.Voice.Unity;

namespace Fusion.VR.Networking
{
    public class FusionVRRunner : MonoBehaviour, INetworkRunnerCallbacks
    {
        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            Debug.Log("Player entered");

            if (!runner.IsRunning)
                return;

            bool b = true;

            switch (runner.GameMode)
            {
                case GameMode.AutoHostOrClient:
                case GameMode.Host:
                case GameMode.Client:
                    b = runner.IsServer;
                    break;
                case GameMode.Shared:
                    b = player == runner.LocalPlayer;
                    break;
                default:
                    break;
            }

            if (b)
            {
                Debug.Log("Spawning player");
                Vector3 spawnPosition = Vector3.zero;
                NetworkObject networkedPlayer = runner.Spawn(FusionVRManager.Manager.NetworkedPlayerPrefab, spawnPosition, Quaternion.identity, player);

                Debug.Log("Adding player to cache");
                FusionVRManager.Manager.playerCache.Add(player, networkedPlayer);
            }
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            if (FusionVRManager.Manager.playerCache.TryGetValue(player, out NetworkObject networkedPlayer))
            {
                runner.Despawn(networkedPlayer);
                FusionVRManager.Manager.playerCache.Remove(player);
            }
        }

        private FusionVRNetworkedPlayerData lastData = new FusionVRNetworkedPlayerData();

        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
            FusionVRNetworkedPlayerData data = new FusionVRNetworkedPlayerData();

            // Head
            data.headPosition = FusionVRManager.Manager.Head.position;
            data.headRotation = FusionVRManager.Manager.Head.rotation;
            // Left hand
            data.leftHandPosition = FusionVRManager.Manager.LeftHand.position;
            data.leftHandRotation = FusionVRManager.Manager.LeftHand.rotation;

            // Right hand
            data.rightHandPosition = FusionVRManager.Manager.RightHand.position;
            data.rightHandRotation = FusionVRManager.Manager.RightHand.rotation;

            if (data != lastData)
            {
                input.Set(data);
                lastData = data;
            }
        }

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
        
        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
            Debug.Log($"Runner shutdown: {shutdownReason}");
            Destroy(gameObject);
        }
        
        public void OnConnectedToServer(NetworkRunner runner) { }
        public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }

        public async void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
        {
            await runner.Shutdown(shutdownReason: ShutdownReason.HostMigration);

            FusionVRManager.Connect();

            StartGameResult result = await FusionVRManager.Manager.Runner.StartGame(new StartGameArgs()
            {
                HostMigrationToken = hostMigrationToken,
                HostMigrationResume = Resume
            });
        }

        private void Resume(NetworkRunner runner)
        {
            Debug.Log("Resumed");

            Debug.LogWarning("Host migration is not yet implemented fully");

            if (FusionVRManager.OnHostMigrationResume != null)
                FusionVRManager.OnHostMigrationResume.Invoke(runner);
        }

        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
        public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
        public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
        public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
        public void OnSceneLoadDone(NetworkRunner runner) { }
        public void OnSceneLoadStart(NetworkRunner runner) { }
    }
}