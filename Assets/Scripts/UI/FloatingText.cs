using System.Collections;
using TMPro;
using UnityEngine;

namespace UI
{
    public class FloatingText : MonoBehaviour
    {
        public float moveUpSpeed = 40f;
        public float lifetime = 1.0f;
        public float fadeDuration = 0.4f;

        private TextMeshProUGUI _text;
        private CanvasGroup _group;

        private void Awake()
        {
            _text = GetComponent<TextMeshProUGUI>();
            _group = GetComponent<CanvasGroup>();
        }

        public void Show(string value)
        {
            if (_text) _text.text = value;
            StartCoroutine(Animate());
        }

        private IEnumerator Animate()
        {
            var time = 0f;
            while (time < lifetime)
            {
                transform.localPosition += Vector3.up * (moveUpSpeed * Time.unscaledDeltaTime);
                time += Time.unscaledDeltaTime;
                yield return null;
            }

            var fade = 0f;
            var startAlpha = _group.alpha;

            while (fade < fadeDuration)
            {
                fade += Time.unscaledDeltaTime;
                _group.alpha = Mathf.Lerp(startAlpha, 0f, fade / fadeDuration);
                yield return null;
            }

            Destroy(gameObject);
        }
    }
}