// Copyright HTC Corporation All Rights Reserved.

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR;
using VIVE.OpenXR;

[RequireComponent(typeof(Text))]
public class DebugPoseData : MonoBehaviour
{
    public string PoseName = "";
    public InputActionReference isTracked = null;
    public InputActionReference trackingState = null;
    public InputActionReference position = null;
    public InputActionReference rotation = null;
    public InputActionReference pose = null;

    private Text m_Text = null;
    private void Start()
    {
        m_Text = GetComponent<Text>();
    }

    void Update()
    {
        if (m_Text == null) { return; }

        m_Text.text = PoseName + " ----";

        if (isTracked != null)
        {
            m_Text.text += "\nisTracked: ";
            {
                if (OpenXRHelper.GetButton(isTracked, out bool value, out string msg))
                {
                    m_Text.text += value;
                }
                else
                {
                    m_Text.text += msg;
                }
            }
        }
        if (trackingState != null)
        {
            m_Text.text += "\ntrackingState: ";
            {
                if (OpenXRHelper.GetInteger(trackingState, out InputTrackingState value, out string msg))
                {
                    m_Text.text += value;
                }
                else
                {
                    m_Text.text += msg;
                }
            }
        }
        if (position != null)
        {
            m_Text.text += "\nposition: ";
            {
                if (OpenXRHelper.GetVector3(position, out Vector3 value, out string msg))
                {
                    m_Text.text += "(" + value.x.ToString("N3") + ", " + value.y.ToString("N3") + ", " + value.z.ToString("N3") + ")";
                }
                else
                {
                    m_Text.text += msg;
                }
            }
        }
        if (rotation != null)
        {
            m_Text.text += "\nrotation: ";
            {
                if (OpenXRHelper.GetQuaternion(rotation, out Quaternion value, out string msg))
                {
                    m_Text.text += "(" + value.x.ToString("N3") + ", " + value.y.ToString("N3") + ", " + value.z.ToString("N3") + ", " + value.w.ToString("N3") + ")";
                }
                else
                {
                    m_Text.text += msg;
                }
            }
        }

        if (pose != null)
        {
            m_Text.text += "\npose.isTracked: ";
            {
                if (OpenXRHelper.GetPoseIsTracked(pose, out bool value, out string msg))
                {
                    m_Text.text += value;
                }
                else
                {
                    m_Text.text += msg;
                }
            }
            m_Text.text += "\npose.trackingState: ";
            {
                if (OpenXRHelper.GetPoseTrackingState(pose, out InputTrackingState value, out string msg))
                {
                    m_Text.text += value;
                }
                else
                {
                    m_Text.text += msg;
                }
            }
            m_Text.text += "\npose.position: ";
            {
                if (OpenXRHelper.GetPosePosition(pose, out Vector3 value, out string msg))
                {
                    m_Text.text += "(" + value.x.ToString("N3") + ", " + value.y.ToString("N3") + ", " + value.z.ToString("N3") + ")";
                }
                else
                {
                    m_Text.text += msg;
                }
            }
            m_Text.text += "\npose.rotation: ";
            {
                if (OpenXRHelper.GetPoseRotation(pose, out Quaternion value, out string msg))
                {
                    m_Text.text += "(" + value.x.ToString("N3") + ", " + value.y.ToString("N3") + ", " + value.z.ToString("N3") + ", " + value.w.ToString("N3") + ")";
                }
                else
                {
                    m_Text.text += msg;
                }
            }
        }
    }
}
