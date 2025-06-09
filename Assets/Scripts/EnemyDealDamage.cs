using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EnemyDealDamage : MonoBehaviour
{
    private UnityEngine.AI.NavMeshAgent agent;
    private Transform player;
    private PlayerInteractions playerInteractions;

    public float killDistance = 1.5f; // Distancia mínima para "matar" al jugador
    public AudioClip deathSound;      // Sonido de muerte
    public Image deathOverlay;        // Imagen de pantalla roja
    public float fadeDuration = 0.5f; // Tiempo de fundido

    private bool isKilling = false;

    void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
            playerInteractions = playerObject.GetComponent<PlayerInteractions>();
        }

        if (deathOverlay != null)
        {
            Color temp = deathOverlay.color;
            temp.a = 0f;
            deathOverlay.color = temp;
        }
    }

    void Update()
    {
        if (player == null || playerInteractions == null || isKilling) return;

        if (playerInteractions.IsTransformed()) return;

        if (Vector3.Distance(transform.position, player.position) <= killDistance)
        {
            isKilling = true;
            StartCoroutine(DeathSequence());
        }
    }

    IEnumerator DeathSequence()
    {
        // Reproducir sonido
        AudioSource.PlayClipAtPoint(deathSound, player.position);

        // Fundido a rojo
        if (deathOverlay != null)
        {
            float timer = 0f;
            Color color = deathOverlay.color;
            while (timer < fadeDuration)
            {
                timer += Time.unscaledDeltaTime;
                float alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
                deathOverlay.color = new Color(color.r, color.g, color.b, alpha);
                yield return null;
            }
        }

        yield return new WaitForSecondsRealtime(1f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}