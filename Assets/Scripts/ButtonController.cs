using UnityEngine;

public class ButtonController : MonoBehaviour
{
    public Color color;
    SpriteRenderer spriteRenderer;
    private Collider2D collider;
    public GameController.Action action;

    // Use this for initialization
    void Start()
    {
        collider = GetComponent<Collider2D>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.color = color;
    }

    // Update is called once per frame
    void Update()
    {

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
        PerformAction();
    }

    public void SetColor(Color color)
    {
        spriteRenderer.color = color;
    }

    void PerformAction()
    {
        action();
    }

}
