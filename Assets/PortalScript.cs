using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalScript : MonoBehaviour
{
    public string menuSceneName = "MainMenu";
    public ParticleSystem portalParticles;
    public AudioClip portalSound;
    public float pullRadius = 5f; // Raio de atração
    public float pullForce = 2f; // Força de atração
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        AttractPlayer();
    }

    void AttractPlayer()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, pullRadius);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                Rigidbody2D rb = collider.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    Vector2 direction = (transform.position - collider.transform.position).normalized;
                    rb.AddForce(direction * pullForce);
                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayPortalSound();
            PlayPortalParticles();
            Invoke("LoadMenuScene", 1f);
        }
    }

    void PlayPortalSound()
    {
        if (audioSource != null && portalSound != null)
        {
            audioSource.PlayOneShot(portalSound);
        }
    }

    void PlayPortalParticles()
    {
        if (portalParticles != null)
        {
            portalParticles.Play();
        }
    }

    void LoadMenuScene()
    {
        SceneManager.LoadScene(menuSceneName);
    }

    // Desenha o raio de atração no editor para visualização
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pullRadius);
    }
}
