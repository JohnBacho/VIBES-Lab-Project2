using UnityEngine;
using TMPro;
using System.Collections;



public class Bucket : MonoBehaviour
{
    public  BallType bucketType;
    private Coroutine currentFade;
    private ManageWallet CurrentWalletScript;
    private bool isEnabled = false;
    [SerializeField] private float moneyPerBall = 0.5f;
    [SerializeField] private bool isTutorial = false;
    [SerializeField] private EffortTaskHandler EffortTaskHandler;

    private void OnDisable() {
     isEnabled = false;   
    }

    private void OnEnable()
    {
        StartCoroutine(EnableAfterDelay());
    }

    private IEnumerator EnableAfterDelay()
    {
        yield return new WaitForSeconds(0.5f);
        isEnabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        Ball ball = other.GetComponent<Ball>();
        if (ball != null)
        {
                SoundManager.SoundManager.PlaySound3D(
                    SoundType.minigamePointSound,
                    transform.position,
                    1f,
                    1f
                );
                ball.gameObject.SetActive(false);
                if(isTutorial)
                {
                    EffortTaskHandler.WaitforBasketScore();
                    return;
                }
                EffortTaskHandler.AddScore();
            }
        }







}
