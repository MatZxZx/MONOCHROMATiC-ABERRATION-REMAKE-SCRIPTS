using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rain : MonoBehaviour
{

    public GameObject prefab;
    public int quantity = 10;
    public float wideRange = 2;
    public float rainDelay = 0.1f;
    public Vector3 customDirection;
    public float rowDistance = 1;
    public Transform positionToRain;
    private List<Transform> rainObjects = new List<Transform>();
    private Vector3 rainPos;

    void Start()
    {
        rainPos += positionToRain.position;
        for (int i = 0; i < quantity; i++)
        {
            if (customDirection == Vector3.zero)
            {
                rainPos += (positionToRain.forward * rowDistance) + ((int)Random.Range(-wideRange, wideRange) * positionToRain.right); //+ ((int)Random.Range(-wideRange, wideRange) * positionToRain.right);
            }
            else
            {
                rainPos += (customDirection * rowDistance) + ((int)Random.Range(-wideRange, wideRange) * positionToRain.right);
            }
            GameObject rainObj = Instantiate(prefab, rainPos, Quaternion.identity); //poner rotation en random
            rainObj.SetActive(false);
            rainObjects.Add(rainObj.transform);
        }
    }

    private void OnTriggerEnter(Collider other){

        if (other.CompareTag("Player"))
        {
            RainStart();
        }

    }
    public void RainStart()
    {
        StartCoroutine(Raining());
    }

    private IEnumerator Raining()
    {
        foreach (Transform item in rainObjects)
        {
            item.gameObject.SetActive(true);
            yield return new WaitForSeconds(rainDelay);
        }
        yield return null;
    }

    [ContextMenu("Draw Ray")]
    public void OnDrawGizmos() {
        if (customDirection != Vector3.zero)
        {
            Gizmos.DrawRay(transform.position, customDirection * 10);
        }
        else
        {
            Gizmos.DrawRay(transform.position, transform.forward * 10);
        }
    }

}
