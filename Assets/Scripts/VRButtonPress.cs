using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class VRButtonPress : MonoBehaviour
{
    [SerializeField] private Button button; // Assign your Button in inspector


    private void OnTriggerEnter(Collider other)
    {
        XRBaseController controller = other.GetComponent<XRBaseController>();
        sxr.SendHaptic(0.5f, 0.1f, false, 0);    
        Debug.Log("Button Pressed via VR Controller");    

        if(controller != null)
        {
            button.onClick.Invoke();
        }
    }
}
