using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OpeningScroller : MonoBehaviour
{
    public Text scrollerText;
    public float scrollerTimeLength = 15.0f; // in seconds
    public float transitionTimeDelay = 2.0f; // in seconds (before and after scroller)
    public int fontSizeStart = 60;
    public int fontSizeEnd = 120;
    float timeTextScrollerStarted;
    float timeToLoadNextScene;

    string scriptActual =
        "In A.D. 2101,\n" +
        "\n" +
        "there was too much peace.\n" +
        "\n" +
        "It needed to be...\n" +
        "\n" +
        "REHABILITATED.";
    string scriptDisplay = ""; // slowly fills out into actual full script

    // Start is called before the first frame update
    void Start()
    {
        timeTextScrollerStarted = Time.time + transitionTimeDelay;
        timeToLoadNextScene = timeTextScrollerStarted + scrollerTimeLength + transitionTimeDelay;
        scrollerText.text = scriptDisplay;
    }

    // Update is called once per frame
    void Update()
    {
        // calculate percent of scroller time passed
        float scrollerTimePassed = Time.time - timeTextScrollerStarted;
        float percentPassed = scrollerTimePassed / scrollerTimeLength;
        if (percentPassed < 0.0) percentPassed = 0.0f; // cap at 0%
        if (percentPassed > 1.0) percentPassed = 1.0f; // cap at 100%

        // calculate font size (60..120)
        float fontSize = percentPassed * (fontSizeEnd - fontSizeStart) + fontSizeStart;
        scrollerText.GetComponent<Text>().fontSize = (int)fontSize;

        // calculate string length
        float scrollerStringLength = percentPassed * scriptActual.Length;
        scriptDisplay = scriptActual.Substring(0,((int)scrollerStringLength));

        // update script!
        scrollerText.text = scriptDisplay;

        // load next scene
        if (Time.time > timeToLoadNextScene)
        {
            SceneManager.LoadScene(1); // may have to adjust the number
        }
    }
}
