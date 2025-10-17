using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public class PlayerAction
{
    public int playerId;
    public string actionType;
    public Vector2 target;
    private TurnPhase currentPhase = TurnPhase.Planning;

    public PlayerAction(int id, string type, Vector2 tgt)
    {
        playerId = id;
        actionType = type;
        target = tgt;
    }
}

public class TurnManagerScript : MonoBehaviour
{
    public float turnDuration;
    public float timer;
    public TMP_Text timerText;
    public List<PlayerScript> players = new List<PlayerScript>();
    private Dictionary<int, PlayerAction> actions = new Dictionary<int, PlayerAction>();
    private TurnPhase currentPhase = TurnPhase.Planning;
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
        StartPlanningPhase();
    }
    
    
    void Update()
    {
        if (timerText)
        {
            timerText.text = $"Time left: {Mathf.Ceil(timer)}s";
        }
        if (currentPhase == TurnPhase.Planning)
        {
            timer -= Time.deltaTime;

            if (timer <= 0f)
            {
                EndPlanningPhase();
            }
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
        if (currentPhase != TurnPhase.Planning)
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
        return currentPhase == TurnPhase.Planning;
    }

    public bool HasAction(int playerId)
    {
        return actions.ContainsKey(playerId);
    }
    
    void StartPlanningPhase()
    {
        Debug.Log("Fase de planificación comienza!");
        currentPhase = TurnPhase.Planning;
        timer = turnDuration;
        actions.Clear();
    }

    void EndPlanningPhase()
    {
        Debug.Log("Fase de planificación termina.");
        currentPhase = TurnPhase.Action;
        foreach (PlayerScript player in players)
        {
            if (player.tank)
                player.tank.HideTrajectory();
        }
        // Assign pending actions
        foreach (var kvp in actions)
        {
            PlayerScript player = players.Find(p => p.playerId == kvp.Key);
            if (player != null)
                player.pendingAction = kvp.Value;
        }

        StartCoroutine(ExecuteActionsPhase());
    }
    
    IEnumerator ExecuteActionsPhase()
    {
        Debug.Log("Fase de acción comienza!");

        // Execute all pending actions
        foreach (PlayerScript player in players)
        {
            if (player.pendingAction != null)
            {
                player.tank.ExecuteAction(player.pendingAction);
                player.pendingAction = null;
            }
        }

        float stationaryTime = 0f;
        float requiredStationaryTime = 2f; // 2 seconds of stationariness
        while (stationaryTime < requiredStationaryTime)
        {
            yield return null;

            if (CheckStationaryState())
            {
                stationaryTime += Time.deltaTime; // accumulate time while stationary
            }
            else
            {
                stationaryTime = 0f; // reset timer if anything moves
            }
        }

        Debug.Log("Fase de acción termina.");
        StartPlanningPhase();
    }

    bool CheckStationaryState()
    {
        Rigidbody2D[] allRigidbodies = FindObjectsByType<Rigidbody2D>(FindObjectsSortMode.None);

        foreach (Rigidbody2D rb in allRigidbodies)
        {
            if (rb.linearVelocity.magnitude >= 0.05f)
            {
                return false;
            }
        }
        return true;
    }

}

public enum TurnPhase
{
    Planning,
    Action
}
