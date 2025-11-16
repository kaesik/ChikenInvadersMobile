using System.Collections;
using TMPro;
using UnityEngine;

namespace UI
{
    public class WaveIntroUI : MonoBehaviour
    {
        #region UI
        [Header("UI")]
        public CanvasGroup canvasGroup;
        public TextMeshProUGUI waveText;
        public TextMeshProUGUI dangerText;
        public TextMeshProUGUI meteorText;
        #endregion

        #region Timing
        [Header("Timing")]
        public float fadeInTime = 0.2f;
        public float holdTime = 1.0f;
        public float fadeOutTime = 0.3f;
        #endregion

        private Coroutine _routine;

        private void Awake()
        {
            if (!canvasGroup)
                canvasGroup = GetComponent<CanvasGroup>();

            if (canvasGroup)
                canvasGroup.alpha = 0f;

            if (dangerText) dangerText.color = Color.red;
            if (meteorText) meteorText.color = Color.red;
        }

        public void ShowWave(int waveNumber, bool meteorWave)
        {
            if (_routine != null)
                StopCoroutine(_routine);

            gameObject.SetActive(true);
            _routine = StartCoroutine(ShowRoutine(waveNumber, meteorWave));
        }

        private IEnumerator ShowRoutine(int waveNumber, bool meteorWave)
        {
            if (!canvasGroup) yield break;

            if (waveText)
                waveText.text = "WAVE " + waveNumber;

            if (meteorWave)
            {
                if (dangerText)
                {
                    dangerText.text = "DANGER";
                    dangerText.gameObject.SetActive(true);
                }

                if (meteorText)
                {
                    meteorText.text = "METEOR RAIN";
                    meteorText.gameObject.SetActive(true);
                }
            }
            else
            {
                if (dangerText) dangerText.gameObject.SetActive(false);
                if (meteorText) meteorText.gameObject.SetActive(false);
            }

            var t = 0f;
            while (t < fadeInTime)
            {
                t += Time.unscaledDeltaTime;
                canvasGroup.alpha = Mathf.Clamp01(t / fadeInTime);
                yield return null;
            }

            canvasGroup.alpha = 1f;

            t = 0f;
            while (t < holdTime)
            {
                t += Time.unscaledDeltaTime;
                yield return null;
            }

            t = 0f;
            while (t < fadeOutTime)
            {
                t += Time.unscaledDeltaTime;
                canvasGroup.alpha = 1f - Mathf.Clamp01(t / fadeOutTime);
                yield return null;
            }

            canvasGroup.alpha = 0f;
        }
    }
}
