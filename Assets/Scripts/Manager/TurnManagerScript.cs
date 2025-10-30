using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Actions;
using Tank;
using TMPro;
using UnityEngine;

namespace Manager
{
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
    
    public enum TurnPhase { Planning, Action }

    public class TurnManagerScript : MonoBehaviour
    {
        [Header("Turn Settings")]
        public float turnDuration = 10f;
        public TMP_Text timerText;

        [Header("References")]
        [SerializeField] private ActionSelectorScript actionSelectorScript;

        private List<PlayerScript> _players = new();
        private Queue<PlayerScript> _playerQueue;
        private readonly Dictionary<int, PlayerAction> _actions = new();
        [SerializeField] private TurnPhase currentPhase;
        [SerializeField] private float timer;
        private bool _gameStarted;

        private const float StationaryThreshold = 0.05f;
        private const float RequiredStationaryTime = 2f;
        
        private PlayerScript _currentPlayer;

        private void Update()
        {
            if (_gameStarted && currentPhase == TurnPhase.Planning)
                UpdateTimer();
        }

        public void AssignIds(TankScript[] tanks)
        {
            _players = tanks.Select((tank, i) => new PlayerScript(i, tank)).ToList();
            // Debug.Log($"Assigned {_players.Count} players to tanks.");
        }

        public void StartGame()
        {
            _gameStarted = true;
            StartPlanningPhase();
        }

        private void StartPlanningPhase()
        {
            currentPhase = TurnPhase.Planning;
            _actions.Clear();
            foreach (var player in _players)
            {
                player.Tank.ApplyTurnStartEffects();
            }
            _playerQueue = new Queue<PlayerScript>(_players);
            StartNextPlayerTurn();
        }

        private void StartNextPlayerTurn()
        {
            if (_playerQueue.Count == 0)
            {
                EndPlanningPhase();
                return;
            }

            _currentPlayer = _playerQueue.Dequeue();
            timer = turnDuration;

            actionSelectorScript.SetTank(_currentPlayer.Tank);
            _currentPlayer.Tank.SetControlEnabled(true);
        }


        private void UpdateTimer()
        {
            timer -= Time.deltaTime;
            timerText.text = $"Time left: {Mathf.Ceil(timer)}s";

            if (timer <= 0f) ConfirmCurrentPlayerAction();
        }

        private void ConfirmCurrentPlayerAction()
        {
            if (_currentPlayer == null) return;

            _currentPlayer.Tank.SetControlEnabled(false);

            _actions.TryAdd(_currentPlayer.PlayerId, null);
            
            _currentPlayer = null;
            StartNextPlayerTurn();
        }


        public void RegisterAction(int playerId, string actionType, Vector2 origin, Vector2 target)
        {
            if (currentPhase != TurnPhase.Planning) return;

            var player = _players.FirstOrDefault(p => p.PlayerId == playerId);
            if (player == null) return;

            _actions[playerId] = new PlayerAction(playerId, actionType, origin, target);
            // Debug.Log($"Action registered for player {playerId}: {actionType} -> {target}");
        }

        private void EndPlanningPhase()
        {
            // Debug.Log("Planning phase ends.");
            currentPhase = TurnPhase.Action;

            foreach (var (id, action) in _actions)
            {
                var player = _players.FirstOrDefault(p => p.PlayerId == id);
                if (player != null)
                    player.PendingAction = action;
            }

            StartCoroutine(ExecuteActionsPhase());
        }

        private IEnumerator ExecuteActionsPhase()
        {
            // Debug.Log("Action phase begins!");

            foreach (var player in _players.Where(player => player.PendingAction != null))
            {
                player.Tank.ExecuteAction(player.PendingAction);
                player.PendingAction = null;
            }

            yield return WaitForStationaryState();

            // Debug.Log("Action phase ends.");
            StartPlanningPhase();
        }

        private static IEnumerator WaitForStationaryState()
        {
            var stationaryTime = 0f;
            while (stationaryTime < RequiredStationaryTime)
            {
                yield return null;
                stationaryTime = CheckStationaryState() ? stationaryTime + Time.deltaTime : 0f;
            }
        }

        private static bool CheckStationaryState()
        {
            return FindObjectsByType<Rigidbody2D>(FindObjectsSortMode.None)
                .All(rb => rb.linearVelocity.magnitude < StationaryThreshold);
        }

        // ---------- Utilities ----------
        public bool IsPlanningPhase() => currentPhase == TurnPhase.Planning;
        public bool HasAction(int playerId) => _actions.ContainsKey(playerId);
    }
}
