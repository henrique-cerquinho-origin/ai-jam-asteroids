using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float thrustSpeed = 5f;
    [SerializeField] private float rotationSpeed = 180f;
    [SerializeField] private float maxSpeed = 10f;
    
    [Header("Shooting Settings")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private float shootingCooldown = 0.2f;

    private Rigidbody2D rb;
    private float lastShootTime;
    private bool isThrusting;
    private bool wasThrusting;
    private string currentThrustSoundName = "thrust";
    
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
    }

    private void Update()
    {
        // Rotation
        float rotation = Input.GetAxis("Horizontal");
        transform.Rotate(Vector3.forward * -rotation * rotationSpeed * Time.deltaTime);

        // Thrust
        wasThrusting = isThrusting;
        isThrusting = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);

        // Handle thrust sound
        if (isThrusting && !wasThrusting)
        {
            AudioManager.Instance.PlaySFX(currentThrustSoundName);
        }
        else if (!isThrusting && wasThrusting)
        {
            AudioManager.Instance.StopAllSFX();
        }

        // Shooting
        if (Input.GetKey(KeyCode.Space) && Time.time > lastShootTime + shootingCooldown)
        {
            Shoot();
        }

        // Screen wrapping
        WrapAroundScreen();
    }

    private void FixedUpdate()
    {
        if (isThrusting)
        {
            rb.AddForce(transform.up * thrustSpeed);
            
            // Limit speed
            if (rb.linearVelocity.magnitude > maxSpeed)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
            }
        }
    }

    private void Shoot()
    {
        lastShootTime = Time.time;
        GameObject bullet = Instantiate(bulletPrefab, transform.position + transform.up, transform.rotation);
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        bulletRb.linearVelocity = transform.up * bulletSpeed;
        
        // Play shoot sound
        AudioManager.Instance.PlaySFX("shoot");
    }

    private void OnDestroy()
    {
        // Stop thrust sound if it's playing when destroyed
        if (isThrusting)
        {
            AudioManager.Instance.StopAllSFX();
        }
        
        // Play explosion sound when player is destroyed
        if (gameObject.scene.isLoaded) // Only play if destroyed during gameplay, not scene unload
        {
            AudioManager.Instance.PlaySFX("player_explosion");
        }
    }

    private void WrapAroundScreen()
    {
        Vector3 viewportPosition = Camera.main.WorldToViewportPoint(transform.position);
        Vector3 newPosition = transform.position;

        if (viewportPosition.x < 0)
            newPosition.x = Camera.main.ViewportToWorldPoint(new Vector3(1, viewportPosition.y, viewportPosition.z)).x;
        else if (viewportPosition.x > 1)
            newPosition.x = Camera.main.ViewportToWorldPoint(new Vector3(0, viewportPosition.y, viewportPosition.z)).x;

        if (viewportPosition.y < 0)
            newPosition.y = Camera.main.ViewportToWorldPoint(new Vector3(viewportPosition.x, 1, viewportPosition.z)).y;
        else if (viewportPosition.y > 1)
            newPosition.y = Camera.main.ViewportToWorldPoint(new Vector3(viewportPosition.x, 0, viewportPosition.z)).y;

        transform.position = newPosition;
    }
} 