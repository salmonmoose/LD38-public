using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Level : MonoBehaviour
{
  public GameObject coinPrefab;

  public GameObject[] crushers;
  public Vector3[] starts;
  public Vector3 coinPosition;
  public TextMeshProUGUI scoreText;
  public TextMeshProUGUI valueText;

  public int northIndex;
  public int eastIndex;
  public int southIndex;
  public int westIndex;
  public int exitIndex;

  public int levelTime = 60;
  public float coinValue = 1f;
  public float initialCoinValue = 1f;
  public int score = 0;

  private float startTime;
  private float coinTime;

  private GameObject currentCoin;

  void OnEnable()
  {
    EventManager.StartListening("ResetLevel", ResetLevel);
    EventManager.StartListening("AddScore", AddScore);
  }

  // Use this for initialization
  void Start()
  {
    starts = new Vector3[crushers.Length];

    SetExit();

    for (int i = 0; i < crushers.Length; i++)
    {
      starts[i] = crushers[i].transform.position;
    }

    startTime = Time.time;
  }

  // Update is called once per frame
  void Update()
  {
    UpdateWall();
    UpdateCoin();

    scoreText.text = string.Format("score: {0}", score);
    valueText.text = string.Format("${0}", Mathf.FloorToInt(coinValue));
  }

  void UpdateWall()
  {
    float timeDelta = (Time.time - startTime) / levelTime;

    for(int i = 0; i < crushers.Length; i++)
    {
      crushers[i].transform.position = Vector3.Lerp(starts[i], Vector3.zero, timeDelta);
    }
  }

  void UpdateCoin()
  {
    float timeDelta = (Time.time - startTime) / levelTime;

    if (!currentCoin)
    {
      Vector2 spawnPosition = new Vector2(
        Random.Range(crushers[westIndex].transform.position.x + 1, crushers[eastIndex].transform.position.x - 1),
        Random.Range(crushers[southIndex].transform.position.y + 1, crushers[northIndex].transform.position.y - 1)
        );

      currentCoin = Instantiate(coinPrefab, new Vector3(
        spawnPosition.x,
        spawnPosition.y,
        0
        ), Quaternion.identity);

      coinPosition = currentCoin.transform.position;
    }

    currentCoin.transform.position = Vector3.Lerp(coinPosition, Vector3.zero, timeDelta);
  }

  void SetExit()
  {
    Vector2 spawnPosition = new Vector2(
      Random.Range(crushers[westIndex].transform.position.x + 1, crushers[eastIndex].transform.position.x - 1),
      Random.Range(crushers[southIndex].transform.position.y + 1, crushers[northIndex].transform.position.y - 1)
      );

    crushers[exitIndex].transform.position = new Vector3(spawnPosition.x, spawnPosition.y, 10);
  }

  void ResetLevel()
  {
    startTime = Time.time;
    UpdateWall();
    SetExit();
    
    if (levelTime > 5) levelTime = levelTime - 5;
    initialCoinValue = initialCoinValue * 2f;
    coinValue = initialCoinValue;

    starts[exitIndex] = crushers[exitIndex].transform.position;
  }

  void AddScore()
  {
    score = score + Mathf.FloorToInt(coinValue);
    coinValue = coinValue * 1.1f;
  }
}
