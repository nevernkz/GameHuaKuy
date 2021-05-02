using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTest : MonoBehaviour
    
{
    public int randomspawn;
    public SpawnerManager spawnerManager;
    private void Start()
    {
        randomspawn = Random.Range(1, 3);
       
        Debug.Log(randomspawn);

    }
    void Update()
    {
        //if (randomspawn == 2)
        //{
        //    SpawnerManager.instance.Spawn();
        //    randomspawn = 0;
        //    return;
        //}

    }
}
