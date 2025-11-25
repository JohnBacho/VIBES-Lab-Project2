using UnityEngine;

public class WalletManager : MonoBehaviour
{
    public static WalletManager Instance;

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

    void AddWallet(float amount)
{
    GameManager.Instance.wallet += amount;
}

void RemoveWallet(float amount)
{
    GameManager.Instance.wallet -= amount;
    GameManager.Instance.wallet = Mathf.Max(0, GameManager.Instance.wallet);
}

}
