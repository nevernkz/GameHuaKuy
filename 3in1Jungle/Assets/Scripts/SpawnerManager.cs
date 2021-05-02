using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SpawnerManager : MonoBehaviour
{
    public List<Spawner> spawnerListMonsterRandom1 = new List<Spawner>();
    public List<Spawner> spawnerListMonsterRandom2 = new List<Spawner>();
    public List<Spawner> RepairKit = new List<Spawner>();

    public static SpawnerManager instance;

    void Start()
    {
        instance = this;
        Init();
    }

    private void Init()
    {
        Spawner[] spawnerArr = FindObjectsOfType<Spawner>();
        spawnerListMonsterRandom1 = spawnerArr.ToList<Spawner>();
        spawnerListMonsterRandom2 = spawnerArr.ToList<Spawner>();
        RepairKit = spawnerArr.ToList<Spawner>();
    }

    public void Spawn()
    {
        foreach(Spawner spawner in spawnerListMonsterRandom1)
        {
            spawner.Spawn();
        }
        foreach (Spawner spawner in spawnerListMonsterRandom2)
        {
            spawner.Spawn();
        }
        foreach (Spawner spawner in RepairKit)
        {
            spawner.Spawn();
        }
    }

}
