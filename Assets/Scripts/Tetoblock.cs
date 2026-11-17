using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public enum tetoblockEnum {OrangeRicky, BlueRicky, ClevelandZ,RhodeIslandZ,Hero,Teewee, Smashboy}

public class Tetoblock : MonoBehaviour
{
    [SerializeField] private Vector3[] possibleMovement;
    private int movementIndex = 0;
    [SerializeField] private float tetoblockRegularFallingSpeed = 2;
    [SerializeField] private float tetoblockHasteFallingSpeed = 4;
    [SerializeField] private Sprite[] sprites;
    //[SerializeField] private tetoblockEnum tetoblockType;
    private float tetoblockFallingSpeed;
    private Vector3 movementDirection = Vector3.down;
    public bool isMoving {get; private set;} = true;
    [SerializeField] private float tetoblockSize = .7f;
    public Tetosquare[] tetosquares {get; private set;} = new Tetosquare[4];

    private void Awake()
    {
        tetosquares = GetComponentsInChildren<Tetosquare>().ToArray();
    }

    void Start()
    {
        tetoblockFallingSpeed = tetoblockRegularFallingSpeed;
    }

    void Update()
    {
        if(!isMoving)
            return;

        if(Input.GetKeyDown(KeyCode.Q))
            Tracker.instance.RequestRotation(true);
        else if (Input.GetKeyDown(KeyCode.E))
            Tracker.instance.RequestRotation(false);

        if(Input.GetKeyDown(KeyCode.S))
            tetoblockFallingSpeed = tetoblockHasteFallingSpeed;
        else if(Input.GetKeyUp(KeyCode.S))
            tetoblockFallingSpeed = tetoblockRegularFallingSpeed;

        MoveBlock();
    }

    public void Rotate(bool clockwise = true, bool validationNotPassed = false)
    {
        foreach(Tetosquare tetosquare in tetosquares)
            tetosquare.RotateSquare(clockwise);

        transform.Rotate(0, 0, clockwise ? 90 : -90);

        movementIndex = clockwise ? (3 + movementIndex) % 4 : (5 + movementIndex) % 4;
        movementDirection = possibleMovement[movementIndex];

        UpdateTetosquareCords(); // <- ISSUE HERE EDGES

        if(!validationNotPassed)
            Tracker.instance.ValidateRotation(clockwise);
    }

    private void MoveBlock()
    {
        transform.Translate(movementDirection * tetoblockFallingSpeed * Time.deltaTime);

        CheckMovement();
    }

    private void CheckMovement()
    {
        if (Input.GetKeyDown(KeyCode.D)) 
            Tracker.instance.RequestMovement();
        else if(Input.GetKeyDown(KeyCode.A))
            Tracker.instance.RequestMovement(false);
        
    }

    public void Move(bool right = true)
    {
        foreach(Tetosquare tetosquare in tetosquares)
            tetosquare.horizontal += right ? 1 : -1;

        transform.Translate(possibleMovement[((right ? 5 : 3) + movementIndex) % 4] * tetoblockSize);
    }


    public void StopMoving()
    {
        isMoving = false;

        foreach(Tetosquare tetosquare in tetosquares)
            tetosquare.SnapToPosition();
    }

    public void UpdateTetosquareCords()
    {
        foreach(Tetosquare tetosquare in tetosquares)
            tetosquare.UpdateCords();
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    public void Initialize(tetoblockEnum tetoblockType, int spriteIndex)
    {
        //this.tetoblockType = tetoblockType;

        AssignEnum(tetoblockType);

        foreach(Tetosquare tetosquare in tetosquares)
            tetosquare.SetSprite(sprites[spriteIndex]);
    }

    private void AssignEnum(tetoblockEnum tetoblock)
    {
        switch (tetoblock)
        {
            case tetoblockEnum.OrangeRicky:
            OrangeRicky();
            break;
            case tetoblockEnum.BlueRicky:
            BlueRicky();
            break;
            case tetoblockEnum.ClevelandZ:
            ClevelandZ();
            break;
            case tetoblockEnum.RhodeIslandZ:
            RhodeIslandZ();
            break;
            case tetoblockEnum.Hero:
            Hero();
            break;
            case tetoblockEnum.Teewee:
            Teewee();
            break;
            default:
            Smashboy();
            break;
        }
    }

    private void OrangeRicky()
    {
        tetosquares[0].transform.localPosition = new Vector3(0f, 0f);
        tetosquares[1].transform.localPosition = new Vector3(-.5f, 0f);
        tetosquares[2].transform.localPosition = new Vector3(.5f, 0f);
        tetosquares[3].transform.localPosition = new Vector3(.5f, .5f);
    }

    private void BlueRicky()
    {
        tetosquares[0].transform.localPosition = new Vector3(0f, 0f);
        tetosquares[1].transform.localPosition = new Vector3(-.5f, 0f);
        tetosquares[2].transform.localPosition = new Vector3(-.5f, .5f);
        tetosquares[3].transform.localPosition = new Vector3(.5f, 0f);
    }

    private void ClevelandZ()
    {
        tetosquares[0].transform.localPosition = new Vector3(0f, 0f);
        tetosquares[1].transform.localPosition = new Vector3(-.5f, .5f);
        tetosquares[2].transform.localPosition = new Vector3(0f, .5f);
        tetosquares[3].transform.localPosition = new Vector3(.5f, 0f);
    }

    private void RhodeIslandZ()
    {
        tetosquares[0].transform.localPosition = new Vector3(0f, 0f);
        tetosquares[1].transform.localPosition = new Vector3(-.5f, 0f);
        tetosquares[2].transform.localPosition = new Vector3(0f, .5f);
        tetosquares[3].transform.localPosition = new Vector3(.5f, .5f);
    }

    private void Hero()
    {
        tetosquares[0].transform.localPosition = new Vector3(0f, 0f);
        tetosquares[1].transform.localPosition = new Vector3(-.5f, 0f);
        tetosquares[2].transform.localPosition = new Vector3(.5f, 0f);
        tetosquares[3].transform.localPosition = new Vector3(1f, 0f);
    }

    private void Teewee()
    {
        tetosquares[0].transform.localPosition = new Vector3(0f, 0f);
        tetosquares[1].transform.localPosition = new Vector3(.5f, 0f);
        tetosquares[2].transform.localPosition = new Vector3(-.5f, 0f);
        tetosquares[3].transform.localPosition = new Vector3(0f, .5f);
    }

    private void Smashboy()
    {
        tetosquares[0].transform.localPosition = new Vector3(0f, 0f);
        tetosquares[1].transform.localPosition = new Vector3(0f, .5f);
        tetosquares[2].transform.localPosition = new Vector3(.5f, .5f);
        tetosquares[3].transform.localPosition = new Vector3(.5f, 0f);
    }

    public Sprite[] GetTetosprites() => sprites;
}
