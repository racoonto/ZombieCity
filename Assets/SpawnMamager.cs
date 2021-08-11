using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnMamager : SingletonMonoBehavior<SpawnMamager>
{
    public int currentWaveIndex;

    [System.Serializable]
    public class RegenMonsterInfo
    {
        public GameObject monster;
        public float ratio;
    }

    [System.Serializable]
    public class WaveInfo
    {
        public int spawnCount = 10;

        public List<RegenMonsterInfo> monsters;
        //public GameObject monster;

        public float time;
    }

    public List<WaveInfo> waves;

    private float nextWaveStartTime;

    public void OnClearAllMonster()
    {
        nextWaveStartTime = 0;
    }

    public float randomRegionDelayMax = 0.5f;

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
                var monster = item.monsters.OrderBy(x => Random.Range(0, x.ratio)).Last().monster;
                Instantiate(monster, spawnPoint, Quaternion.identity);
                yield return new WaitForSeconds(Random.Range(0, randomRegionDelayMax));
            }
            nextWaveStartTime = Time.time + item.time;

            while (Time.time < nextWaveStartTime)
                yield return null;

            LightManager.Instance.ToggleLight();
        }
    }
}