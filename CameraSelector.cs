using System;
using System.Collections.Generic;   
using UnityEngine;
//C 2025 Daniel Snapir alias Baltazar Benoni

public class CameraSelector : MonoBehaviour
{
    CameraMovement.Id activeCamera;
    public CameraMovement.Id ActiveCamera {get; private set;}  
    [SerializeField] List<GameObject> cameras = new List<GameObject>();
    int enumLength = Enum.GetValues(typeof(CameraMovement.Id)).Length; 
    void Awake()
    {
        activeCamera = CameraMovement.Id.Default;
    }
    void Start()
    {
        InitCamera(activeCamera);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            ChangeCamera(activeCamera);
        }
    }
    void ChangeCamera(CameraMovement.Id currentlyActive)
    {
        int activeIndex = (int)activeCamera;
        int nextIndex = activeIndex + 1 < enumLength ? activeIndex + 1 : 0;
        cameras[nextIndex].SetActive(true);
        cameras[nextIndex].GetComponent<CameraMovement>().AlignCameraOnChange();
        cameras[activeIndex].SetActive(false);
        activeCamera = (CameraMovement.Id)nextIndex;
    }
    void InitCamera(CameraMovement.Id currentlyActive)
    {
        int activeIndex = (int)activeCamera;
        int nextIndex = activeIndex + 1 < enumLength ? activeIndex + 1 : 0;
        cameras[activeIndex].SetActive(true);
        cameras[activeIndex].GetComponent<CameraMovement>().AlignCameraOnChange();
        cameras[nextIndex].SetActive(false);
    }
}
