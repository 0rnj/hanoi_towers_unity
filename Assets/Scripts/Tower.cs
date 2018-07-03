using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public Stack<GameObject> DiscHolder;

    private void Awake()
    {
        DiscHolder = new Stack<GameObject>();
    }
}
