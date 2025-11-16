using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Sign : MonoBehaviour
{
    [SerializeField] private string[] messages;
    [SerializeField] private List<TextMeshProUGUI> texts;
    [SerializeField] private GameObject label;
    void Awake()
    {
        //label = GameObject.Find("SignLabel");
        for (int i = 0; i < label.transform.childCount; i++)
        {
            if (label.transform.GetChild(i).GetComponent<TextMeshProUGUI>())
            {
                texts.Add(label.transform.GetChild(i).GetComponent<TextMeshProUGUI>());
            }
        }
        label.SetActive(false);

    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            label.SetActive(true);
            texts[0].text = messages[0];
            texts[1].text = messages[1];
            texts[2].text = messages[2];
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player"))
        {
            label.SetActive(false);
        }
    }
}
