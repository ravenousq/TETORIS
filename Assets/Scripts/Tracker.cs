using UnityEngine;
using UnityEngine.UIElements;

public class Tracker : MonoBehaviour
{
   public static Tracker instance {get; private set;}
   private bool[,] board = new bool[20,10];
   [SerializeField] private float[] upperLeftEdgeCords = new float[2];
   [SerializeField] private float blockSize = .5f;
   [SerializeField] private Transform tetosquarespot;
   [SerializeField] private GameObject testBlock;
   [SerializeField] private Transform tetoblockStartingCords;
   private Transform[] tetosquareSpots;
   private float[,,] cords = new float[20, 10, 2];
   private float[,] currentTetoblockCords = new float[4, 2];
   private Tetoblock currentTetoblock;
   public float[] verticals {get; private set;} = new float[20];
   public float[] horizontals {get; private set;} = new float[20];

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
            tetosquareSpots[squareSpotIndex].name = $"{i}, {j}";
            //tetosquareSpots[squareSpotIndex].Translate(new Vector3(currentCords[0], currentCords[1]));
            currentCords[0] += blockSize;
            squareSpotIndex++;
         }
         
         verticals[i] = currentCords[1];
         currentCords[0] = upperLeftEdgeCords[0];
         currentCords[1] -= blockSize;
      }
   }

   void Start()
   {  
      // for(int i = 0; i < 20; i++)
      //    Debug.Log(verticals[i] + ";\n");

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
            currentTetoblockCords[i, 1] = currentTetoblock.tetosquares[i].transform.position.x;
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
         foreach(Tetosquare tetosquare in currentTetoblock.tetosquares)
         {
            //Debug.Log(tetosquare.vertical + ";" + tetosquare.horizontal);
            board[tetosquare.vertical, tetosquare.horizontal] = true;
         }
      }

      currentTetoblock = SpawnTetoblock(tetoblockStartingCords).GetComponent<Tetoblock>();

      for(int i = 0; i < 4; i++)
      {
         currentTetoblockCords[i, 0] = tetoblockStartingCords.position.x;
         currentTetoblockCords[i, 1] = verticals[0];
      }
   }

   public void RequestMovement(bool right = true)
   {
      foreach(Tetosquare tetosquare in currentTetoblock.tetosquares)
         if((tetosquare.horizontal == 9 && right) || tetosquare.horizontal == 0 && !right)
            return;

      currentTetoblock.Move(right);

      for (int i = 0; i < 4; i++)
      {
         currentTetoblockCords[i, 0] += right ? .5f : -.5f;
         if(debugMode)
            Debug.Log($"{++i}) x:{currentTetoblockCords[i, 0]}; y: {currentTetoblockCords[i, 1]};\n ");
      }
   }

   public void RequestRotation(bool clockwise = true)
   {
      currentTetoblock.Rotate(clockwise);
   }

   public GameObject SpawnTetoblock(Transform spawnPoint)
   {
      Tetoblock newTetoblock = Instantiate(testBlock, spawnPoint.position, Quaternion.identity).GetComponent<Tetoblock>();

      newTetoblock.Initialize((tetoblockEnum)Random.Range(0, tetoblockEnum.GetValues(typeof(tetoblockEnum)).Length), Random.Range(0, newTetoblock.GetTetosprites().Length));

      return newTetoblock.gameObject;
   }
}
