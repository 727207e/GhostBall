using System;
using System.Collections;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private Vector2 startTouch = Vector2.zero;
    private Vector2 endTouch;

    private float _timeCheck = 0f;
    private IEnumerator _timeCheckCor;

    //터치되면 호출 (Start , EndPoint)
    public Action<Vector2,Vector2> WhenTouchMove;
    public Action<Vector2, Vector2> WhenTouchEnd;
    public Action WhenTouchBegan;
    public Action WhenDoubleTouch;

    private void Update()
    {
        UpdateInput();
    }

    private void UpdateInput()
    {
#if UNITY_EDITOR
        if(Input.GetMouseButtonDown(0))
        {
            WhenTouchBegan?.Invoke();
        }

        else if (Input.GetMouseButton(0))
        {
            if (startTouch == Vector2.zero)
                startTouch = Input.mousePosition;

            endTouch = Input.mousePosition;

            WhenTouchMove?.Invoke(startTouch, endTouch);
        }

        else if (Input.GetMouseButtonUp(0))
        {
            WhenTouchEnd?.Invoke(startTouch, endTouch);
            startTouch = Vector2.zero;

            if(_timeCheckCor == null)
            {
                _timeCheckCor = DoubleClickTimeCheck();
                StartCoroutine(_timeCheckCor);
            }
            else
            {
                _timeCheck = 0;
                StopCoroutine(_timeCheckCor);
                _timeCheckCor = null;
                WhenDoubleTouch?.Invoke();
            }
        }
#endif

#if UNITY_ANDROID
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                WhenTouchBegan?.Invoke();
            }

            if (touch.phase == TouchPhase.Moved)
            {
                if (startTouch == Vector2.zero)
                    startTouch = Input.mousePosition;

                endTouch = Input.mousePosition;

                WhenTouchMove?.Invoke(startTouch, endTouch);
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                startTouch = Vector2.zero;
                WhenTouchEnd?.Invoke(startTouch, endTouch);

                if (_timeCheckCor == null)
                {
                    _timeCheckCor = DoubleClickTimeCheck();
                    StartCoroutine(_timeCheckCor);
                }
                else
                {
                    _timeCheck = 0;
                    StopCoroutine(_timeCheckCor);
                    _timeCheckCor = null;
                    WhenDoubleTouch?.Invoke();
                }
            }
        }
#endif
    }

    private IEnumerator DoubleClickTimeCheck()
    {
        while(_timeCheck < 1.0f)
        {
            _timeCheck += Time.deltaTime;
            yield return null;
        }

        _timeCheckCor = null;
        _timeCheck = 0;
    }
}