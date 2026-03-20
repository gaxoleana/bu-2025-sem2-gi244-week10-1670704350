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
    private InputAction shiftAction;
    private bool isOnGround = true;
    private bool doubleJump;

    private Animator playerAnim;
    private AudioSource playerAudio;

    public bool gameOver = false;

    private int healthPoint;
    private MoveLeft moveLeft;

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
        shiftAction = InputSystem.actions.FindAction("Sprint");

        gameOver = false;

        healthPoint = 3;
    }

    // Update is called once per frame
    void Update()
    {
        if (jumpAction.triggered && !gameOver)
        {
            if (isOnGround)
            {
                rb.AddForce(jumpForce * Vector3.up, ForceMode.Impulse);
                isOnGround = false;
                doubleJump = true;
                playerAnim.SetTrigger("Jump_trig");
                dirtParticle.Stop();
                playerAudio.PlayOneShot(jumpSfx);
            }
            else if (doubleJump)
            {
                rb.AddForce(jumpForce * Vector3.up, ForceMode.Impulse);
                doubleJump = false;
                playerAnim.SetTrigger("Jump_trig");
                playerAudio.PlayOneShot(jumpSfx);
            }
        }

        if (shiftAction.triggered)
        {
            moveLeft.speed = 20;
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