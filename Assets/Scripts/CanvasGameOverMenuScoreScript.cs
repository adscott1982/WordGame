using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CanvasGameOverMenuScoreScript : MonoBehaviour
{
    public GameController gameController;
    public Text uiText;

    // Use this for initialization
    void Start()
    {

    }

    public void OnEnable()
    {
        if (gameController.score.controller != null)
        {
            uiText.text = gameController.score.controller.score.ToString();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
