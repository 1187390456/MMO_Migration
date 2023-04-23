using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using CustomTools;

public class ListView : UIBase
{
    public UnityAction<ListViewItem> OnItemSelected; // 选择事件回调

    private ListViewItem selectedItem = null;

    public ListViewItem SelectedItem
    {
        get => selectedItem;
        set
        {
            if (selectedItem != null && selectedItem != value) selectedItem.Selected = false; // 值改变 取消上次的选择
            selectedItem = value;
            OnItemSelected?.Invoke(value);
        }
    }

    private List<GameObject> listViewItems = new List<GameObject>(); // 当前列表item集合

    public void Add(ListViewItem item) // 添加item
    {
        item.Owner = this;
        listViewItems.Add(item.gameObject);
    }

    public void RemoveAll() => CommonTools.DestoryAllChild(listViewItems);

    public class ListViewItem : MonoBehaviour, IPointerClickHandler
    {
        private bool selected;

        public bool Selected
        {
            get => selected;
            set
            {
                selected = value;
                OnSelectedHandler(selected);
            }
        }

        public virtual void OnSelectedHandler(bool seleted)
        {
        }

        [HideInInspector] public ListView Owner;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!selected) Selected = true; // 修改当前选中
            if (Owner != null) Owner.SelectedItem = this; // 修改父级通知上次选中改变 && Owner.SelectedItem != null
        }
    }
}