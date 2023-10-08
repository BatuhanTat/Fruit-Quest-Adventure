using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Utils;
using UnityEngine.Tilemaps;

public class Match3Visual : MonoBehaviour
{
    public event EventHandler OnStateChanged;

    public enum State
    {
        Busy,
        WaitingForUser,
        TryFindMatches,
        GameOver,
    }

    [SerializeField] private Transform pfGemGridVisual;
    [SerializeField] private Transform pfGlassGridVisual;
    [SerializeField] private Transform pfBackgroundGridVisual;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Match3 match3;
    [Tooltip("'Object' refers to gems, fruits etc. It is the thing that fills the grid.")]
    [SerializeField] private Transform objectHolder;
    [Header("Tile Map")]
    [SerializeField] Tilemap tilemap; // Reference to your Tilemap
    [SerializeField] Tilemap tilemapBlock; // Reference to  Tilemap Block
    [SerializeField] TileBase tileToInstantiate; // Reference to your tile asset
    [Space]
    [SerializeField] UIManager UIManager;
    [Space]
    [SerializeField] Transform BGImageTransform;

    private Grid<Match3.GemGridPosition> grid;
    private Dictionary<Match3.GemGrid, GemGridVisual> gemGridDictionary;
    private Dictionary<Match3.GemGridPosition, GlassGridVisual> glassGridDictionary;

    private bool isSetup;
    private State state;
    private float busyTimer;
    private Action onBusyTimerElapsedAction; // Look it later, what is "Action" , "System.Action"

    private int startDragX;
    private int startDragY;
    private Vector3 startDragMouseWorldPosition;

    private void Awake()
    {
        state = State.Busy;
        isSetup = false;

        match3.OnLevelSet += Match3_OnLevelSet;
    }

    private void Match3_OnLevelSet(object sender, Match3.OnLevelSetEventArgs e)
    {
        FunctionTimer.Create(() => { Setup(sender as Match3, e.grid); }, 0.1f);
    }

