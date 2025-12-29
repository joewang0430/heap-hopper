using UnityEngine;
using System.Collections.Generic;
using System;
using Unity.VisualScripting;
using JetBrains.Annotations;

// 泛型类 T：必须是 MonoBehaviour 的子类（因为我们要操作 GameObject）
public class ObjectPool<T> where T : MonoBehaviour
{
    // 1. 预制体模板
    private T prefab;
    // 2. 真正的池子（用栈 Stack 比 队列 Queue 性能稍微好一点点，因为是后进先出，利于 CPU 缓存）
    private Stack<T> pool = new Stack<T>();
    // 3. 父节点（可选，为了让 Hierarchy 面板整洁，把生成的对象都归到一个父物体下）
    private Transform parentTransform;

    // 构造函数：初始化池子
    public ObjectPool(T prefab, int initialSize, Transform parent = null)
    {
        this.prefab = prefab;
        this.parentTransform = parent;

        // 初始化池子
        for (int i = 0; i < initialSize; i++)
        {
            T obj = CreateNewObject();
            obj.gameObject.SetActive(false);
            pool.Push(obj);
        }
    }

    public T Get()
    {
        T obj;

        // 如果池子里有货，直接拿
        if(pool.Count > 0)
        {
            obj = pool.Pop();
        }
        else
        {
            // 如果池子空了，被迫生成新的（自动扩容）
            obj = CreateNewObject();
        }

        obj.gameObject.SetActive(true);
        return obj;
    }

    public void Return(T obj)
    {
        obj.gameObject.SetActive(false);
        pool.Push(obj);
    }

    // 内部方法：真正的 Instantiate
    private T CreateNewObject()
    {
        T newObj = GameObject.Instantiate(prefab, parentTransform);
        return newObj;
    }
}
