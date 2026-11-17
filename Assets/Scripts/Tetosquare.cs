using System;
using Unity.VisualScripting;
using UnityEngine;

public class Tetosquare : MonoBehaviour
{
    private SpriteRenderer sr;
    [SerializeField] private GameObject destroyFX;

    public int vertical = 0;
    public int horizontal;

    void Awake() => sr = GetComponent<SpriteRenderer>();


    void Start()
    {
        UpdateHorizontal();
    }


    private void UpdateHorizontal()
    {
        float[] horizontals = Tracker.instance.horizontals;

        int bestIndex = 0;

        for (int i = 0; i < 10; i++)
            if (Math.Abs(horizontals[i] - transform.position.x) < Math.Abs(horizontals[bestIndex] - transform.position.x))
                bestIndex = i;

        horizontal = bestIndex;
    }

    public void UpdateVertical()
    {
        float[] verticals = Tracker.instance.verticals;

        int bestIndex = 0;

        for (int i = 0; i < 20; i++)
            if (Math.Abs(verticals[i] - transform.position.y) < Math.Abs(verticals[bestIndex] - transform.position.y))
                bestIndex = i;

        vertical = bestIndex;
    }


    public void DestroyMe()
    {
        Destroy(gameObject);
        Instantiate(destroyFX, transform.position, Quaternion.identity);
    }

    public void RotateSquare(bool clockwise = true)
    {
        sr.transform.Rotate(new Vector3(0,0,clockwise ? -90 : 90));
    }

    public void UpdateCords()
    {
        UpdateHorizontal();
        UpdateVertical();
    }

    public void SetSprite(Sprite sprite) => sr.sprite = sprite;

    public void SnapToPosition()
    {
        transform.position = new Vector3 (Tracker.instance.horizontals[horizontal], Tracker.instance.verticals[vertical]);
    }

    private void SetNewCords(int vertical, int horizontal)
    {
        this.vertical = vertical;
        this.horizontal = horizontal;
    }

    public void MoveTo(int vertical, int horizontal)
    {
        SetNewCords(vertical, horizontal);
        SnapToPosition();
    }
}
