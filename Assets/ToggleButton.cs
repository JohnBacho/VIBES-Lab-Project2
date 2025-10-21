using UnityEngine;
using UnityEngine.UI;
public class ToggleButton : MonoBehaviour
{
    public Color normalColor = Color.white;
    public Color toggledColor = Color.green;
    public Image targetImage; 
   
    private bool isToggled = false;
    private Button button;
   
    void Start()
    {
        button = GetComponent<Button>();
       
        if (targetImage == null)
        {
            targetImage = GetComponent<Image>();
        }
              
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
    
    public void SetToggled(bool toggled)
    {
        isToggled = toggled;
        if (targetImage != null)
        {
            targetImage.color = isToggled ? toggledColor : normalColor;
        }
    }
}