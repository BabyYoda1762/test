using UnityEngine;

public class ShootingRangeActivator : MonoBehaviour
{
    public ShootingRangeManager manager;
    private bool playerInZone = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) playerInZone = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = false;
            if (manager.IsActive) manager.DeactivateShooting();
        }
    }

    void Update()
    {
        if (!playerInZone || manager.IsActive) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            manager.ActivateShooting();
            manager.PickupRevolver();  // ÑÐÀÇÓ ןמהבטנאול נוגמכüגונ
        }
    }
}