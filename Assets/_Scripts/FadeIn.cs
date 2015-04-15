using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeIn : MonoBehaviour
{

  // Use this for initialization
  void Start()
  {
    Image some = this.GetComponent<Image>();
    some.CrossFadeAlpha(0, 2, true);
  }

  // Update is called once per frame
  void Update()
  {

  }

}
