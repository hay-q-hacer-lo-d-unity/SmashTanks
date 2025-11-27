using Actions;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace SkillsetUI
{
    public abstract class Tooltipable : MonoBehaviour
    {
        [FormerlySerializedAs("statName")]
        [Header("Attribute Info")]
        [SerializeField] protected string id = "New Attribute";
        [SerializeField] protected string displayName = "Attribute Name";
        [TextArea(5, 20)] [SerializeField] protected string description = "Attribute description";
        protected LegendScript Legend;
        
        public virtual void ShowTooltip()
        {
            var background = GetComponentInChildren<Image>();
            Legend?.Show(displayName, description, background.sprite);
        }

        public void HideTooltip()
        {
            Legend?.Hide();
        }
    }
}