    public void Setup(Match3 match3, Grid<Match3.GemGridPosition> grid)
    {
        this.match3 = match3;
        this.grid = grid;

        float cameraYOffset = 2f;
        cameraTransform.position = new Vector3(grid.GetWidth() * 0.5f,
                                               grid.GetHeight() * 0.5f + cameraYOffset,
                                               cameraTransform.position.z);
        BGImageTransform.position = new Vector3(grid.GetWidth() * 0.5f,
                                               grid.GetHeight() * 0.5f + cameraYOffset,
                                               0.0f);
        match3.OnGemGridPositionDestroyed += Match3_OnGemGridPositionDestroyed;
        match3.OnNewGemGridSpawned += Match3_OnNewGemGridSpawned;

        // Initialize Visual
        gemGridDictionary = new Dictionary<Match3.GemGrid, GemGridVisual>();
        glassGridDictionary = new Dictionary<Match3.GemGridPosition, GlassGridVisual>();

        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                Match3.GemGridPosition gemGridPosition = grid.GetGridObject(x, y);
                Match3.GemGrid gemGrid = gemGridPosition.GetGemGrid();

                Vector3 position = grid.GetWorldPosition(x, y);
                position = new Vector3(position.x, 12);

                if (!gemGridPosition.IsHole())
                {
                    // Visual Transform
                    Transform gemGridVisualTransform = Instantiate(pfGemGridVisual, position, Quaternion.identity);
                    // Set the parent to the objectHolder
                    gemGridVisualTransform.parent = objectHolder;

                    gemGridVisualTransform.Find("sprite").GetComponent<SpriteRenderer>().sprite = gemGrid.GetGem().sprite;

                    GemGridVisual gemGridVisual = new GemGridVisual(gemGridVisualTransform, gemGrid);
                    gemGridDictionary[gemGrid] = gemGridVisual;




                    //      Background Grid Visual
                    //Transform backgroundVisualTransform = Instantiate(pfBackgroundGridVisual, grid.GetWorldPosition(x, y), Quaternion.identity);
                    //      Set the parent to the objectHolder
                    //backgroundVisualTransform.parent = objectHolder;
                }

                // Glass Visual Transform
                Transform glassGridVisualTransform = Instantiate(pfGlassGridVisual, grid.GetWorldPosition(x, y), Quaternion.identity);
                // Set the parent to the objectHolder
                glassGridVisualTransform.parent = objectHolder;

                GlassGridVisual glassGridVisual = new GlassGridVisual(glassGridVisualTransform, gemGridPosition);

                glassGridDictionary[gemGridPosition] = glassGridVisual;

            }
        }

        DrawTileBackground();
        SetBusyState(0.5f, () => SetState(State.TryFindMatches));

        isSetup = true;
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

                Match3.GemGridPosition gemGridPosition = grid.GetGridObject(x, y);
                if (gemGridPosition != null && gemGridPosition.IsHole())
                {
                    tilemapBlock.SetTile(cellPosition, tileToInstantiate);
                    // Force the Tilemap to Refresh
                    tilemapBlock.RefreshAllTiles();
                }   
                // Force the Tilemap to Refresh
                tilemap.RefreshAllTiles();
            }
        }
    }

    private void Match3_OnNewGemGridSpawned(object sender, Match3.OnNewGemGridSpawnedEventArgs e)
    {
        Vector3 position = e.gemGridPosition.GetWorldPosition();
        position = new Vector3(position.x, 12);

        Transform gemGridVisualTransform = Instantiate(pfGemGridVisual, position, Quaternion.identity);
        // Set the parent to the objectHolder
        gemGridVisualTransform.parent = objectHolder;
        gemGridVisualTransform.Find("sprite").GetComponent<SpriteRenderer>().sprite = e.gemGrid.GetGem().sprite;

        GemGridVisual gemGridVisual = new GemGridVisual(gemGridVisualTransform, e.gemGrid);

        gemGridDictionary[e.gemGrid] = gemGridVisual;
    }

    private void Match3_OnGemGridPositionDestroyed(object sender, System.EventArgs e)
    {
        Match3.GemGridPosition gemGridPosition = sender as Match3.GemGridPosition;
        if (gemGridPosition != null && gemGridPosition.GetGemGrid() != null)
        {
            gemGridDictionary.Remove(gemGridPosition.GetGemGrid());
        }
    }

    // Backlog: Update inputs with "IPointerDownHandler and IPointerUpHandler" 
    private void Update()
    {
        if (!isSetup) return;

        UpdateVisual();

        switch (state)
        {
            case State.Busy:
                busyTimer -= Time.deltaTime;
                if (busyTimer <= 0f)
                { onBusyTimerElapsedAction(); }
                break;

            case State.WaitingForUser:

                // Mouse input
                if (Input.GetMouseButtonDown(0) && !UIManager.onPause)
                {
                    Vector3 mouseWorldPosition = UtilsClass.GetMouseWorldPosition();
                    grid.GetXY(mouseWorldPosition, out startDragX, out startDragY);
                }

                if (Input.GetMouseButtonUp(0) && !UIManager.onPause)
                {
                    Vector3 mouseWorldPosition = UtilsClass.GetMouseWorldPosition();
                    grid.GetXY(mouseWorldPosition, out int x, out int y);

                    if (x != startDragX)
                    {
                        // Different X
                        y = startDragY;

                        if (x < startDragX)
                        { x = startDragX - 1; }
                        else
                        { x = startDragX + 1; }
                    }
                    else
                    {
                        // Different Y
                        x = startDragX;

                        if (y < startDragY)
                        { y = startDragY - 1; }
                        else
                        { y = startDragY + 1; }
                    }

                    if (match3.CanSwapGridPositions(startDragX, startDragY, x, y))
                    { SwapGridPositions(startDragX, startDragY, x, y); }
                }
                // Touch input
                if (Input.touchCount > 0)
                {
                    Touch touch = Input.GetTouch(0);

                    if (touch.phase == TouchPhase.Began && !UIManager.onPause)
                    {
                        Vector3 touchWorldPosition = UtilsClass.GetWorldPosition(touch.position);
                        grid.GetXY(touchWorldPosition, out startDragX, out startDragY);
                    }
                    else if (touch.phase == TouchPhase.Ended && !UIManager.onPause)
                    {
                        Vector3 touchWorldPosition = UtilsClass.GetWorldPosition(touch.position);
                        grid.GetXY(touchWorldPosition, out int x, out int y);


                        if (x != startDragX)
                        {
                            // Different X
                            y = startDragY;

                            if (x < startDragX)
                            { x = startDragX - 1; }
                            else
                            { x = startDragX + 1; }
                        }
                        else
                        {
                            // Different Y
                            x = startDragX;

                            if (y < startDragY)
                            { y = startDragY - 1; }
                            else
                            { y = startDragY + 1; }
                        }

                        if (match3.CanSwapGridPositions(startDragX, startDragY, x, y))
                        { SwapGridPositions(startDragX, startDragY, x, y); }
                    }
                }
                break;
            case State.TryFindMatches:
                if (match3.TryFindMatchesAndDestroyThem())
                {
                    SetBusyState(0.3f, () =>
                    {
                        match3.FallGemsIntoEmptyPositions();

                        SetBusyState(0.3f, () =>
                        {
                            match3.SpawnNewMissingGridPositions();
                            SetBusyState(0.5f, () => SetState(State.TryFindMatches));
                        });
                    });
                }
                else
                {
                    TrySetStateWaitingForUser();
                }
                break;
            case State.GameOver:
                break;
        }
    }

    private void UpdateVisual()
    {
        foreach (Match3.GemGrid gemGrid in gemGridDictionary.Keys)
        {
            gemGridDictionary[gemGrid].Update();
        }
    }

    public void SwapGridPositions(int startX, int startY, int endX, int endY)
    {
        match3.SwapGridPositions(startX, startY, endX, endY);
        match3.UseMove();

        SetBusyState(0.5f, () => SetState(State.TryFindMatches));
    }

    private void SetBusyState(float busyTimer, Action onBusyTimerElapsedAction)
    {
        SetState(State.Busy);
        this.busyTimer = busyTimer;
        this.onBusyTimerElapsedAction = onBusyTimerElapsedAction;
    }

    private void TrySetStateWaitingForUser()
    {
        if (match3.TryIsGameOver())
        {
            // Game Over!
            Debug.Log("Game Over! - Level is Won or Lost");
            SetState(State.GameOver);
        }
        else
        {
            // Keep Playing
            SetState(State.WaitingForUser);
        }
    }



    private void SetState(State state)
    {
        this.state = state;
        OnStateChanged?.Invoke(this, EventArgs.Empty);
    }

    public State GetState()
    { return state; }

    public class GemGridVisual
    {
        private Transform transform;
        private Match3.GemGrid gemGrid;

        public GemGridVisual(Transform transform, Match3.GemGrid gemGrid)
        {
            this.transform = transform;
            this.gemGrid = gemGrid;

            gemGrid.OnDestroyed += GemGrid_OnDestroyed;
        }

        private void GemGrid_OnDestroyed(object sender, System.EventArgs e)
        {
            transform.GetComponent<Animation>().Play();
            Destroy(transform.gameObject, 1f);
        }

        public void Update()
        {
            Vector3 targetPosition = gemGrid.GetWorldPosition();
            Vector3 moveDir = (targetPosition - transform.position);
            float moveSpeed = 10f;
            transform.position += moveDir * moveSpeed * Time.deltaTime;
        }
    }

    public class BackgroundGridVisual
    {
        private Transform transform;
        private Match3.GemGridPosition gemGridPosition;

        public BackgroundGridVisual(Transform transform, Match3.GemGridPosition gemGridPosition)
        {
            this.transform = transform;
            this.gemGridPosition = gemGridPosition;

            transform.gameObject.SetActive(!gemGridPosition.IsHole());
        }
    }

    public class GlassGridVisual
    {
        private Transform transform;
        private Match3.GemGridPosition gemGridPosition;

        public GlassGridVisual(Transform transform, Match3.GemGridPosition gemGridPosition)
        {
            this.transform = transform;
            this.gemGridPosition = gemGridPosition;

            transform.gameObject.SetActive(gemGridPosition.HasGlass());

            gemGridPosition.OnGlassDestroyed += GemGridPosition_OnGlassDestroyed;
        }

        private void GemGridPosition_OnGlassDestroyed(object sender, EventArgs e)
        {
            transform.gameObject.SetActive(gemGridPosition.HasGlass());
        }
    }
}
