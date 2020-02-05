using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Text

public class ScoreManager : MonoBehaviour
{
    public Text scoreText; 
    int scoreActual = 0; // actual score
    int scoreDisplay = 0; // score being displayed; lags behind but eventually reaches actual score
 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // DEBUG CODE:
        //AddScore(Random.Range(1, 10)); // REMOVE ME!!!
        if (Input.GetKeyDown(KeyCode.Alpha1)) AddScore(100);
        if (Input.GetKeyDown(KeyCode.Alpha2)) AddScore(1000);
        if (Input.GetKeyDown(KeyCode.Alpha3)) AddScore(10000);
        if (Input.GetKeyDown(KeyCode.Alpha4)) AddScore(100000);
        if (Input.GetKeyDown(KeyCode.Alpha5)) AddScore(1000000);
        if (Input.GetKeyDown(KeyCode.Alpha6)) AddScore(10000000);
        if (Input.GetKeyDown(KeyCode.Alpha7)) AddScore(100000000);
        if (Input.GetKeyDown(KeyCode.Alpha8)) AddScore(1000000000);

        // update display score (every frame) if it has not caught up to actual score
        if (scoreDisplay < scoreActual)
        {
            int scoreDifference = scoreActual - scoreDisplay;
            int scoreDisplayIncrease = scoreDifference / 10; // calculate score display increase based on how far off it is
            if (scoreDisplayIncrease == 0) scoreDisplayIncrease = 1; // increase by at least 1 point
            scoreDisplay += scoreDisplayIncrease;
        }

        // update HUD
        scoreText.text = scoreDisplay.ToString();
    }

    public void AddScore(int points)
    {
        scoreActual += points;
    }
}
