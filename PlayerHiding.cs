using UnityEngine;
//C 2025 Daniel Snapir alias Baltazar Benoni

public class PlayerHiding : MonoBehaviour
{
    public bool hiding;
    int defaultMask;
    int hidingMask;

    void Awake()
    {
        defaultMask = LayerMask.NameToLayer("Player"); 
        hidingMask = LayerMask.NameToLayer("PlayerHidden");
        CheckMasks();
    }
    void CheckMasks()
    {
        if (defaultMask < 0 || hidingMask < 0)
        {
            Debug.LogWarning("LayerMask not found!");
        }
    }
    void Update()
    {
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "HidingPlace")
        {
            hiding = true;
            ChangeLayerMask();
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "HidingPlace")
        {
            hiding = false;
            ChangeLayerMask();
        }
    }
    void ChangeLayerMask()
    {
        int newMask = hiding ? hidingMask : defaultMask;
        this.gameObject.layer = newMask;
        Actions.PlayerHidden?.Invoke(hiding);
    }
}
