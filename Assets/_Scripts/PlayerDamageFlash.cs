using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using System.Collections;

public class PlayerDamageFlash : MonoBehaviour
{

  SpriteRenderer playerSR;
  public float blinkTime;
  public float blinkIterations;
  public int health = 15;

  public AudioClip hurtSound;

  // Use this for initialization
  void Start()
  {
    playerSR = GetComponent<SpriteRenderer>();
  }

  // Update is called once per frame
  void Update()
  {
    if (health <= 0)
      Application.LoadLevel(Application.loadedLevel);
  }

  public IEnumerator makeFlash()
  {
    AudioSource.PlayClipAtPoint(hurtSound, transform.position, 0.09f);
    for (int i = 0; i <blinkIterations; i++)
    {
      playerSR.GetComponent<SpriteRenderer>().enabled = false;
      yield return new WaitForSeconds(blinkTime);
      playerSR.GetComponent<SpriteRenderer>().enabled = true;
      yield return new WaitForSeconds(blinkTime);
    }
    playerSR.GetComponent<SpriteRenderer>().enabled = true;
  }

}