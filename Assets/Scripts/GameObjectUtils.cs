using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets
{
    public class GameObjectUtils
    {
        static public Bounds GetCombinedRenderBounds(Transform parent)
        {
            Renderer[] renderers = parent.GetComponentsInChildren<Renderer>();
            Bounds bounds = new Bounds(parent.position, Vector3.zero);

            foreach (Renderer renderer in renderers)
            {
                bounds.Encapsulate(renderer.bounds);
            }

            return bounds;
        }

        static public Bounds GetCombinedOriginBounds(Transform parent)
        {
            //获取当前模型原始高度
            MeshFilter[] meshFilters = parent.GetComponentsInChildren<MeshFilter>();
            Bounds bounds = new Bounds(parent.position, Vector3.zero);
            foreach (MeshFilter meshFilter in meshFilters)
            {
                bounds.Encapsulate(meshFilter.mesh.bounds);
            }
            return bounds;
        }

        static public float GetCurrentHeight(GameObject gameObject)
        {
            //获取当前模型原始高度
            Vector3 size = gameObject.GetComponent<Renderer>().bounds.size;
            return size.y;
            //float originY = gameObject.GetComponent<MeshFilter>().mesh.bounds.size.y;
            //return originY* gameObject.transform.localScale.y;
        }

        static public float GetOriginHeight(GameObject gameObject)
        {
            //获取当前模型原始高度
            return gameObject.GetComponent<MeshFilter>().mesh.bounds.size.y;
        }
    }
}
