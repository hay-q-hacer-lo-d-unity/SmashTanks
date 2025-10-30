using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI
{
    public abstract class Tooltipable : MonoBehaviour
    {
        [FormerlySerializedAs("statName")]
        [Header("Attribute Info")]
        [SerializeField] protected new string name = "New Attribute";
        [TextArea(5, 20)] [SerializeField] protected string description = "Attribute description";
        protected LegendScript Legend;
        
        public void ShowTooltip()
        {
            var background = GetComponentInChildren<Image>();
            Legend?.Show(name, description, background.sprite);
        }

        public void HideTooltip()
        {
            Legend?.Hide();
        }
    }
}