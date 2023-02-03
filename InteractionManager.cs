using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionManager : MonoBehaviour
{
    [SerializeField] LayerMask interactionLayer;
    [SerializeField] float interactionRange = 5f;

    private TestInputActions playerInput;

    private void Awake() => playerInput = new TestInputActions();
    private void OnEnable() => playerInput.Enable();

    public void TryToInteract(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        Collider[] colliders = Physics.OverlapSphere(this.transform.position, interactionRange, interactionLayer);
        if (colliders.Length == 0) return;
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.TryGetComponent<IInteractable>(out IInteractable interactable)) interactable.Interact();
        }
    }

    #region Debug
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(this.transform.position, interactionRange);
    }
    #endregion
}
