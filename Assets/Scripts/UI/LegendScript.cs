using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace UI
{
    public class LegendScript : MonoBehaviour
    {
        [SerializeField] private GameObject legendPanel;
        [SerializeField] private TMP_Text titleTMP;
        [SerializeField] private TMP_Text legendTMP;
        [SerializeField] private Image backgroundImage; // New: background image
        [SerializeField] private float fadeDuration = 0.5f;

        private CanvasGroup canvasGroup;
        private Coroutine fadeCoroutine;

        private void Awake()
        {
            if (legendPanel == null)
                return;

            // Ensure the panel has a CanvasGroup for fading
            canvasGroup = legendPanel.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = legendPanel.AddComponent<CanvasGroup>();

            canvasGroup.alpha = 0f;
            legendPanel.SetActive(false);
        }

        // Show with title, text, and optional background sprite
        public void Show(string title, string body, Sprite background = null)
        {
            if (legendPanel == null) return;

            legendPanel.SetActive(true);

            if (titleTMP != null)
                titleTMP.text = title;

            if (legendTMP != null)
                legendTMP.text = body;

            if (backgroundImage != null)
            {
                if (background != null)
                {
                    backgroundImage.sprite = background;
                    backgroundImage.enabled = true;
                }
                else
                {
                    backgroundImage.enabled = false;
                }
            }

            if (fadeCoroutine != null)
                StopCoroutine(fadeCoroutine);

            fadeCoroutine = StartCoroutine(FadeCanvasGroup(1f));
        }

        public void Hide()
        {
            if (legendPanel == null) return;

            if (fadeCoroutine != null)
                StopCoroutine(fadeCoroutine);

            fadeCoroutine = StartCoroutine(FadeCanvasGroup(0f, disableOnEnd: true));
        }

        private IEnumerator FadeCanvasGroup(float targetAlpha, bool disableOnEnd = false)
        {
            float startAlpha = canvasGroup.alpha;
            float elapsed = 0f;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / fadeDuration);
                yield return null;
            }

            canvasGroup.alpha = targetAlpha;

            if (disableOnEnd && targetAlpha == 0f)
                legendPanel.SetActive(false);
        }
    }
}
