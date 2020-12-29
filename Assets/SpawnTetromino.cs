using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTetromino : MonoBehaviour
{

    public GameObject[] Tetrominos;

    // Start is called before the first frame update
    void Start()
    {
        NewTetromino();
    }

    public void NewTetromino()
    {
        Instantiate(Tetrominos[Random.Range(0, Tetrominos.Length)], transform.position, Quaternion.identity);
    }
}
