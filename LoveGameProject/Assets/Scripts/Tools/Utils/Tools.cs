using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

    public static class Tools
    {
        public static void AlphaSprites(GameObject obj, float from, float to, float time, LeanTweenType ease)
        {
            if (obj == null)
            {
                return;
            }
            if (!obj.activeInHierarchy)
            {
                obj.SetActive(true);
            }
            SpriteRenderer[] srs = obj.GetComponentsInChildren<SpriteRenderer>();
            foreach (var v in srs)
            {
                LeanTween.value(v.gameObject, from, to, time).setOnUpdate((float val) =>
                {
                    Color c = v.color;
                    c.a = val;
                    v.color = c;
                }).setEase(ease).updateNow();
            }
        }
        public static LTDescr AlphaSprite(GameObject obj, float from, float to, float time, LeanTweenType ease)
        {
            if (obj == null)
            {
                return null;
            }
            if (!obj.activeInHierarchy)
            {
                obj.SetActive(true);
            }
            SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
            if (sr == null)
            {
                return null;
            }
            return LeanTween.value(obj, from, to, time).setOnUpdate((float val) =>
            {
                Color c = sr.color;
                c.a = val;
                sr.color = c;
            }).setEase(ease);
        }

        //public static void AlphaTransform(Transform trans, float from, float to, float time, LeanTweenType ease)
        //{
        //    var sps = trans?.GetComponentsInChildren<SpriteRenderer>();
        //    if (sps != null)
        //    {
        //        LeanTween.value(trans.gameObject, from, to, time).setOnUpdate((float value) =>
        //        {
        //            for (int i = 0; i < sps.Length; ++i)
        //            {
        //                var sp = sps[i];
        //                if (sp == null)
        //                {
        //                    continue;
        //                }
        //                Color c = sp.color;
        //                c.a = value;
        //                sp.color = c;
        //            }
        //        }).setEase(ease).updateNow();
        //    }
        //    var texts = trans?.GetComponentsInChildren<TextMeshPro>();
        //    if (texts != null)
        //    {
        //        LeanTween.value(trans.gameObject, from, to, time).setOnUpdate((float value) =>
        //        {
        //            for (int i = 0; i < texts.Length; ++i)
        //            {
        //                var sp = texts[i];
        //                if (sp == null)
        //                {
        //                    continue;
        //                }
        //                Color c = sp.color;
        //                c.a = value;
        //                sp.color = c;
        //            }
        //        }).setEase(ease).updateNow();
        //    }

        //    var anims = trans?.GetComponentsInChildren<SkeletonAnimation>();
        //    if (texts != null)
        //    {
        //        LeanTween.value(trans.gameObject, from, to, time).setOnUpdate((float value) =>
        //        {
        //            for (int i = 0; i < anims.Length; ++i)
        //            {
        //                var anim = anims[i];
        //                if (anim == null)
        //                {
        //                    continue;
        //                }
        //                anim.Skeleton.A = value;
        //            }
        //        }).setEase(ease).updateNow();
        //    }
        //}

        //public static void AlphaRectTransform(Transform trans, float from, float to, float time, LeanTweenType ease)
        //{
        //    var sps = trans?.GetComponentsInChildren<Graphic>();
        //    if (sps != null)
        //    {
        //        LeanTween.value(trans.gameObject, from, to, time).setOnUpdate((float value) =>
        //        {
        //            for (int i = 0; i < sps.Length; ++i)
        //            {
        //                var sp = sps[i];
        //                if (sp == null)
        //                {
        //                    continue;
        //                }
        //                Color c = sp.color;
        //                c.a = value;
        //                sp.color = c;
        //            }
        //        }).setEase(ease).updateNow();
        //    }
        //    var texts = trans?.GetComponentsInChildren<TextMeshProUGUI>();
        //    if (texts != null)
        //    {
        //        LeanTween.value(trans.gameObject, from, to, time).setOnUpdate((float value) =>
        //        {
        //            for (int i = 0; i < texts.Length; ++i)
        //            {
        //                var sp = texts[i];
        //                if (sp == null)
        //                {
        //                    continue;
        //                }
        //                Color c = sp.color;
        //                c.a = value;
        //                sp.color = c;
        //            }
        //        }).setEase(ease).updateNow();
        //    }

        //    var anims = trans?.GetComponentsInChildren<SkeletonGraphic>();
        //    if (texts != null)
        //    {
        //        LeanTween.value(trans.gameObject, from, to, time).setOnUpdate((float value) =>
        //        {
        //            for (int i = 0; i < anims.Length; ++i)
        //            {
        //                var anim = anims[i];
        //                if (anim == null)
        //                {
        //                    continue;
        //                }
        //                anim.Skeleton.A = value;
        //            }
        //        }).setEase(ease).updateNow();
        //    }
        //}

        public static LTDescr AlphaImage(GameObject obj, float from, float to, float time)
        {
            var img = obj?.GetComponent<Image>();
            if (img == null)
            {
                return null;
            }
            return LeanTween.value(obj, from, to, time).setOnUpdate((float val) =>
            {
                if (img == null)
                {
                    return;
                }
                Color c = img.color;
                c.a = val;
                img.color = c;
            });
        }

    public static LTDescr AlphaGameObject(GameObject obj, float from, float to, float time)
    {
        var img = obj?.GetComponent<MeshRenderer>().material;
        if (img == null)
        {
            return null;
        }
        return LeanTween.value(obj, from, to, time).setOnUpdate((float val) =>
        {
            if (img == null)
            {
                return;
            }
            Color c = img.GetColor("_Color");
            c.a = val;
            img.SetColor("_Color", c);
        });
    }


    /// <summary>
    /// 格式化时间
    /// </summary>
    /// <param name="seconds">秒</param>
    /// <returns></returns>
    public static string FormatTime(float seconds)
    {
        TimeSpan ts = new TimeSpan(0, 0, Convert.ToInt32(seconds));
        string str = "";
        if (ts.Hours > 0)
        {
            str = ts.Hours.ToString("00") + ":" + ts.Minutes.ToString("00") + ":" + ts.Seconds.ToString("00");
        }
        if (ts.Hours == 0 && ts.Minutes > 0)
        {
            str = ts.Minutes.ToString("00") + ":" + ts.Seconds.ToString("00");
        }
        if (ts.Hours == 0 && ts.Minutes == 0)
        {
            str = "00:" + ts.Seconds.ToString("00");
        }
        return str;
    }



    /// <summary>
    /// 随机数生成器
    /// </summary>
    private static System.Random Srand = null;

    /// <summary>
    /// 初始化随机种子
    /// </summary>
    /// <param name="seed"></param>
    public static void InitSeed(int seed)
    {
        Srand = new System.Random(seed);
    }

    /// <summary>
    /// 初始化随机数，用默认的种子
    /// </summary>
    public static void InitSeed()
    {
        Srand = new System.Random();
    }
    /// <summary>
    /// 获取一个[min-max)之间的随机整数
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static int GetRandom(int min, int max)
    {
        if (Srand == null)
        {
            InitSeed();
        }
        return Srand.Next(min, max);
    }

    /// <summary>
    /// 获取一个min-max之间的随机浮点数
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static float GetRandom(float min, float max)
    {
        if (Srand == null)
        {
            InitSeed();
        }
        return (float)((Srand.NextDouble() * (max - min)) + min);
    }
    //public static LTDescr AlphaSpine(GameObject obj, float from, float to, float time)
    //{
    //    var anim = obj?.GetComponent<SkeletonAnimation>();
    //    if (anim == null)
    //    {
    //        return null;
    //    }
    //    return LeanTween.value(obj, from, to, time).setOnUpdate((float val) =>
    //    {
    //        if (anim != null && anim.Skeleton != null)
    //        {
    //            anim.Skeleton.A = val;
    //        }
    //    });
    //}
    public static LTDescr AlphaText(GameObject obj, float from, float to, float time)
        {
            object txt;
            bool isTextMesh = false;
            if (obj == null)
            {
                return null;
            }
            if (obj.GetComponent<Text>() != null)
            {
                txt = obj.GetComponent<UnityEngine.UI.Text>();
            }
            else if (obj.GetComponent<TextMeshProUGUI>() != null)
            {
                txt = obj.GetComponent<TextMeshProUGUI>();
                isTextMesh = true;
            }
            else
            {
                return null;
            }
            return LeanTween.value(obj, from, to, time).setOnUpdate((float val) =>
            {
                if (isTextMesh)
                {
                    Color c = (txt as TextMeshProUGUI).color;
                    c.a = val;
                    (txt as TextMeshProUGUI).color = c;
                }
                else
                {
                    Color c = (txt as UnityEngine.UI.Text).color;
                    c.a = val;
                    (txt as UnityEngine.UI.Text).color = c;
                }
            });
        }
        public static LTDescr AlphaTextMeshPro(GameObject obj, float from, float to, float time)
        {
            if (obj == null)
            {
                return null;
            }
            var txt = obj.GetComponent<TextMeshPro>();
            if (txt == null)
            {
                return null;
            }
            return LeanTween.value(obj, from, to, time).setOnUpdate((float val) =>
            {
                Color c = txt.color;
                c.a = val;
                txt.color = c;
            });
        }
        public static LTDescr AlphaTextMeshProUGUI(GameObject obj, float from, float to, float time)
        {
            if (obj == null)
            {
                return null;
            }
            var txt = obj.GetComponent<TextMeshProUGUI>();
            if (txt == null)
            {
                return null;
            }
            return LeanTween.value(obj, from, to, time).setOnUpdate((float val) =>
            {
                Color c = txt.color;
                c.a = val;
                txt.color = c;
            });
        }
        public static LTDescr AlphaSpecialTextMesh(GameObject obj, float from, float to, float time)
        {
            var txt = obj?.GetComponent<MeshRenderer>();
            if (txt == null)
            {
                return null;
            }
            Color tempColor;
            return LeanTween.value(obj, from, to, time).setOnUpdate((float val) =>
            {
                tempColor = txt.material.GetColor("_Color");
                tempColor.a = val;
                txt.material.SetColor("_Color", tempColor);
            });
        }
        //public static LTDescr AlphaSpineUI(GameObject obj, float from, float to, float time)
        //{
        //    SkeletonGraphic sg = obj?.GetComponent<SkeletonGraphic>();
        //    if (sg == null)
        //    {
        //        return null;
        //    }
        //    return LeanTween.value(obj, from, to, time).setOnUpdate((float val) =>
        //    {
        //        Color c = sg.color;
        //        c.a = val;
        //        sg.color = c;
        //    });
        //}
        public static void AlphaParticle(GameObject obj, float to, float time)
        {
            if (obj == null)
            {
                return;
            }
            List<Material> mats = new List<Material>();
            List<float> alphas = new List<float>();
            void loopChildren(Transform trans)
            {
                foreach (Transform child in trans)
                {
                    if (child.GetComponent<Renderer>() != null && child.GetComponent<Renderer>().material != null)
                    {
                        var mat = child.GetComponent<Renderer>().material;
                        if (mat.HasProperty("_TintColor"))
                        {
                            var color = mat.GetColor("_TintColor");
                            if (color != null)
                            {
                                mats.Add(mat);
                                alphas.Add(color.a);
                            }
                        }
                    }
                    loopChildren(child);
                }
            }
            loopChildren(obj.transform);

            for (int i = 0; i < mats.Count && i < alphas.Count; ++i)
            {
                var index = i;
                LeanTween.value(obj, alphas[i], to, time).setOnUpdate((float val) =>
                {
                    if (mats[index].HasProperty("_TintColor"))
                    {
                        var color = mats[index].GetColor("_TintColor");
                        if (color != null)
                        {
                            color.a = val;
                            mats[index].SetColor("_TintColor", color);
                        }
                    }
                });
            }
        }
        public static LTDescr AlphaParticle(GameObject obj, float from, float to, float time)
        {
            if (obj == null)
            {
                return null;
            }
            List<Material> mats = new List<Material>();
            if (mats == null)
            {
                return null;
            }
            void loopChildren(Transform trans)
            {
                foreach (Transform child in trans)
                {
                    if (child.GetComponent<Renderer>() != null && child.GetComponent<Renderer>().material != null)
                    {
                        var mat = child.GetComponent<Renderer>().material;
                        if (mat.HasProperty("_TintColor"))
                        {
                            mats.Add(mat);
                            var color = mat.GetColor("_TintColor");
                            if (color != null)
                            {
                                color.a = from;
                                mat.SetColor("_TintColor", color);
                            }
                        }
                    }
                    loopChildren(child);
                }
            }
            loopChildren(obj.transform);
            return LeanTween.value(obj, from, to, time).setOnUpdate((float val) =>
            {
                for (int i = 0; i < mats.Count; i++)
                {
                    if (mats[i].HasProperty("_TintColor"))
                    {
                        var color = mats[i].GetColor("_TintColor");
                        if (color != null)
                        {
                            color.a = val;
                            mats[i].SetColor("_TintColor", color);
                        }
                    }
                }
            });
        }
    }

