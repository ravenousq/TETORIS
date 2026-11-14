using UnityEngine;

public class Tetosquare : MonoBehaviour
{
    private SpriteRenderer sr;
    [SerializeField] private GameObject destroyFX;
    [SerializeField] private Sprite[] sprites;


    void Awake() => sr = GetComponent<SpriteRenderer>();


    void Start()
    {
        
    }

    void Update()
    {
    
    }

    public void DestroyMe()
    {
        Destroy(gameObject);
        Instantiate(destroyFX, transform.position, Quaternion.identity);
    }

    public void RotateSquare(bool clockwise = true) => sr.transform.Rotate(new Vector3(0,0,clockwise ? -90 : 90));

    public void SetSprite(Sprite sprite) => sr.sprite = sprite;
}
