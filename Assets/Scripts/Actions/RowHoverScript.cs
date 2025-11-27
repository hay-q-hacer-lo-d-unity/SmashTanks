using UnityEngine;

namespace Actions
{
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class RowHoverScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private RowTriggerScript trigger;

        public void OnPointerEnter(PointerEventData eventData)
        {
            trigger.rowHovered = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            trigger.rowHovered = false;
            trigger.RequestHide();
        }
    }
}
