using UnityEngine;
using UnityEngine.UI;

public class ToggleButton : MonoBehaviour
{
    public Color normalColor = Color.white;
    public Color toggledColor = Color.green;
    public Image targetImage; 
   
    private bool isToggled = false;
    private Button button;
    private ColorBlock originalColors;
   
    void Start()
    {
        button = GetComponent<Button>();
       
        if (targetImage == null)
        {
            targetImage = GetComponent<Image>();
        }
        
        if (button != null)
        {
            originalColors = button.colors;
        }
              
        UpdateButtonColors(false);
    }
   
    public void Toggle()
    {
        isToggled = !isToggled;
        UpdateButtonColors(isToggled);
    }
   
    public bool IsToggled()
    {
        return isToggled;
    }
    
    public void SetToggled(bool toggled)
    {
        isToggled = toggled;
        UpdateButtonColors(isToggled);
    }
    
    private void UpdateButtonColors(bool toggled)
    {
        if (button != null)
        {
            ColorBlock colors = button.colors;
            Color baseColor = toggled ? toggledColor : normalColor;
            
            colors.normalColor = baseColor;
            colors.highlightedColor = baseColor * 1.2f; // Slightly brighter on hover
            colors.pressedColor = baseColor * 0.8f; // Slightly darker when pressed
            colors.selectedColor = baseColor;
            colors.disabledColor = baseColor * 0.5f;
            
            button.colors = colors;
        }
    }
}