using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NFL_NBA_UI_Toggle : MonoBehaviour
{
    public GameObject NFLParlays;
    public GameObject NBAParlays;

    public void ShowNFL()
    {
        NFLParlays.SetActive(true);
        NBAParlays.SetActive(false);
    }

    public void ShowNBA()
    {
        NFLParlays.SetActive(false);
        NBAParlays.SetActive(true);
    }
}
