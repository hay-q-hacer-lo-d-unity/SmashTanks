using Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SkillsetUI
{
    public class PlayerQuantityScript : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button increaseButton;
        [SerializeField] private Button decreaseButton;
        [SerializeField] private TMP_Text playerCountText;
        
        [Header("Players")]
        [SerializeField] private int minPlayers = 2;
        [SerializeField] private int maxPlayers = 8;
        [SerializeField] private int currentPlayers = 2;
        
        private void Start()
        {
            increaseButton.onClick.AddListener(IncreasePlayers);
            decreaseButton.onClick.AddListener(DecreasePlayers);
            confirmButton.onClick.AddListener(() =>
            {
                GameManagerScript.Instance.SetPlayerCount(currentPlayers);
                GameManagerScript.Instance.ShowSkillsetScreen();
            });
            decreaseButton.interactable = false;
        }
        
        private void IncreasePlayers()
        {
            if (currentPlayers < maxPlayers)
            {
                playerCountText.text = (++currentPlayers).ToString();
                SetButtonsInteractable(true, true);
                if (currentPlayers == maxPlayers)
                {
                    SetButtonsInteractable(false, true);
                }
            }
        }
        
        private void DecreasePlayers()
        {
            if (currentPlayers > minPlayers)
            {
                playerCountText.text = (--currentPlayers).ToString();
                SetButtonsInteractable(true, true);
                if (currentPlayers == minPlayers)
                {
                   SetButtonsInteractable(true, false);
                }
            }
        }

        private void SetButtonsInteractable(bool canIncrease, bool canDecrease)
        {
            if (increaseButton) increaseButton.interactable = canIncrease;
            if (decreaseButton) decreaseButton.interactable = canDecrease;
        }
    }
}