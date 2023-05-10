using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardMode
{
    ChangeOn = 0,
    Changeing = 1,
    ChangeOff = 2,
    MergeModeReady = 3,
    Merge = 4,
}

public class CardController : MonoBehaviour
{
    public GameObject _targetObject;
    public GameObject _targetObjectModel;

    private InputManager _inputManager;

    private IEnumerator _resetCoroutine;
    private float _turnnSmoothVelocity;
    private float _changeDistance = 2f;

    private bool _isCardReady = false;

    private CardMode _curMode = CardMode.ChangeOff;
    public CardMode CurMode
    {
        get
        {
            return _curMode;
        }
        set
        {
            if (_curMode == CardMode.Changeing)
            {
                _curMode = value;
                return;
            }
            else
            {
                if (!_isCardReady)
                    _curMode = CardMode.ChangeOff;

                else
                {
                    StartCoroutine(ChangeMode(value));
                    _curMode = CardMode.Changeing;
                }
            }
        }
    }

    public void Init()
    {
        _isCardReady = true;
        if (_inputManager == null)
            _inputManager = GameManager.instance.InputManager;

        _inputManager.WhenTouchMove += UpdateSwipe;
        _inputManager.WhenTouchEnd += ResetPosition;
    }

    private void OnDisable()
    {
        if(_isCardReady)
        {
            _targetObject.transform.eulerAngles = Vector3.zero;
            _isCardReady = false;
            CurMode = CardMode.ChangeOff;
            _inputManager.WhenTouchMove -= UpdateSwipe;
            _inputManager.WhenTouchEnd -= ResetPosition;
        }
    }

    private void UpdateSwipe(Vector2 startTouch, Vector2 endTouch)
    {
        if (!_isCardReady) return;

        if (_resetCoroutine != null)
        {
            StopCoroutine(_resetCoroutine);
            _resetCoroutine = null;
        }

        float xAngle = startTouch.x - endTouch.x;
        _targetObject.transform.rotation = Quaternion.Slerp(_targetObject.transform.rotation, Quaternion.Euler(0f, xAngle, 0f), 3f * Time.deltaTime);
    }

    private void ResetPosition(Vector2 startTouch, Vector2 endTouch)
    {
        //두 값이 비슷한가? 비슷하면 변신
        if (Vector2.Distance(startTouch, endTouch) < _changeDistance)
        {
            CurMode = CardMode.ChangeOn;
        }

        _resetCoroutine = ResetPoistion();
        StartCoroutine(_resetCoroutine);
    }

    private IEnumerator ResetPoistion()
    {
        while (_targetObject.transform.eulerAngles.y != 0)
        {
            _targetObject.transform.rotation = Quaternion.Slerp(_targetObject.transform.rotation, Quaternion.Euler(0f, 0f, 0f), 3f * Time.deltaTime);
            yield return null;
        }
    }

    private IEnumerator ChangeMode(CardMode mode)
    {
        switch (mode)
        {
            case CardMode.ChangeOn:

                Debug.Log("특수 전기 효과 실행");

                while (_targetObjectModel.transform.eulerAngles.x != 270)
                {
                    _targetObjectModel.transform.rotation = Quaternion.Slerp(_targetObjectModel.transform.rotation, Quaternion.Euler(-90, 0, 0f), 3f * Time.deltaTime);

                    yield return null;
                }
                CurMode = CardMode.ChangeOn;
                yield return new WaitForSeconds(2);

                CurMode = CardMode.ChangeOff;
                break;
            case CardMode.ChangeOff:

                Debug.Log("특수 전기 효과 종료");

                while (_targetObjectModel.transform.eulerAngles.x != 0)
                {
                    _targetObjectModel.transform.rotation = Quaternion.Slerp(_targetObjectModel.transform.rotation, Quaternion.Euler(0f, 0, 0f), 3f * Time.deltaTime);

                    yield return null;
                }
                CurMode = CardMode.ChangeOff;
                break;
        }
    }
}