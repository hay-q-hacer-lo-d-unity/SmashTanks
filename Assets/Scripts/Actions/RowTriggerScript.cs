using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

namespace Actions
{
    public class RowTriggerScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private GameObject buttonsRow;
        [SerializeField] private float slideDuration;
        [SerializeField] private float hiddenX;
        [SerializeField] private float shownX;
        [SerializeField] private GameObject triggerArea;
        private float _currentTargetX;
        private Vector2 _velocity;


        private RectTransform _rowRect;
        private Coroutine _slideRoutine;

        public bool rowHovered;

        private bool _isAnimating;

        private void Awake()
        {
            hiddenX = buttonsRow.GetComponent<RectTransform>().anchoredPosition.x;
            shownX = -hiddenX;
            _rowRect = buttonsRow.GetComponent<RectTransform>();

            var pos = _rowRect.anchoredPosition;
            pos.x = hiddenX;
            _rowRect.anchoredPosition = pos;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            rowHovered = true;
            Sounds.Play2DSound(Sounds.Instance.jump);
            SlideTo(shownX);
        }


        public void OnPointerExit(PointerEventData eventData)
        {
            if (_isAnimating && Mathf.Approximately(_currentTargetX, shownX)) return;

            if (rowHovered) return;
            Sounds.Play2DSound(Sounds.Instance.jump);
            SlideTo(hiddenX);
        }


        public void RequestHide()
        {
            if (!_isAnimating) SlideTo(hiddenX);
        }

        private void SlideTo(float targetX)
        {
            _currentTargetX = targetX;

            if (_slideRoutine != null)
                StopCoroutine(_slideRoutine);

            _slideRoutine = StartCoroutine(SlideCoroutine());
        }



        private IEnumerator SlideCoroutine()
        {
            _isAnimating = true;

            while (true)
            {
                var pos = _rowRect.anchoredPosition;
                var target = new Vector2(_currentTargetX, pos.y);

                _rowRect.anchoredPosition =
                    Vector2.SmoothDamp(pos, target, ref _velocity, slideDuration);

                var newPos = _rowRect.anchoredPosition;
                if (Mathf.Abs(newPos.x - _currentTargetX) < 0.05f)
                {
                    _rowRect.anchoredPosition = target;
                    break;
                }

                yield return null;
            }

            _slideRoutine = null;
            _isAnimating = false;
        }

    }
}
