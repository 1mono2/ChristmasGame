using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Rand = UnityEngine.Random;
using UniRx;
using MoNo.Utility;



public class ItemManager : MonoBehaviour
{

    // property
    [HideInInspector] public int finalItemsNum { get { return _currentItemNum.Value; }}
    [HideInInspector] public IntReactiveProperty currentItemNum => _currentItemNum;


    // field
    [SerializeField] Item[] _itemPrefabs;
    ExtendObjectPool<Item> _itemPool;
    IntReactiveProperty _currentItemNum = new IntReactiveProperty(0);


    // const value
    const int LIMIT_ITEM_NUM = 1000;


    private void Start()
    {
        SetItemPool();  
    }

    void SetItemPool()
    {
        _itemPool = new ExtendObjectPool<Item>(
           createFunc: () => {
               Item randomItemPref = _itemPrefabs[Rand.Range(0, _itemPrefabs.Length)];
               return Instantiate(randomItemPref);
           },
           actionOnGet: target => {
               target.gameObject.SetActive(true);
                // set name of pin
                if (target.name.EndsWith("(Clone)"))
               {
                   target.name = _itemPool.CountAll.ToString();
               }
           },
           actionOnRelease: target => {
               target.gameObject.SetActive(false);
           },
           actionOnDestroy: target => Destroy(target),
           collectionCheck: true,
           defaultCapacity: 10,
           maxSize: LIMIT_ITEM_NUM);
    }


    public void GenerateItem(Vector3 where , Transform parent)
    {
        Vector3 pos = where + new Vector3(Rand.insideUnitCircle.x, 0, Rand.insideUnitCircle.y);
        Item item = _itemPool.Get();

        item.transform.position = pos;
        item.transform.SetParent(parent);

        _currentItemNum.Value = _itemPool.CountActive;

    }

    public int GetItemNum()
    {
        return _itemPool.CountActive;
    }

    public bool DeleteItem()
    {
        bool success = _itemPool.Release();
        _currentItemNum.Value = _itemPool.CountActive;
        return success;

    }

    public void DeleteItem(Item target)
    {
        _itemPool.Release(target);
        _currentItemNum.Value = _itemPool.CountActive;
    }

}

public class ExtendObjectPool<T> : ObjectPool<T> where T : class
{
    public List<T> m_ItemList = new List<T>();

    public ExtendObjectPool(System.Func<T> createFunc,
                            System.Action<T> actionOnGet = null,
                            System.Action<T> actionOnRelease = null,
                            System.Action<T> actionOnDestroy = null,
                            bool collectionCheck = true,
                            int defaultCapacity = 10,
                            int maxSize = 10000) : base(createFunc, actionOnGet, actionOnRelease, actionOnDestroy, collectionCheck, defaultCapacity, maxSize) { }

    public new T Get()
    {
        T t = base.Get();
        m_ItemList.Add(t);
        return t;
    }

    /// <summary>
    /// If don't give argument, one is released from active objects.
    /// </summary>
    public bool Release()
    {
        if (!m_ItemList.Any()) return false;
        T target = m_ItemList.Pop();
        m_ItemList.Remove(target);
        base.Release(target);
        return true;
    }

    public new bool Release(T element)
    {
        if (!m_ItemList.Any()) return false;
        m_ItemList.Remove(element);
        base.Release(element);
        return true;
    }

}
