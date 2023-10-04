using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;
using TMPro;
using UnityEngine.Tilemaps;

/*
 * Controls: 
 *   Number Keys 1-5 = Set Gem Type
 *   Right Click = Toggle Glass
 * */
public class LevelEditor : MonoBehaviour
{

    [SerializeField] private LevelSO levelSO;
    [SerializeField] private Transform pfGemGridVisual;
    [SerializeField] private Transform pfGlassGridVisual;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private TextMeshProUGUI levelText;
    [Tooltip("'Object' refers to gems, fruits etc. It is the thing that fills the grid.")]
    [SerializeField] private Transform objectHolder;
    [Header("Tile Map")]
    [SerializeField] Tilemap tilemap; // Reference to  Tilemap
    [SerializeField] Tilemap tilemapBlock; // Reference to  Tilemap Block
    [SerializeField] TileBase tileToInstantiate; // Reference to "rule tile" asset
    [SerializeField] TileBase tileToInstantiate_Block; // Reference to "block tile" asset

    private Grid<GridPosition> grid;

    private void Awake()
    {
        grid = new Grid<GridPosition>(levelSO.width, levelSO.height, 1f, Vector3.zero, (Grid<GridPosition> g, int x, int y) => new GridPosition(levelSO, g, x, y));

        levelText.text = levelSO.name;

        if (levelSO.levelGridPositionList == null || levelSO.levelGridPositionList.Count != levelSO.width * levelSO.height)
        {
            // Create new Level
            Debug.Log("Creating new level...");
            levelSO.levelGridPositionList = new List<LevelSO.LevelGridPosition>();

            for (int x = 0; x < grid.GetWidth(); x++)
            {
                for (int y = 0; y < grid.GetHeight(); y++)
                {
                    GemSO gem = levelSO.gemList[Random.Range(0, levelSO.gemList.Count)];

                    LevelSO.LevelGridPosition levelGridPosition = new LevelSO.LevelGridPosition { x = x, y = y, gemSO = gem };
                    levelSO.levelGridPositionList.Add(levelGridPosition);

                    CreateVisual(grid.GetGridObject(x, y), levelGridPosition);
                }
            }
        }
        else
        {
            // Load Level
            Debug.Log("Loading level...");
            for (int x = 0; x < grid.GetWidth(); x++)
            {
                for (int y = 0; y < grid.GetHeight(); y++)
                {

                    LevelSO.LevelGridPosition levelGridPosition = null;

                    foreach (LevelSO.LevelGridPosition tmpLevelGridPosition in levelSO.levelGridPositionList)
                    {
                        if (tmpLevelGridPosition.x == x && tmpLevelGridPosition.y == y)
                        {
                            levelGridPosition = tmpLevelGridPosition;
                            break;
                        }
                    }

                    if (levelGridPosition == null)
                    {
                        Debug.LogError("Error! Null!");
                    }

                    CreateVisual(grid.GetGridObject(x, y), levelGridPosition);
                }
            }
            DrawTileBackground();
        }

        cameraTransform.position = new Vector3(grid.GetWidth() * .5f, grid.GetHeight() * .5f, cameraTransform.position.z);
    }

    private void Update()
    {
        Vector3 mouseWorldPosition = UtilsClass.GetMouseWorldPosition();

        grid.GetXY(mouseWorldPosition, out int x, out int y);

        if (IsValidPosition(x, y))
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) grid.GetGridObject(x, y).SetGemSO(levelSO.gemList[0], grid.GetGridObject(x, y).GetIsHole());
            if (Input.GetKeyDown(KeyCode.Alpha2)) grid.GetGridObject(x, y).SetGemSO(levelSO.gemList[1], grid.GetGridObject(x, y).GetIsHole());
            if (Input.GetKeyDown(KeyCode.Alpha3)) grid.GetGridObject(x, y).SetGemSO(levelSO.gemList[2], grid.GetGridObject(x, y).GetIsHole());
            if (Input.GetKeyDown(KeyCode.Alpha4)) grid.GetGridObject(x, y).SetGemSO(levelSO.gemList[3], grid.GetGridObject(x, y).GetIsHole());
            if (Input.GetKeyDown(KeyCode.Alpha5)) grid.GetGridObject(x, y).SetGemSO(levelSO.gemList[4], grid.GetGridObject(x, y).GetIsHole());
            if (Input.GetKeyDown(KeyCode.Space))
            {
                grid.GetGridObject(x, y).SetIsHole(!grid.GetGridObject(x, y).GetIsHole());
                grid.GetGridObject(x, y).SetGemSO(levelSO.gemList[0], grid.GetGridObject(x, y).GetIsHole());

                Vector3 worldPosition = grid.GetWorldPosition(x, y);
                Vector3Int cellPosition = new Vector3Int(Mathf.FloorToInt(worldPosition.x), Mathf.FloorToInt(worldPosition.y), Mathf.FloorToInt(worldPosition.z));

                if (grid.GetGridObject(x, y).GetIsHole())
                {
                    tilemapBlock.SetTile(cellPosition, tileToInstantiate_Block);
                }
                else
                { tilemapBlock.SetTile(cellPosition, null); }
                // Force the Tilemap to Refresh
                tilemapBlock.RefreshAllTiles();
            }

