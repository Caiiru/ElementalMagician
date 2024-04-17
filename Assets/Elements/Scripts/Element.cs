using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class Element : ScriptableObject
{
    [SerializeField] private string ElementName;
    public Sprite _sprite;
    [Tooltip("Display de cor representando a sprite em cima do jogador")]
    public Color PlayerDisplayColor;

}
