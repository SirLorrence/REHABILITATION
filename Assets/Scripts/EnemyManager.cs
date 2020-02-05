using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public GameObject Enemy;
    public float spawnTime = 3f;

    //transform is a shortcut from gameobject..... so it gets the location
    public Transform[] spawnPoints;

    private void Start()
    {
        InvokeRepeating("Spawn", spawnTime, spawnTime);
    }
    // Update is called once per frame
  void Spawn()
    {
        int spawnIndex = Random.Range(0, spawnPoints.Length);
        Instantiate(Enemy, spawnPoints[spawnIndex].position, spawnPoints[spawnIndex].rotation);
    }
}
