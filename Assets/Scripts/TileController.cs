using UnityEngine;
using System.Collections;

public class TileController : MonoBehaviour 
{
    public Vector3 target;
    public float smoothTime;
    private Vector3 velocity = Vector3.zero;
    public bool selected = false;
    private GameController gameController;
    private Collider2D collider;

	// Use this for initialization
	void Start () 
    {
        gameController = Camera.main.GetComponent<GameController>();
        collider = GetComponent<Collider2D>();
        //SetPublicPositionVariables();
        //SetPublicColorVariables();

        //PositionChangePublicMethod();
        //ColourChangePublicMethod();
    }
	
	// Update is called once per frame
	void Update () 
    {
        transform.position = Vector3.SmoothDamp(transform.position, target, ref velocity, smoothTime);
	}

    void CheckInput()
    {
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                if (collider.OverlapPoint(touch.position))
                {
                    OnMouseDown();
                }
            }
        }
    }

    void OnMouseDown()
    {
        Selected();
    }

    void Selected()
    {
        gameController.LetterSelected(gameObject);
    }
}
