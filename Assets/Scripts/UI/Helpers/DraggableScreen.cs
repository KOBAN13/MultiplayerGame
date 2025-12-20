using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Helpers
{
    public class DraggableScreen : MonoBehaviour, IPointerDownHandler, IDragHandler
    {
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private Canvas _canvas;
        [SerializeField] private RectTransform _canvasRect;
        private Vector2 _offset;
        
        public void OnPointerDown(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _rectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out _offset
            );
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    _canvas.transform as RectTransform,
                    eventData.position,
                    eventData.pressEventCamera,
                    out var localPoint)) 
                return;
            
            _rectTransform.anchoredPosition = localPoint - _offset;
            ClampToWindow();
        }
        
        private void ClampToWindow()
        {
            var pos = _rectTransform.localPosition;
            var minPosition = _canvasRect.rect.min - _rectTransform.rect.min;
            var maxPosition = _canvasRect.rect.max - _rectTransform.rect.max;

            pos.x = Mathf.Clamp(pos.x, minPosition.x, maxPosition.x);
            pos.y = Mathf.Clamp(pos.y, minPosition.y, maxPosition.y);

            _rectTransform.localPosition = pos;
        }
    }
}