using Network;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreComponent : MonoBehaviour
{
    private void Awake()
    {
        gameObject.AddComponent<NetClient>();
        gameObject.AddComponent<MsgCenter>();
    }
}