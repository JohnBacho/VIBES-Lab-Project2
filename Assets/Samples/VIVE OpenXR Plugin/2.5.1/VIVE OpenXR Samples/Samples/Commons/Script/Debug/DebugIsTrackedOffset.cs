// Copyright HTC Corporation All Rights Reserved.

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR;
using VIVE.OpenXR;

[RequireComponent(typeof(Text))]
public class DebugIsTrackedOffset : MonoBehaviour
{
    public InputActionReference isTracked1 = null;
    public InputActionReference isTracked2 = null;
    public InputActionReference isTracked3 = null;
    public InputActionReference isTracked4 = null;

    private Text m_Text = null;
    private void Start()
    {
        m_Text = GetComponent<Text>();
    }
    void Update()
    {
        if (m_Text == null) { return; }

        m_Text.text = "";
		{
            if (OpenXRHelper.GetButton(isTracked1, out bool value, out string msg))
			{
                m_Text.text += isTracked1.action.name + ": " + value;
            }
            else
            {
                m_Text.text += msg;
            }
        }
        m_Text.text += "\n";
        {
            if (OpenXRHelper.GetButton(isTracked2, out bool value, out string msg))
            {
                m_Text.text += isTracked2.action.name + ": " + value;
            }
            else
            {
                m_Text.text += msg;
            }
        }
        m_Text.text += "\n";
        {
            if (OpenXRHelper.GetButton(isTracked3, out bool value, out string msg))
            {
                m_Text.text += isTracked3.action.name + ": " + value;
            }
            else
            {
                m_Text.text += msg;
            }
        }
        m_Text.text += "\n";
        {
            if (OpenXRHelper.GetButton(isTracked4, out bool value, out string msg))
            {
                m_Text.text += isTracked4.action.name + ": " + value;
            }
            else
            {
                m_Text.text += msg;
            }
        }
    }
}
