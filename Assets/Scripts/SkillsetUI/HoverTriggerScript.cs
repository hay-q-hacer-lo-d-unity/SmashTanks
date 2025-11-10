using UnityEngine;
using UnityEngine.EventSystems;

namespace SkillsetUI
{
    public class HoverTriggerScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Tooltipable tooltipable;
        private LegendScript _legend;

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!tooltipable) return;
            Debug.Log("HoverTriggerScript: OnPointerEnter called for " + tooltipable.name);
            tooltipable.ShowTooltip();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (tooltipable)
                tooltipable.HideTooltip();
        }
    }
}