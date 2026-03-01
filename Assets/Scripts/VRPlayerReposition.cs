using UnityEngine;
using System.Collections;

public class VRPlayerReposition : MonoBehaviour
{
    [SerializeField] private Transform xrRig;             // The parent XR Rig GameObject
    [SerializeField] private Transform xrCamera;          // The HMD / Main Camera
    [SerializeField] private Transform cameraSpawnPoint;  // The desired world-space location for the camera to end up

    private void Start()
    {
        StartCoroutine(WaitAndRecenter());
    }

    private IEnumerator WaitAndRecenter()
{
    yield return new WaitForSeconds(1.5f);

    if (xrRig == null || xrCamera == null || cameraSpawnPoint == null)
    {
        Debug.LogWarning("VRPlayerReposition: Missing references.");
        yield break;
    }

    Vector3 currentCameraWorldPos = xrCamera.position;

    Vector3 offset = cameraSpawnPoint.position - currentCameraWorldPos;

    xrRig.position += offset;

    Debug.Log($"[VRPlayerReposition] Moved XR Rig by offset (X,Z only): {offset}");
    yield return new WaitForSeconds(3.5f);
    StartCoroutine(CheckCameraPosition());
}

    private IEnumerator CheckCameraPosition()
        {
            Vector3 currentCameraWorldPos = xrCamera.position;
            Vector3 offset = cameraSpawnPoint.position - currentCameraWorldPos;
            float currentZ = xrCamera.position.z;
            float targetZ = cameraSpawnPoint.position.z;
            if(Vector3.Distance(cameraSpawnPoint.position, currentCameraWorldPos) > 0.35f)
            {
                offset = cameraSpawnPoint.position - currentCameraWorldPos;
                xrRig.position += offset;
            }

            if(Mathf.Abs(targetZ - currentZ) > 0.25f)
            {
                float offsetZ = targetZ - currentZ;

                Vector3 newPos = xrRig.position;
                newPos.z += offsetZ;
                xrRig.position = newPos;
            }
            yield return new WaitForSeconds(4f);
            StartCoroutine(CheckCameraPosition());
        }


}
