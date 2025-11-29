using UnityEngine;
using TMPro;
using System.Collections;



public class Bucket : MonoBehaviour
{
    public BallType bucketType;
    public TextMeshPro label;
    private Coroutine currentFade;
    private void OnTriggerEnter(Collider other)
    {
        Ball ball = other.GetComponent<Ball>();
        if (ball != null)
        {
            if (ball.ballType == bucketType)
            {
                Debug.Log("Correct!");
                GameManager.Instance.AddWallet(1);
                ShowText("+1", Color.green, 1f);

            }
            else
            {
                Debug.Log("Wrong bucket!");
                GameManager.Instance.RemoveWallet(1);
                ShowText("-1", Color.red, 1f);
            }

            Destroy(other.gameObject);
        }
    }

    public void ShowText(string text, Color color, float duration = 1f)
    {
        label.text = text;                
        StartFade(color, duration);
    }

public void StartFade(Color startColor, float duration)
{
    if (currentFade != null)
    {
        StopCoroutine(currentFade);
        currentFade = null;
    }

    currentFade = StartCoroutine(FadeText(startColor, duration));
}

private IEnumerator FadeText(Color startColor, float duration)
{
    label.ForceMeshUpdate();
    TMP_TextInfo textInfo = label.textInfo;

    float elapsed = 0f;

    while (elapsed < duration)
    {
        elapsed += Time.deltaTime;
        float t = elapsed / duration;
        Color32 fadeColor = Color.Lerp(startColor, new Color(0, 0, 0, 0), t);

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible) continue;

            int vertexIndex = charInfo.vertexIndex;
            int materialIndex = charInfo.materialReferenceIndex;
            Color32[] vertexColors = textInfo.meshInfo[materialIndex].colors32;

            vertexColors[vertexIndex + 0] = fadeColor;
            vertexColors[vertexIndex + 1] = fadeColor;
            vertexColors[vertexIndex + 2] = fadeColor;
            vertexColors[vertexIndex + 3] = fadeColor;
        }

        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            textInfo.meshInfo[i].mesh.colors32 = textInfo.meshInfo[i].colors32;
            label.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
        }

        yield return null;
    }

    label.text = "";
    currentFade = null; // clear reference when done
}

}
