using UnityEngine;
using System.Collections;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public BetManager betManager;

    int[][] outcomeRows = new int[][]
    {
        new int[] {1, 1, 1},
        new int[] {2, 4, 1},
        new int[] {5, 3, 8},
        new int[] {7, 7, 7}
    };
    
    public float wallet = 100f;

    private bool coinInserted = false;

    public int[] results;

    public bool TrialCompleted = false;

    [SerializeField] private Handle handle;
    [SerializeField] private Reel[] reels;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddWallet(float amount)
    {
        wallet += amount;
        betManager.UpdateUI();
    }

    public void RemoveWallet(float amount)
    {
        wallet -= amount;
        wallet = Mathf.Max(0, wallet);
        betManager.UpdateUI();
    }

    public void SpinReceived()
    {
        foreach (Reel reel in reels)
        {
            reel.Spin();
        }

        StartCoroutine(StopReels());
    }

    public void Win(int amount)
    {
        AddWallet(amount);
    }

    public void SetOutcome(int[] outcome)
    {
        results = outcome;

    }

private IEnumerator StopReels()
    {
        for (int i = 0; i < reels.Length; i++)
        {
            yield return new WaitForSeconds(1); 
            reels[i].StopSpin(results[i]);
        }
        
        yield return new WaitForSeconds(1); 

        TrialCompleted = true;
        Driver driver = FindObjectOfType<Driver>();

        handle.ResetHandle();
        
    }

    public void StartNewTrial()
    {
        TrialCompleted = false;
    }
}