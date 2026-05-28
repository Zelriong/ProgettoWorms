using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public enum GameState
{
    moving,
    shooting,
    jumping,
}
public class PlayerMovement : MonoBehaviour
{
    [Header("Movimento")]
    [SerializeField] float movementSpeed;
    [SerializeField] float jumpForce;

    [Header("Mira)")]
    [SerializeField] GameObject aim;
    [SerializeField] Image aimCharge;
    [SerializeField] float chargeSpeed;

    [Header("Sparo")]
    [SerializeField] GameObject bullet;
    [SerializeField] Transform spawnPoint;
    public float power;
    [SerializeField] float maxPower = 250;

    Rigidbody2D rb;
    BoxCollider2D boxCollider2d;

    Vector2 moveDirection;
    float isJumping;

    public LayerMask mask;

    public PlayerInputs inputs;

    float direction;


    GameState state;

    private void Awake()
    {
    }
    private void OnEnable()
    {
        inputs = new PlayerInputs();   
        inputs.Player.Jump.performed += Jump;
        inputs.Player.BeginShooting.performed += IsShooting;
        inputs.Player.BackFromShooting.performed += CancelShooting;
        inputs.Player.Shoot.performed += Shot;
        state = GameState.moving;
        
    }

    

    private void OnDisable()
    {
        inputs.Player.BeginShooting.performed -= IsShooting;
        inputs.Player.Jump.performed -= Jump;
        inputs.Player.BackFromShooting.performed -= CancelShooting;
        inputs.Player.Shoot.performed -= Shot;

    }
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider2d = GetComponent<BoxCollider2D>();
        direction = 1;
    }
    private void Update()
    {
        moveDirection = inputs.Player.Movement.ReadValue<Vector2>();
        isJumping = inputs.Player.Jump.ReadValue<float>();

        if (moveDirection.x < 0) { direction = -1; } // se ci sportiamo a sx si gira a sx

        else if (moveDirection.x > 0) { direction = 1; } // se ci giriamo a dx si gira a dx

        //if (moveDirection.x != 0) { anim.SetBool("IsMoving", true); }
        //else { anim.SetBool("IsMoving", false); }

        transform.localScale = new Vector3(direction, transform.localScale.y, transform.localScale.z); //la scale va in base alla direction

        Debug.Log(state);
        
    }

    private void FixedUpdate()
    {
        if (IsGrounded() == true && state == GameState.moving) Move();
         

    }

    private bool IsGrounded()
    {
        /*bool isGrounded*/
        ;
        RaycastHit2D rayHit = Physics2D.BoxCast(boxCollider2d.bounds.center, boxCollider2d.bounds.size, 0f, Vector2.down, 0.1f, mask);

        //Collider2D colliders = Physics2D.OverlapCircle(boxCollider2d.bounds.center, 1f, 6);

        //if (rayHit == null) return false;

        //else return true;a

        return rayHit;
    }

    void Move()
    {
        rb.linearVelocity = new Vector2(moveDirection.x * movementSpeed * Time.fixedDeltaTime, 0);


    }

    void Jump(InputAction.CallbackContext context)
    {
        rb.AddForce(new Vector2(0.8f * direction, 1f) * jumpForce, ForceMode2D.Impulse);

    }

    private void IsShooting(InputAction.CallbackContext context)
    {
        if (IsGrounded() && state == GameState.moving)
        {
            state = GameState.shooting;
            aim.gameObject.SetActive(true);
            StartCoroutine(Shooting());
            
        }
        
    }

    private void CancelShooting(InputAction.CallbackContext context)
    {
        if (state != GameState.shooting) return;

        state = GameState.moving;
    }

    private void Shot(InputAction.CallbackContext context)
    {
        StartCoroutine(Shoot());
    }

    IEnumerator Shooting() 
    {
        
        Vector3 rotateZ = new Vector3(0, 0, moveDirection.y);
        while (power < 0)
        {
            aim.transform.Rotate(rotateZ);

            if(state != GameState.shooting) yield return null;

            break;
        }
        yield return null;
    }


    IEnumerator Shoot()
    {
        if (state != GameState.shooting) yield return null;
        aimCharge.fillAmount = maxPower / power;

        while (true)
        {
            if(power >= maxPower) break;

            power += chargeSpeed * Time.deltaTime;

            if (Input.GetMouseButtonUp(0))
            {
                Instantiate(bullet, spawnPoint.position, spawnPoint.rotation, transform);
                aim.gameObject.SetActive(false);
                break;
            }
        }

        //Instantiate(bullet, spawnPoint.position, spawnPoint.rotation, transform);

        yield return new WaitForSeconds(3f);

        power = 0f;

        state = GameState.moving; //poi da cambiare in "fineTurno"

        StopAllCoroutines();

    }
    //private void OnDrawGizmos()
    //{
    //    Gizmos.DrawCube(new Vector3(boxCollider2d.bounds.center.x, boxCollider2d.bounds.center.y - 0.1f, boxCollider2d.bounds.center.z), boxCollider2d.bounds.size);
    //    Gizmos.color = Color.greenYellow;
    //}
}
