using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Score : MonoBehaviour
{
    public int totalScore = 0;

    void Update()
    {
        totalScore = JigManager.Instance.GetPoints() + ArcManager.Instance.GetPoints();
    }

    public int GetTotalScore()
    {

        return totalScore;

    }

}
