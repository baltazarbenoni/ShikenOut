using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Unity.VisualScripting;

public class EggSpawn : MonoBehaviour
{
    [SerializeField] int eggCount = 16;
    public int EggCount { get { return eggCount; } }
    [SerializeField] Transform mapCenter;
    [SerializeField] GameObject egg;
    [SerializeField] int mapSize = 40;
    [SerializeField] int areaCount = 4;
    [SerializeField] float eggHeight = 0.4f;
    float eggAdjustHeight = 0.2f;
    List<Vector3> forbiddenValues = new();
    float eggsOneArea;
    Map map;
    LayerMask groundLayer;

    void Awake()
    {
        map = new Map(mapSize, areaCount);
        eggsOneArea = eggCount / areaCount;
    }
    void OnEnable()
    {
        groundLayer = LayerMask.GetMask("Ground");
        SpawnEggsToMap();
    }
    void SpawnEggsToMap()
    {
        int i = 0;
        foreach (var area in map.areas)
        {
            SpawnEggsToArea(i);
            i++;
        }
    }
    void SpawnEggsToArea(int index)
    {
        for (int i = 0; i < (int)eggsOneArea; i++)
        {
            float x = UnityEngine.Random.Range(map.areas[index].xMin, map.areas[index].xMax);
            float z = UnityEngine.Random.Range(map.areas[index].zMin, map.areas[index].zMax);
            Vector3 eggPosition = new Vector3(x, eggHeight, z) + new Vector3(mapCenter.position.x, 0f, mapCenter.position.z);
           // Vector3 positionAdjusted = AdjustPositionHeight(eggPosition);
            Instantiate(egg, eggPosition, Quaternion.identity);
            Debug.Log("Egg number " + i + " in area " + index + " created in pos " + eggPosition);
        }
    }
    Vector3 AdjustPositionHeight(Vector3 eggPosition)
    {
        float adjustement = GetGroundDistance(eggPosition);
        if (Mathf.Abs(eggPosition.y - adjustement) > 0.2f)
        {
            eggPosition += new Vector3(0, adjustement, 0);
            Debug.Log("Adjust position!");
        }
        return eggPosition;
    }
    float GetGroundDistance(Vector3 placement)
    {
        Physics.Raycast(placement, Vector3.down, out RaycastHit hit, 5f, groundLayer);
        float distance = hit.point.y;
        Debug.Log("Ground distance is " + distance);
        return eggAdjustHeight - distance;
    }
    bool CheckCoordinates(float x, float z)
    {
        if(forbiddenValues.Count == 0)
        {
            return true;
        }
        foreach (Vector3 num in forbiddenValues)
        {
            if (num.x == x && num.z == z)
            {
                return false;
            }
        }
        return true;
    }
}
public class Map 
{
    int mapSize;
    int areaCount;
    float areaSize;
    public Area[] areas;
    public Map (int mapSize, int areaCount)
    {
        this.mapSize = mapSize;
        //Either 4 or 16 divisions / areas in the map.
        this.areaCount = areaCount <= 4 ? 4 : 16;
        int oneRow = (int)Mathf.Sqrt(this.areaCount);
        areaSize = mapSize / oneRow;
        areas = new Area[this.areaCount];
        for(int i = 0; i < oneRow; i++)
        {
            for(int j = 0; j < oneRow; j++)   
            {
                areas[i * oneRow + j] = new Area(j, i, areaSize, oneRow);
            }
        }
    }
    public struct Area
    {
        public float xMax { get ; private set; }
        public float zMax { get ; private set; }
        public float xMin { get ; private set; }
        public float zMin { get ; private set; }
        public Area(int x, int z, float areaSize, int oneRow)
        {
            xMin = areaSize * (x - oneRow / 2); 
            xMax = xMin + areaSize;
            zMin = areaSize * (z - oneRow / 2);  
            zMax = zMin + areaSize;
        }
    }
}
