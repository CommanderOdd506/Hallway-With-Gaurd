using UnityEngine;

public class WinchDoor : MonoBehaviour
{
    public Transform door;
    public Transform topPosition;
    public Transform bottomPosition;
    public WinchInteractable winchInteractable;

    public float raiseSpeed = 2f;
    public float fallSpeed = 3f;

    private float _progress;
    private bool _raising;
    private bool _locked;

    void Update()
    {
        if (_locked) return;

        if (_raising)
            _progress += Time.deltaTime * raiseSpeed;
        else
            _progress -= Time.deltaTime * fallSpeed;

        _progress = Mathf.Clamp01(_progress);

        door.position = Vector3.Lerp(
            bottomPosition.position,
            topPosition.position,
            _progress
        );

        if (_progress >= 1f)
        {
            _locked = true;
            winchInteractable.enabled = false;
        }
    }

    public void WinchUp()
    {
        if (_locked) return;
        _raising = true;
    }

    public void StopWinch()
    {
        _raising = false;
    }
}