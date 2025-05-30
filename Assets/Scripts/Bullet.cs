using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float lifetime = 2f;
    
    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Asteroid"))
        {
            other.GetComponent<Asteroid>().Split();
            Destroy(gameObject);
        }
    }
} 