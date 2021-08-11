using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnMamager : MonoBehaviour
{
    public int currentWaveIndex;

    //[System.Serializable]
    //public class RegenInfo
    //{
    //    public GameObject monster;
    //    public float ratio;
    //}

    [System.Serializable]
    public class WaveInfo
    {
        public int spawnCount = 10;

        //public List<RegenInfo> monster;
        public GameObject monster;

        public float time;
    }

    public List<WaveInfo> waves;

    private IEnumerator Start()
    {
        var spawnPoints = GetComponentsInChildren<SpawnPoint>(true);

        foreach (var item in waves)
        {
            currentWaveIndex++;
            Debug.LogWarning($"{currentWaveIndex} 시작됨");

            int spawnCount = item.spawnCount;
            for (int i = 0; i < spawnCount; i++)
            {
                int spawnIndex = Random.Range(0, spawnPoints.Length);
                Vector3 spawnPoint = spawnPoints[spawnIndex].transform.position;
                Instantiate(item.monster, spawnPoint, Quaternion.identity);
            }
            float nextWaveStartTime = Time.time + item.time;

            while (Time.time < nextWaveStartTime)
                yield return null;
        }
    }
}