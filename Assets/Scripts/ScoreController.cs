using UnityEngine;
using System.Collections;

public class ScoreController : MonoBehaviour 
{
    TextMesh textMesh;
    public int score;

    Vector3 baseScale = Vector3.one;
    Vector3 scale;
    Vector3 upScale;
    Vector3 velocity;
    private bool isScalingUp = false;

    public float addScoreScaleFactor;
    public float smoothTime;

    // Use this for initialization
	void Start () 
    {
        textMesh = gameObject.GetComponent<TextMesh>();
        score = 0;

        scale = baseScale;
        upScale = baseScale * addScoreScaleFactor;
	}

	// Update is called once per frame
	void Update ()
    {
        Vector3 target;
	    if (isScalingUp)
        {
            target = upScale;
            if (Mathf.Approximately(transform.localScale.x, target.x))
            {
                isScalingUp = false;
            }
        }
        else
        {
            target = baseScale;
        }

        transform.localScale = Vector3.SmoothDamp(transform.localScale, target, ref velocity, smoothTime);
	}

    public void AddScore(int addedScore)
    {
        score += addedScore;
        textMesh.text = score.ToString();
        isScalingUp = true;
    }
}
