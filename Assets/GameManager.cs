using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public BetManager betManager;

    public float wallet = 100f;

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
}
