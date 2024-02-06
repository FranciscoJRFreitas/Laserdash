using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public GameObject projectilePrefab;
    public float step = 0.0001f;
    public float projectileSpeed = 2.0f;
    private float xRange = 4.0f;
    public float despawnTime = 2.0f;
    private float timeBetweenSpawn = 1f;
    private int damage = 1;
    public int CurrentDamage { get { return damage; } }
    public bool isPaused = false;
    public int points = 0;
    public int CurrentPoints { get { return points; } }
    public UIManager uiManager;
    public int health = 1;
    private float rotationDuration = 0f;
    private float maxRotationAngle = 30f;
    private float rotationSpeed = 30f;
    private float returnRotationSpeed = 5f;
    private float currentRotationAngle = 0f;
    private int lastDirection = 0;
    private float lastTapTime = 0f;
    private float doubleTapThreshold = 0.25f;
    private bool isDashing = false;
    private float dashDistance = 4.0f;
    private float dashTime = 0.5f;
    private float dashTimer = 0f;
    private Vector3 dashTarget;

    private void Start()
    {
        StartCoroutine(ShootProjectile());

    }

    private IEnumerator ShootProjectile()
    {
        while (true)
        {
            GameObject projectile = Instantiate(projectilePrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z + 1), projectilePrefab.transform.rotation);
            Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();
            projectileRb.velocity = Vector3.forward * projectileSpeed;

            Destroy(projectile, despawnTime);

            yield return new WaitForSeconds(timeBetweenSpawn);
        }
    }

    void Update()
    {
        bool isMoving = false;
        int direction = 0; // -1 for left, 1 for right, 0 for no movement

        if (Input.touchCount > 0 && !IsPointerOverUIObject())
        {
            Touch touch = Input.GetTouch(0);

            if (touch.position.x < Screen.width / 2 && transform.position.x > -xRange && Time.timeScale == 1)
            {
                // Move left
                transform.position = new Vector3(transform.position.x - step, transform.position.y, transform.position.z);
                rotationDuration += Time.deltaTime;
                isMoving = true;
                direction = -1;
            }
            else if (touch.position.x > Screen.width / 2 && transform.position.x < xRange && Time.timeScale == 1)
            {
                // Move right
                transform.position = new Vector3(transform.position.x + step, transform.position.y, transform.position.z);
                rotationDuration += Time.deltaTime;
                isMoving = true;
                direction = 1;
            }
        }

        if (isMoving)
        {
            float targetRotationAngle = Mathf.Clamp(rotationDuration * rotationSpeed * direction, -maxRotationAngle, maxRotationAngle);
            if (direction != lastDirection)
            {
                rotationDuration = Mathf.Abs(currentRotationAngle) / rotationSpeed;
            }
            currentRotationAngle = Mathf.Lerp(currentRotationAngle, targetRotationAngle, Time.deltaTime * rotationSpeed);
            transform.rotation = Quaternion.Euler(0, 0, currentRotationAngle);
            lastDirection = direction;
        }
        else
        {
            if (transform.rotation != Quaternion.identity)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.identity, Time.deltaTime * returnRotationSpeed);
                currentRotationAngle = Mathf.Lerp(currentRotationAngle, 0, Time.deltaTime * returnRotationSpeed);
            }
            rotationDuration = 0f;
            lastDirection = 0;
        }

        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -xRange, xRange), transform.position.y, transform.position.z);


        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            if (Time.time - lastTapTime < doubleTapThreshold && !isDashing && !IsPointerOverUIObject())
            {
                // Double tap detected
                StartDash(Input.GetTouch(0).position);
            }
            lastTapTime = Time.time;
        }

        if (isDashing)
        {
            Dash();
        }

        if (health < 1)
        {
            GameOver();
        }

    }

    void GameOver()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1;
    }

    private void StartDash(Vector2 tapPosition)
    {
        isDashing = true;
        dashTimer = dashTime;

        // Determine direction based on tap position
        float direction = tapPosition.x < Screen.width / 2 ? -1f : 1f;
        dashTarget = transform.position + new Vector3(direction * dashDistance, 0, 0);
        dashTarget.x = Mathf.Clamp(dashTarget.x, -xRange, xRange); // Ensure target is within bounds

        // Start rotation
        StartCoroutine(Perform360Rotation());
    }

    private void Dash()
    {
        if (dashTimer > 0)
        {
            transform.position = Vector3.Lerp(transform.position, dashTarget, Time.deltaTime / dashTimer);
            dashTimer -= Time.deltaTime;
        }
        else
        {
            isDashing = false;
        }
    }

    private IEnumerator Perform360Rotation()
    {
        float rotationSpeed = 720f / dashTime; // Complete 360 rotation in dashTime
        float startRotation = transform.rotation.eulerAngles.z;
        float endRotation = startRotation + 360f;
        float currentRotation = startRotation;

        while (currentRotation < endRotation)
        {
            currentRotation += rotationSpeed * Time.deltaTime;
            transform.rotation = Quaternion.Euler(0, 0, currentRotation);
            yield return null;
        }

        // Ensure the rotation is exactly 360 degrees at the end
        transform.rotation = Quaternion.Euler(0, 0, startRotation);
    }

    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }


    public void IncreaseHealth(int healthAmount)
    {
        health += healthAmount;
        if (uiManager != null)
        {
            uiManager.UpdateHP(health);
        }
    }

    public void DecreaseHealth(int amount)
    {
        health -= amount;
        if (health <= 0)
        {
            GameOver();
        }

        uiManager.UpdateHP(health);
    }

    public void IncreaseDamage()
    {
        damage += 1;
    }

    public void ExtendLaserLife()
    {
        despawnTime += 1;
    }

    public void DecreaseShootingInterval()
    {
        timeBetweenSpawn *= 0.9f;
        projectileSpeed *= 1.1f;
    }

    public void AddPoints(int pointsToAdd)
    {
        points += pointsToAdd;
        if (uiManager != null)
        {
            uiManager.UpdatePoints(points);
        }
    }
}
