using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public enum Tetomood {Happy, Angry, Scared, Random}
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
  
        vlTracker.Add(Tetomood.Happy, new List<int>());
        vlTracker.Add(Tetomood.Angry, new List<int>());
        vlTracker.Add(Tetomood.Scared, new List<int>());
        vlTracker.Add(Tetomood.Random, new List<int>());
    }

    private Dictionary<Tetomood, int[]> tetoDictionary = new Dictionary<Tetomood, int[]>();
    private Dictionary<Tetomood, List<int>> vlTracker = new Dictionary<Tetomood, List<int>>();
    [SerializeField] private int[] happy;
    [SerializeField] private int[] angry;
    [SerializeField] private int[] scared;
    [SerializeField] private int[] random;
    [Space]
    [SerializeField] private float voiceLineBreak = 10;
    private float timer;

    private void Start()
    {
        timer = 5;
    }

    private void Update()
    {
        if(!AudioManager.instance.isPlayingSFX())
        {
            timer -= Time.deltaTime;

            if(timer <= 0)
            {
                TetoSpeak(Tetomood.Random);
                timer = voiceLineBreak;
            }
        }
    }

    public void TetoSpeak(Tetomood tetomood = Tetomood.Random ,bool important = false)
    {
        int tetoChoice;

        if(tetomood != Tetomood.Random)
            StopRandoms();
        else if(tetomood != Tetomood.Random && AudioManager.instance.isPlayingSFX())
            return;

        do
            tetoChoice = tetoDictionary[tetomood][Random.Range(0, tetoDictionary[tetomood].Length)];
        while(vlTracker[tetomood].Contains(tetoChoice));

        if(vlTracker[tetomood].Count == 3)
            vlTracker[tetomood].RemoveAt(vlTracker[tetomood].Count - 1);
        
        vlTracker[tetomood].Insert(0, tetoChoice);

        AudioManager.instance.PlaySFX(tetoChoice, important);
    }

    private void StopRandoms()
    {
        foreach(var sfx in tetoDictionary[Tetomood.Random])
            AudioManager.instance.StopSFX(sfx);

        timer += voiceLineBreak;
    }
}
