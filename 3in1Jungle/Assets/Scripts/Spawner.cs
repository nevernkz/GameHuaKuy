using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject monster;

    public void Spawn()
    {
        Instantiate(monster, this.transform.position, this.transform.rotation);
    }


}
