using UnityEngine;
using UnityEngine.UIElements;
//C 2025 Daniel Snapir alias Baltazar Benoni

public class CameraMovement : MonoBehaviour
{
    [SerializeField] Transform Player;
    [SerializeField] float smoothTime = 0.3f; 
    [SerializeField] float cameraHeight = 3f;
    [SerializeField] float zDistance = 3f;
    [SerializeField] Id CameraId = Id.Default;
    public enum Id
    {
        Default,
        Second
    }
    void Awake()
    {
        if(CameraId == Id.Default)
        {
            zDistance *= -1;
        }
    }
    void Start()
    {
        transform.position = CameraId == Id.Default ? GetPosition() : GetSecondCamPosition();  
        transform.forward = Player.position - transform.position;
    }

    void Update()
    {
        MoveCamera();
    }
    Vector3 GetPosition()
    {
        return new Vector3(Player.position.x, cameraHeight + Player.position.y, Player.position.z - zDistance);
    }
    Vector3 GetSecondCamPosition()
    {
        return new Vector3(Player.position.x, cameraHeight + Player.position.y, Player.position.z - zDistance);
    }
    void MoveCamera()
    {
        Vector3 target = CameraId == Id.Default ? GetPosition() : GetSecondCamPosition();
        Vector3 velocity = Vector3.zero;
        transform.position = Vector3.SmoothDamp(transform.position, target, ref velocity, smoothTime);
    }
    public void AlignCameraOnChange()
    {
        if(this.CameraId == Id.Default)
        {
            transform.position = GetPosition();
        }
        else if(this.CameraId == Id.Second)
        {
            transform.position = GetSecondCamPosition();
        }
        transform.forward = Player.position - transform.position;
    }
}
