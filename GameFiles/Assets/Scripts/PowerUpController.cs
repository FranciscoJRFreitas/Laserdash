using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpController : MonoBehaviour
{
    public enum PowerUpType { IncreaseDamage, LongLaser, QuickFire }
    public PowerUpType powerUpType;
    public float speed = 20.0f; // Speed at which the power-up moves towards the player
    public float despawnZ = -4.0f; // Z position at which power-ups despawn

    void Update()
    {
        
        transform.position += Vector3.back * speed * Time.deltaTime;

        
        if (transform.position.z < despawnZ)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.AddPoints(25);
                switch (powerUpType)
                {
                    case PowerUpType.IncreaseDamage:
                        playerController.IncreaseDamage();
                        break;
                    case PowerUpType.LongLaser:
                        playerController.ExtendLaserLife();
                        break;
                    case PowerUpType.QuickFire:
                        playerController.DecreaseShootingInterval();
                        break;
                }
            }
            Destroy(gameObject);
        }
    }
}
