using UnityEngine;

public class RevolverShooter : MonoBehaviour
{
    public ShootingRangeManager manager;
    public Transform muzzle; // Точка откуда летят пули (обычно конец ствола), но я еблан и нахуй это послал
    public Camera playerCamera; 

    public float shootRange = 100f;
    private float lastShotTime = -1f;
    private const float COOLDOWN = 0.4f; // Нельзя спамить выстрелы

    [Header("Bullets")]
    public GameObject[] bullets; // Гильзы в барабане, тоже уже нахуй идут крч потом можешь в нейронку закинуть и лишние отъебнуть, а так мне кажется похуй всем будет
    private int currentBulletIndex = 0;

    void Start()
    {
        
        if (playerCamera == null)
            playerCamera = Camera.main;

        if (manager == null)
            manager = FindObjectOfType<ShootingRangeManager>();

        // Автопоиск точки выстрела
        if (muzzle == null)
        {
            muzzle = FindMuzzle(transform);
            if (muzzle == null)
            {
                muzzle = transform; //стреляем из центра так я ебанат и заебался искать в чем ошибка
            }
        }

        FindBullets();
    }

    // Ищем гильзы в револьвере по именам типа Cylinder_Bullet_01
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
        // Если игра не активна - не стреляем
        if (manager == null || !manager.IsActive) return;

        // ЛКМ и проверка кулдауна
        if (Input.GetMouseButtonDown(0) && Time.time - lastShotTime > COOLDOWN)
        {
            Shoot();
        }
    }

    // ГЛАВНЫЙ МЕТОД - ВЫСТРЕЛ
    private void Shoot()
    {
        lastShotTime = Time.time;

        // Тратим патрон
        if (manager != null)
            manager.UseAmmo();

        // ВЫЛЕТ ГИЛЬЗЫ НЕ РАБОТАЕТ ТАК КАК ЕБАНАТ
        if (currentBulletIndex < bullets.Length && bullets[currentBulletIndex] != null)
        {
            GameObject bullet = bullets[currentBulletIndex];
            bullet.transform.SetParent(null); // Отрываем от револьвера

            Rigidbody rb = bullet.GetComponent<Rigidbody>() ?? bullet.AddComponent<Rigidbody>();
            rb.useGravity = true;
            rb.mass = 0.05f;

            // Кидаем гильзу вправо-вверх
            Vector3 ejectDir = transform.right + Vector3.up * 0.5f + transform.forward * 0.1f;
            rb.AddForce(ejectDir * Random.Range(3f, 5f), ForceMode.Impulse);
            rb.AddTorque(Random.insideUnitSphere * 20f, ForceMode.Impulse);
            Destroy(bullet, 5f); // Через 5 секунд удаляем

            currentBulletIndex++;
        }

        //Тут написал хуету чтобы всегда из центра экрана стрелять потому что я тупой нахуй
        Ray ray;

        if (playerCamera != null)
        {
            // Берем центр экрана 
            Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);
            ray = playerCamera.ScreenPointToRay(screenCenter);
        }
        else
        {
            // Запасной вариант для даунов
            ray = new Ray(transform.position, transform.forward);
        }

        // ПРОВЕРЯЕМ ПОПАДАНИЕ ЛУЧОМ
        if (Physics.Raycast(ray, out RaycastHit hit, shootRange))
        {
            
            PopUpTarget target = hit.collider.GetComponent<PopUpTarget>();

            
            if (target == null)
                target = hit.collider.GetComponentInParent<PopUpTarget>();

            if (target == null)
                target = hit.collider.GetComponentInChildren<PopUpTarget>();

            // УРА попал нахуй  
            if (target != null)
            {
                target.Hit(true);
            }
        }
    }

    // Ищем точку выстрела (muzzle) 
    private Transform FindMuzzle(Transform parent)
    {
        foreach (Transform child in parent.GetComponentsInChildren<Transform>())
        {
            if (child.name.ToLower().Contains("muzzle"))
                return child;
        }
        return null;
    }

    // Рекурсивно ищем дочерний объект по имени
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

    // Тут нахуй нейронка ебнула прицел, так как опять ебанат и почемута в канвасе он у меня перестал отображатся
    void OnGUI()
    {
        if (manager != null && manager.IsActive)
        {
            // Просто красный крестик, че тебе еще надо?
            float size = 10f;
            float centerX = Screen.width / 2f;
            float centerY = Screen.height / 2f;

            GUI.color = Color.red;
            // Горизонтальная палка
            GUI.DrawTexture(new Rect(centerX - size * 2, centerY - 1, size * 4, 2), Texture2D.whiteTexture);
            // Вертикальная палка
            GUI.DrawTexture(new Rect(centerX - 1, centerY - size * 2, 2, size * 4), Texture2D.whiteTexture);
            // Точка посередине для красоты
            GUI.DrawTexture(new Rect(centerX - 2, centerY - 2, 4, 4), Texture2D.whiteTexture);
        }
    }
}