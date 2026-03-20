using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float jumpForce;
    public float gravityModifier;
    public ParticleSystem explosionParticle;
    public ParticleSystem dirtParticle;

    public AudioClip jumpSfx;
    public AudioClip crashSfx;

    private Rigidbody rb;
    private InputAction jumpAction;
    private bool isOnGround = true;

    private Animator playerAnim;
    private AudioSource playerAudio;

    public bool gameOver = false;

    private int healthPoint;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerAnim = GetComponent<Animator>();
        playerAudio = GetComponent<AudioSource>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Physics.gravity *= gravityModifier;

        jumpAction = InputSystem.actions.FindAction("Jump");

        gameOver = false;

        healthPoint = 3;
    }

    // Update is called once per frame
    void Update()
    {
        if (jumpAction.triggered && isOnGround && !gameOver)
        {
            rb.AddForce(jumpForce * Vector3.up, ForceMode.Impulse);
            isOnGround = false;
            playerAnim.SetTrigger("Jump_trig");
            dirtParticle.Stop();
            playerAudio.PlayOneShot(jumpSfx);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isOnGround = true;
            dirtParticle.Play();
        }
        else if (collision.gameObject.CompareTag("Obstacle"))
        {
            healthPoint -= 1;
            Debug.Log(healthPoint);
            Debug.Log("Hit obstacle!");
            explosionParticle.Play();
            dirtParticle.Stop();
            playerAudio.PlayOneShot(crashSfx);
            Destroy(collision.gameObject);
            if (healthPoint <= 0)
            {
                Debug.Log("Game Over!");
                gameOver = true;
                playerAnim.SetBool("Death_b", true);
                playerAnim.SetInteger("DeathType_int", 1);
            }
        }
        /*
        else if (collision.gameObject.CompareTag("Obstacle") && healthPoint <= 0)
        {
            Debug.Log("Game Over!");
            gameOver = true;
            playerAnim.SetBool("Death_b", true);
            playerAnim.SetInteger("DeathType_int", 1);
            explosionParticle.Play();
            dirtParticle.Stop();
            playerAudio.PlayOneShot(crashSfx);
        }
        */
    }

}