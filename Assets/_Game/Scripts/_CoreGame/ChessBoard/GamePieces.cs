using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// check tất cả các image mà giống nhau
// lưu trữ bằng gamepieces thay vì gameobject
public class GamePieces : MonoBehaviour
{
    // -----------Properties
    private int _x;
    private int _y;
    private Grid.PieceType _type;  // loại mảng ghép
    private Grid _grid;
    private MovablePiece movableComponent;
    private ItemPieces itemComponent;
    //-----------Constructor
    public int X
    {
        get { return _x; }
        set
        {
            if (IsMoveable())
            {
                _x = value;
            }
        }
    }
    public int Y
    {
        get { return _y; }
        set
        {
            if (IsMoveable())
            {
                _y = value;
            }
        }
    }
    public Grid.PieceType Type
    {
        get { return _type; }
    }
    public Grid GridRef
    {
        get { return _grid; }
    }
    public MovablePiece MovableComponent
    {
        get { return movableComponent; }
    }
    public ItemPieces ItemComponent
    {
        get { return itemComponent; }
    }
    private ClearablePiece clearablePiece;
    public ClearablePiece ClearableComponent
    {
        get { return clearablePiece; }
    }

    private BoxCollider2D cachedCollider;

    private void Awake()
    {
        movableComponent = GetComponent<MovablePiece>();
        itemComponent = GetComponent<ItemPieces>();
        clearablePiece = GetComponent<ClearablePiece>();

        // OPTIMIZED: Find collider on this object or any child
        cachedCollider = GetComponent<BoxCollider2D>();
        if (cachedCollider == null)
        {
            // Try to find on children (prefab might have collider on child "piece")
            cachedCollider = GetComponentInChildren<BoxCollider2D>();
        }

        if (cachedCollider == null)
        {
            Debug.LogError($"GamePieces at ({_x}, {_y}): No BoxCollider2D found on this object or children! OnMouse events won't work!");
        }
        else if (!cachedCollider.enabled)
        {
            Debug.LogWarning($"GamePieces at ({_x}, {_y}): BoxCollider2D is disabled!");
        }
        else
        {
            Debug.Log($"GamePieces at ({_x}, {_y}): BoxCollider2D found and enabled on {cachedCollider.gameObject.name}");
        }
    }
    public void Init(int x, int y, Grid grid, Grid.PieceType type)
    {
        _x = x;
        _y = y;
        _grid = grid;
        _type = type;
    }
    // sự kiện chuột để tương tác với mảng ghép
    // NOTE: OnMouse events only work if:
    // 1. GameObject has a Collider (2D or 3D)
    // 2. Camera has PhysicsRaycaster (for 3D) or Physics2DRaycaster (for 2D)
    // 3. No UI element is blocking the raycast
    private void OnMouseEnter()
    {
        // Check if collider is still valid
        if (cachedCollider != null && !cachedCollider.enabled)
            return;

        Debug.Log($"[MOUSE] OnMouseEnter called on piece at ({X}, {Y})");
        if (_grid == null)
        {
            Debug.LogWarning($"GamePieces OnMouseEnter: _grid is null at ({X}, {Y})");
            return;
        }
        _grid.EnterPiece(this);
    }

    private void OnMouseDown()
    {
        // Check if collider is still valid
        if (cachedCollider != null && !cachedCollider.enabled)
        {
            Debug.LogWarning($"[MOUSE] OnMouseDown blocked - collider disabled on piece at ({X}, {Y})");
            return;
        }

        Debug.Log($"[MOUSE] OnMouseDown called on piece at ({X}, {Y})");
        if (_grid == null)
        {
            Debug.LogWarning($"GamePieces OnMouseDown: _grid is null at ({X}, {Y})");
            return;
        }
        _grid.PressPiece(this);
    }

    private void OnMouseUp()
    {
        // Check if collider is still valid
        if (cachedCollider != null && !cachedCollider.enabled)
            return;

        Debug.Log($"[MOUSE] OnMouseUp called on piece at ({X}, {Y})");
        if (_grid == null)
        {
            Debug.LogWarning($"GamePieces OnMouseUp: _grid is null at ({X}, {Y})");
            return;
        }
        _grid.ReleasePiece();
    }

    // DEBUG: Alternative input method - only enable if OnMouse events don't work
    // Uncomment this if OnMouseDown/Up don't fire
    /*
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = transform.position.z;
            
            BoxCollider2D col = GetComponent<BoxCollider2D>();
            if (col != null && col.enabled && col.bounds.Contains(mousePos))
            {
                Debug.Log($"[DEBUG INPUT] Manual click detected on piece at ({X}, {Y})");
                if (_grid != null)
                {
                    _grid.PressPiece(this);
                }
            }
        }
        
        if (Input.GetMouseButtonUp(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = transform.position.z;
            
            BoxCollider2D col = GetComponent<BoxCollider2D>();
            if (col != null && col.enabled && col.bounds.Contains(mousePos))
            {
                Debug.Log($"[DEBUG INPUT] Manual release detected on piece at ({X}, {Y})");
                if (_grid != null)
                {
                    _grid.ReleasePiece();
                }
            }
        }
    }
    */
    // các phương thức kiểm tra trạng thái
    public bool IsMoveable()
    {
        return movableComponent != null;
    }
    public bool IsItemed()
    {
        return itemComponent != null;
    }
    public bool IsClearable()
    {
        return ClearableComponent != null;
    }
}