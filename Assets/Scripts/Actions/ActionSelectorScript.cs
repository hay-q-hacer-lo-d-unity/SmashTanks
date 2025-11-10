using System.Collections.Generic;
using SkillsetUI;
using Tank;
using UnityEngine;

namespace Actions
{
    public class ActionSelectorScript : MonoBehaviour
    {
        [SerializeField] private LegendScript legend;
        [Header("Action Buttons")]
        [SerializeField] private List<ActionButtonScript> actionButtons = new();

        private TankScript _tank;

        public void SetTank(TankScript newTank)
        {
            _tank = newTank;

            foreach (var btn in actionButtons)
                btn.Initialize(this, _tank, legend);

            SelectAction("Missile");
            UpdateButtons();
        }

        private void UpdateButtons()
        {
            if (!_tank) return;
            foreach (var btn in actionButtons)
                btn.UpdateState();
        }

        public void SelectAction(string actionId)
        {
            if (!_tank) return;
            _tank.SetAction(ActionFactory.Create(actionId, _tank));
        }
    }
}