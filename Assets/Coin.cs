using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
  public GameObject deathPrefab;


  // Use this for initialization
  void Start()
  {
  }
  // Update is called once per frame
  void Update()
  {
  }

  void Collect()
  {
    EventManager.TriggerEvent("AddScore");
    Instantiate(deathPrefab, transform.position, Quaternion.identity);
    Destroy(gameObject);
  }
}
