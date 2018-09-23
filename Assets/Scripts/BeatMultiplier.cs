using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BeatMultiplier : MonoBehaviour
{

    public PlayerSettings PlayerSettings;

    public float BeatLevel = 0f;
    public Image beatLevelUI;

    void Update()
    {
        if (BeatLevel > 0)
        {
            BeatLevel -= PlayerSettings.beatDecrease;
        }
        else
        {
            BeatLevel = 0f;
        }

        beatLevelUI.fillAmount = Mathf.Lerp(beatLevelUI.fillAmount, BeatLevel/100f, 1f);
    }

    // Update is called once per frame
    public void Beat(float beatInterval)
    {
        BeatLevel += beatInterval;
    }

    public void MissedBeat()
    {
        BeatLevel -= PlayerSettings.beatMissedPenalty;
    }
}
