using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Debug script to test input system and identify why OnMouse events don't work
/// Attach this to any GameObject in the scene to test
/// </summary>
public class InputDebugger : MonoBehaviour
{
    private void Update()
    {
        // Test 1: Check if mouse input is detected at all
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("[INPUT TEST] Mouse button down detected!");

            // Test 2: Check if EventSystem exists
            if (EventSystem.current == null)
            {
                Debug.LogError("[INPUT TEST] ❌ EventSystem.current is NULL!");
            }
            else
            {
                Debug.Log($"[INPUT TEST] ✓ EventSystem exists: {EventSystem.current.name}");

                // Test 3: Check if pointer is over UI
                bool overUI = EventSystem.current.IsPointerOverGameObject();
                Debug.Log($"[INPUT TEST] IsPointerOverGameObject: {overUI}");

                if (overUI)
                {
                    Debug.LogWarning("[INPUT TEST] ⚠️ Mouse is over UI - this might block OnMouse events!");
                }
            }

            // Test 4: Check Camera
            Camera mainCam = Camera.main;
            if (mainCam == null)
            {
                mainCam = FindFirstObjectByType<Camera>();
            }

            if (mainCam != null)
            {
                Debug.Log($"[INPUT TEST] ✓ Camera found: {mainCam.name}");

                // Test 5: Check Physics2DRaycaster
                var raycaster = mainCam.GetComponent<UnityEngine.EventSystems.Physics2DRaycaster>();
                if (raycaster == null)
                {
                    Debug.LogError("[INPUT TEST] ❌ Physics2DRaycaster NOT FOUND on Camera!");
                    Debug.LogError("[INPUT TEST] ⚠️ This is why OnMouse events don't work!");
                }
                else
                {
                    Debug.Log($"[INPUT TEST] ✓ Physics2DRaycaster found on Camera");
                }

                // Test 6: Raycast test
                Vector3 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
                mousePos.z = 0;
                RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

                if (hit.collider != null)
                {
                    Debug.Log($"[INPUT TEST] ✓ Raycast hit: {hit.collider.gameObject.name}");
                    Debug.Log($"[INPUT TEST]   Collider enabled: {hit.collider.enabled}");
                    Debug.Log($"[INPUT TEST]   Layer: {LayerMask.LayerToName(hit.collider.gameObject.layer)}");

                    // Check if it's a GamePieces
                    GamePieces piece = hit.collider.GetComponent<GamePieces>();
                    if (piece == null)
                    {
                        piece = hit.collider.GetComponentInParent<GamePieces>();
                    }

                    if (piece != null)
                    {
                        Debug.Log($"[INPUT TEST] ✓ GamePieces found at ({piece.X}, {piece.Y})");
                    }
                    else
                    {
                        Debug.LogWarning($"[INPUT TEST] ⚠️ No GamePieces component found on {hit.collider.gameObject.name}");
                    }
                }
                else
                {
                    Debug.LogWarning($"[INPUT TEST] ⚠️ Raycast hit NOTHING at mouse position {mousePos}");
                }
            }
            else
            {
                Debug.LogError("[INPUT TEST] ❌ No Camera found!");
            }
        }
    }

    private void OnGUI()
    {
        // Display test info on screen
        GUIStyle style = new GUIStyle();
        style.fontSize = 20;
        style.normal.textColor = Color.white;

        GUI.Label(new Rect(10, 10, 500, 30), "[INPUT DEBUGGER ACTIVE]", style);
        GUI.Label(new Rect(10, 40, 500, 30), "Click anywhere to see test results in Console", style);
    }
}

