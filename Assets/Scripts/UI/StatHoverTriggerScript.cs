using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class StatHoverTriggerScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private StatRowScript row; // reference the parent row

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (row)
            {
                Debug.Log("Showing tooltip for stat: " + row.StatName);
                row.ShowTooltip();
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (row)
                row.HideTooltip();
        }
    }
}