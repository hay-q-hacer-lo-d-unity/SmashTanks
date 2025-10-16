using Fusion;
using UnityEngine;

public class PlayerScript
{
    public int playerId;
    public NetworkId tankNetworkId; // Guardamos el ID de red en lugar de la referencia directa
    public PlayerAction pendingAction;

    private TankScript _cachedTank; // Caché para evitar búsquedas repetidas

    public PlayerScript(int id, TankScript assignedTank)
    {
        playerId = id;
        if (assignedTank != null && assignedTank.Object != null)
        {
            tankNetworkId = assignedTank.Object.Id;
            _cachedTank = assignedTank;
            Debug.Log($"[PlayerScript] Jugador {id} asignado a tanque con NetworkId: {tankNetworkId}");
        }
        else
        {
            Debug.LogError($"[PlayerScript] No se pudo asignar tanque para jugador {id}");
        }
    }

    public TankScript GetTank(NetworkRunner runner)
    {
        // Intenta usar la caché primero
        if (_cachedTank != null && _cachedTank.gameObject != null)
        {
            return _cachedTank;
        }

        // Si se perdió la referencia, búscala por NetworkId
        if (runner != null && runner.TryFindObject(tankNetworkId, out NetworkObject networkObject))
        {
            _cachedTank = networkObject.GetComponent<TankScript>();
            Debug.Log($"[PlayerScript] Tanque recuperado para jugador {playerId}");
            return _cachedTank;
        }

        Debug.LogError($"[PlayerScript] No se pudo encontrar tanque para jugador {playerId} con NetworkId {tankNetworkId}");
        return null;
    }
}