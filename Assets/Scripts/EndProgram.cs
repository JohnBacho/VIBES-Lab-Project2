using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;


public class EndProgram : MonoBehaviour
{
    [SerializeField] private TextMeshPro EndingProgramText;
    [SerializeField] private GameObject EndProgramScene;

    const float TextTime = 0.20f;
    public void StartProgramEnding()
    {
        EndProgramScene.SetActive(true);
        StartCoroutine(EndingProgram());
    }

    IEnumerator EndingProgram()
    {
    for(int i = 0; i < 4; i++)
        {
            yield return new WaitForSeconds(TextTime);
            EndingProgramText.text = $"Program Complete!\nExiting";
            yield return new WaitForSeconds(TextTime);
            EndingProgramText.text = $"Program Complete!\nExiting.";
            yield return new WaitForSeconds(TextTime);
            EndingProgramText.text = $"Program Complete!\nExiting..";
            yield return new WaitForSeconds(TextTime);
            EndingProgramText.text = $"Program Complete!\nExiting...";
        }
        Application.Quit();
    }
}
