
using UnityEngine;

public class TestSpawner : MonoBehaviour
{
    public static TestSpawner instance { get; private set; }

    void Awake()
    {
        if(!instance)
            instance = this;
        else
            Destroy(gameObject);
    }

    [SerializeField] private GameObject[] testBlock;

    void Start()
    {
        SpawnTetoblock();
    }


    public void SpawnTetoblock()
    {
        Instantiate(testBlock[Random.Range(0, testBlock.Length)], new Vector3(0, 7, 0), Quaternion.identity);
    }

}
