
using UnityEngine;

public class TestSpawner : MonoBehaviour
{
    
    [SerializeField] private GameObject[] testBlock;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.X))
            Instantiate(testBlock[Random.Range(0, testBlock.Length)], new Vector3(0, 7, 0), Quaternion.identity);
    }
}
