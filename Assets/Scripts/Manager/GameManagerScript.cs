using Tank;
using UI;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace Manager
{
    public class GameManagerScript : MonoBehaviour
    {
        public static GameManagerScript Instance { get; private set; }

        [Header("References")]
        [SerializeField] private GameObject tankPrefab;
        [SerializeField] private GameObject skillsetScreen;
        [SerializeField] private GameObject gameplayRoot;
        [SerializeField] private TurnManagerScript turnManager;

        [Header("Game Settings")]
        [SerializeField] private int playerCount = 2;
        [SerializeField] private float spawnSpacing = 50f;

        private int _confirmedPlayers;
        private readonly List<Skillset> _pendingSkillsets = new();
        private readonly List<TankScript> _tanks = new();

        // ---------- EVENTS ----------
        public static event Action<int, int> OnTankConfirmed;
        public static event Action OnAllPlayersConfirmed;
        public static event Action OnGameStarted;
        public static event Action<TankScript> OnTankSpawned;
        public static event Action OnAllTanksSpawned;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Debug.LogWarning("[GameManager] Duplicate instance found, destroying new one.");
                Destroy(gameObject);
                return;
            }

            Instance = this;
            gameplayRoot?.SetActive(false);
        }

        public void ConfirmTank(Skillset skillset)
        {
            if (skillset == null)
            {
                Debug.LogWarning("[GameManager] Tried to confirm a null skillset.");
                return;
            }

            _pendingSkillsets.Add(skillset);
            _confirmedPlayers++;

            OnTankConfirmed?.Invoke(_confirmedPlayers, playerCount);
            Debug.Log($"[GameManager] Confirmed tank {_confirmedPlayers}/{playerCount}");

            if (_confirmedPlayers < playerCount) return;
            OnAllPlayersConfirmed?.Invoke();
            StartGame();
        }

        private void StartGame()
        {
            if (!tankPrefab || !gameplayRoot || !turnManager)
            {
                Debug.LogError("[GameManager] Missing references! Cannot start game.");
                return;
            }

            if (_pendingSkillsets.Count != playerCount)
            {
                Debug.LogError("[GameManager] Skillset count does not match player count.");
                return;
            }

            skillsetScreen?.SetActive(false);
            gameplayRoot.SetActive(true);

            SpawnTanks();
            _pendingSkillsets.Clear();

            turnManager.AssignIds(_tanks.ToArray());

            OnGameStarted?.Invoke();
            turnManager.StartGame();
        }

        private void SpawnTanks()
        {
            _tanks.Clear();

            var totalWidth = (playerCount - 1) * spawnSpacing;
            var startX = -totalWidth / 2f;

            for (var i = 0; i < playerCount; i++)
            {
                var spawnPos = new Vector3(startX + i * spawnSpacing, 0f, 0f);
                var tankGo = Instantiate(tankPrefab, spawnPos, Quaternion.identity, gameplayRoot.transform);

                if (!tankGo.TryGetComponent(out TankScript newTank))
                {
                    Debug.LogError($"[GameManager] Tank prefab missing TankScript at index {i}.");
                    continue;
                }

                newTank.SetOwnerId(i);
                newTank.Initialize(_pendingSkillsets[i]);
                _tanks.Add(newTank);

                OnTankSpawned?.Invoke(newTank);
            }

            // Debug.Log($"[GameManager] Spawned {_tanks.Count} tanks.");
            OnAllTanksSpawned?.Invoke();
        }

        public IReadOnlyList<TankScript> Tanks => _tanks.AsReadOnly();
    }
}
