using System.Collections.Generic;
using Actions.ulti;
using SkillsetUI;
using Tank;
using UnityEngine;

namespace Actions
{
    public class ActionSelectorScript : MonoBehaviour
    {
        [SerializeField] private LegendScript legend;
        [Header("Buttons")]
        [SerializeField] private List<ActionButtonScript> actionButtons = new();
        [SerializeField] private List<UltiButtonScript> ultiButtons = new();
        
        private TankScript _tank;

        public void SetTank(TankScript newTank)
        {
            _tank = newTank;

            foreach (var btn in actionButtons) btn.Initialize(this, _tank, legend);

            foreach (var ultiBtn in ultiButtons)
            {
                ultiBtn.Initialize(this, _tank, legend);
            }
            
            SelectAction("action_missile");
            UpdateButtons();
        }

        private void UpdateButtons()
        {
            if (!_tank) return;
            foreach (var btn in actionButtons) btn.UpdateState();
            foreach (var ultiBtn in ultiButtons) ultiBtn.UpdateState();
        }

        public void SelectAction(string actionId)
        {
            if (!_tank) return;
            _tank.SetAction(ActionFactory.Create(actionId, _tank));
        }
    }
}