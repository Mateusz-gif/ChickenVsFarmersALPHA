using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public bool hasPowerup;
    public GameObject powerupIndicator;
    public int powerUpDuration = 8;

    public GameObject eggPrefab;
    private float eggCd = 0.6f;
    private float eggDefaultCd = 0.6f;
    private float eggSpawn = 0.0f;

    public float speed = 5.0f;
    private Vector3 shootDirection = Vector3.forward;
    private SpawnManager spawnManager; // Reference to SpawnManager

    void Start()
    {
        spawnManager = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();
    }

    void Update()
    {
        if (spawnManager.isGameActive) // Check if the game is active
        {
            // Movement Code
            float speedX = Input.GetAxis("Horizontal");
            float speedZ = Input.GetAxis("Vertical");

            if (speedX != 0 || speedZ != 0)
            {
                shootDirection = new Vector3(speedX, 0, speedZ).normalized;
                transform.rotation = Quaternion.LookRotation(shootDirection);
            }

            transform.Translate(Vector3.forward * Time.deltaTime * speed * speedZ);
            transform.Translate(Vector3.right * Time.deltaTime * speed * speedX);

            // Projectile Code
            eggSpawn += Time.deltaTime;

            if (eggSpawn >= eggCd)
            {
                GameObject egg = Instantiate(eggPrefab, transform.position, Quaternion.LookRotation(shootDirection));
                eggSpawn = 0.0f;
            }

            powerupIndicator.transform.position = transform.position + new Vector3(75, 1f, 75);
        }
    }

    private void OnTriggerEnter(Collider bonus)
    {
        if (bonus.gameObject.CompareTag("Experience"))
        {
            Destroy(bonus.gameObject);
        }
        else if (bonus.gameObject.CompareTag("Powerup"))
        {
            hasPowerup = true;
            powerupIndicator.gameObject.SetActive(true);
            Destroy(bonus.gameObject);
            eggCd = eggDefaultCd / 10;
            StartCoroutine(PowerupCooldown());
        }
    }

    IEnumerator PowerupCooldown()
    {
        yield return new WaitForSeconds(powerUpDuration);
        hasPowerup = false;
        eggCd = eggDefaultCd;
        powerupIndicator.gameObject.SetActive(false);
    }
}
