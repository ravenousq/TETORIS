using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Tracker : MonoBehaviour
{
   public static Tracker instance {get; private set;}
   private bool[,] board = new bool[20,10];
   [SerializeField] private float[] upperLeftEdgeCords = new float[2];
   [SerializeField] private float tetoblockSize = .5f;
   [SerializeField] private Transform tetosquarespot;
   [SerializeField] private GameObject testBlock;
   [SerializeField] private Transform tetoblockStartingCords;
   private Transform[] tetosquareSpots;
   private float[,,] cords = new float[20, 10, 2];
   private Tetoblock currentTetoblock;
   public float[] verticals {get; private set;} = new float[20];
   public float[] horizontals {get; private set;} = new float[20];

   private float[] tetoblockCordContainer = new float[8];
   [SerializeField] private bool debugMode;

   void Awake()
   {
      if(instance)
         Destroy(gameObject);
      
      instance = this;
      DontDestroyOnLoad(gameObject);

      System.Array.Fill(horizontals, -10);

      //tetosquareSpots = GetComponentsInChildren<Transform>().Where(c => c.CompareTag("Tetosquare")).ToArray();
      tetosquareSpots = new Transform[200];

      float[] currentCords = new float[2];
      currentCords[0] = upperLeftEdgeCords[0];
      currentCords[1] = upperLeftEdgeCords[1];

      int squareSpotIndex = 0;

      for(int i = 0; i < 20; i++)
      {
         for(int j = 0; j < 10; j++)
         {
            cords[i,j,0] = currentCords[0];
            cords[i,j,1] = currentCords[1];

            if(horizontals[j] == -10)
               horizontals[j] = currentCords[0];

            Vector3 tetosquareSpotCords = new Vector3(currentCords[0], currentCords[1]);
            tetosquareSpots[squareSpotIndex] = Instantiate(tetosquarespot, tetosquareSpotCords, Quaternion.identity, transform);
            tetosquareSpots[squareSpotIndex].name = $"{j}, {i}";
            //tetosquareSpots[squareSpotIndex].Translate(new Vector3(currentCords[0], currentCords[1]));
            currentCords[0] += tetoblockSize;
            squareSpotIndex++;
         }
         
         verticals[i] = currentCords[1];
         currentCords[0] = upperLeftEdgeCords[0];
         currentCords[1] -= tetoblockSize;
      }
   }

   void Start()
   {  
      AssignNewTetoblock();
   }

   void Update()
   {
      TrackCurrentTetoblock();
   }


   private void TrackCurrentTetoblock()
   {
      bool flag = false;
      for(int i = 0; i < 4; i++)
      {
         Tetosquare current = currentTetoblock.tetosquares[i];
         if(verticals[current.vertical] > current.transform.position.y)
         {
            flag = true;
            if(current.vertical == 19 || board[current.vertical + 1, current.horizontal])
            {
               AssignNewTetoblock(true);
               return;
            }
         }
         if(i == 3 && flag)
            foreach(Tetosquare tetosquare in currentTetoblock.tetosquares)
               tetosquare.UpdateVertical();
      }
   }

   public void AssignNewTetoblock(bool replace = false)
   {
      if(replace)
        {
            currentTetoblock.StopMoving();
            FillBoard();
        }

        currentTetoblock = SpawnTetoblock(tetoblockStartingCords).GetComponent<Tetoblock>();
   }

    private void FillBoard()
    {
        foreach (Tetosquare tetosquare in currentTetoblock.tetosquares)
        {
            //Debug.Log(tetosquare.vertical + ";" + tetosquare.horizontal);
            board[tetosquare.vertical, tetosquare.horizontal] = true;
        }
    }

    public void RequestMovement(bool right = true)
   {
      foreach(Tetosquare tetosquare in currentTetoblock.tetosquares)
      {
         if(right && (tetosquare.horizontal == 9 || board[tetosquare.vertical, tetosquare.horizontal + 1]))
            return;

         if(!right &&(tetosquare.horizontal == 0 || board[tetosquare.vertical, tetosquare.horizontal - 1]))
            return;
      }
         // if((tetosquare.horizontal == 9 && right) || (tetosquare.horizontal == 0 && !right))
         //    return;
         

      currentTetoblock.Move(right);
   }

   public void RequestRotation(bool clockwise = true)
   {
      for(int i = 0; i < 4; i++)
      {
         tetoblockCordContainer[2 * i] = currentTetoblock.tetosquares[i].horizontal;
         tetoblockCordContainer[2 * i + 1] = currentTetoblock.tetosquares[i].vertical;
      }

      currentTetoblock.Rotate(clockwise);
   }

   public void ValidateRotation(bool clockwise = true)
   {
      bool validateRotation = true;
      List<float[,]> tetosquaresCords = new List<float[,]>();
      foreach(Tetosquare tetosquare in currentTetoblock.tetosquares)
      {
         if(!tetosquaresCords.Contains(new float[tetosquare.horizontal, tetosquare.vertical]))
            tetosquaresCords.Add(new float[tetosquare.horizontal, tetosquare.vertical]);
         else
         {
            validateRotation = false;
            break;
         }
         if(tetosquare.horizontal < 0 || tetosquare.horizontal > 9
         || tetosquare.vertical < 0 || tetosquare.vertical > 19
         || board[tetosquare.vertical, tetosquare.horizontal] ) // <- ISSUE HERE
         {
            validateRotation = false;
            break;
         }
      }

      if(validateRotation)
         currentTetoblock.UpdateTetosquareCords();
      else 
         currentTetoblock.Rotate(!clockwise, true);
   }

   public GameObject SpawnTetoblock(Transform spawnPoint)
   {
      Tetoblock newTetoblock = Instantiate(testBlock, spawnPoint.position, Quaternion.identity).GetComponent<Tetoblock>();

      newTetoblock.Initialize((tetoblockEnum)Random.Range(0, tetoblockEnum.GetValues(typeof(tetoblockEnum)).Length), Random.Range(0, newTetoblock.GetTetosprites().Length));

      return newTetoblock.gameObject;
   }
}
