using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField] private float minSpeed = 1f;
    [SerializeField] private float maxSpeed = 3f;
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private GameObject asteroidPrefab;
    [SerializeField] private int points = 100;
    
    private float size;
    private Rigidbody2D rb;
    
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        size = transform.localScale.x;
        
        // Set random velocity
        float speed = Random.Range(minSpeed, maxSpeed);
        Vector2 direction = Random.insideUnitCircle.normalized;
        rb.linearVelocity = direction * speed;
        
        // Set random rotation
        rb.angularVelocity = Random.Range(-rotationSpeed, rotationSpeed);
    }
    
    private void Update()
    {
        WrapAroundScreen();
    }
    
    public void Split()
    {
        if (size > 0.5f)
        {
            SpawnSmallerAsteroid();
            SpawnSmallerAsteroid();
        }
        
        GameManager.Instance.AddScore(points);
        AudioManager.Instance.PlaySFX("asteroid_explosion");
        Destroy(gameObject);
    }
    
    private void SpawnSmallerAsteroid()
    {
        Vector3 newScale = transform.localScale * 0.5f;
        GameObject newAsteroid = Instantiate(asteroidPrefab, transform.position, Quaternion.identity);
        newAsteroid.transform.localScale = newScale;
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
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.GameOver();
        }
    }
} 