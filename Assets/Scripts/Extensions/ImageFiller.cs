using Cysharp.Threading.Tasks;
using UnityEngine;

namespace UI.Future_Expansions
{
    public class ImageFiller : MonoBehaviour
    {
        [SerializeField] private float fillSpeed = 0.5f;
        [SerializeField]private bool isResettable;
        
        private UnityEngine.UI.Image _image;
        private bool _isFilling;

        void Awake()
        {
            _image = GetComponent<UnityEngine.UI.Image>();
            _image.fillAmount = 0f;
        }
        
        public async void FillImage()
        {
            _isFilling = true;
            while (_image.fillAmount < 1f)
            {
                _image.fillAmount += fillSpeed * Time.deltaTime;
                await UniTask.Yield();
            }
            _image.fillAmount = 1f;
            _isFilling = false;
            if (isResettable)
            {
                await UniTask.Delay(1000);
                if (!_isFilling)
                {
                    _image.fillAmount = 0f;
                }
            }
        }
    }
}
