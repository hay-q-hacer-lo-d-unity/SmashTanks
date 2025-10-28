using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class HoverTriggerScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Tooltipable tooltipable;
        private LegendScript _legend;

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!tooltipable) return;
            tooltipable.ShowTooltip();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (tooltipable)
                tooltipable.HideTooltip();
        }
    }
}