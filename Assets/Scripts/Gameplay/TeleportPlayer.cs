using UnityEngine;

public class TeleportOnTrigger : MonoBehaviour
{
  public Transform player;
  public Transform teleportTarget;

  private void OnTriggerEnter2D(Collider2D other)
  {
    if(other.CompareTag("Player"))
    {
      player.position = teleportTarget.position;
    }
  }
}