using System.Collections.Generic;
using Tank;
using UnityEngine;
using UnityEngine.UI;

namespace Actions
{
    [System.Serializable]
    public class ActionButton
    {
        public ActionType type;
        public Button button;
        public float magickaCost;
    }

    public class ActionSelectorScript : MonoBehaviour
    {
        [Header("Actions Configuration")]
        [SerializeField] private List<ActionButton> actionButtons = new();

        private TankScript _tank;

        private void Start()
        {
            // Link each button to its corresponding action
            foreach (var action in actionButtons)
            {
                var capturedAction = action; // Prevent closure issue in loop
                capturedAction.button.onClick.AddListener(() => SelectAction(capturedAction.type));
            }
        }

        public void SetTank(TankScript newTank)
        {
            _tank = newTank;
            SelectAction(ActionType.Missile); // Default action
            UpdateButtons();
        }

        /// <summary>
        /// Enables or disables buttons depending on available magicka.
        /// </summary>
        private void UpdateButtons()
        {
            if (!_tank) return;

            var currentMagicka = _tank.Magicka;
            foreach (var action in actionButtons)
            {
                action.button.interactable = currentMagicka >= action.magickaCost;
            }
        }

        private void SelectAction(ActionType type)
        {
            if (!_tank) return;
            _tank.SetAction(ActionFactory.Create(type, _tank));
        }
    }
}