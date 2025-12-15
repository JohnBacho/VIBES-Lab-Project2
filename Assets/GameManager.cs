using UnityEngine;
using System.Collections;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public BetManager betManager;
    public float wallet = 100f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AddWallet(float amount)
    {
        wallet += amount;
        sxr.SetWallet(wallet);
        betManager.UpdateUI();
    }

    public void RemoveWallet(float amount)
    {
        wallet -= amount;
        wallet = Mathf.Max(0, wallet);
        sxr.SetWallet(wallet);
        betManager.UpdateUI();
    }

    public void SetWallet(float amount)
    {
        wallet = amount;
        wallet = Mathf.Max(0, wallet);
        sxr.SetWallet(wallet);
        betManager.UpdateUI();
    }
}
