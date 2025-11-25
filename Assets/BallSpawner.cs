using System.Collections;
using UnityEngine;

public class BallSpawner : MonoBehaviour
{
    public GameObject ballPrefab;
    public Transform spawnArea;
    public float spawnInterval = 2f;

    public BallType[] ballTypes;

    private void Start()
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

        Ball ballScript = newBall.GetComponent<Ball>();
        ballScript.ballType = ballTypes[Random.Range(0, ballTypes.Length)];
    }
}
