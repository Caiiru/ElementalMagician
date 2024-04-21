using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


[CreateAssetMenu]
public class Element : ScriptableObject
{
    public string ElementName;
    public Sprite _sprite;
    [FormerlySerializedAs("PlayerDisplayColor")] [Tooltip("Display de cor representando a sprite em cima do jogador")]
    public Color ElementColor;

}
