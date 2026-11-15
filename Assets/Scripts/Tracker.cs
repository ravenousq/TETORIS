using System;
using UnityEngine;

public class Tracker : MonoBehaviour
{
   private bool[,] board = new bool[20,10];
   private bool[] board2 = new bool[200];
   [SerializeField] private float[] edgeCords = new float[10];

   void Start()
   {
      Array.Fill(edgeCords, -4.75f);
   }
}
