using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tank;
using UnityEngine;
using TMPro;

public class PlayerAction
{
    public int PlayerId { get; }
    public string ActionType { get; }
    public Vector2 Target { get; }

    public PlayerAction(int id, string type, Vector2 target)
    {
        PlayerId = id;
        ActionType = type;
        Target = target;
    }
}

public enum TurnPhase
{
    Planning,
    Action
}

public class TurnManagerScript : MonoBehaviour
{
    [Header("Turn Settings")]
    public float turnDuration = 10f;
    public TMP_Text timerText;

    [Header("Runtime Data")]
    private float _timer;
    private TurnPhase _currentPhase;
    private readonly Dictionary<int, PlayerAction> _actions = new();
    private readonly List<PlayerScript> _players = new();

    private const float StationaryThreshold = 0.05f;
    private const float RequiredStationaryTime = 2f;
    
    private bool _gameStarted = false;

    
    public void StartGame()
    {
        _gameStarted = true;
        StartPlanningPhase();
    }

    private void Update()
    {
        UpdateTimer();
    }

    // ---------- TURN SETUP ----------

    public void AssignIds(TankScript[] tanks)
    {
        _players.Clear();
        for (int i = 0; i < tanks.Length; i++)
        {
            var player = new PlayerScript(i, tanks[i]);
            _players.Add(player);
        }

        Debug.Log($"Asignados {_players.Count} jugadores a tanques.");
    }

    // ---------- PLANNING PHASE ----------
    
    public void StartPlanningPhase()
    {
        if (!_gameStarted) return; // prevent early start

        Debug.Log("Fase de planificación comienza!");
        _currentPhase = TurnPhase.Planning;
        _timer = turnDuration;
        _actions.Clear();
    }

    private void UpdateTimer()
    {
        if (timerText)
            timerText.text = $"Time left: {Mathf.Ceil(_timer)}s";

        if (_currentPhase == TurnPhase.Planning)
        {
            _timer -= Time.deltaTime;
            if (_timer <= 0f)
                EndPlanningPhase();
        }
    }

    public void RegisterAction(int playerId, string actionType, Vector2 target)
    {
        if (_currentPhase != TurnPhase.Planning)
        {
            Debug.LogWarning("No se pueden registrar acciones fuera de la fase de planificación");
            return;
        }

        if (_actions.ContainsKey(playerId))
        {
            Debug.Log($"Jugador {playerId} ya tiene una acción registrada.");
            return;
        }

        _actions[playerId] = new PlayerAction(playerId, actionType, target);
        Debug.Log($"Acción registrada para jugador {playerId}: {actionType} -> {target}");
    }

    private void EndPlanningPhase()
    {
        Debug.Log("Fase de planificación termina.");
        _currentPhase = TurnPhase.Action;

        foreach (var player in _players)
            player.tank?.HideTrajectory();

        // assign actions
        foreach (var (playerId, action) in _actions)
        {
            var player = _players.FirstOrDefault(p => p.playerId == playerId);
            if (player != null)
                player.pendingAction = action;
        }

        StartCoroutine(ExecuteActionsPhase());
    }

    // ---------- ACTION PHASE ----------

    private IEnumerator ExecuteActionsPhase()
    {
        Debug.Log("Fase de acción comienza!");

        foreach (var player in _players)
        {
            if (player.pendingAction == null) continue;
            player.tank.ExecuteAction(player.pendingAction);
            player.pendingAction = null;
        }

        yield return WaitForStationaryState();

        Debug.Log("Fase de acción termina.");
        StartPlanningPhase();
    }

    private IEnumerator WaitForStationaryState()
    {
        float stationaryTime = 0f;

        while (stationaryTime < RequiredStationaryTime)
        {
            yield return null;
            stationaryTime = CheckStationaryState()
                ? stationaryTime + Time.deltaTime
                : 0f;
        }
    }

    private bool CheckStationaryState()
    {
        foreach (var rb in FindObjectsByType<Rigidbody2D>(FindObjectsSortMode.None))
        {
            if (rb.linearVelocity.magnitude >= StationaryThreshold)
                return false;
        }
        return true;
    }

    // ---------- UTILITY ----------

    public bool IsPlanningPhase() => _currentPhase == TurnPhase.Planning;
    public bool HasAction(int playerId) => _actions.ContainsKey(playerId);
}
