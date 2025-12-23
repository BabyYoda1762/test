using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class ShootingRangeManager : MonoBehaviour
{
    [Header("Player & Gun")]
    public Transform playerCamera;
    public Transform gunHand;
    public GameObject tableRevolver;

    [Header("Targets")]
    public PopUpTarget[] targets;

    [Header("UI")]
    public Canvas scoreCanvas;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI ammoText;
    public Transform guySpeechPos;
    public GameObject speechBubblePrefab;

    [Header("Game Settings")]
    public float popDelayMax = 1.5f;
    public int maxTargets = 6;
    public float targetStayTime = 2f; // Бля, это сколько мишень торчит сверху, потом сама спиздует

    private int score = 0;
    private int ammoLeft = 6;
    public bool IsActive { get; private set; } = false;
    private bool revolverPicked = false;
    private Vector3 originalRevolverPos;
    private Quaternion originalRevolverRot;
    private Coroutine targetCycleCoroutine;

    void Awake()
    {
        // Запоминаем где ревик лежал на столе, чтоб потом вернуть
        originalRevolverPos = tableRevolver.transform.position;
        originalRevolverRot = tableRevolver.transform.rotation;
    }

    void Start()
    {
        
        if (playerCamera == null)
        {
            Camera cam = Camera.main;
            if (cam != null) playerCamera = cam.transform;
        }

        // Кидаем ссылку на себя всем мишеням
        foreach (var target in targets)
        {
            if (target != null)
            {
                target.manager = this;
            }
        }
    }

    // Запуск игры
    public void ActivateShooting()
    {
        if (IsActive) return;
        IsActive = true;
        score = 0;
        ammoLeft = 6;
        revolverPicked = false;
        scoreCanvas.gameObject.SetActive(true);
        UpdateUI();

        // Опускаем тебя(шучу мешени)
        foreach (var target in targets)
        {
            if (target != null)
                target.ResetTarget();
        }

        // Запуск мишений
        if (targetCycleCoroutine != null)
            StopCoroutine(targetCycleCoroutine);

        targetCycleCoroutine = StartCoroutine(TargetCycle());
    }

    // Это корутина которая поднимает мишени по очереди
    private IEnumerator TargetCycle()
    {
        // Список доступных мишеней которые еще не выскакивали в этом цикле
        List<PopUpTarget> availableTargets = new List<PopUpTarget>(targets);

        // Пока игра активна и есть патроны
        while (IsActive && ammoLeft > 0)
        {
            // Ждем рандомное время перед следующей мишенью
            yield return new WaitForSeconds(Random.Range(0.5f, popDelayMax));

            if (!IsActive || ammoLeft <= 0) break;

            // Если все мишени уже выскакивали - начинаем заново
            if (availableTargets.Count == 0)
            {
                availableTargets = new List<PopUpTarget>(targets);
            }

            // Тыкаем пальцем в небо и выбираем мишень
            int randomIndex = Random.Range(0, availableTargets.Count);
            PopUpTarget target = availableTargets[randomIndex];

            if (target != null)
            {
                // Подъем мешений
                target.PopUp();

                // Ждем пока мишень постоит, если не сбили - сама уедет
                yield return new WaitForSeconds(targetStayTime);

                // Если еще не сбили - опускаем нахуй
                if (target.IsUp)
                {
                    target.Hit(false); // false - не засчитываем как попадание
                }

                // Убираем из списка доступных, чтоб не повторялась сразу
                availableTargets.RemoveAt(randomIndex);
            }
        }

        // Кончились патроны = конец игры
        if (ammoLeft <= 0)
        {
            EndGame();
        }
    }

    
    public void PickupRevolver()
    {
        if (revolverPicked || !IsActive) return;
        revolverPicked = true;

        // Вешаем ревик на руку
        tableRevolver.transform.SetParent(gunHand);
        tableRevolver.transform.localPosition = new Vector3(0.35f, -0.3f, 0.6f);
        tableRevolver.transform.localRotation = Quaternion.Euler(-10f, 0, 0);

        // Выключаем коллайдеры чтоб не мешались
        foreach (Collider c in tableRevolver.GetComponentsInChildren<Collider>())
            c.enabled = false;

        // Добавляем скрипт который стреляет
        RevolverShooter shooter = tableRevolver.GetComponent<RevolverShooter>();
        if (shooter == null) shooter = tableRevolver.AddComponent<RevolverShooter>();

        shooter.manager = this;

        
        if (playerCamera != null)
            shooter.playerCamera = playerCamera.GetComponent<Camera>();
        else
        {
            shooter.playerCamera = Camera.main;
        }

        
        shooter.FindBullets();
    }

    // Эту хуйню вызывает мишень когда в нее попадают
    public void OnTargetHit()
    {
        if (!IsActive) return;

        score++;
        UpdateUI();

        // Если все мишени повалил = победа тока нахуй хуй пайми почему можно уложить тока 5 это либо из-за того что первая мишень target а все остальные target(1)... target(5) ну или я еблан
        if (score >= maxTargets)
            EndGame();
    }

    // Обновляем UI чтобы даун видел сколько попаданий и патронов
    private void UpdateUI()
    {
        scoreText.text = $"Hits: {score}/{maxTargets}";
        ammoText.text = $"Ammo: {ammoLeft}";
    }

    // Тратятся патрон при выстреле
    public void UseAmmo()
    {
        if (!IsActive) return;

        ammoLeft--;
        UpdateUI();

        if (ammoLeft <= 0)
        {
            ReturnRevolverToTable();
            EndGame();
        }
    }

    // Возвращатся ревик на стол когда кончились патроны
    private void ReturnRevolverToTable()
    {
        if (!revolverPicked) return;

        // Открепляем от руки и кладем на место
        tableRevolver.transform.SetParent(null);
        tableRevolver.transform.position = originalRevolverPos;
        tableRevolver.transform.rotation = originalRevolverRot;

        // Удаляем скрипт стрельбы - больше не палим
        RevolverShooter shooter = tableRevolver.GetComponent<RevolverShooter>();
        if (shooter != null) Destroy(shooter);

        // Включаем коллайдер обратно
        Collider col = tableRevolver.GetComponent<Collider>();
        if (col) col.enabled = true;

        revolverPicked = false;
    }

    // Конец игры, все, пиздец
    private void EndGame()
    {
        IsActive = false;

        if (targetCycleCoroutine != null)
        {
            StopCoroutine(targetCycleCoroutine);
            targetCycleCoroutine = null;
        }

        // Опускаем все мишени
        foreach (var target in targets)
        {
            if (target != null)
                target.ResetTarget();
        }

        ShowSpeechBubble();
    }

    // Показываем баббл с результатами
    private void ShowSpeechBubble()
    {
        if (speechBubblePrefab == null || guySpeechPos == null) return;

        GameObject bubble = Instantiate(speechBubblePrefab, guySpeechPos.position, guySpeechPos.rotation);
        bubble.transform.SetParent(guySpeechPos);
        bubble.transform.localPosition = Vector3.up * 1.5f;
        bubble.transform.localRotation = Quaternion.identity;

        TextMeshProUGUI text = bubble.GetComponentInChildren<TextMeshProUGUI>();
        if (text)
        {
            string msg = $"Ты попал {score}/{maxTargets}! ";
            if (score >= 5) msg += "ПЕРФЕКТ! Так не стреляют даже 10lvl faceit!";
            else if (score >= 3) msg += "Нормально стреляешь, ковбой!";
            else msg += "Бро, тебе нужно больше тренероваться";
            text.text = msg;
        }

        StartCoroutine(FadeBubble(bubble, 4f));
    }

    // Баббл плавно исчезает
    private IEnumerator FadeBubble(GameObject bubble, float duration)
    {
        CanvasGroup cg = bubble.GetComponent<CanvasGroup>();
        if (cg == null) cg = bubble.AddComponent<CanvasGroup>();
        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            cg.alpha = 1 - (t / duration);
            yield return null;
        }
        Destroy(bubble);
    }

    // Выключение нужно сделать на кнопку, крч сам сделаешь не маленький
    public void DeactivateShooting()
    {
        IsActive = false;

        if (targetCycleCoroutine != null)
        {
            StopCoroutine(targetCycleCoroutine);
            targetCycleCoroutine = null;
        }

        scoreCanvas.gameObject.SetActive(false);
        ReturnRevolverToTable();
    }
}