            if (Input.GetMouseButtonDown(1))
            {
                grid.GetGridObject(x, y).SetHasGlass(!grid.GetGridObject(x, y).GetHasGlass());
            }
        }
    }

    private void CreateVisual(GridPosition gridPosition, LevelSO.LevelGridPosition levelGridPosition)
    {
        Transform gemGridVisualTransform = Instantiate(pfGemGridVisual, gridPosition.GetWorldPosition(), Quaternion.identity);
        // Set the parent to the objectHolder
        gemGridVisualTransform.parent = objectHolder;

        Transform glassGridVisualTransform = Instantiate(pfGlassGridVisual, gridPosition.GetWorldPosition(), Quaternion.identity);
        glassGridVisualTransform.parent = objectHolder; 


        gridPosition.spriteRenderer = gemGridVisualTransform.Find("sprite").GetComponent<SpriteRenderer>();
        gridPosition.glassVisualGameObject = glassGridVisualTransform.gameObject;
        gridPosition.levelGridPosition = levelGridPosition;

        gridPosition.SetGemSO(levelGridPosition.gemSO, levelGridPosition.isHole);


        gridPosition.SetHasGlass(levelGridPosition.hasGlass);


    }
    private void DrawTileBackground()
    {
        for (int x = -1; x < grid.GetWidth() + 1; x++)
        {
            for (int y = -1; y < grid.GetHeight() + 1; y++)
            {
                Vector3 worldPosition = grid.GetWorldPosition(x, y);
                Vector3Int cellPosition = new Vector3Int(Mathf.FloorToInt(worldPosition.x), Mathf.FloorToInt(worldPosition.y), Mathf.FloorToInt(worldPosition.z));
                tilemap.SetTile(cellPosition, tileToInstantiate);
   
                if (grid.GetGridObject(x, y) != null && grid.GetGridObject(x, y).IsHole())
                {
                    tilemapBlock.SetTile(cellPosition, tileToInstantiate_Block);
                    // Force the Tilemap to Refresh
                    tilemapBlock.RefreshAllTiles();
                }
                // Force the Tilemap to Refresh
                tilemap.RefreshAllTiles();
            }
        }
    }

    private bool IsValidPosition(int x, int y)
    {
        if (x < 0 || y < 0 ||
            x >= grid.GetWidth() || y >= grid.GetHeight())
        {
            // Invalid position
            return false;
        }
        else
        {
            return true;
        }
    }

    private class GridPosition
    {

        public SpriteRenderer spriteRenderer;
        public LevelSO.LevelGridPosition levelGridPosition;
        public GameObject glassVisualGameObject;

        private LevelSO levelSO;
        private Grid<GridPosition> grid;
        private int x;
        private int y;

        public GridPosition(LevelSO levelSO, Grid<GridPosition> grid, int x, int y)
        {
            this.levelSO = levelSO;
            this.grid = grid;
            this.x = x;
            this.y = y;
        }

        public Vector3 GetWorldPosition()
        {
            return grid.GetWorldPosition(x, y);
        }

        public void SetGemSO(GemSO gemSO, bool isHole)
        {
            if (!isHole)
            {
                spriteRenderer.sprite = gemSO.sprite;
                levelGridPosition.gemSO = gemSO;
#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(levelSO);
#endif
            }
            else
            {
                spriteRenderer.sprite = null;
                levelGridPosition.gemSO = null;
            }

        }

        public bool IsHole()
        { return levelGridPosition.isHole; }

        public void SetIsHole(bool isHole)
        {
            levelGridPosition.isHole = isHole;
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(levelSO);
#endif
        }

        public void SetHasGlass(bool hasGlass)
        {
            levelGridPosition.hasGlass = hasGlass;
            glassVisualGameObject.SetActive(hasGlass);
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(levelSO);
#endif
        }

        public bool GetHasGlass()
        {
            return levelGridPosition.hasGlass;
        }

        public bool GetIsHole()
        {
            return levelGridPosition.isHole;
        }

    }

}
