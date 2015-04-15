using UnityEngine;
using System.Collections;

public class BasicEnemyDropBullets : MonoBehaviour {

  public GameObject dropBullet;
  public float randomTime;
  
	// Use this for initialization
	void Start () {
    StartCoroutine(shootWait());
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

  IEnumerator shootWait()
  {
    while (true)
    {
      randomTime = Random.Range(1, 4);
      yield return new WaitForSeconds(randomTime);
      Instantiate(dropBullet, transform.position, transform.rotation);
    }
  }

}