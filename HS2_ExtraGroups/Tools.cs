using System;
using System.Collections.Generic;

using HarmonyLib;

using UnityEngine;
using UnityEngine.UI;

using Illusion.Component.UI;

using Object = UnityEngine.Object;

namespace HS2_ExtraGroups
{
    public static class Tools
    {
        private static readonly string[] targets =
        {
            "CharaEdit/Group/SelectGroups/Panel/Filter And Sort",
            "CharaEdit/Coordinate/SelectGroup/Panel/Filter And Sort"
        };

        public static void ExpandUI(object __instance, ref Array ___groupUIInfos, SpriteChangeCtrl ___sccBasePanel, int k)
        {
            var panel = GameObject.Find("HomeScene/Canvas/Panel");

            var core = panel.transform.Find(targets[k]);
        
            var ScrollView = new GameObject("ScrollView", typeof(RectTransform));
            ScrollView.transform.SetParent(core.transform, false);
            
            var ViewPort = new GameObject("ViewPort", typeof(RectTransform));
            ViewPort.transform.SetParent(ScrollView.transform, false);
            ViewPort.AddComponent<RectMask2D>();
            ViewPort.AddComponent<Image>().color = new Color(0, 0, 0, 0);
            
            var vpRectTransform = ViewPort.GetComponent<RectTransform>();
            
            var filter = core.transform.Find("Filter");
            filter.SetParent(ViewPort.transform, false);
            
            var svScrollRect = ScrollView.AddComponent<ScrollRect>();
            svScrollRect.content = filter.GetComponent<RectTransform>();
            svScrollRect.viewport = vpRectTransform;
            svScrollRect.horizontal = true;
            svScrollRect.vertical = false;
            svScrollRect.scrollSensitivity = 40;
            
            var fitter = filter.gameObject.AddComponent<ContentSizeFitter>();
            fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
            
            vpRectTransform.offsetMin = new Vector2(-80, 5);
            vpRectTransform.offsetMax = new Vector2(190, 50);

            var sRectTransform = ScrollView.GetComponent<RectTransform>();
            sRectTransform.offsetMin = new Vector2(-135, -71);
            sRectTransform.offsetMax = new Vector2(5.5f, 29);

            var trav = Traverse.Create(__instance);

            var type = ___groupUIInfos.GetType().GetElementType(); // why did you make this private?...
            if (type != null)
            {
                var newInfos = Array.CreateInstance(type, HS2_ExtraGroups.groupCount);
                Array.Copy(___groupUIInfos, newInfos, Math.Min(___groupUIInfos.Length, newInfos.Length));
                
                ___groupUIInfos = newInfos;
            }

            var ctrl1 = panel.transform.Find("Home/imgSelectGroup").GetComponent<SpriteChangeCtrl>();
            var oldSprites1 = ctrl1.sprites;
            ctrl1.sprites = new Sprite[HS2_ExtraGroups.groupCount];
            for (var i = 0; i < oldSprites1.Length; i++)
                ctrl1.sprites[i] = oldSprites1[i];
            
            var ctrl2 = panel.transform.Find("Home/imgSelectGroup/imgState").GetComponent<SpriteChangeCtrl>();
            var oldSprites2 = ctrl2.sprites;
            ctrl2.sprites = new Sprite[HS2_ExtraGroups.groupCount];
            for (var i = 0; i < oldSprites2.Length; i++)
                ctrl2.sprites[i] = oldSprites2[i];
            
            var oldSprites = ___sccBasePanel.sprites;
            ___sccBasePanel.sprites = new Sprite[HS2_ExtraGroups.groupCount];
            for (var i = 0; i < oldSprites.Length; i++)
                ___sccBasePanel.sprites[i] = oldSprites[i];

            var selectGroup = panel.transform.Find(k == 0 ? "CharaEdit/Group/SelectGroups" : "CharaEdit/Coordinate/SelectGroup");
            var gFilter5 = selectGroup.transform.Find("Panel/Filter And Sort/ScrollView/ViewPort/Filter/tglFilter5");
            
            if (k == 0)
            {
                var oldFilters = trav.Field("groupCharaSelectUI").Field("tglFilters").GetValue<Toggle[]>();
                var newFilters = new Toggle[HS2_ExtraGroups.groupCount + 1];
            
                for (var i = 0; i < oldFilters.Length; i++)
                    newFilters[i] = oldFilters[i];
                
                var selectChara = panel.transform.Find("CharaEdit/Group/SelectChara");
                var sFilter5 = selectChara.transform.Find("Panel/Filter And Sort/Filter/tglFilter5");
                
                for (var i = 5; i < HS2_ExtraGroups.groupCount; i++)
                {
                    var cCopy = Object.Instantiate(sFilter5, sFilter5.parent);
                    cCopy.name = "tglFilter" + (i + 1);
                
                    newFilters[i + 1] = cCopy.GetComponentInChildren<Toggle>();
                }
                
                trav.Field("groupCharaSelectUI").Field("tglFilters").SetValue(newFilters);
            }
            
            for (var i = 5; i < HS2_ExtraGroups.groupCount; i++)
            {
                ___sccBasePanel.sprites[i] = Object.Instantiate(___sccBasePanel.sprites[4]);
                ctrl1.sprites[i] = Object.Instantiate(ctrl1.sprites[4]);
                ctrl2.sprites[i] = Object.Instantiate(ctrl2.sprites[4]);

                if (type == null)
                    continue;

                var gCopy = Object.Instantiate(gFilter5, gFilter5.parent);
                gCopy.name = "tglFilter" + (i + 1);
                
                var obj = Activator.CreateInstance(type, true);

                var tglFieldInfo = type.GetField("tgl");
                tglFieldInfo.SetValue(obj, gCopy.GetComponentInChildren<Toggle>());
                
                var imageFieldInfo = type.GetField("image");
                imageFieldInfo.SetValue(obj, gCopy.GetComponentInChildren<Image>());

                ___groupUIInfos.SetValue(obj, i);
            }
            
            filter.GetComponent<HorizontalLayoutGroup>().enabled = true;
        }
        
        public static List<int> NewGroupsList()
        {
            var list = new List<int>();

            for (var i = 0; i < HS2_ExtraGroups.groupCount + 1; i++)
                list.Add(i);

            return list;
        }
    }
}