using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    public int health = 1;
    private Text healthText;
    private float speed;
    private float despawnZ;

    void Start()
    {

        float timeSinceRestart = Time.time - PauseManager.lastRestartTime;
        float timeFactor = timeSinceRestart / 10.0f;
        int randomFactor = Random.Range(1, 3);
        health = Mathf.RoundToInt(health + timeFactor * randomFactor);

        GameObject textObj = new GameObject("HealthText");
        textObj.transform.SetParent(transform, false);
        textObj.transform.localPosition = new Vector3(0, 1.5f, -0.5f);

        healthText = textObj.AddComponent<Text>();
        healthText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        healthText.fontSize = 12;
        healthText.alignment = TextAnchor.MiddleCenter;
        healthText.color = Color.white;

        Canvas canvas = textObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        CanvasScaler scaler = textObj.AddComponent<CanvasScaler>();
        scaler.dynamicPixelsPerUnit = 10;
        textObj.AddComponent<GraphicRaycaster>();

        RectTransform rectTransform = textObj.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(100, 50);
        rectTransform.localScale = Vector3.one * 0.1f;

        UpdateHealthText();
    }


    public void TakeDamage(int damageAmount)
    {
        health -= damageAmount;
        UpdateHealthText();

        if (health <= 0)
        {
            Destroy(gameObject);
            PlayerController playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.AddPoints(100);
                playerController.IncreaseHealth(1);
            }
        }
    }


    private void UpdateHealthText()
    {
        healthText.text = health.ToString();
    }

    public void SetMovement(float movementSpeed, float zDespawn)
    {
        speed = movementSpeed;
        despawnZ = zDespawn;
    }

    void Update()
    {
        transform.position += Vector3.back * speed * Time.deltaTime;

        if (transform.position.z < despawnZ)
        {
            PlayerController playerController = FindObjectOfType<PlayerController>();
            if (playerController != null)
            {
                playerController.AddPoints(-100);
            }
                Destroy(gameObject);
        }

    }

    void OnTriggerEnter(Collider other)
    {
        PlayerController playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();

        if (other.CompareTag("Laser"))
        {

            if (playerController != null)
            {
                int currentDamage = playerController.CurrentDamage;
                TakeDamage(currentDamage);
            }

            Destroy(other.gameObject);
        }

        if (other.CompareTag("Player"))
        {
            if (playerController != null)
            {
                playerController.DecreaseHealth(health);
            }

            Destroy(this.gameObject);
        }
    }

}
