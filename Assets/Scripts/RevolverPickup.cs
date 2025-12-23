using UnityEngine;

public class RevolverPickup : MonoBehaviour
{
    public ShootingRangeManager manager;

    void OnTriggerStay(Collider other)
    {
        if (!manager.IsActive) return;
        if (!other.CompareTag("Player")) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            manager.PickupRevolver();
            Destroy(this); // —крипт исчезает Ч нормально
        }
    }
}