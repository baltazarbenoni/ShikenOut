using System.Collections;
using UnityEngine;
//C 2025 Daniel Snapir alias Baltazar Benoni

public class Egg : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] float eggCrackDelay = 1f;
    Rigidbody rBody;
    //[SerializeField] float eggHeight = 0.2f;
    [SerializeField] float eggHeightFixSpeed = 1f;
    LayerMask obstructionMask;

    void OnEnable()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        InitRigBody();
    }
    void InitRigBody()
    {
        obstructionMask = LayerMask.GetMask("Obstruction"); 
        rBody = GetComponent<Rigidbody>();
        if (rBody == null)
        {
            Debug.LogWarning("Couldn't get egg rigidbody!");
            return;
        }
        UpdateRigidBodyOnDrop();
        Invoke("ActivateKinematic", 1f);
    }
    void ActivateKinematic()
    {
        CheckPosition();
        rBody.isKinematic = true;
        GetComponent<Collider>().isTrigger = true;
        transform.rotation = Quaternion.identity;
    }
    void CheckPosition()
    {
        bool obstructPosition = Physics.Raycast(transform.position, Vector3.down, 1f, obstructionMask);
        if (obstructPosition)
        {
            Debug.Log("Destroyed egg! " + this.gameObject.name);
            Destroy(gameObject);
        }
    }
    public void UpdateRigidbodyOnPicking()
    {
        //rBody.constraints = RigidbodyConstraints.None;
        rBody.isKinematic = true;
    }
    public void UpdateRigidBodyOnDrop()
    {
        rBody.constraints = RigidbodyConstraints.FreezePositionX;
        rBody.constraints = RigidbodyConstraints.FreezePositionZ;
        rBody.constraints = RigidbodyConstraints.FreezeRotationZ;
        rBody.constraints = RigidbodyConstraints.FreezeRotationX;
    }
    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Ground")
        {
            MoveEggUp();
        }
    }
    void MoveEggUp()
    {
        Debug.Log("CORRECTING EGG HEIGHT");
        transform.position += Vector3.up * eggHeightFixSpeed * Time.deltaTime;
    }
    public void PickEgg()
    {
        animator.SetBool("withPlayer", true);
    }
    public void DropEgg()
    {
        animator.SetBool("withPlayer", false);
    }
    public void SetEggSpeed(float speed)
    {
        bool idle = Mathf.Abs(speed) <= 0.15f;
        animator.SetBool("idle", idle);
    }
    public void DestroyEgg()
    {
        Actions.EggCrack?.Invoke();
        StartCoroutine(EggDestroy());
    }
    IEnumerator EggDestroy()
    {
        yield return new WaitForSeconds(eggCrackDelay);
        Destroy(this.gameObject);
    }
}
