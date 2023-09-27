using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell<T>
{

    T size;
    //public Cell(int width, int height, Func<Cell<T>, int, int, T> delegatedFunction)

    // Define the delegate type for the Cell class
    public delegate T CellDelegate(Cell<T> cell, int x, int y);
 
    public Cell(int width, int height, CellDelegate delegatedFunction)
    {
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                size = delegatedFunction(this, x, y);
                Debug.Log("X: " + x + " Y: " + y  + "Size: " + size);
            }
        }
        Debug.Log(" ");
    }
    
}
