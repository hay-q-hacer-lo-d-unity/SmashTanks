using System;
using Fusion;
using Fusion.Sockets;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GameNetworkManager : MonoBehaviour, INetworkRunnerCallbacks
{
    private NetworkRunner runner;

    async void Start()
    {
        runner = gameObject.AddComponent<NetworkRunner>();
        runner.ProvideInput = true;

        // Inicia el juego como Host (puede ser Client si cambiás el modo)
        await runner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.Host,
            SessionName = "SmashTanksRoom",
            Scene = SceneRef.FromIndex(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex),
            SceneManager = runner.gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
    }

    public async void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"👥 Jugador conectado: {player.PlayerId}");

        // Carga el prefab del tanque desde Resources
        var tankPrefab = Resources.Load<GameObject>("Tank");
        if (tankPrefab == null)
        {
            Debug.LogError("❌ No se encontró el prefab Tank en Resources/");
            return;
        }

        // Spawnea el tanque en una posición distinta por jugador
        var spawnPos = new Vector3(player.PlayerId * 3, 0, 0);
        var tankObject = runner.Spawn(tankPrefab, spawnPos, Quaternion.identity, player);

        // 🔄 Espera brevemente para garantizar que la autoridad local esté establecida
        await Task.Delay(200);

        var tankScript = tankObject.GetComponent<TankScript>();
        if (tankScript == null)
        {
            Debug.LogError("❌ El objeto spawneado no tiene TankScript");
            return;
        }

        // 🔗 Vincula automáticamente el ActionSelector al tanque local
        if (tankObject.HasInputAuthority)
        {
            var selector = FindObjectOfType<ActionSelectorScript>();
            if (selector != null)
            {
                selector.SetTank(tankScript);
                Debug.Log($"✅ ActionSelector vinculado automáticamente al tanque local (Jugador {player.PlayerId})");
            }
            else
            {
                Debug.LogWarning("⚠️ No se encontró ningún ActionSelector en la escena.");
            }
        }
        else
        {
            Debug.Log($"[GameNetworkManager] Jugador {player.PlayerId} spawneado sin autoridad local (remoto).");
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        var data = new NetworkInputData
        {
            Horizontal = Input.GetAxisRaw("Horizontal"),
            Vertical = Input.GetAxisRaw("Vertical")
        };
        input.Set(data);
    }

    // Callbacks que no usamos todavía:
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remote, NetConnectFailedReason reason) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason reason) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }

}

// Estructura de input para enviar a la red
public struct NetworkInputData : INetworkInput
{
    public float Horizontal;
    public float Vertical;
}
