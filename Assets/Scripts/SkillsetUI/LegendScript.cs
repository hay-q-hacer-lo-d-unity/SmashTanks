using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SkillsetUI
{
    public class LegendScript : MonoBehaviour
    {
        [SerializeField] private GameObject legendPanel;
        [SerializeField] private TMP_Text titleTMP;
        [SerializeField] private TMP_Text legendTMP;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private float fadeDuration = 0.5f;

        private CanvasGroup _canvasGroup;
        private Coroutine _fadeCoroutine;

        private void Awake()
        {
            if (legendPanel == null)
                return;

            // Ensure the panel has a CanvasGroup for fading
            _canvasGroup = legendPanel.GetComponent<CanvasGroup>();
            if (_canvasGroup == null)
                _canvasGroup = legendPanel.AddComponent<CanvasGroup>();

            _canvasGroup.alpha = 0f;
            legendPanel.SetActive(false);
        }

        // Show with title, text, and optional background sprite
        public void Show(string title, string body, Sprite background = null, float? cooldown = null)
        {
            if (legendPanel == null) return;
            legendPanel.SetActive(true);

            titleTMP.text = title;
            legendTMP.text = body;

            if (cooldown.HasValue)
                legendTMP.text += $"\n<color=#FFA500>Cooldown: {cooldown:F1}s</color>";

            if (backgroundImage)
            {
                backgroundImage.sprite = background;
                backgroundImage.enabled = background != null;
            }

            if (_fadeCoroutine != null) StopCoroutine(_fadeCoroutine);
            _fadeCoroutine = StartCoroutine(FadeCanvasGroup(1f));
        }

        public void Hide()
        {
            if (legendPanel == null) return;

            if (_fadeCoroutine != null)
                StopCoroutine(_fadeCoroutine);

            _fadeCoroutine = StartCoroutine(FadeCanvasGroup(0f, disableOnEnd: true));
        }

        private IEnumerator FadeCanvasGroup(float targetAlpha, bool disableOnEnd = false)
        {
            var startAlpha = _canvasGroup.alpha;
            var elapsed = 0f;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                _canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / fadeDuration);
                yield return null;
            }

            _canvasGroup.alpha = targetAlpha;

            if (disableOnEnd && targetAlpha == 0f)
                legendPanel.SetActive(false);
        }
    }
}
