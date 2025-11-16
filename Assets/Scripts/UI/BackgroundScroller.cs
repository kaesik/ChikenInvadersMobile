using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class BackgroundScroller : MonoBehaviour
    {
        public RawImage img;
        public float speed = 0.03f;

        private void Update()
        {
            var uv = img.uvRect;
            uv.y += speed * Time.deltaTime;
            img.uvRect = uv;
        }
    }
}