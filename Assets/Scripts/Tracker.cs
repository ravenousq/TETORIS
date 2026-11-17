using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class Tracker : MonoBehaviour
{
   public static Tracker instance {get; private set;}
   private Tetosquare[,] board = new Tetosquare[20,10];
   [SerializeField] private float[] upperLeftEdgeCords = new float[2];
   [SerializeField] private float tetoblockSize = .5f;
   [SerializeField] private Transform tetosquarespot;
   [SerializeField] private GameObject testBlock;
   [SerializeField] private Transform tetoblockStartingCords;
   private List<Tetosquare> tetosquares = new List<Tetosquare>();
   private Transform[] tetosquareSpots;
   private Tetoblock currentTetoblock;
   public float[] verticals {get; private set;} = new float[20];
   public float[] horizontals {get; private set;} = new float[10];
   private float[] tetoblockCordContainer = new float[8];

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
      currentTetoblock = SpawnTetoblock(tetoblockStartingCords).GetComponent<Tetoblock>();
      AudioManager.instance.PlayBGM(0, true);
   }

   void Update()
   {
      TrackCurrentTetoblock();

      if(Input.GetKeyDown(KeyCode.G))
         PrintBoard();
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
               AssignNewTetoblock();
               return;
            }
         }
         if(i == 3 && flag)
            foreach(Tetosquare tetosquare in currentTetoblock.tetosquares)
               tetosquare.UpdateVertical();
      }
   }

   public void AssignNewTetoblock()
   {
      FillBoard();
      currentTetoblock.StopMoving();
      

      List<float> verticalsToDestroy = CheckRows();

      if(verticalsToDestroy.Any())
      {
         currentTetoblock = SpawnTetoblock(tetoblockStartingCords).GetComponent<Tetoblock>();
         for(int i = tetosquares.Count - 1; i >= 0; i--)
         {
            Tetosquare tetosquare = tetosquares[i];

            //Debug.Log($"Match -> {tetosquare.vertical} {verticalsToDestroy.Contains(tetosquare.vertical)}");
      
            if(verticalsToDestroy.Contains(tetosquare.vertical))
               DestroyTetosquare(tetosquare);
         }

         CleanUpRows();
      }
      else
         currentTetoblock = SpawnTetoblock(tetoblockStartingCords).GetComponent<Tetoblock>();
   }

   private List<Tetosquare> VerticalEmpty(float vertical)
   {
      List<Tetosquare> tetosquaresInVertical = new List<Tetosquare>();

      for(int i = 0; i < 10; i++)
         if(board[(int)vertical , i] != null)
            tetosquaresInVertical.Add(board[(int)vertical, i]);

      return tetosquaresInVertical;
   }

   private void CleanUpRows()
   {
      float bottom = 19;
      for(int i = 19; i >= 0; i--)
      {
         if(VerticalEmpty(i).Any())
            continue;

         bottom = i;
         break;
      }
      
      bool flag = true;
      for(int i = (int)bottom - 1; i >= 0; i--)
      {
         foreach(Tetosquare tetosquare in VerticalEmpty(i))
         {
            board[tetosquare.vertical, tetosquare.horizontal] = null;
            tetosquare.MoveTo(tetosquare.vertical + 1, tetosquare.horizontal);
            board[tetosquare.vertical, tetosquare.horizontal] = tetosquare;
            flag = false;
         }
      }

      if(!flag)
         CleanUpRows();
   }

   private List<float> CheckRows(bool onlyThisBlock = true)
   {
      List<float> foundRows = new List<float>();
      List<float> verticalsToCheck = new List<float>();


      if(onlyThisBlock)
      {
         foreach(Tetosquare tetosquare in currentTetoblock.tetosquares)
            if(!verticalsToCheck.Contains(tetosquare.vertical))
               verticalsToCheck.Add(tetosquare.vertical);
      }
      
      verticalsToCheck.Sort();

      for(int i = 0; i < verticalsToCheck.Count; i++)
      {
         bool rowFound = true;

         for(int j = 0; j < 10; j++)
         {
            if(board[(int)verticalsToCheck[i], j] == null)
            {
               rowFound = false;
               break;
            }
         }

         if(rowFound)
            foundRows.Add(verticalsToCheck[i]);
      }

      return foundRows;
   }

   private void PrintBoard()
   {
      string boardString = "";
      for(int i = 0; i < 20; i++)
      {
         string row = "";
         for(int j = 0; j < 10; j++)
         {
            if(board[i, j] == null)
               row += "X";
            else
               row += "O";
         }
         boardString += row;
         boardString += "\n";
      }
      
      Debug.Log(boardString); 
   }

   private void FillBoard()
   {
      foreach (Tetosquare tetosquare in currentTetoblock.tetosquares)
         board[tetosquare.vertical, tetosquare.horizontal] = tetosquare;
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
      SaveValidationCords();

      currentTetoblock.Rotate(clockwise);
   }

   private void SaveValidationCords()
   {
      for (int i = 0; i < 4; i++)
      {
         tetoblockCordContainer[2 * i] = currentTetoblock.tetosquares[i].horizontal;
         tetoblockCordContainer[2 * i + 1] = currentTetoblock.tetosquares[i].vertical;
      }
   }

    public void ValidateRotation(bool clockwise = true)
   {

      bool validateRotation = true;

      for(int i = 0; i < 4; i++)
      {
         // if(!tetosquaresCords.Contains(new float[tetosquare.horizontal, tetosquare.vertical]))
         //    tetosquaresCords.Add(new float[tetosquare.horizontal, tetosquare.vertical]);
         // else
         // {
         //    validateRotation = false;
         //    break;
         // }
         Tetosquare tetosquare = currentTetoblock.tetosquares[i];
            
         for(int j = i + 1; j < 4; j++)
         {
            if((tetosquare.horizontal == currentTetoblock.tetosquares[j].horizontal)
            && (tetosquare.vertical == currentTetoblock.tetosquares[j].vertical))
            {
               validateRotation = false;
               break;
            }
         }

         if(!validateRotation)
            break;

         if(board[tetosquare.vertical, tetosquare.horizontal])
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

      newTetoblock.Initialize((tetoblockEnum)UnityEngine.Random.Range(0, tetoblockEnum.GetValues(typeof(tetoblockEnum)).Length), UnityEngine.Random.Range(0, newTetoblock.GetTetosprites().Length));

      foreach(Tetosquare tetosquare in newTetoblock.tetosquares)
         tetosquares.Add(tetosquare);

      return newTetoblock.gameObject;
   }

   private void DestroyTetosquare(Tetosquare tetosquare)
   {
      tetosquares.Remove(tetosquare);
      board[tetosquare.vertical, tetosquare.horizontal] = null;
      tetosquare.DestroyMe();
   }
}
