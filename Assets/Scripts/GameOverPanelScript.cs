using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverPanelScript : MonoBehaviour
{
    public TMP_Text winnerText;

    [SerializeField] private GameObject panel;
    public Button playAgainButton;
    private CanvasGroup _canvasGroup;
    private Coroutine _fadeCoroutine;

    private void Awake()
    {
        if (panel == null)
            return;

        // Ensure the panel has a CanvasGroup for fading
        _canvasGroup = panel.GetComponent<CanvasGroup>();
        if (_canvasGroup == null)
            _canvasGroup = panel.AddComponent<CanvasGroup>();

        _canvasGroup.alpha = 0f;
        panel.SetActive(false);
            
    }

    public void Show(string winner)
    {
        winnerText.text = (winner != null ? $"Player {winner}" : "No one") + " wins!";
        gameObject.SetActive(true);
        if (_fadeCoroutine != null) StopCoroutine(_fadeCoroutine);
        _fadeCoroutine = StartCoroutine(FadeCanvasGroup(1f));
    }
    
    private IEnumerator FadeCanvasGroup(float targetAlpha, bool disableOnEnd = false)
    {
        var startAlpha = _canvasGroup.alpha;
        var elapsed = 0f;

        while (elapsed < 0.25f)
        {
            elapsed += Time.deltaTime;
            _canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / 0.25f);
            yield return null;
        }

        _canvasGroup.alpha = targetAlpha;

        if (disableOnEnd && targetAlpha == 0f)
            panel.SetActive(false);
    }
}
