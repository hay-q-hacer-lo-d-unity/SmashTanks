using Tank;
using UI;

namespace Manager
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;

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
        private int _confirmedPlayers = 0;

        private readonly List<Skillset> _pendingSkillsets = new();
        private readonly List<TankScript> _tanks = new();

        // ---------- EVENTS ----------

        public static event Action<int, int> OnTankConfirmed;  // (confirmedCount, totalCount)
        public static event Action OnAllPlayersConfirmed;
        public static event Action OnGameStarted;
        public static event Action<TankScript> OnTankSpawned;  // invoked for each tank
        public static event Action OnAllTanksSpawned;

        // ---------- UNITY LIFECYCLE ----------

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Debug.LogWarning("[GameManager] Duplicate instance found, destroying new one.");
                Destroy(gameObject);
                return;
            }

            Instance = this;
            // Uncomment if you want persistence between scenes
            // DontDestroyOnLoad(gameObject);

            gameplayRoot?.SetActive(false);
        }

        // ---------- PUBLIC METHODS ----------

        public void ConfirmTank(Skillset skillset)
        {
            if (skillset == null)
            {
                Debug.LogWarning("[GameManager] Tried to confirm a null skillset.");
                return;
            }

            _confirmedPlayers++;
            _pendingSkillsets.Add(skillset);

            Debug.Log($"[GameManager] Confirmed tank {_confirmedPlayers}/{playerCount}");

            OnTankConfirmed?.Invoke(_confirmedPlayers, playerCount);

            if (_confirmedPlayers >= playerCount)
            {
                OnAllPlayersConfirmed?.Invoke();
                StartGame();
            }
        }

        // ---------- GAME START ----------

        private void StartGame()
        {
            Debug.Log("[GameManager] All players confirmed. Starting game!");

            if (!tankPrefab || !gameplayRoot || !turnManager)
            {
                Debug.LogError("[GameManager] Missing references! Cannot start game.");
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

        // ---------- INTERNAL LOGIC ----------

        private void SpawnTanks()
        {
            _tanks.Clear();

            const float spacing = 5f;
            var totalWidth = (playerCount - 1) * spacing;
            var startX = -totalWidth / 2f;

            for (var i = 0; i < _pendingSkillsets.Count; i++)
            {
                var spawnPos = new Vector3(startX + i * spacing, 0f, 0f);
                var tankGo = Instantiate(tankPrefab, spawnPos, Quaternion.identity, gameplayRoot.transform);

                if (tankGo.TryGetComponent(out TankScript newTank))
                {
                    newTank.SetOwnerId(i);
                    newTank.Initialize(_pendingSkillsets[i]);
                    _tanks.Add(newTank);
                    OnTankSpawned?.Invoke(newTank);
                }
                else
                {
                    Debug.LogError($"[GameManager] Tank prefab missing TankScript component at index {i}.");
                }
            }

            Debug.Log($"[GameManager] Spawned {_tanks.Count} tanks.");
            OnAllTanksSpawned?.Invoke();
        }

        // ---------- GETTERS ----------

        public IReadOnlyList<TankScript> Tanks => _tanks.AsReadOnly();
    }
}
