using System;
using UnityEngine;

public class FovScript : MonoBehaviour
{
    [SerializeField] LayerMask playerLayer;
    [SerializeField] LayerMask obstructionLayer;
    [SerializeField] float viewDistance = 5f;
    [SerializeField, Range(0, 360f)] float viewAngle = 90f;
    [SerializeField, Range(0, 1)] float turnSpeed = .75f;
    [SerializeField] Transform lookPoint;
    [SerializeField] bool detectedPlayer = false;
    [SerializeField] GameObject player;
    Enemy enemy = null;

    private void Awake()
    {
        SetDetection(false);
        TryGetComponent<Enemy>(out enemy);
    }
    private void Update()
    {
        if (!CanSeePlayer()) return;
        LookAtTarget();
    }

    private bool CanSeePlayer()
    {
        if (!IsPlayerInRange()) return SetDetection(false);
        if (!IsPlayerInViewAngle(out Vector3 dir)) return SetDetection(false);
        if (IsViewObstructed(dir)) return SetDetection(false);
        return SetDetection(true);
    }

    private bool IsViewObstructed(Vector3 dir)
    {
        float viewDistance = Vector3.Distance(player.transform.position, transform.position);
        if (!Physics.Raycast(lookPoint.transform.position, dir, out RaycastHit hit, viewDistance, obstructionLayer)) return false;
        if(debugShowObstruction)Debug.DrawLine(lookPoint.position, hit.point, Color.cyan, 0.1f);
        return true;
    }

    private bool IsPlayerInViewAngle(out Vector3 dir)
    {
        dir = (player.transform.position - transform.position).normalized;
        return Vector3.Angle(transform.forward, dir) < viewAngle / 2;
    }

    private bool IsPlayerInRange() => Physics.CheckSphere(lookPoint.position, viewDistance, playerLayer);

    private void LookAtTarget()
    {
        Vector3 dir = (player.transform.position - transform.position).normalized;
        Quaternion rotation = Quaternion.LookRotation(dir);
        rotation.x = 0;
        rotation.z = 0;

        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, turnSpeed);
    }

    bool SetDetection(bool value)
    {
        detectedPlayer = value;
        if (enemy != null) enemy.PlayerDetected(detectedPlayer);
        if(debugChangeMaterial) meshRenderer.material = detectedPlayer ? detectedMaterial : defaultMaterial;
        return detectedPlayer;
    }

    #region Debug
    private static bool debugChangeMaterial = true;
    private static bool debugShowObstruction = true;
    private static bool debugShowViewRadius = true;
    private static bool debugShowViewAngle = true;
    [SerializeField] Material defaultMaterial;
    [SerializeField] Material detectedMaterial;
    [SerializeField] SkinnedMeshRenderer meshRenderer;

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        if (debugShowViewAngle)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(lookPoint.transform.position, lookPoint.position + AngleToDirection(lookPoint.eulerAngles.y + (viewAngle / 2)) * viewDistance);
            Gizmos.DrawLine(lookPoint.transform.position, lookPoint.position + AngleToDirection(lookPoint.eulerAngles.y + (-viewAngle / 2)) * viewDistance);
        }

        if (debugShowViewRadius)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(lookPoint.position, viewDistance);
        }

        if (!detectedPlayer) return;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(lookPoint.transform.position, player.transform.position);
    }

    public Vector3 AngleToDirection(float angle)
    {
        return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
    }

    public static void ToggleViewRadius()
    {
        debugShowViewRadius = !debugShowViewRadius;
        Debug.Log($"Show view radius = {debugShowViewRadius}");
    }

    public static void ToggleViewAngle()
    {
        debugShowViewAngle = !debugShowViewAngle;
        Debug.Log($"Show view angle = {debugShowViewAngle}");
    }
    #endregion
}

