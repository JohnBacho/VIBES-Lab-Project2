using UnityEngine;
using UnityEngine.UI;
public class ToggleButton : MonoBehaviour
{
    public Color normalColor = Color.white;
    public Color toggledColor = Color.green;
    public Image targetImage; // Drag the image you want to change here in Inspector
   
    private bool isToggled = false;
    private Button button;
   
    void Start()
    {
        button = GetComponent<Button>();
       
        // If you didn't assign targetImage in Inspector, try to get it from this object
        if (targetImage == null)
        {
            targetImage = GetComponent<Image>();
        }
       
        button.onClick.AddListener(Toggle);
       
        if (targetImage != null)
        {
            targetImage.color = normalColor;
        }
    }
   
    void Toggle()
    {
        isToggled = !isToggled;
       
        if (targetImage != null)
        {
            targetImage.color = isToggled ? toggledColor : normalColor;
        }
    }
   
    public bool IsToggled()
    {
        return isToggled;
    }
    
    // Add this new method
    public void SetToggled(bool toggled)
    {
        isToggled = toggled;
        
        if (targetImage != null)
        {
            targetImage.color = isToggled ? toggledColor : normalColor;
        }
    }
}