using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class Testing : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;

    Grid2 grid;

    void Start()
    {
        grid = new Grid2(2, 5, 5f, Vector3.zero);

        cameraTransform.position = new Vector3(grid.GetWidth() * grid.GetCellSize() * 0.5f,
                                               grid.GetHeight() * grid.GetCellSize() * 0.5f,
                                               cameraTransform.position.z);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            grid.SetValue(Camera.main.ScreenToWorldPoint(Input.mousePosition), 40);
        }

        if (Input.GetMouseButtonDown(1))
        {
            int x, y;
            grid.GetXY(Camera.main.ScreenToWorldPoint(Input.mousePosition), out x, out y);
            Debug.Log("(" + x + ", " + y + ")");
        }
    }
}
