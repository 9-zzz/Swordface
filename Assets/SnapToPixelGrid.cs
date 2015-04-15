using UnityEngine;
using System.Collections;

public class SnapToPixelGrid : MonoBehaviour
{

  // Use this for initialization
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {
    var currentPos = transform.position;
    transform.position = new Vector2(Mathf.Round(currentPos.x), currentPos.y);
  }

}