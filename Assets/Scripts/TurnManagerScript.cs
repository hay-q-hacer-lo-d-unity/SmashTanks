using System.Collections.Generic;
using Fusion;
using UnityEngine;

[System.Serializable]
public class PlayerAction
{
    public int playerId;
    public string actionType;
    public Vector2 target;

    public PlayerAction(int id, string type, Vector2 tgt)
    {
        playerId = id;
        actionType = type;
        target = tgt;
    }
}

public class TurnManagerScript : NetworkBehaviour
{
    [Networked] public float timer { get; set; }
    [Networked] public bool isPlanningPhase { get; set; }
    [Networked] public int currentTurn { get; set; }

    public float turnDuration = 30f;

    public List<PlayerScript> players = new List<PlayerScript>();
    private Dictionary<int, PlayerAction> actions = new Dictionary<int, PlayerAction>();
    private bool gameStarted = false;

    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            Debug.Log("[TurnManager] Host listo para recibir tanques...");
        }
        else
        {
            Debug.Log("[TurnManager] Cliente sincronizado con host.");
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_RegisterTank(NetworkId tankNetworkId)
    {
        Debug.Log($"[TurnManager] RPC_RegisterTank recibido para NetworkId: {tankNetworkId}");

        // Verifica que el tanque no esté ya registrado
        if (players.Exists(p => p.tankNetworkId == tankNetworkId))
        {
            Debug.LogWarning($"[TurnManager] Tanque {tankNetworkId} ya está registrado");
            return;
        }

        // Busca el tanque por su NetworkId
        if (Runner.TryFindObject(tankNetworkId, out NetworkObject networkObject))
        {
            TankScript tank = networkObject.GetComponent<TankScript>();
            if (tank != null)
            {
                int playerId = players.Count;
                PlayerScript newPlayer = new PlayerScript(playerId, tank);
                players.Add(newPlayer);
                tank.SetOwnerId(playerId);

                Debug.Log($"[TurnManager] Tank {playerId} registrado: {tank.name}. Total tanques: {players.Count}");

                // Si el juego no ha iniciado, inicia inmediatamente con el primer jugador
                if (!gameStarted)
                {
                    gameStarted = true;
                    Debug.Log($"[TurnManager] Iniciando juego con el primer jugador");
                    StartTurn();
                }
                else
                {
                    // Late join: el jugador se une al juego en curso
                    Debug.Log($"[TurnManager] Late join: Jugador {playerId} se une al juego en progreso");
                }
            }
        }
        else
        {
            Debug.LogError($"[TurnManager] No se pudo encontrar el tanque con NetworkId: {tankNetworkId}");
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority)
            return;

        if (isPlanningPhase)
        {
            timer -= Runner.DeltaTime;

            if (timer <= 0f)
            {
                EndTurn();
            }
        }
    }

    private void StartTurn()
    {
        Debug.Log("[TurnManager] Nuevo turno comienza!");
        currentTurn++;
        timer = turnDuration;
        isPlanningPhase = true;
        actions.Clear();
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_RegisterAction(int playerId, string type, Vector2 target)
    {
        if (!isPlanningPhase)
        {
            Debug.Log("[TurnManager] No se pueden registrar acciones fuera de la fase de planificación");
            return;
        }

        if (!actions.ContainsKey(playerId))
        {
            actions[playerId] = new PlayerAction(playerId, type, target);
            Debug.Log($"[TurnManager] Acción RPC registrada para jugador {playerId}: {type} -> {target}");
        }
        else
        {
            Debug.Log($"[TurnManager] Jugador {playerId} ya tiene acción registrada.");
        }
    }

    private void EndTurn()
    {
        isPlanningPhase = false;
        Debug.Log("[TurnManager] Turno terminado. Acciones registradas:");

        foreach (var kvp in actions)
        {
            Debug.Log($"Jugador {kvp.Key}: {kvp.Value.actionType} hacia {kvp.Value.target}");

            PlayerScript player = players.Find(p => p.playerId == kvp.Key);
            if (player != null)
            {
                player.pendingAction = kvp.Value;
            }
        }

        ExecuteTurn();
        StartTurn();
    }

    private void ExecuteTurn()
    {
        Debug.Log($"[TurnManager] Ejecutando turno con {players.Count} jugadores");
        
        foreach (PlayerScript player in players)
        {
            if (player.pendingAction != null)
            {
                TankScript tank = player.GetTank(Runner);
                if (tank != null)
                {
                    Debug.Log($"[TurnManager] Ejecutando acción de jugador {player.playerId}: {player.pendingAction.actionType} hacia {player.pendingAction.target}");
                    tank.ExecuteAction(player.pendingAction);
                }
                else
                {
                    Debug.LogWarning($"[TurnManager] Tank null para jugador {player.playerId}");
                }
                player.pendingAction = null;
            }
            else
            {
                Debug.Log($"[TurnManager] Jugador {player.playerId} no tiene acción pendiente");
            }
        }
    }

    public bool IsPlanningPhase()
    {
        return isPlanningPhase;
    }

    public bool HasAction(int playerId)
    {
        return actions.ContainsKey(playerId);
    }
}
