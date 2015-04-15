using UnityEngine;
using System.Collections;

public class HeadSword : MonoBehaviour
{

  public GameObject destroyParticles;
  public AudioClip droneDeath;

  // Use this for initialization
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {

  }

  void OnTriggerEnter2D(Collider2D other)
  {
    if(other.tag == "drone")
    {
      Instantiate(destroyParticles, other.transform.position, other.transform.rotation);
      AudioSource.PlayClipAtPoint(droneDeath, other.transform.position);
      Destroy(other.gameObject);
    }
  }

}