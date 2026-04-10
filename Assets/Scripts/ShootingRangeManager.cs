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
    public float targetStayTime = 2f;

    private int score = 0;
    private int ammoLeft = 6;
    public bool IsActive { get; private set; } = false;
    private bool revolverPicked = false;
    private Vector3 originalRevolverPos;
    private Quaternion originalRevolverRot;
    private Coroutine targetCycleCoroutine;

    void Awake()
    {
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

        foreach (var target in targets)
        {
            if (target != null)
            {
                target.manager = this;
            }
        }
    }

    public void ActivateShooting()
    {
        if (IsActive) return;
        IsActive = true;
        score = 0;
        ammoLeft = 6;
        revolverPicked = false;
        scoreCanvas.gameObject.SetActive(true);
        UpdateUI();

        foreach (var target in targets)
        {
            if (target != null)
                target.ResetTarget();
        }

        if (targetCycleCoroutine != null)
            StopCoroutine(targetCycleCoroutine);

        targetCycleCoroutine = StartCoroutine(TargetCycle());
    }

    private IEnumerator TargetCycle()
    {

        List<PopUpTarget> availableTargets = new List<PopUpTarget>(targets);

        while (IsActive && ammoLeft > 0)
        {
            yield return new WaitForSeconds(Random.Range(0.5f, popDelayMax));

            if (!IsActive || ammoLeft <= 0) break;
            if (availableTargets.Count == 0)
            {
                availableTargets = new List<PopUpTarget>(targets);
            }

            int randomIndex = Random.Range(0, availableTargets.Count);
            PopUpTarget target = availableTargets[randomIndex];

            if (target != null)
            {
                target.PopUp();
                yield return new WaitForSeconds(targetStayTime);
                if (target.IsUp)
                {
                    target.Hit(false); 
                }
                availableTargets.RemoveAt(randomIndex);
            }
        }

        if (ammoLeft <= 0)
        {
            EndGame();
        }
    }

    
    public void PickupRevolver()
    {
        if (revolverPicked || !IsActive) return;
        revolverPicked = true;

        tableRevolver.transform.SetParent(gunHand);
        tableRevolver.transform.localPosition = new Vector3(0.35f, -0.3f, 0.6f);
        tableRevolver.transform.localRotation = Quaternion.Euler(-10f, 0, 0);

        foreach (Collider c in tableRevolver.GetComponentsInChildren<Collider>())
            c.enabled = false;

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

    public void OnTargetHit()
    {
        if (!IsActive) return;
        score++;
        UpdateUI();
    }

    private void UpdateUI()
    {
        scoreText.text = $"Hits: {score}/{maxTargets}";
        ammoText.text = $"Ammo: {ammoLeft}";
    }

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

    private void ReturnRevolverToTable()
    {
        if (!revolverPicked) return;

        tableRevolver.transform.SetParent(null);
        tableRevolver.transform.position = originalRevolverPos;
        tableRevolver.transform.rotation = originalRevolverRot;

        RevolverShooter shooter = tableRevolver.GetComponent<RevolverShooter>();
        if (shooter != null) Destroy(shooter);

        Collider col = tableRevolver.GetComponent<Collider>();
        if (col) col.enabled = true;

        revolverPicked = false;
    }

    private void EndGame()
    {
        IsActive = false;

        if (targetCycleCoroutine != null)
        {
            StopCoroutine(targetCycleCoroutine);
            targetCycleCoroutine = null;
        }

        foreach (var target in targets)
        {
            if (target != null)
                target.ResetTarget();
        }

        ShowSpeechBubble();
    }

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

        StartCoroutine(FadeBubble(bubble, 7f));
    }

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