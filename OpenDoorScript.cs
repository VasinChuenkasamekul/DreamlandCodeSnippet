using UnityEngine;

enum DoorState
{
    Opened,
    Closed
}

public class OpenDoorScript : MonoBehaviour, IInteractable
{
    Animation doorAnimation;
    string clipName;
    DoorState doorState;

    private void Start()
    {
        doorState = DoorState.Closed;

        doorAnimation = GetComponent<Animation>();
        clipName = doorAnimation.clip.name;
    }

    [ContextMenu("Open Door")]
    void OpenDoor()
    {
        if (doorState == DoorState.Opened) return;
        doorState = DoorState.Opened;

        Debug.Log("Door opened", this.gameObject);
        doorAnimation.Play(clipName);
    }

    public void Interact()
    {
        OpenDoor();
    }
}
