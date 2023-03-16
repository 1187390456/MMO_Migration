#if UNITY_ANDROID && !UNITY_EDITOR
#define ANDROID
#endif

#if UNITY_IPHONE && !UNITY_EDITOR
#define IPHONE
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Nirvana;

public class UIMouseClick : MonoBehaviour
{
    [SerializeField]
    private GameObject[] effects;

    private Canvas canvas;
    private int index = 0;

    private void Start() => canvas = this.GetComponentInParent<Canvas>();

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
#if IPHONE || ANDROID
			if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
#else
            if (EventSystem.current.IsPointerOverGameObject())
#endif
            {
                this.ShowClickEffect();
            }
        }
    }

    private void ShowClickEffect()
    {
        if (effects.Length > 0)
        {
            var obj = effects[index];

            // 索引限制
            index++;
            index %= effects.Length;

            if (obj != null)
            {
                var effect = Instantiate(obj);
                effect.transform.SetParent(transform, false);
                effect.transform.localScale = Vector3.one;

                // 鼠标点击坐标转屏幕坐标
                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform,
                            Input.mousePosition, canvas.worldCamera, out Vector2 _pos);
                effect.transform.localPosition = new Vector3(_pos.x, _pos.y, 0);

                effect.GetComponent<Animator>().WaitEvent("exit", (name, info) =>
                {
                    if (effect != null) Destroy(effect);
                    effect = null;
                });
            }
        }
    }
}