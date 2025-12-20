using Db;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using VContainer;

namespace UI.Helpers
{
    public class FadeButtonAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private FadeButtonAnimationData _config;
        
        private Tween _tween;
        private CanvasGroup _group;
        
        private void Start()
        {
            _group = GetComponent<CanvasGroup>();
            
            if (_group == null)
                _group = gameObject.AddComponent<CanvasGroup>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _tween?.Kill();
            _tween = _group.DOFade(_config.OnHoverAlpha, _config.FadeTime);
        }
        
        public void OnPointerExit(PointerEventData eventData)
        {
            _tween?.Kill();
            _tween = _group.DOFade(1.0f, _config.FadeTime);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _group.alpha = _config.OnClickAlpha;
        }
        
        public void OnPointerUp(PointerEventData eventData)
        {
            _group.alpha = 1.0f;
        }
    }
}