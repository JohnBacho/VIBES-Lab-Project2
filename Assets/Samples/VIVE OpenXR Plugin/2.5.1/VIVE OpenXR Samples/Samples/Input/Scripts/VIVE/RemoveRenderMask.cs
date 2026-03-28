using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace VIVE.OpenXR.Samples.OpenXRInput
{
    public class RemoveRenderMask : MonoBehaviour
    {
        private void OnEnable()
        {
            StartCoroutine(RemoveOcclusionMask());
        }
        IEnumerator RemoveOcclusionMask()
        {
            // Find DisplaySubsystem
            XRDisplaySubsystem display = null;
            List<XRDisplaySubsystem> displaySubsystems = new List<XRDisplaySubsystem>();
            do
            {
                SubsystemManager.GetSubsystems(displaySubsystems);
                foreach (var d in displaySubsystems)
                {
                    if (d.running)
                    {
                        display = d;
                        break;
                    }
                }
                yield return null;
            } while (display == null);
            Debug.Log("RemoveOcclusionMask XRSettings.occlusionMaskScale = 0");
            XRSettings.occlusionMaskScale = 0;
            XRSettings.useOcclusionMesh = false;
        }
    }
}