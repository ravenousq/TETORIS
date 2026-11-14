using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Tetoblock : MonoBehaviour
{
    [SerializeField] private Vector3[] possibleMovement;
    private int movementIndex = 0;
    [SerializeField] private float blockRegularFallingSpeed = 2;
    [SerializeField] private float blockHasteFallingSpeed = 4;
    [SerializeField] private float blockFlashFallingSpeed;
    [SerializeField] private GameObject spriteParent;
    private SpriteRenderer[] sprites;
    private float blockFallingSpeed;
    private Vector3 movementDirection = Vector3.down;
    private bool isMoving = true;
    [SerializeField] private float blockSize = .7f;
    private List<Tetosquare> squares = new List<Tetosquare>();

    private void Awake()
    {
        squares = GetComponentsInChildren<Tetosquare>().ToList();
    }

    void Start()
    {
        sprites = spriteParent.GetComponentsInChildren<SpriteRenderer>();

        blockFallingSpeed = blockRegularFallingSpeed;

        int randomSprite = Random.Range(0, sprites.Length);

        foreach(Tetosquare tetosquare in squares)
            tetosquare.SetSprite(sprites[randomSprite].sprite);

    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
            RotateBlock();
        else if (Input.GetKeyDown(KeyCode.E))
            RotateBlock(false);

        if(Input.GetKeyDown(KeyCode.S))
            blockFallingSpeed = blockHasteFallingSpeed;
        else if(Input.GetKeyUp(KeyCode.S))
            blockFallingSpeed = blockRegularFallingSpeed;

        MoveBlock();
    }

    private void RotateBlock(bool clockwise = true)
    {
        transform.Rotate(0, 0, clockwise ? 90 : -90);

        movementIndex = clockwise ? (3 + movementIndex) % 4 : (5 + movementIndex) % 4;
        movementDirection = possibleMovement[movementIndex];

        foreach(Tetosquare square in squares)
            square.RotateSquare(clockwise);
    }

    private void MoveBlock()
    {
        if(isMoving)
            transform.Translate(movementDirection * blockFallingSpeed * Time.deltaTime);

        if(Input.GetKeyDown(KeyCode.A))
            transform.Translate(possibleMovement[(3 + movementIndex) % 4 ] * blockSize);

        if(Input.GetKeyDown(KeyCode.D))
            transform.Translate(possibleMovement[(5 + movementIndex) % 4 ] * blockSize);
    }

    private void OecameInvisible()
    {
        Destroy(gameObject);
    }
}
