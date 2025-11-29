using UnityEngine;
using System.Collections;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public BetManager betManager;
    
    public float wallet = 100f;

    // Whether spinning is allowed
    private bool coinInserted = false;

    // References to slot machine components
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

    // Called when the handle is pulled
    public void SpinReceived()
    {
        foreach (Reel reel in reels)
        {
            reel.Spin();
        }

        StartCoroutine(StopReels());
    }

    // Awards winnings
    public void SpawnCoins(int amount)
    {
        AddWallet(amount);
    }

    private IEnumerator StopReels()
    {
        int[] results = new int[3];

        // Stop each reel with a 1 second delay between them
        for (int i = 0; i < reels.Length; i++)
        {
            yield return new WaitForSeconds(1);
            int result = Random.Range(0, 10);
            results[i] = result;
            reels[i].StopSpin(result);
        }

        // Check for matches and award coins
        int distinctCount = results.Distinct().Count();
        
        if (distinctCount < results.Length) // At least 2 matching
        {
            SpawnCoins(3);
            
            if (distinctCount == 1) // All 3 matching (jackpot)
            {
                SpawnCoins(17); // Total of 20 coins
            }
        }

        handle.ResetHandle();
        coinInserted = false;
    }
}