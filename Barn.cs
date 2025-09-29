using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
//C 2025 Daniel Snapir alias Baltazar Benoni

public class Barn : MonoBehaviour
{
    [SerializeField] int eggCount = 0;
    [SerializeField] int eggsRemaining = 0;
    [SerializeField] EggSpawn eggSpawner;
    [SerializeField] float eggSpeed = 1f;
    public List<GameObject> eggCollected = new ();
    [SerializeField] float barnCenterLimit = 0.8f;
    [SerializeField] Transform barnCenter;
    void OnEnable()
    {
        Actions.EggDropAll += EggDrop;
        if (eggSpawner == null)
        {
            eggSpawner = GameObject.Find("EggSpawn").GetComponent<EggSpawn>();
        }
    }
    void OnDisable()
    {
        Actions.EggDropAll -= EggDrop;
    }
    void Update()
    {
        if (eggCollected.Count > 0)
        {
            MoveEgg();
        }
    } 
    void EggDrop(int droppedEggs)
    {
        eggCount += droppedEggs;
        eggsRemaining = eggSpawner.EggCount - eggCount;
        InitRigidbodyStatus();
    }
    void InitRigidbodyStatus()
    {
        foreach (GameObject egg in eggCollected)
        {
            egg.GetComponent<Rigidbody>().isKinematic = false;
            egg.GetComponent<Collider>().isTrigger = false;
        }
    }
    void MoveEgg()
    {
        Debug.Log("Trying to move eggs");
        //foreach(GameObject egg in eggCollected)
        for(int i = 0; i < eggCollected.Count; i++)
        {
            if(eggCollected[i] == null)
            {
                return;
            }
            Debug.Log("Moving eggs");
            GameObject egg = eggCollected[i];
            //Vector3 direction = new Vector3(transform.position.x, egg.transform.position.y, transform.position.z) - egg.transform.position;
            Vector3 direction = barnCenter.position - egg.transform.position;
            egg.transform.position += Vector3.Normalize(direction) * eggSpeed * Time.deltaTime;
            RemoveEggWhenInBarn(egg);
        }
    }
    void RemoveEggWhenInBarn(GameObject egg)
    {
        bool inBarn = (egg.transform.position - transform.position).sqrMagnitude < barnCenterLimit;
        if(inBarn)
        {
            eggCollected.Remove(egg);
        }
    }
}
