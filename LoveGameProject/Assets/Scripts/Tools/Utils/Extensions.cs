using System;
using System.Text;
using System.IO;
using UnityEngine;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using UnityEditor;
using MM.Config;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using MM.Common;

namespace MM.Common {
    public static class Extensions {
        public static void Destroy(UnityEngine.Object gameObject) {
            GameObject.Destroy(gameObject);
        }

        public static void SetLeft(this RectTransform rt, float left) {
            rt.offsetMin = new Vector2(left, rt.offsetMin.y);
        }

        public static void SetRight(this RectTransform rt, float right) {
            rt.offsetMax = new Vector2(-right, rt.offsetMax.y);
        }

        public static void SetTop(this RectTransform rt, float top) {
            rt.offsetMax = new Vector2(rt.offsetMax.x, -top);
        }

        public static void SetBottom(this RectTransform rt, float bottom) {
            rt.offsetMin = new Vector2(rt.offsetMin.x, bottom);
        }
        public static T AddComponentIfNotExist<T>(this GameObject gameObject) where T : Component {
            if (gameObject == null) {
                return default(T);
            }
            var t = gameObject.GetComponent<T>();
            if (t == null) {
                t = gameObject.AddComponent<T>();
            }
            return t;
        }
        public static T AddComponentIfNotExist<T>(this Transform transform) where T : Component {
            if (transform == null) {
                return null;
            }
            return AddComponentIfNotExist<T>(transform.gameObject);
        }
        /// <summary>
        ///获取某对象从根节点到自身的路径
        /// </summary>
        public static string GetPath(this Transform _tra) {
            if (_tra == null) {
                return "";
            }
            StringBuilder tempPath = new StringBuilder(_tra.name);
            Transform tempTra = _tra;
            while (tempTra.parent != null) {
                tempTra = tempTra.parent;
                tempPath.Insert(0, tempTra.name + "/");
            }
            return tempPath.ToString();
        }

        /// <summary>
        ///获取某对象从根节点到自身的路径
        /// </summary>
        public static string GetPath(this Transform _tra,Transform root,out List<string> paths)
        {
            paths = null;
            if (_tra == null)
            {
                return "";
            }
            paths = new List<string>();
            StringBuilder tempPath = new StringBuilder(_tra.name);
            Transform tempTra = _tra;
            while (tempTra.parent != null && tempTra.transform != root)
            {
                tempTra = tempTra.parent;
                tempPath.Insert(0, tempTra.name + "/");
                paths.Add(tempTra.name);
            }
            return tempPath.ToString();
        }

        public static string GetPath(this Component component) {
            return component.transform.GetPath() + "/" + component.GetType().ToString();
        }

        public static bool Contains(this RectTransform container, RectTransform rt)
        {
            // 获取容器的四个顶点
            Vector3[] containerCorners = new Vector3[4];
            container.GetWorldCorners(containerCorners);
            // 获取容器宽高
            float width = Mathf.Abs(containerCorners[2].x - containerCorners[0].x);
            float height = Mathf.Abs(containerCorners[2].y - containerCorners[0].y);
            Rect rect = new Rect(containerCorners[0].x, containerCorners[0].y, width, height);

            // 获取要判断UI的四个顶点
            Vector3[] rtCorners = new Vector3[4];
            rt.GetWorldCorners(rtCorners);
            // 依次判断四个顶点是否都在矩形中
            foreach (var corner in rtCorners)
            {
                if (!rect.Contains(corner))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
