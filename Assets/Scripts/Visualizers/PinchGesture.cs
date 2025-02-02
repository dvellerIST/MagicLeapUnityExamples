using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.Timeline;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

public class PinchGesture : MonoBehaviour
{
    [SerializeField]
    protected GameObject visual;

    [SerializeField]
    protected Hands hand;

    [SerializeField]
    protected TextMesh text;

    [SerializeField]
    private GameObject markerPrefab;

    [SerializeField]
    private float minPinchDistance = 10f;

    protected InputActionMap actionMap;

    public enum Hands
    {
        Left,
        Right
    }

    private bool hasPinched = false;

    private void Start()
    {
        var ism = FindObjectOfType<InputActionManager>();
        var mapAsset = ism.actionAssets[0];

        actionMap = hand switch
        {
            Hands.Left => mapAsset.FindActionMap("LeftHand"),
            Hands.Right => mapAsset.FindActionMap("RightHand"),
            _ => throw new System.NotImplementedException()
        };
    }

    private void Update()
    {
        if (text != null)
        {
            var lookAway = text.transform.position - Camera.main.transform.position;
            text.transform.rotation = Quaternion.LookRotation(lookAway);
        }

        var pose = actionMap.FindAction("Pinch").ReadValue<PoseState>();
        float value = actionMap.FindAction("PinchValue").ReadValue<float>();
        bool ready = actionMap.FindAction("PinchReady").ReadValue<float>() > 0f;

        visual.transform.localScale = Vector3.one / Mathf.Max(value, 0.2f);
        transform.SetPositionAndRotation(pose.position, pose.rotation);

        visual.SetActive(ready);
        
        if(value > .9f)
        {
            if (!hasPinched)
            {
                var marker = Instantiate(markerPrefab);
                marker.SetActive(true);
                marker.transform.SetPositionAndRotation(visual.transform.position, visual.transform.rotation);
                var mat = marker.GetComponent<Renderer>().material.color = Random.ColorHSV(0f, 1, 1f, 1f, 1f, 1f, .01f, 1f);
                hasPinched = true;
            }
        }
        else
        {
            hasPinched = false;
        }
    }
}
