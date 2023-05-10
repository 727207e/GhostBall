using easyar;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageTrackingManager : MonoBehaviour
{
    [SerializeField] private List<ImageTargetController> _imageTargetController;
    [SerializeField] private GameObject _popUpImage;
    [SerializeField] private ImageTrackerFrameFilter _imageTrackerFrameFilter;

    private CardController _targetImage;
    private CardController _secondTargetImage;

    public void Start()
    {
        foreach (var target in _imageTargetController)
        {
            target.TargetFound += () => TrackingImage(target);
            target.TargetLost += () => TrackingImageLost(target);
        }

        GameManager.instance.InputManager.WhenDoubleTouch += TargetImageChangeMerge;
    }

    private void TrackingImage(ImageTargetController target)
    {
        if (_targetImage == null)
        {
            _targetImage = target.GetComponent<CardController>();
        }

        else
        {
            _secondTargetImage = target.GetComponent<CardController>();
            if (_targetImage.CurMode != CardMode.MergeModeReady)
            {
                _secondTargetImage._targetObject.SetActive(false);
            }
            return;
        }

        ARPopUp();
    }

    private void TrackingImageLost(ImageTargetController target)
    {
        _targetImage = null;
        StopAllCoroutines();
    }

    private void TargetImageChangeMerge()
    {
        if(_targetImage != null)
        {
            _targetImage.CurMode = CardMode.MergeModeReady;
            StartCoroutine(MergeReady());
        }
    }

    private IEnumerator MergeReady()
    {
        while(_targetImage != null)
        {
            if(_secondTargetImage != null)
            {
                if(Vector3.Distance(_targetImage._targetObject.transform.position, _secondTargetImage._targetObject.transform.position) < 0.1f)
                {
                }
            }

            yield return null;
        }
    }

    private void ARPopUp()
    {
        _popUpImage.SetActive(true);
        GameManager.instance.InputManager.WhenTouchBegan += PopUpTouch;
    }

    private void PopUpTouch()
    {
        _targetImage.Init();
        _popUpImage.SetActive(false);
        GameManager.instance.InputManager.WhenTouchBegan -= PopUpTouch;
    }
}
