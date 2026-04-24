using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuProtagonist : MonoBehaviour
{
    [Header("Walk Points")]
    public Transform point1;           
    public Transform point2;           

    [Header("Speed")]
    public float minSpeed = 2f;
    public float maxSpeed = 2f;

    private float currentSpeed;
    private bool goingRight = false;
    private bool isExiting = false;
    private SpriteRenderer sr;
    private Animator anim;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        goingRight = false;
        isExiting = false;
    transform.position = point1.position;
        PickNewSpeed();
    }

    void Update()
    {
        if (isExiting)
        {
          
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(point2.position.x + 20f, transform.position.y), currentSpeed * Time.deltaTime);
            return;
        }

       
        Vector2 target = goingRight ? point2.position : point1.position;

        transform.position = Vector2.MoveTowards(transform.position, target, currentSpeed * Time.deltaTime);

 


        sr.flipX = !goingRight;

      
        if (Vector2.Distance(transform.position, target) < 0.05f)
        {
            goingRight = !goingRight;
            PickNewSpeed();
        }
    }

    void PickNewSpeed()
    {
        currentSpeed = Random.Range(minSpeed, maxSpeed);
    }

    public void StartExit()
    {
        isExiting = true;
        goingRight = true;
        sr.flipX = false;
        currentSpeed = 5f;
        anim.SetBool("isWalking", true);

        Invoke("LoadTutorial", 1.5f);
    }

    void LoadTutorial()
    {
        SceneManager.LoadScene("TutorialScene"); 
    }
}