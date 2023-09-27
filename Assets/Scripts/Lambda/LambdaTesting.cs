using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LambdaTesting : MonoBehaviour
{

    private Cell<int> cell;
    private Cell<string> cellString;
    
    void Start()
    {
        cell = new Cell<int>(2, 3, (Cell<int> c, int x, int y) => x * y);
        cellString = new Cell<string>(2, 3, (Cell<string> c, int x, int y) => (x + y).ToString()); 
    }
}
