using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class TextChanger : MonoBehaviour
{
    [SerializeField] private UnityEvent OnChange;
    [SerializeField] private TextMeshProUGUI textToChange;
    private Player player;
    [SerializeField] private Color baseColor;
    [SerializeField] private Color color;

    void Awake()
    {
        player = GetComponentInParent<Player>();
        baseColor = textToChange.color;
    }

    public void Update()
    {
        OnChange.Invoke();
    }

    public void Time()
    {
       
    }
    public void Speed(bool inverted = false)
    {
        if (!inverted)
        {
            textToChange.text = player.rb.velocity.magnitude.ToString("f0") + " KM/h";
        }
        else
        {
            textToChange.text = "KM/h " + player.rb.velocity.magnitude.ToString("f0");
        }
    }

    public void Jig(bool inverted = false)
    {
        if (!inverted)
        {
            textToChange.text = "x " + player.playerScore.GetCounter().ToString();
        }
        else
        {
            textToChange.text = player.playerScore.GetCounter().ToString() +  " x"; 
        }
    }

    public void ChangeColor(bool inverted = false)
    {
        textToChange.color = Color.Lerp(baseColor, color, player.rb.velocity.magnitude / (player.maxSpeed * 4));
    }

}
