using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ChainController : MonoBehaviour
{
    public Mesh mesh;
    GameObject chain;
    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        //GetComponent<MeshFilter>().mesh = mesh;

        chain = GameObject.Find("Chain");
        //chain.GetComponentInChildren<MeshFilter>().mesh = mesh;

        //获取所有网格过滤器
        MeshFilter[] meshFilters = chain.GetComponentsInChildren<MeshFilter>();


        var filter = chain.AddComponent<MeshFilter>();
        filter.mesh = mesh;

        var renderer = chain.AddComponent<MeshRenderer>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
