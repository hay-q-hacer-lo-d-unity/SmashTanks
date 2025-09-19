using System.Collections.Generic;
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

public class TurnManagerScript : MonoBehaviour
{
    public float turnDuration = 30f;
    public float timer;
    
    public List<PlayerScript> players = new List<PlayerScript>();
    private Dictionary<int, PlayerAction> actions = new Dictionary<int, PlayerAction>();
    
    private bool isPlanningPhase = false;
    
    void Start()
    {
        TankScript[] tanks = FindObjectsOfType<TankScript>();
        
        for (int i = 0; i < tanks.Length; i++)
        {
            PlayerScript newPlayer = new PlayerScript(i, tanks[i]);
            players.Add(newPlayer);
            
            tanks[i].SetOwnerId(i);
        }
        Debug.Log("Asignados " + players.Count + " jugadores a tanques.");
        StartTurn();
    }
    
    
    void Update()
    {
        if (isPlanningPhase)
        {
            timer -= Time.deltaTime;

            if (timer <= 0f)
            {
                EndTurn();
            }
            
            if (Input.GetKeyDown(KeyCode.A))
                RegisterAction(0, "Move", new Vector2(1, 0));

            if (Input.GetKeyDown(KeyCode.B))
                RegisterAction(1, "Fire", new Vector2(0, 1));

            if (Input.GetKeyDown(KeyCode.C))
                RegisterAction(2, "Defend", new Vector2(-1, 0));

            if (Input.GetKeyDown(KeyCode.D))
                RegisterAction(3, "Wait", new Vector2(0, -1));
        }
    }
    
    void StartTurn()
    {
        Debug.Log("Nuevo turno comienza!");
        timer = turnDuration;
        isPlanningPhase = true;
        actions.Clear();
    }
    
    public void RegisterAction(int playerId, string type, Vector2 target)
    {
        if (!isPlanningPhase)
        {
            Debug.Log("No se pueden registrar acciones fuera de la fase de planificación");
            return;
        }

        if (!actions.ContainsKey(playerId))
        {
            actions[playerId] = new PlayerAction(playerId, type, target);
            Debug.Log($"Acción registrada para jugador {playerId}: {type} -> {target}");
        }
        else
        {
            Debug.Log($"Jugador {playerId} ya tiene una acción registrada.");
        }
    }
    
    void EndTurn()
    {
        isPlanningPhase = false;
        Debug.Log("Turno terminado. Acciones registradas:");

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

    public void ExecuteTurn()
    {
        foreach (PlayerScript player in players)
        {
            if (player.pendingAction != null)
            {
                player.tank.ExecuteAction(player.pendingAction);
                player.pendingAction = null; // limpiar para el próximo turno
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
