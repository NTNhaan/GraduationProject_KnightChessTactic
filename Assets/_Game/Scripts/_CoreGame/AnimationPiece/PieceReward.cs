using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PieceReward : MonoBehaviour
{
    public Transform target;
    public Transform targetDemon;
    public Camera cam;
    private TimeController timeController;
    private Vector3 targetPos = Vector3.zero;
    public static PieceReward Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        timeController = FindFirstObjectByType<TimeController>();
    }
    public void Update()
    {
        if (timeController == null) return;

        if (timeController.role == Role.Player)
        {
            targetPos = target.position;
        }
        else if (timeController.role == Role.Demon)
        {
            targetPos = targetDemon.position;
        }
        // OPTIMIZED: Removed debug log
    }
    public void StartCoinMove(Vector3 _intialPos, GameObject Coiprefab)
    {
        GameObject _coin = Instantiate(Coiprefab, _intialPos, Quaternion.identity);
        _coin.transform.localScale = new Vector3(1, 1, 1);

        // OPTIMIZED: Disable collider when coin starts moving
        DisableColliderOnCoin(_coin);

        StartCoroutine(MoveCoin(_coin.transform, _intialPos, targetPos));
    }

    private void DisableColliderOnCoin(GameObject coin)
    {
        // Disable all colliders (2D and 3D) on coin and its children
        Collider2D[] colliders2D = coin.GetComponentsInChildren<Collider2D>();
        foreach (var col in colliders2D)
        {
            col.enabled = false;
        }

        Collider[] colliders3D = coin.GetComponentsInChildren<Collider>();
        foreach (var col in colliders3D)
        {
            col.enabled = false;
        }
    }

    IEnumerator MoveCoin(Transform obj, Vector3 StartPos, Vector3 EndPos)
    {
        float time = 0;
        EndPos = new Vector3(EndPos.x, EndPos.y + 3, EndPos.z);
        while (time < 1)
        {
            time += 1 * Time.deltaTime;

            obj.position = Vector3.Lerp(StartPos, EndPos, time);
            yield return null;
        }
        Destroy(obj.gameObject);
    }
}
