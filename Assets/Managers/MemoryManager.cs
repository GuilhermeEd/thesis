﻿using UnityEngine;

[RequireComponent(typeof(Collider))]
public class MemoryManager : Singleton<MemoryManager>
{
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) { return; }
        Destroy(other.gameObject);
    }

    void OnDrawGizmos()
    {
        Collider col = GetComponent<Collider>();
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(col.bounds.center, col.bounds.size);
    }

    public void Clear()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}