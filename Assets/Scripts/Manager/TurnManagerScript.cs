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

        public PlayerAction(int playerId, string actionType, Vector2 origin, Vector2 target)
        {
            PlayerId = playerId;
            ActionType = actionType;
            Origin = origin;
            Target = target;
        }
    }

    public enum TurnPhase { Planning, Action }

    public class TurnManagerScript : MonoBehaviour
    {
        [Header("Turn Settings")]
        [SerializeField] private float turnDuration = 10f;
        [SerializeField] private TMP_Text timerText;

        [Header("References")]
        [SerializeField] private ActionSelectorScript actionSelectorScript;

        private List<PlayerScript> _players = new();
        private Queue<PlayerScript> _playerQueue;
        private readonly Dictionary<int, PlayerAction> _actions = new();

        private TurnPhase _currentPhase;
        [SerializeField] private float timer;
        private bool _gameStarted;
        private PlayerScript _currentPlayer;

        private const float StationaryThreshold = 0.05f;
        private const float RequiredStationaryTime = 1f;

        private void Update()
        {
            if (!_gameStarted || _currentPhase != TurnPhase.Planning) return;

            UpdateTimer();

            if (_currentPlayer != null && HasAction(_currentPlayer.PlayerId))
                FinalizeCurrentPlayerAction();
        }

        #region Game Flow

        public void AssignIds(TankScript[] tanks)
        {
            _players = tanks.Select((tank, i) => new PlayerScript(i, tank)).ToList();
        }

        public void StartGame()
        {
            _gameStarted = true;
            StartPlanningPhase();
        }

        private void StartPlanningPhase()
        {
            _currentPhase = TurnPhase.Planning;
            _actions.Clear();

            foreach (var player in _players)
                player.Tank.ApplyTurnStartEffects();

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

        private void EndPlanningPhase()
        {
            _currentPhase = TurnPhase.Action;
            timerText.text = "Action!";

            foreach (var player in _players)
            {
                if (_actions.TryGetValue(player.PlayerId, out var action))
                    player.PendingAction = action;
            }

            StartCoroutine(ExecuteActionsPhase());
        }

        private IEnumerator ExecuteActionsPhase()
        {
            _lastDamageTime = Time.time;
            foreach (var player in _players.Where(p => p.PendingAction != null))
            {
                if (player.PendingAction.ActionType == "Idle") continue; // skip
    
                player.Tank.ExecuteAction(player.PendingAction);
                player.PendingAction = null;
            }


            yield return WaitForPhysicsToSettle();
            RemoveDeadPlayers();
            if (_players.Count <= 1)
            {
                timerText.text = "Game Over!";
                GameManagerScript.Instance.NotifyEndGame(_players.FirstOrDefault()?.PlayerId);
                yield break;
            }
            StartPlanningPhase();
        }

        #endregion

        #region Timer Management
        
        private void UpdateTimer()
        {
            timer = Mathf.Max(0, timer - Time.deltaTime);
            timerText.text = $"Time left: {Mathf.Ceil(timer)}s";

            if (timer <= 0f)
                FinalizeCurrentPlayerAction();
        }

        private void FinalizeCurrentPlayerAction()
        {
            if (_currentPlayer == null || !IsPlanningPhase()) return;
    
            _currentPlayer.Tank.SetControlEnabled(false);
            _actions.TryAdd(_currentPlayer.PlayerId, null);

            _currentPlayer = null;
            StartNextPlayerTurn();
        }

        #endregion

        #region Action Registration

        public void RegisterAction(int playerId, string actionType, Vector2 origin, Vector2 target)
        {
            if (_currentPhase != TurnPhase.Planning) return;

            if (_players.All(p => p.PlayerId != playerId)) return;

            _actions[playerId] = new PlayerAction(playerId, actionType, origin, target);
        }

        #endregion

        #region Stationary Check

        private IEnumerator WaitForPhysicsToSettle()
        {
            var stationaryTime = 0f;
            const float maxWaitTime = 30f;
            var elapsed = 0f;

            while (stationaryTime < RequiredStationaryTime && elapsed < maxWaitTime)
            {
                yield return null;
                elapsed += Time.deltaTime;

                var rigidbodies = FindObjectsByType<Rigidbody2D>(FindObjectsSortMode.None)
                    .Where(rb => rb && rb.gameObject.activeInHierarchy)
                    .ToArray();

                var allStill = rigidbodies.All(rb => rb.linearVelocity.magnitude < StationaryThreshold);
                var noRecentDamage = Time.time - _lastDamageTime >= RequiredStationaryTime;

                if (allStill && noRecentDamage) stationaryTime += Time.deltaTime;
                else stationaryTime = 0f;
            }
        }

        private float _lastDamageTime;

        public void NotifyDamageApplied() => _lastDamageTime = Time.time;
        
        #endregion

        #region Utilities

        public bool IsPlanningPhase() => _currentPhase == TurnPhase.Planning;
        public bool HasAction(int playerId) => _actions.ContainsKey(playerId);

        private void RemoveDeadPlayers()
        {
            _players = _players
                .Where(p => !p.Tank.IsDead)
                .ToList();
        }

        #endregion
    }
}
