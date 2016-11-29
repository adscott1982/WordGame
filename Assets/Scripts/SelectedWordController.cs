using UnityEngine;

public class SelectedWordController : MonoBehaviour
{
    MeshRenderer meshRenderer;
    BoxCollider2D collider;
    GameController gameController;
    TextMesh textMesh;

    // Use this for initialization
    void Start()
    {
        meshRenderer = gameObject.GetComponent<MeshRenderer>();
        textMesh = gameObject.GetComponent<TextMesh>();
        collider = gameObject.GetComponent<BoxCollider2D>();
        gameController = Camera.main.GetComponent<GameController>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckInput();
    }

    public void LettersChanged()
    {
        UpdateColliderWidth();
    }

    void UpdateColliderWidth()
    {
        int textLength = textMesh.text.Length;

        if (textLength > 0)
        {
            collider.size = new Vector2(meshRenderer.bounds.size.x, collider.size.y);
        }

    }

    // TODO: Add touch selection
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
        gameController.WordSelected();
        Debug.Log("Word Selected");
    }
}
