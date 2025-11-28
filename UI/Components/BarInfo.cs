using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class BarInfo : MonoBehaviour
{
    [SerializeField] private UnityEvent OnChange;
    [SerializeField] private Image barToChange;
    [SerializeField] private float barLimit = 100;
    [SerializeField] private Color baseColor;
    [SerializeField] private Color color;
    private Player player;

    void Awake()
    {
        player = GetComponentInParent<Player>();
        barToChange = GetComponent<Image>();
    }

    public void Update()
    {
        OnChange.Invoke();
    }
    public void Speed()
    {
        barToChange.fillAmount = player.rb.velocity.magnitude / barLimit;
    }
    public void ChangeColor()
    {
        barToChange.color = Color.Lerp(baseColor, color, player.rb.velocity.magnitude / barLimit);
    }
}
