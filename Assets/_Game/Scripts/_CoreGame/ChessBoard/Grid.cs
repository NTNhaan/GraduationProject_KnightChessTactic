using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Grid : MonoBehaviour
{
    // public EnemyCharacter enemy;
    // public HeroCharater player;
    private GameManager gameManager;
    public enum PieceType
    {
        NORMAL,
        BUBBLE,
        EMPTY,
        COUNT,
    }
    [System.Serializable]
    public struct PiecePrefab
    {
        public PieceType type;
        public GameObject prefab;
    }
    [System.Serializable]
    public struct PiecePosition
    {
        public PieceType type;
        public int x;
        public int y;
    };

    public int xDim;
    public int yDim;
    public float FillTime;
    public PiecePrefab[] piecePrefabs;   // the array containing the image object is displayed on chessboard
    public GameObject backgroundPrefab;   // backgound for chessboard
    public PiecePosition[] initialPieces;

    private Dictionary<PieceType, GameObject> _piecePrefabDict;
    private Dictionary<ItemPieces.ItemType, float> _itemWeights;
    public GamePieces[,] _pieces;
    private bool _inverse;

    public Vector2[,] backgroundPositions;

    // biến nãy sinh trong quá trình
    private GamePieces pressedPiece;
    private GamePieces enteredPiece;
    // Update is called once per 
    private Role role;
    [SerializeField] private TimeController timeswap;

    public PieceReward pieceReward;
    public bool isFilling = false;
    private Coroutine fillCoroutine; // Track current fill coroutine

    public void SetFilling(bool value)
    {
        isFilling = value;
    }

    private bool hasEnemySwapped = false;

    // Thêm event để thông báo khi board đã fill xong
    public static event Action OnBoardFilled;  // observer để theo dõi sự kiện

    public void Awake()
    {
        role = Role.Player;
        // OPTIMIZED: Use FindFirstObjectByType instead of deprecated FindObjectOfType
        if (timeswap == null)
            timeswap = FindFirstObjectByType<TimeController>();
        if (gameManager == null)
            gameManager = GetComponent<GameManager>();

        // CRITICAL: Ensure Main Camera has Physics2DRaycaster for OnMouse events to work
    }


    public void Start()
    {
        _piecePrefabDict = new Dictionary<PieceType, GameObject>();
        _itemWeights = new Dictionary<ItemPieces.ItemType, float>
            {
                { ItemPieces.ItemType.Sword, 0.1f },
                { ItemPieces.ItemType.Shield, 0.1f },
                { ItemPieces.ItemType.Apple, 0.1f },
                { ItemPieces.ItemType.AppleGreen, 0.1f },
                { ItemPieces.ItemType.Beer, 0.1f },
                { ItemPieces.ItemType.Heart, 0.1f },
                { ItemPieces.ItemType.Armor, 0.1f },
                { ItemPieces.ItemType.Mushroom, 0.1f },
            };
        for (int i = 0; i < piecePrefabs.Length; i++)
        {
            if (!_piecePrefabDict.ContainsKey(piecePrefabs[i].type))
            {
                _piecePrefabDict.Add(piecePrefabs[i].type, piecePrefabs[i].prefab);
            }
        }
        for (int x = 0; x < xDim; x++)
        {
            for (int y = 0; y < yDim; y++)
            {
                GameObject bg = (GameObject)Instantiate(backgroundPrefab, GetWorldPosition(x * 2, y * 2, 0), Quaternion.identity);
                bg.transform.parent = transform;
            }
        }
        //instantiating pieces
        _pieces = new GamePieces[xDim, yDim];
        for (int i = 0; i < initialPieces.Length; i++)
        {
            if (initialPieces[i].x >= 0 && initialPieces[i].x < xDim && initialPieces[i].y >= 0 && initialPieces[i].y < yDim)
            {
                SpawnNewPiece(initialPieces[i].x, initialPieces[i].y, initialPieces[i].type);
            }
        }
        for (int x = 0; x < xDim; x++)
        {
            for (int y = 0; y < yDim; y++)
            {
                if (_pieces[x, y] == null)
                {
                    SpawnNewPiece(x, y, PieceType.EMPTY);
                }
            }
        }
        StartCoroutine(InitialFill());
    }

    private IEnumerator InitialFill()
    {
        isFilling = true;
        yield return StartCoroutine(FillCoroutine(true));
        isFilling = false;

        // Thông báo cho tất cả các observer khi board đã fill xong
        OnBoardFilled?.Invoke();
        Debug.Log("[FILL] Initial fill completed");
    }

    // OPTIMIZED: Removed infinite loop - Fill() is now called explicitly when needed
    // CheckAndFill() removed - no longer needed as Fill() handles its own completion
    public IEnumerator Fill()
    {
        // OPTIMIZED: Prevent multiple Fill() coroutines from running simultaneously
        if (isFilling && fillCoroutine != null)
        {
            Debug.Log("[FILL] Fill() already running, skipping...");
            yield break;
        }

        bool needRefill = true;
        isFilling = true;
        fillCoroutine = StartCoroutine(FillCoroutine(needRefill));
    }

    private IEnumerator FillCoroutine(bool initialNeedRefill)
    {
        bool needRefill = initialNeedRefill;

        while (needRefill && isFilling)
        {
            yield return new WaitForSeconds(FillTime);
            // Thực hiện fill trong một frame
            bool movedPiece = false;
            do
            {
                movedPiece = FillStep();
                if (movedPiece)
                {
                    yield return new WaitForSeconds(FillTime);
                }
            } while (movedPiece);

            // Kiểm tra và xóa matches
            needRefill = ClearAllValidMatches();

            // Cho phép game update trạng thái
            yield return null;
        }

        isFilling = false;
        fillCoroutine = null;
        Debug.Log("[FILL] Fill completed, isFilling = false");
    }
    public bool FillStep()
    {
        bool movedPiece = false;
        for (int y = yDim - 2; y >= 0; y--)
        {
            for (int loopX = 0; loopX < xDim; loopX++)
            {
                int x = loopX;
                GamePieces piece = _pieces[x, y];
                // OPTIMIZED: Only set name in debug builds to reduce string allocations
#if UNITY_EDITOR
                piece.name = "Piece(" + x + "," + y + ")" + "[" + piece.X + "," + piece.Y + "]";
#endif
                if (piece.IsMoveable())
                {
                    GamePieces pieceBelow = _pieces[x, y + 1];

                    if (pieceBelow.Type == PieceType.EMPTY)
                    {
                        Destroy(pieceBelow.gameObject);
                        //Debug.Log("Spawn piece for the second row and fill chessboard:  " + "[" + tmpx + "," + tmpy + "]");
                        piece.MovableComponent.Move(x, y + 1, FillTime);
                        _pieces[x, y + 1] = piece;
                        SpawnNewPiece(x, y, PieceType.EMPTY);
                        movedPiece = true;
                    }
                }
            }
        }
        for (int x = 0; x < xDim; x++)
        {
            GamePieces pieceBelow = _pieces[x, 0];
            if (pieceBelow.Type == PieceType.EMPTY)
            {
                Destroy(pieceBelow.gameObject);
                GameObject newPiece = (GameObject)Instantiate(_piecePrefabDict[PieceType.NORMAL], GetWorldPosition(x, -1, 0), Quaternion.identity);
                newPiece.transform.parent = transform;

                _pieces[x, 0] = newPiece.GetComponent<GamePieces>();
                _pieces[x, 0].Init(x, -1, this, PieceType.NORMAL);
                _pieces[x, 0].MovableComponent.Move(x, 0, FillTime);

                // Lấy danh sách các item type không tạo match
                ItemPieces.ItemType newType = GetNonMatchingItemType(x, 0);
                _pieces[x, 0].ItemComponent.SetItem(newType);
                movedPiece = true;
            }
        }
        return movedPiece;
    }
    // OPTIMIZED: Cache lists to reduce allocations
    private List<ItemPieces.ItemType> _cachedAvailableTypes = new List<ItemPieces.ItemType>();
    private List<ItemPieces.ItemType> _cachedInvalidTypes = new List<ItemPieces.ItemType>();

    private ItemPieces.ItemType GetNonMatchingItemType(int x, int y)
    {
        _cachedAvailableTypes.Clear();
        _cachedAvailableTypes.AddRange(_itemWeights.Keys);
        _cachedInvalidTypes.Clear();

        // Kiểm tra match ngang
        if (x >= 2)
        {
            if (_pieces[x - 1, y].IsItemed() && _pieces[x - 2, y].IsItemed() &&
                _pieces[x - 1, y].ItemComponent.Item == _pieces[x - 2, y].ItemComponent.Item)
            {
                _cachedInvalidTypes.Add(_pieces[x - 1, y].ItemComponent.Item);
            }
        }

        // Kiểm tra match dọc
        if (y >= 2)
        {
            if (_pieces[x, y - 1].IsItemed() && _pieces[x, y - 2].IsItemed() &&
                _pieces[x, y - 1].ItemComponent.Item == _pieces[x, y - 2].ItemComponent.Item)
            {
                _cachedInvalidTypes.Add(_pieces[x, y - 1].ItemComponent.Item);
            }
        }

        // Loại bỏ các type không hợp lệ
        foreach (var invalidType in _cachedInvalidTypes)
        {
            _cachedAvailableTypes.Remove(invalidType);
        }

        // Nếu không còn type hợp lệ, sử dụng tất cả các type
        if (_cachedAvailableTypes.Count == 0)
        {
            _cachedAvailableTypes.Clear();
            _cachedAvailableTypes.AddRange(_itemWeights.Keys);
        }

        // Chọn ngẫu nhiên từ các type còn lại dựa trên trọng số
        float totalWeight = 0;
        foreach (var type in _cachedAvailableTypes)
        {
            totalWeight += _itemWeights[type];
        }

        float randomValue = UnityEngine.Random.Range(0, totalWeight);
        float currentWeight = 0;

        foreach (var type in _cachedAvailableTypes)
        {
            currentWeight += _itemWeights[type];
            if (randomValue <= currentWeight)
            {
                return type;
            }
        }

        return _cachedAvailableTypes[0];
    }
    public GamePieces SpawnNewPiece(int x, int y, PieceType type)
    { // Tạo mảnh ghép mới tại vị trí xác định
        GameObject newPiece = (GameObject)Instantiate(_piecePrefabDict[type], GetWorldPosition(x, y, 0), Quaternion.identity);
        newPiece.transform.parent = transform;
        _pieces[x, y] = newPiece.GetComponent<GamePieces>();

        if (_pieces[x, y] == null)
        {
            Debug.LogError($"SpawnNewPiece: GamePieces component not found on prefab {_piecePrefabDict[type].name}!");
            return null;
        }

        _pieces[x, y].Init(x, y, this, type);

        // OPTIMIZED: Ensure collider exists and is enabled after spawn
        BoxCollider2D col = _pieces[x, y].GetComponent<BoxCollider2D>();
        if (col == null)
        {
            col = _pieces[x, y].GetComponentInChildren<BoxCollider2D>();
        }

        if (col != null && !col.enabled)
        {
            Debug.LogWarning($"SpawnNewPiece: Enabling collider on piece at ({x}, {y})");
            col.enabled = true;
        }

        return _pieces[x, y];
    }

    public static bool IsAdjacent(GamePieces piece1, GamePieces piece2)
    {  // kiểm tra 2 mảnh có liền kề hay không
        return (piece1.X == piece2.X && Mathf.Abs(piece1.Y - piece2.Y) == 1) ||
            (piece1.Y == piece2.Y && Mathf.Abs(piece1.X - piece2.X) == 1);
    }

    public void SwapPiece(GamePieces piece1, GamePieces piece2, bool isManualSwap = false)
    {
        if (piece1 == null || piece2 == null)
        {
            Debug.LogWarning("SwapPiece: One or both pieces are null");
            return;
        }

        if (piece1.MovableComponent == null || piece2.MovableComponent == null)
        {
            Debug.LogWarning("SwapPiece: One or both pieces don't have MovableComponent");
            return;
        }

        // OPTIMIZED: Allow swap even during fill - fill will handle itself
        if (isFilling)
        {
            Debug.Log("[SWAP] Board is filling, but allowing swap to proceed");
            // Don't return - allow swap to proceed
        }

        if (gameManager == null)
        {
            gameManager = FindFirstObjectByType<GameManager>();
            if (gameManager == null)
            {
                Debug.LogError("GameManager not found!");
                return;
            }
        }

        // Check if it's the enemy's turn and if they've already swapped
        if (timeswap != null && timeswap.role == Role.Demon)
        {
            if (hasEnemySwapped)
            {
                Debug.Log("Enemy has already swapped this turn");
                return;
            }
            // Mark enemy swap for this turn
            hasEnemySwapped = true;
        }
        // If it's player's turn, only allow manual swaps
        else if (timeswap != null && timeswap.role == Role.Player)
        {
            // Player can only swap manually, not through AI
            if (!isManualSwap)
            {
                Debug.Log("Player can only swap manually");
                return;
            }
        }
        else if (timeswap == null)
        {
            Debug.LogWarning("TimeBar (timeswap) is null - cannot determine turn");
            return;
        }

        _pieces[piece1.X, piece1.Y] = piece2;
        _pieces[piece2.X, piece2.Y] = piece1;
        var match1 = GetMatch(piece1, piece2.X, piece2.Y);
        var match2 = GetMatch(piece2, piece1.X, piece1.Y);
        int piece1X = piece1.X;
        int piece1Y = piece1.Y;
        piece1.MovableComponent.Move(piece2.X, piece2.Y, FillTime);
        piece2.MovableComponent.Move(piece1X, piece1Y, FillTime);

        // Add safety checks for timeswap
        // if (timeswap != null)
        // {
        //     try
        //     {
        //         if (timeswap.role == TimeBar.Role.Player)
        //         {
        //             timeswap.Pause();
        //             timeswap.PlayAnimation("StartTurn");
        //         }
        //         else if (timeswap.role == TimeBar.Role.Demon)
        //         {
        //             timeswap.Pause();
        //             timeswap.PlayAnimation("StartTurnBack");
        //             hasEnemySwapped = true;
        //         }
        //     }
        //     catch (System.Exception e)
        //     {
        //         Debug.LogError($"Error during timeswap animation: {e.Message}");
        //     }
        // }

        Debug.Log($"[SWAP] SwapPiece completed - pieces swapped at ({piece1.X},{piece1.Y}) and ({piece2.X},{piece2.Y})");

        ClearAllValidMatches();

        // OPTIMIZED: Start Fill() but don't block - allow user to swap again immediately
        StartCoroutine(Fill());
    }

    public void ResetEnemySwapState()
    {
        hasEnemySwapped = false;
    }

    IEnumerator SwapPiecesBack(GamePieces piece1, GamePieces piece2, float delay)
    {
        yield return new WaitForSeconds(delay);
        _pieces[piece1.X, piece1.Y] = piece2;
        _pieces[piece2.X, piece2.Y] = piece1;
        int piece1X = piece1.X;
        int piece1Y = piece1.Y;
        int piece2X = piece2.X;
        int piece2Y = piece2.Y;
        piece1.MovableComponent.Move(piece2.X, piece2.Y, FillTime);
        piece2.MovableComponent.Move(piece1X, piece1Y, FillTime);

        // Wait for another delay
        yield return new WaitForSeconds(delay);

        // Swap back
        _pieces[piece1.X, piece1.Y] = piece1;
        _pieces[piece2.X, piece2.Y] = piece2;
        piece1.MovableComponent.Move(piece1X, piece1Y, FillTime);
        piece2.MovableComponent.Move(piece2X, piece2Y, FillTime);
    }
    public List<GamePieces> GetMatch(GamePieces piece, int NewX, int NewY)
    {
        if (piece.IsItemed())
        {
            ItemPieces.ItemType type = piece.ItemComponent.Item;
            var horizontalPieces = new List<GamePieces>();
            var verticalPieces = new List<GamePieces>();
            var matchingPieces = new List<GamePieces>();
            horizontalPieces.Add(piece);
            for (int dir = 0; dir <= 1; dir++)
            {
                for (int xOffset = 1; xOffset < xDim; xOffset++)
                {
                    int x;
                    if (dir == 0) x = NewX - xOffset; // left
                    else x = NewX + xOffset; // right
                    if (x < 0 || x >= xDim) break;

                    // piece is the same item type  => add to the list
                    if (_pieces[x, NewY].IsItemed() && _pieces[x, NewY].ItemComponent.Item == type)
                    {
                        horizontalPieces.Add(_pieces[x, NewY]);
                    }
                    else break;
                }
            }
            if (horizontalPieces.Count >= 3)
            {
                matchingPieces.AddRange(horizontalPieces);
                for (int i = 0; i < horizontalPieces.Count; i++)
                {
                    matchingPieces.Add(horizontalPieces[i]);
                    //Debug.Log("PieceTypeHorizontalList: " + horizontalPieces[i].ItemComponent.Item);
                }
            }

            // Traverse vertically if we found a match (3 or more pieces)
            if (horizontalPieces.Count >= 3)
            {
                for (int i = 0; i < horizontalPieces.Count; i++)
                {
                    for (int dir = 0; dir <= 1; dir++)
                    {
                        for (int yOffset = 1; yOffset < yDim; yOffset++)
                        {
                            int y;
                            if (dir == 0) y = NewY - yOffset;
                            else y = NewY + yOffset;
                            if (y < 0 || y >= yDim) break;
                            if (_pieces[horizontalPieces[i].X, y].IsItemed() && _pieces[horizontalPieces[i].X, y].ItemComponent.Item == type)
                            {
                                verticalPieces.Add(_pieces[horizontalPieces[i].X, y]);
                            }
                            else break;
                        }
                    }
                    if (verticalPieces.Count < 2)
                        verticalPieces.Clear();
                    else
                    {
                        for (int j = 0; j < verticalPieces.Count; j++)
                        {
                            matchingPieces.Add(verticalPieces[j]);
                        }
                        matchingPieces.AddRange(verticalPieces);
                        break;
                    }
                }
            }
            if (matchingPieces.Count >= 3)
            {
                return matchingPieces;
            }
            horizontalPieces.Clear();
            verticalPieces.Clear();

            //check verticalPiece---m----------------
            verticalPieces.Add(piece);
            for (int dir = 0; dir <= 1; dir++)
            {
                for (int yOffset = 1; yOffset < xDim; yOffset++)  //(*****************)
                {
                    int y;
                    if (dir == 0) y = NewY - yOffset; // left
                    else y = NewY + yOffset; // right
                    if (y < 0 || y >= yDim)
                    {
                        break;
                    }
                    if (_pieces[NewX, y].IsItemed() && _pieces[NewX, y].ItemComponent.Item == type)
                    {
                        verticalPieces.Add(_pieces[NewX, y]);
                    }
                    else break;
                }
            }
            if (verticalPieces.Count >= 3)
            {
                matchingPieces.AddRange(verticalPieces);
                for (int i = 0; i < verticalPieces.Count; i++)
                {
                    matchingPieces.Add(verticalPieces[i]);
                    //Debug.Log("PieceTypeVerticalList: " + verticalPieces[i].ItemComponent.Item);
                }
            }
            // Traverse horizontal if we found a match (3 or more pieces)
            if (verticalPieces.Count >= 3)
            {
                for (int i = 0; i < verticalPieces.Count; i++)
                {
                    for (int dir = 0; dir <= 1; dir++)
                    {
                        for (int xOffset = 1; xOffset < yDim; xOffset++)
                        {
                            int x;
                            if (dir == 0) x = NewX - xOffset; // left
                            else x = NewX + xOffset; //right
                            if (x < 0 || x >= xDim) break;
                            if (_pieces[x, verticalPieces[i].Y].IsItemed() && _pieces[x, verticalPieces[i].Y].ItemComponent.Item == type)
                            {
                                horizontalPieces.Add(_pieces[x, verticalPieces[i].Y]);
                            }
                            else break;
                        }
                    }
                    if (horizontalPieces.Count < 2)
                        horizontalPieces.Clear();
                    else
                    {
                        for (int j = 0; j < horizontalPieces.Count; j++)
                        {
                            matchingPieces.Add(horizontalPieces[j]);
                        }
                        matchingPieces.AddRange(horizontalPieces);
                        break;
                    }
                }
            }
            if (matchingPieces.Count >= 3)
            {
                return matchingPieces;
            }
        }
        return null;
    }
    // OPTIMIZED: Cache HashSet to track cleared pieces and avoid duplicate processing
    private HashSet<GamePieces> _processedPieces = new HashSet<GamePieces>();

    private bool ClearAllValidMatches()
    {
        bool needRefill = false;
        _processedPieces.Clear(); // Clear cache at start

        // OPTIMIZED: Ensure gameManager is initialized
        if (gameManager == null)
        {
            gameManager = GetComponent<GameManager>() ?? FindFirstObjectByType<GameManager>();
        }

        for (int y = 0; y < yDim; y++)
        {
            for (int x = 0; x < xDim; x++)
            {
                GamePieces piece = _pieces[x, y];
                if (piece == null || !piece.IsClearable() || _processedPieces.Contains(piece))
                    continue;

                List<GamePieces> match = GetMatch(piece, x, y);
                if (match == null || match.Count == 0) continue;

                foreach (var gamePiece in match)
                {
                    // OPTIMIZED: Add null check for gamePiece (might be destroyed during loop)
                    if (gamePiece == null || _processedPieces.Contains(gamePiece)) continue;
                    _processedPieces.Add(gamePiece);

                    // OPTIMIZED: Add null checks for gameManager and pieceReward
                    if (gameManager != null)
                    {
                        gameManager.HandleItemBehaviour(gamePiece);  // xử lý action khi match piece
                    }

                    BoxCollider2D boxCollider = gamePiece.GetComponent<BoxCollider2D>();
                    if (boxCollider != null)
                    {
                        // OPTIMIZED: Don't disable collider - it prevents OnMouse events from working
                        // Instead, we'll check if piece is being cleared before allowing input
                        // boxCollider.enabled = false;
                        Debug.Log($"[SWAP] Piece at ({gamePiece.X}, {gamePiece.Y}) matched - keeping collider enabled for cleanup");
                    }

                    if (pieceReward != null)
                    {
                        pieceReward.StartCoinMove(gamePiece.transform.position, gamePiece.gameObject);
                    }

                    // OPTIMIZED: Check gamePiece is still valid before clearing (might be destroyed)
                    if (gamePiece != null && gamePiece.gameObject != null)
                    {
                        if (!ClearPiece(gamePiece.X, gamePiece.Y)) continue;
                        needRefill = true;
                    }
                }
            }
        }
        return needRefill;
    }
    public bool ClearPiece(int x, int y)
    {
        // OPTIMIZED: Add null check for piece
        if (x < 0 || x >= xDim || y < 0 || y >= yDim)
            return false;

        GamePieces piece = _pieces[x, y];
        if (piece == null || !piece.IsClearable() || piece.ClearableComponent == null)
            return false;

        if (!piece.ClearableComponent.IsBeingCleared)
        {
            piece.ClearableComponent.Clear();
            SpawnNewPiece(x, y, PieceType.EMPTY);
            return true;
        }
        return false;
    }

    // auxiliary methods
    Vector2 GetPrefabSize(GameObject prefab)
    {
        Renderer prefabRederer = prefab.GetComponent<Renderer>();
        if (prefabRederer != null)
        {
            return new Vector2(prefabRederer.bounds.size.x, prefabRederer.bounds.size.y);
        }
        else
        {
            //Debug.LogError("check prefab Renderer");
            return Vector2.zero;
        }
    }
    public Vector3 GetWorldPosition(float x, float y, float z)
    {
        return new Vector3(
            transform.position.x - xDim / 2.0f + x - 3,
            transform.position.y + yDim / 2.0f - y + 2,
            transform.position.z);
    }


    public void PressPiece(GamePieces piece)
    {
        pressedPiece = piece;
        Debug.Log($"[INPUT] PressPiece: Piece at ({piece.X}, {piece.Y})");
    }

    public void EnterPiece(GamePieces piece)
    {
        enteredPiece = piece;
        Debug.Log($"[INPUT] EnterPiece: Piece at ({piece.X}, {piece.Y})");
    }

    public void ReleasePiece()
    {
        Debug.Log($"[INPUT] ReleasePiece called - isFilling: {isFilling}");

        // OPTIMIZED: Allow swap even during fill - only block if actively moving pieces
        // The fill process will handle itself, user should be able to swap immediately
        if (isFilling)
        {
            Debug.Log("[SWAP] Board is filling, but allowing swap (fill will continue in background)");
            // Don't return - allow swap to proceed
        }

        // Check if it's player's turn
        if (timeswap == null)
        {
            Debug.LogWarning("[SWAP] TimeController (timeswap) is null!");
            return;
        }

        Debug.Log($"[SWAP] Current role: {timeswap.role}");

        if (timeswap.role != Role.Player)
        {
            Debug.Log($"[SWAP] Not player's turn (current: {timeswap.role}) - cannot swap");
            return;
        }

        bool Swaped = TurnController.Instance != null && TurnController.Instance.IsSwapping;
        Debug.Log($"[SWAP] TurnController.IsSwapping: {Swaped}, pressedPiece: {pressedPiece != null}, enteredPiece: {enteredPiece != null}");

        if (!Swaped)
        {
            if (pressedPiece == null || enteredPiece == null)
            {
                Debug.Log($"[SWAP] Pressed or entered piece is null - pressed: {pressedPiece != null}, entered: {enteredPiece != null}");
                return;
            }

            if (pressedPiece == enteredPiece)
            {
                Debug.Log($"[SWAP] Overlapping piece at ({pressedPiece.X}, {pressedPiece.Y})");
                return;
            }
            else
            {
                bool adjacent = IsAdjacent(pressedPiece, enteredPiece);
                Debug.Log($"[SWAP] Pieces at ({pressedPiece.X},{pressedPiece.Y}) and ({enteredPiece.X},{enteredPiece.Y}) - Adjacent: {adjacent}");

                if (adjacent)
                {
                    Debug.Log("[SWAP] Calling SwapPiece...");
                    SwapPiece(pressedPiece, enteredPiece, true);
                }
                else
                {
                    Debug.Log($"[SWAP] Pieces not adjacent - cannot swap");
                }
            }
        }
        else
        {
            Debug.Log("[SWAP] Already swapping (TurnController.IsSwapping = true) - cannot swap again");
        }

        // Reset pieces after attempt
        pressedPiece = null;
        enteredPiece = null;
    }

    // OPTIMIZED: Use HashSet for O(1) lookup instead of List.Contains() which is O(n)
    private HashSet<GamePieces> _matchCache = new HashSet<GamePieces>();

    public List<GamePieces> FindMatches()
    {
        _matchCache.Clear(); // Clear cache
        List<GamePieces> matchingPieces = new List<GamePieces>();

        // Check horizontal matches
        for (int y = 0; y < yDim; y++)
        {
            for (int x = 0; x < xDim - 2; x++)
            {
                GamePieces piece1 = _pieces[x, y];
                GamePieces piece2 = _pieces[x + 1, y];
                GamePieces piece3 = _pieces[x + 2, y];

                if (piece1 != null && piece2 != null && piece3 != null &&
                    piece1.Type == piece2.Type && piece2.Type == piece3.Type &&
                    piece1.Type != PieceType.EMPTY)
                {
                    // OPTIMIZED: Use HashSet for O(1) lookup
                    if (!_matchCache.Contains(piece1))
                    {
                        _matchCache.Add(piece1);
                        matchingPieces.Add(piece1);
                    }
                    if (!_matchCache.Contains(piece2))
                    {
                        _matchCache.Add(piece2);
                        matchingPieces.Add(piece2);
                    }
                    if (!_matchCache.Contains(piece3))
                    {
                        _matchCache.Add(piece3);
                        matchingPieces.Add(piece3);
                    }
                }
            }
        }

        // Check vertical matches
        for (int x = 0; x < xDim; x++)
        {
            for (int y = 0; y < yDim - 2; y++)
            {
                GamePieces piece1 = _pieces[x, y];
                GamePieces piece2 = _pieces[x, y + 1];
                GamePieces piece3 = _pieces[x, y + 2];

                if (piece1 != null && piece2 != null && piece3 != null &&
                    piece1.Type == piece2.Type && piece2.Type == piece3.Type &&
                    piece1.Type != PieceType.EMPTY)
                {
                    // OPTIMIZED: Use HashSet for O(1) lookup
                    if (!_matchCache.Contains(piece1))
                    {
                        _matchCache.Add(piece1);
                        matchingPieces.Add(piece1);
                    }
                    if (!_matchCache.Contains(piece2))
                    {
                        _matchCache.Add(piece2);
                        matchingPieces.Add(piece2);
                    }
                    if (!_matchCache.Contains(piece3))
                    {
                        _matchCache.Add(piece3);
                        matchingPieces.Add(piece3);
                    }
                }
            }
        }

        return matchingPieces;
    }
}