using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Actions;
using Tank;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;

public class PlayerAction
{
    public int PlayerId { get; }
    public string ActionType { get; }
    public Vector2 Target { get; }
    public Vector2 Origin { get; }

        public PlayerAction(int id, string type, Vector2 origin, Vector2 target)
    {
        PlayerId = id;
        ActionType = type;
        Target = target;
        Origin = origin;
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
    [SerializeField] private ActionSelectorScript actionSelectorScript;

    private int _currentPlayerIndex = 0;
    private bool _gameStarted = false;

    private const float StationaryThreshold = 0.05f;
    private const float RequiredStationaryTime = 2f;

    private void Update()
    {
        if (_gameStarted)
            UpdateTimer();
    }

    // ---------- GAME SETUP ----------

    public void AssignIds(TankScript[] tanks)
    {
        _players.Clear();
        for (int i = 0; i < tanks.Length; i++)
            _players.Add(new PlayerScript(i, tanks[i]));

        Debug.Log($"Asignados {_players.Count} jugadores a tanques.");
    }

    public void StartGame()
    {
        _gameStarted = true;
        StartPlanningPhase();
    }

    // ---------- PLANNING PHASE ----------

    private void StartPlanningPhase()
    {
        if (!_gameStarted) return;

        Debug.Log("Fase de planificación comienza!");
        _currentPhase = TurnPhase.Planning;
        _actions.Clear();
        _currentPlayerIndex = 0;

        StartNextPlayerTurn();
    }

    private void StartNextPlayerTurn()
    {
        if (_currentPlayerIndex >= _players.Count)
        {
            EndPlanningPhase();
            return;
        }

        _timer = turnDuration;
        var currentPlayer = _players[_currentPlayerIndex];
        Debug.Log($"Turno del jugador {_currentPlayerIndex} comienza!");

        actionSelectorScript.SetTank(currentPlayer.Tank);
        currentPlayer.Tank.SetControlEnabled(true);
    }

    private void UpdateTimer()
    {
        if (timerText)
            timerText.text = $"Time left: {Mathf.Ceil(_timer)}s";

        if (_currentPhase == TurnPhase.Planning)
        {
            _timer -= Time.deltaTime;
            if (_timer <= 0f)
                ConfirmCurrentPlayerAction();
        }
    }

    public void ConfirmCurrentPlayerAction()
    {
        if (_currentPhase != TurnPhase.Planning) return;
        if (_currentPlayerIndex >= _players.Count) return;

        var currentPlayer = _players[_currentPlayerIndex];
        currentPlayer.Tank.SetControlEnabled(false);

        if (!_actions.ContainsKey(currentPlayer.PlayerId)) 
            _actions[currentPlayer.PlayerId] = new PlayerAction(currentPlayer.PlayerId, "Idle", Vector2.zero, Vector2.zero);
        

        Debug.Log($"Jugador {_currentPlayerIndex} confirma acción.");
        _currentPlayerIndex++;

        StartNextPlayerTurn();
    }

    public void RegisterAction(int playerId, string actionType, Vector2 origin, Vector2 target)
    {
        if (_currentPhase != TurnPhase.Planning) return;
        if (_players[_currentPlayerIndex].PlayerId != playerId) return;

        _actions[playerId] = new PlayerAction(playerId, actionType, origin, target);
        Debug.Log($"Acción registrada para jugador {playerId}: {actionType} -> {target}");
    }

    private void EndPlanningPhase()
    {
        Debug.Log("Fase de planificación termina.");
        _currentPhase = TurnPhase.Action;

        foreach (var player in _players)
            player.Tank?.HideTrajectory();

        // Assign pending actions
        foreach (var (playerId, action) in _actions)
        {
            var player = _players.FirstOrDefault(p => p.PlayerId == playerId);
            if (player != null)
                player.PendingAction = action;
        }

        StartCoroutine(ExecuteActionsPhase());
    }

    // ---------- ACTION PHASE ----------

    private IEnumerator ExecuteActionsPhase()
    {
        Debug.Log("Fase de acción comienza!");

        foreach (var player in _players)
        {
            if (player.PendingAction == null) continue;
            player.Tank.ExecuteAction(player.PendingAction);
            player.PendingAction = null;
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

