using System.Collections.Generic;
using System;
using System.Collections;
using UnityEngine;
//C 2025 Daniel Snapir alias Baltazar Benoni

public class EggManager : MonoBehaviour
{
    public float PlayerSpeed;
    public float playerMoveInput;
    public Vector3 PlayerMovement;
    public bool playerMoving;
    [SerializeField] GameObject Barn;
    private Barn barnScript;
    [SerializeField] int maxEggAmount = 5;
    [SerializeField] float eggSlowMovementLimit = 0.5f;
    List<GameObject> eggs = new();
    public int EggCount { get; private set; }
    [SerializeField] float eggDistance = 1f;
    //Komento jolla poimii munan.
    [SerializeField] KeyCode eggPickUp = KeyCode.E;
    //Komento jolla pudottaa yhden munan.
    [SerializeField] KeyCode eggDrop = KeyCode.Q;
    bool eggDropOn;
    int groundLayer;
    float eggHeight = 0.2f;
    float foxAttackTimer;
    [SerializeField] float foxAttackLimit = 1f;
    void OnEnable()
    {
        InitReferences();
    }
    void InitReferences()
    {
        groundLayer = LayerMask.NameToLayer("Ground");
        if (Barn == null)
        {
            Barn = GameObject.Find("Barn");
        }
        barnScript = Barn.GetComponent<Barn>();

    }
    void Start()
    {

    }
    void Update()
    {
        if (eggs.Count > 0)
        {
            MoveEggs();
            EggDropOnCommand();
        }
        foxAttackTimer += Time.deltaTime;
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "FoxAttack")
        {
            FoxAttack();
            Actions.ChangeAudio?.Invoke(1, "hurt");
        }
    }
    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Egg")
        {
            EggPickUp(other);
        }
        else if (other.gameObject.tag == "Barn" && eggs.Count > 0)
        {
            Actions.EggDropAll(eggs.Count);
            //Actions.EggDrop();
            EggsDropAll();
            Actions.ChangeAudio(1, "drop");
            EggCount = eggs.Count;
        }
    }
    void EggPickUp(Collider other)
    {
        if (Input.GetKeyDown(eggPickUp) && eggs.Count < maxEggAmount)
        {
            GameObject thisEgg = other.gameObject;
            thisEgg.GetComponent<Egg>().UpdateRigidbodyOnPicking();
            eggs.Add(thisEgg);
            EggAnimationPickDrop(thisEgg, true);
            thisEgg.transform.position = transform.position - transform.forward * eggDistance * eggs.Count;
            EggCount = eggs.Count;
            other.enabled = false;
            Debug.Log("collected new egg!" + EggCount);
            Actions.EggPickUp?.Invoke();
            Actions.ChangeAudio(1, "pickup");
        }
        if (CheckIfNameInList(other.gameObject.name))
        {
            Debug.Log("List already contains this!");
        }
    }
    bool CheckIfNameInList(string name)
    {
        foreach (GameObject egg in eggs)
        {
            if (egg.name == name)
            {
                return true;
            }
        }
        return false;
    }
    void EggsDropAll()
    {
        foreach (GameObject egg in eggs)
        {
            //egg.GetComponent<Collider>().enabled = true;
            if (barnScript != null)
            {
                barnScript.eggCollected.Add(egg);
                GetEggScript(egg).SetEggSpeed(1f);
            }
        }
        eggs.Clear();
        EggCount = 0;
        Actions.EggDrop();
    }
    void EggDropOnCommand()
    {
        if (Input.GetKeyDown(eggDrop) && !eggDropOn)
        {
            eggDropOn = true;
            EggDropSingle();
            Actions.ChangeAudio(1, "drop");
            Actions.EggDrop();
            StartCoroutine(EggDropDelay());
        }
    }
    void EggDropSingle()
    {
        GameObject thisEgg = eggs[eggs.Count - 1];
        thisEgg.GetComponent<Collider>().enabled = true;
        //thisEgg.GetComponent<Egg>().UpdateRigidBodyOnDrop();
        EggAnimationPickDrop(eggs[eggs.Count - 1], false);
        eggs.RemoveAt(eggs.Count - 1);
        EggCount = eggs.Count;
    }
    void FoxAttack()
    {
        Debug.Log("Attack timer is " + foxAttackTimer);
        if (eggs.Count > 0 && foxAttackTimer > foxAttackLimit)
        {
            Debug.Log("Fox attacked egg!");
            foxAttackTimer = 0;
            GameObject droppedEgg = eggs[eggs.Count - 1];
            EggAnimationPickDrop(droppedEgg, false);
            droppedEgg.GetComponent<Egg>().DestroyEgg();
            eggs.Remove(droppedEgg);
            EggCount = eggs.Count;
            Actions.UpdateChickenSpeed?.Invoke();
        }
        else if(foxAttackTimer > foxAttackLimit)
        {
            Debug.Log("Game over!!!");
        }
    }
    IEnumerator EggDropDelay()
    {
        yield return new WaitForSeconds(0.2f);
        eggDropOn = false;
    }
    //Move eggs and apply player speed to egg animation.
    void MoveEggs()
    {
        RotateEggs(this.transform, eggs[0].transform);
        EggAnimationSpeed(eggs[0]);
        for (int i = 1; i < eggs.Count; i++)
        {
            RotateEggs(eggs[i - 1].transform, eggs[i].transform);
            EggAnimationSpeed(eggs[i]);
        }
    }
    void RotateEggs(Transform objToFollow, Transform objFollowing)
    {
        Vector3 rope = objFollowing.position - objToFollow.position;

        Vector3 ropeNormalized = Vector3.Normalize(rope);
        objFollowing.transform.forward = -ropeNormalized;
        //Move egg closer to player.
        if (rope.magnitude > eggDistance)
        {
            float speed = rope.magnitude - eggDistance < eggSlowMovementLimit ? PlayerSpeed * (rope.magnitude - eggDistance) : PlayerSpeed;
            objFollowing.position -= rope * Time.deltaTime * speed;
        }
        //Adjust egg height
        //objFollowing.position += CalculateEggHeightIncrement(objFollowing);

        //Calculate the angle the egg needs to turn.
        float angle = Vector3.SignedAngle(ropeNormalized, -transform.forward, Vector3.up) * Time.deltaTime;
        objFollowing.RotateAround(objToFollow.position, Vector3.up, angle);
    }
    void EggAnimationSpeed(GameObject egg)
    {
        Egg eggScript = GetEggScript(egg);
        if (eggScript == null)
        {
            return;
        }
        else
        {
            eggScript.SetEggSpeed(playerMoveInput);
        }
    }
    //Get the egg height adjustement vector.
    Vector3 CalculateEggHeightIncrement(Transform egg)
    {
        float heightIncrement;
        Physics.Raycast(egg.position, Vector3.down, out RaycastHit hit, 1f, groundLayer);
        if (hit.distance > 0.2f)
        {
            heightIncrement = -hit.distance + eggHeight;
            return new Vector3(0, heightIncrement, 0) * Time.deltaTime;
        }
        return Vector3.zero;
    }
    void EggAnimationPickDrop(GameObject egg, bool picking)
    {
        Egg eggScript = GetEggScript(egg);
        if (eggScript == null)
        {
            return;
        }
        else if (picking)
        {
            eggScript.PickEgg();
        }
        else
        {
            eggScript.DropEgg();
        }
    }
    Egg GetEggScript(GameObject egg)
    {
        try
        {
            Egg eggScript = egg.GetComponent<Egg>();
            return eggScript;
        }
        catch (Exception ex)
        {
            Debug.LogWarning("error! cant animate, 'Egg'-script missing!" + ex.Message);
            return null;
        }
    }
}
