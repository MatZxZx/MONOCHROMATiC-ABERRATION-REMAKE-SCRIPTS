using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Score : MonoBehaviour
{
    public static int totalScore = 0;
    void Update()
    {
        totalScore = JigManager.Instance.GetPoints() + ArcManager.Instance.GetPoints();
    }

    public int GetTotalScore()
    {

        return totalScore;

    }

}
