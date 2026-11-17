using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public enum Tetomood {Happy, Angry, Scared, Random, Neutral, Cheeky}
public class Tetobrain : MonoBehaviour
{
    public static Tetobrain instance;

    private void Awake()
    {
        if(!instance)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);

        tetoDictionary.Add(Tetomood.Happy, happy);
        tetoDictionary.Add(Tetomood.Angry, angry);
        tetoDictionary.Add(Tetomood.Scared, scared);
        tetoDictionary.Add(Tetomood.Random, random);
        tetoDictionary.Add(Tetomood.Neutral, neutral);
        tetoDictionary.Add(Tetomood.Cheeky, cheeky);
    }

    private Dictionary<Tetomood, int[]> tetoDictionary = new Dictionary<Tetomood, int[]>();
    [SerializeField] private int[] happy;
    [SerializeField] private int[] angry;
    [SerializeField] private int[] scared;
    [SerializeField] private int[] random;
    [SerializeField] private int[] neutral;
    [SerializeField] private int[] cheeky;
}
