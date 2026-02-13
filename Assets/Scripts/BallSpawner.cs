using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallSpawner : MonoBehaviour
{
    public GameObject ballPrefab;
    public Transform spawnArea;
    public float spawnInterval = 2f;
    private int index = 0;

    public BallType[] ballTypes;

    private List<GameObject> spawnedBalls = new List<GameObject>();


    public void StopSpawning()
    {
        StopAllCoroutines();
    }

    public void StartSpawning()
    {
        StartCoroutine(SpawnBalls());
    }   

    IEnumerator SpawnBalls()
    {
        while (true)
        {
            SpawnBall();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnBall()
    {
        Vector3 spawnPos = spawnArea.position + new Vector3(
            Random.Range(-0.3f, 0.3f),
            0f,
            Random.Range(-0.3f, 0.3f)
        );

        GameObject newBall = Instantiate(ballPrefab, spawnPos, Quaternion.identity);

        spawnedBalls.Add(newBall);

        Ball ballScript = newBall.GetComponent<Ball>();

        ballScript.ballType = ballTypes[index];
        index = (index + 1) % ballTypes.Length;
    }

    public void DestroyAllBalls()
    {
        foreach (GameObject ball in spawnedBalls)
        {
            if (ball != null)
                Destroy(ball);
        }

        spawnedBalls.Clear();
    }
}