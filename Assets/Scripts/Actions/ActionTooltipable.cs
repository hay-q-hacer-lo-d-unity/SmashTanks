using SkillsetUI;
using UnityEngine;
using UnityEngine.UI;

namespace Actions
{
    [RequireComponent(typeof(Button))]
    public class ActionTooltipable : Tooltipable
    {
        [Header("Action Info")]
        [SerializeField] private string actionId = "shoot";
        [SerializeField] private float magickaCost = 10f;
        [SerializeField] private Sprite icon;

        private void Awake()
        {
            // Optionally autopopulate name from actionId
            if (string.IsNullOrEmpty(name))
                name = char.ToUpper(actionId[0]) + actionId.Substring(1);

            // Ensure we have a reference to the global legend
            Legend ??= FindObjectOfType<LegendScript>(true);
        }

        public override void ShowTooltip()
        {
            // Override default tooltip to include magicka info
            var background = GetComponentInChildren<Image>();
            var fullDescription = $"{description}\n\n<color=#00FFFF>Cost: {magickaCost} MP</color>";

            Legend?.Show(name, fullDescription, icon ? icon : background?.sprite);
        }
    }
}