using UnityEngine;
using System.Collections;

public class TimerController : MonoBehaviour
{
    TextMesh textMesh;
    public int timeTotal;
    float timeRemaining;
    int timeDisplay;
    public bool timerActive = false;

    GameController gameController;

    // Use this for initialization
    void Start()
    {
        gameController = Camera.main.GetComponent<GameController>();
        textMesh = gameObject.GetComponent<TextMesh>();

        timeDisplay = timeTotal;
        timeRemaining = timeTotal;
    }

    // Update is called once per frame
    void Update()
    {
        if (timerActive)
        {
            // Work out how much time is remaining and display it
            timeRemaining = timeRemaining - Time.deltaTime;

            if (timeRemaining > 0f)
            {
                timeDisplay = Mathf.CeilToInt(timeRemaining);
            }
            else
            {
                timerActive = false;
                gameController.TimerEnded();
                timeDisplay = 0;
            }

            textMesh.text = timeDisplay.ToString();
        }        
    }

}
