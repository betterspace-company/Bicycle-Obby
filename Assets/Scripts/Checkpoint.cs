using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Checkpoint : MonoBehaviour
{
    public MeshRenderer modelMr;
    public Transform model;
    public Tween tween;
    public bool flagIsSet;
    public float flagSpeed = 1;
    public Ease flagEase;
    public GameObject flagRoot;
    public GameObject flagStem;
    public GameObject flagCloth;
}
