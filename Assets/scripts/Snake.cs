using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake : MonoBehaviour
{
  private LineRenderer lineRenderer;
  private CircleCollider2D headCollider;
  private Rigidbody2D rigidBody2D;
  private List<Vector3> linePoints;
  private List<GameObject> tailSegments;
  private Vector3 currentPosition;
  private Vector3 targetRotation;

  public GameObject deathPrefab;

  public GameObject head;

  public float speed = 2.0f;
  public float turnSpeed = 2.0f;
  public int maxLength = 3;
  public float length = 0f;



  // Use this for initialization
  void Start()
  {
    linePoints = new List<Vector3>();
    tailSegments = new List<GameObject>();
    lineRenderer = gameObject.GetComponent<LineRenderer>();
    headCollider = gameObject.GetComponent<CircleCollider2D>();
    rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
  }

  // Update is called once per frame
  void Update()
  {
    UpdateRotation();
    UpdatePosition();
    UpdateBody();
    DrawDebug();
  }

  void OnTriggerEnter2D(Collider2D other)
  {
    if (other.gameObject.CompareTag("Pickup"))
    {
      other.gameObject.SendMessage("Collect");
      maxLength = maxLength + 1;
    }
    else if (other.gameObject.CompareTag("Danger"))
    {
      KillSnake();
    }
    else if (other.gameObject.CompareTag("Exit"))
    {
      EventManager.TriggerEvent("ResetLevel");
      ResetSnake();
    }

  }

  void ResetSnake()
  {
    maxLength = 1;
    while (tailSegments.Count > 0)
    {
      GameObject temp = tailSegments[0];
      tailSegments.RemoveAt(0);
      Destroy(temp);
    }
    linePoints = new List<Vector3>();
  }

  void KillSnake()
  {
    Instantiate(deathPrefab, head.transform.position, Quaternion.identity);

    for (int i = 0; i < tailSegments.Count; i++)
    {
      Instantiate(deathPrefab, tailSegments[i].transform.position, Quaternion.identity);
    }
  }

  void DrawDebug()
  {
    Debug.DrawRay(transform.position, head.transform.up, Color.magenta);
  }

  void UpdateRotation()
  {
    float h = Input.GetAxis("Horizontal");
    float v = Input.GetAxis("Vertical");

    if (new Vector2(h, v).magnitude > 0.5f)
    {
      float angle = Mathf.Atan2(-h, v) * Mathf.Rad2Deg;

      Quaternion lookRotation = Quaternion.AngleAxis(angle, Vector3.forward);

      head.transform.rotation = Quaternion.Slerp(head.transform.rotation, lookRotation, Time.deltaTime * turnSpeed);
    }
  }

  void UpdateBody()
  {
    lineRenderer.positionCount = linePoints.Count;
    lineRenderer.SetPositions(linePoints.ToArray());
  }

  float CalculateLength()
  {
    float result = 0;
    int count = 0;

    for (int i = linePoints.Count - 1; i > 0; i--)
    {
      result = result + Vector3.Distance(linePoints[i - 1], linePoints[i]);

      if (result - 1 > count)
      {
        if (tailSegments.Count == count) //if we do not have enough colliders, add a new one.
        {
          GameObject temp = new GameObject(string.Format("tail_section_{0}", count));
          temp.tag = "Danger";

          CircleCollider2D tempCollider = temp.AddComponent<CircleCollider2D>();
          Rigidbody2D tempRigidbody = temp.AddComponent<Rigidbody2D>();
          tempRigidbody.bodyType = RigidbodyType2D.Kinematic;

          temp.transform.parent = transform;
          tempCollider.radius = 0.05f;
          tailSegments.Add(temp);
        }
        tailSegments[count].transform.position = linePoints[i];
        count++;
      }
    }
    return result;
  }

  void UpdatePosition()
  {
    currentPosition = transform.position;
    transform.Translate(head.transform.up * Time.deltaTime * speed);
    linePoints.Add(currentPosition);
    if (CalculateLength() > maxLength)
    {
      linePoints.RemoveAt(0);
    }
  }
}
