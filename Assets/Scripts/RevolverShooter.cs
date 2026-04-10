using UnityEngine;

public class RevolverShooter : MonoBehaviour
{
    public ShootingRangeManager manager;
    public Transform muzzle;
    public Camera playerCamera; 

    public float shootRange = 100f;
    private float lastShotTime = -1f;
    private const float COOLDOWN = 0.4f;

    [Header("Bullets")]
    public GameObject[] bullets;
    private int currentBulletIndex = 0;

    void Start()
    {
        
        if (playerCamera == null)
            playerCamera = Camera.main;

        if (manager == null)
            manager = FindObjectOfType<ShootingRangeManager>();

        if (muzzle == null)
        {
            muzzle = FindMuzzle(transform);
            if (muzzle == null)
            {
                muzzle = transform;
            }
        }

        FindBullets();
    }

    public void FindBullets()
    {
        if (bullets == null || bullets.Length == 0)
        {
            bullets = new GameObject[6];
            for (int i = 0; i < 6; i++)
            {
                string name = "Cylinder_Bullet_0" + (i + 1);
                Transform t = FindDeepChild(transform, name);
                if (t != null)
                    bullets[i] = t.gameObject;
            }
        }
    }

    void Update()
    {
        if (manager == null || !manager.IsActive) return;

        if (Input.GetMouseButtonDown(0) && Time.time - lastShotTime > COOLDOWN)
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        lastShotTime = Time.time;

        if (manager != null)
            manager.UseAmmo();

        if (currentBulletIndex < bullets.Length && bullets[currentBulletIndex] != null)
        {
            GameObject bullet = bullets[currentBulletIndex];
            bullet.transform.SetParent(null);

            Rigidbody rb = bullet.GetComponent<Rigidbody>() ?? bullet.AddComponent<Rigidbody>();
            rb.useGravity = true;
            rb.mass = 0.05f;

            Vector3 ejectDir = transform.right + Vector3.up * 0.5f + transform.forward * 0.1f;
            rb.AddForce(ejectDir * Random.Range(3f, 5f), ForceMode.Impulse);
            rb.AddTorque(Random.insideUnitSphere * 20f, ForceMode.Impulse);
            Destroy(bullet, 5f); 
            currentBulletIndex++;
        }

        Ray ray;

        if (playerCamera != null)
        { 
            Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);
            ray = playerCamera.ScreenPointToRay(screenCenter);
        }
        else
        {
            ray = new Ray(transform.position, transform.forward);
        }

        if (Physics.Raycast(ray, out RaycastHit hit, shootRange))
        {
            
            PopUpTarget target = hit.collider.GetComponent<PopUpTarget>();

            
            if (target == null)
                target = hit.collider.GetComponentInParent<PopUpTarget>();

            if (target == null)
                target = hit.collider.GetComponentInChildren<PopUpTarget>();

            if (target != null)
            {
                target.Hit(true);
            }
        }
    }

    private Transform FindMuzzle(Transform parent)
    {
        foreach (Transform child in parent.GetComponentsInChildren<Transform>())
        {
            if (child.name.ToLower().Contains("muzzle"))
                return child;
        }
        return null;
    }

    private Transform FindDeepChild(Transform parent, string name)
    {
        if (parent.name.Contains(name))
            return parent;

        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            Transform result = FindDeepChild(child, name);
            if (result != null)
                return result;
        }
        return null;
    }

    void OnGUI()
    {
        if (manager != null && manager.IsActive)
        {
            float size = 10f;
            float centerX = Screen.width / 2f;
            float centerY = Screen.height / 2f;

            GUI.color = Color.red;
            GUI.DrawTexture(new Rect(centerX - size * 2, centerY - 1, size * 4, 2), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(centerX - 1, centerY - size * 2, 2, size * 4), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(centerX - 2, centerY - 2, 4, 4), Texture2D.whiteTexture);
        }
    }
}