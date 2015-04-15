using UnityEngine;
using System.Collections;

public class DroneDropBullet : MonoBehaviour
{

  public int i;
  // Use this for initialization
  void Start()
  {
    Destroy(gameObject, 5);
  }

  // Update is called once per frame
  void Update()
  {

  }

  void OnTriggerEnter2D(Collider2D other)
  {
    if (other.tag == "Player")
    {
      StartCoroutine(other.GetComponent<PlayerDamageFlash>().makeFlash());
      other.GetComponent<PlayerDamageFlash>().health--;
    }

    if (other.tag != "drone")
    {
      gameObject.GetComponent<SpriteRenderer>().enabled = false;
      gameObject.GetComponent<Collider2D>().enabled = false;
    }
  }

}