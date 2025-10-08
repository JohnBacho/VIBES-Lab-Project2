using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;


public class TogglePressInteractable : MonoBehaviour
{
    private XRSimpleInteractable interactable;
    private float lastActivationTime = -999f;
    public int[] ListOfOdds;
    private int odds;
    private bool isPressed = false;
    public BetManager BetManager;
    public TextMeshPro OddsText;


    [SerializeField] private float cooldownTime = .7f;

    void Start()
    {
        interactable = GetComponent<XRSimpleInteractable>();
        interactable.selectEntered.AddListener(OnSelectEntered);
        UpdateUI();
    }

    void OnSelectEntered(SelectEnterEventArgs args)
    {
        float currentTime = Time.time;
        float timeSinceLast = currentTime - lastActivationTime;
        UpdateUI();
        if (timeSinceLast > cooldownTime)
        {
            lastActivationTime = currentTime;
            isPressed = !isPressed;
            if(isPressed)
            {
                odds = ListOfOdds[sxr.GetTrial()];
                BetManager.AddToCalculateOdds(odds);
            }
            else
            {
                odds = ListOfOdds[sxr.GetTrial()];
                BetManager.RemoveFromCalculateOdds(odds);
            }
        }
    }

    public void UpdateUI()
    {
        if (OddsText != null)
        {
            OddsText.text = "Odds: \n" + ListOfOdds[sxr.GetTrial()].ToString();
        }
    }


    void OnDestroy()
    {
        interactable.selectEntered.RemoveListener(OnSelectEntered);
    }
}