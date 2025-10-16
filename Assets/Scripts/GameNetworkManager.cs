using System;
using Fusion;
using Fusion.Sockets;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameNetworkManager : MonoBehaviour, INetworkRunnerCallbacks
{
    private NetworkRunner runner;

    async void Start()
    {
        runner = gameObject.AddComponent<NetworkRunner>();
        runner.ProvideInput = true;
        
        var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
        var sceneInfo = new NetworkSceneInfo();
        if (scene.IsValid) {
            sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
        }
        
        // Inicia el juego uniéndose a una sesión o creando una si no existe.
        await runner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.AutoHostOrClient,
            SessionName = "SmashTanksRoom",
            Scene = scene,
            SceneManager = runner.gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
    }

    public async void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"👥 Jugador conectado: {player.PlayerId}");

        // Solo el servidor/host puede spawnear objetos de red.
        if (runner.IsServer)
        {
            // Carga el prefab del tanque desde Resources
            var tankPrefab = Resources.Load<GameObject>("Tank");
            if (tankPrefab == null)
            {
                Debug.LogError("❌ No se encontró el prefab Tank en Resources/");
                return;
            }

            // Spawnea el tanque en una posición distinta por jugador y le da autoridad al jugador que entró.
            var spawnPos = new Vector3(player.PlayerId * 3, 0, 0);
            runner.Spawn(tankPrefab, spawnPos, Quaternion.identity, player);
        }

        // La lógica para vincular el tanque al controlador de acciones debe ejecutarse en el cliente propietario.
        // Esperamos un momento para que el objeto sea creado por el servidor y replicado a los clientes.
        await Task.Delay(200);

        // Buscamos el tanque que nos pertenece (el que tiene nuestra autoridad de input).
        TankScript localTank = null;
        foreach (var tank in FindObjectsOfType<TankScript>()) {
            if (tank.HasInputAuthority) {
                localTank = tank;
                break;
            }
        }

        if (localTank != null)
        {
            var selector = FindObjectOfType<ActionSelectorScript>();
            if (selector != null)
            {
                selector.SetTank(localTank);
                Debug.Log($"✅ ActionSelector vinculado automáticamente al tanque local.");
            }
            else
            {
                Debug.LogWarning("⚠️ No se encontró ningún ActionSelector en la escena.");
            }
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
