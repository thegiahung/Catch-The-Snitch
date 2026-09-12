// Perfect game with prediction model
using System.Collections;
using UnityEngine;
using MathNet.Numerics;

public class Condition_3 : MonoBehaviour
{
    public Rigidbody rb;
    public PredictionProvider predictionProvider;
    public float randomRange = 0.003f; // Range around the player for the random position
    public float courtHeight = 0.027f; // Height of the court (ground level)
    public float moveSpeed = 2.5f; // Increased speed at which the snitch moves to the new position
    public float maxDistance = 1.5f; // Maximum distance from the initial position
    private Vector3 initialPosition; // To store the initial position of the snitch
    private Vector3 targetPosition; // To store the target position
    private bool isMoving = false; // Flag to check if snitch is moving
    private bool isEvading = false; // Flag to check if snitch is evading the player
    private float beta_init = 1.25f; // Lower L�vy flight exponent for shorter steps
    private float beta = 1.25f; // Lower L�vy flight exponent for shorter steps
    private float beta_pred = 1.0f;
    private bool goingUp = true;

    public double c = 0.8;

    // Variables to track attempts and scores
    private int attempts = 0;   // Number of times the player presses a button

    public System.Random random_predictor = new System.Random(); //**
    
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>() ?? gameObject.AddComponent<Rigidbody>();
        rb.mass = 0.005f;
        rb.drag = 0.05f;
        rb.angularDrag = 0.02f;

        initialPosition = transform.position;
        targetPosition = initialPosition;
        StartCoroutine(LevyFlightMovement());
    }

    void Update()
    {
        // Check if the player presses any of the relevant buttons (trigger or grab)
        if (OVRInput.GetDown(OVRInput.Button.Any))
        {
            attempts++;  // Increment attempts when a button is pressed down
            Debug.Log("Attempt made: " + attempts);  // Print the number of attempts
        }

        // Smoothly move the snitch towards the target position
        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            // Check if the snitch has reached the target position
            if (Vector3.Distance(transform.position, targetPosition) < 0.001f)
            {
                isMoving = false;
                if (isEvading)
                {
                    isEvading = false;
                    StartCoroutine(LevyFlightMovement());
                }
            }
        }

        EnsureAboveGround();
    }

    IEnumerator LevyFlightMovement()
    {
        while (!isEvading)
        {
            float stepLength = (float)LevyFlightStep();
            Vector3 randomDirection = Random.onUnitSphere;

            targetPosition = transform.position + randomDirection * stepLength;

            // Ensure the target position is within 0.5 units of the initial position
            ConstrainToBoundary();

            EnsureAboveGround();

            isMoving = true;
            yield return new WaitUntil(() => !isMoving);
        }
    }

    public double LevyFlightStep()
    {
        double sigma = System.Math.Pow(
            (SpecialFunctions.Gamma(1 + beta) * System.Math.Sin(System.Math.PI * beta / 2)) /
            (SpecialFunctions.Gamma((1 + beta) / 2) * beta * System.Math.Pow(2, (beta - 1) / 2)),
            1 / beta
        );
        double u = SampleNormal(0, sigma);
        double v = SampleNormal(0, 1);
        double step = (u / System.Math.Pow(System.Math.Abs(v), 1 / beta)) * 0.008;
        return step;
    }

    private double SampleNormal(double mean, double stddev)
    {
        System.Random rand = new System.Random();
        double u1 = 1.0 - rand.NextDouble();
        double u2 = 1.0 - rand.NextDouble();
        double randStdNormal = System.Math.Sqrt(-2.0 * System.Math.Log(u1)) * System.Math.Sin(2.0 * System.Math.PI * u2);
        return mean + stddev * randStdNormal;
    }

    void PerformAction()
    {
        Vector3 basePosition = transform.position;

        Vector3 randomOffset = new Vector3(
            Random.Range(-randomRange, randomRange),
            Random.Range(-randomRange, randomRange),
            Random.Range(-randomRange, randomRange)
        );

        targetPosition = goingUp ? basePosition + randomOffset : basePosition - randomOffset;
        goingUp = !goingUp;

        // Ensure the target position is within 0.5 units of the initial position
        ConstrainToBoundary();

        EnsureAboveGround();

        isMoving = true;
        isEvading = true;
        StopCoroutine(LevyFlightMovement());
        
        beta = beta_init;
    }

    void OnTriggerEnter(Collider collision)
    {
        // Handle interaction with the prediction model
        if (collision.gameObject.CompareTag("PreModel"))
        {
            beta = beta_pred;
            PerformAction();
        }

        // Handle interaction with the right hand controller
        if (collision.gameObject.CompareTag("RightHand")&& AnyButtonPressed())
        {
            FindObjectOfType<ScoreManager>().AddScore(1);
            ResetSnitchPosition(); // Reset the snitch's position after being hit
        }
    }

    bool AnyButtonPressed()
    {
        // Check if any button is pressed on the Oculus controller
        return OVRInput.Get(OVRInput.Button.Any);
    }
    void ResetSnitchPosition()
    {
        Vector3 randomPosition;

        // Ensure the snitch spawns outside the player's body (0.2 to -0.2 range for x and z)
        randomPosition = new Vector3(
            Random.Range(-0.7f, 0.7f),
            Random.Range(1.2f, 1.6f),
            Random.Range(0.3f, 0.7f)
        );

        transform.position = randomPosition;
        targetPosition = randomPosition;
        initialPosition = randomPosition;
    }

    // Ensure the snitch is always above the ground
    private void EnsureAboveGround()
    {
        // Ensure the new position is above the court by at least ... units
        if (targetPosition.y < courtHeight + 1f)
        {
            targetPosition.y = courtHeight + 1f;
        }
    }

    // Constrain the target position within the maximum distance from the initial position
    private void ConstrainToBoundary()
    {
        // Reflect X direction if hitting the X boundary
        if (targetPosition.x < -0.7f || targetPosition.x > 0.7f)
        {
            Vector3 direction = (targetPosition - transform.position).normalized;
            direction.x = -direction.x; // Reverse X direction
            targetPosition = transform.position + direction * (Vector3.Distance(transform.position, targetPosition) - Mathf.Abs(targetPosition.x - transform.position.x));
        }

        // Reflect Z direction if hitting the Z boundary
        if (targetPosition.z < -0.7f || targetPosition.z > 0.7f)
        {
            Vector3 direction = (targetPosition - transform.position).normalized;
            direction.z = -direction.z; // Reverse Z direction
            targetPosition = transform.position + direction * (Vector3.Distance(transform.position, targetPosition) - Mathf.Abs(targetPosition.z - transform.position.z));
        }

        // Reflect Y direction if exceeding the Y boundary
        if (targetPosition.y > 1.6f)
        {
            Vector3 direction = (targetPosition - transform.position).normalized;
            direction.y = -direction.y; // Reverse Y direction
            targetPosition = transform.position + direction * (Vector3.Distance(transform.position, targetPosition) - Mathf.Abs(targetPosition.y - transform.position.y));
        }
    }
}
