using Common;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LayerIndex
{
    first,
    last,
}

public class GoExtend : Singleton<GoExtend>
{
    public GameObject InstanAndSetLayer(GameObject prefabs, Transform parent, int index = 0, bool active = false, string name = "")
    {
        var go = GameObject.Instantiate(prefabs, parent);
        go.SetActive(active);
        if (name != "") go.name = name;
        if (index != 0) go.transform.SetSiblingIndex(index);
        return go;
    }

    public GameObject InstanAndSetLayer(GameObject prefabs, Transform parent, LayerIndex layer, bool active = false, string name = "")
    {
        var go = GameObject.Instantiate(prefabs, parent);
        go.SetActive(active);
        if (name != "") go.name = name;

        switch (layer)
        {
            case LayerIndex.first:
                go.transform.SetAsFirstSibling();
                break;

            case LayerIndex.last:
                go.transform.SetAsLastSibling();
                break;

            default:
                break;
        }
        return go;
    }
}