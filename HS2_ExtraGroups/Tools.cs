using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using HS2;
using Illusion.Game;

using UniRx;
using UniRx.Triggers;

namespace HS2_ExtraGroups
{
    public static class Tools
    {
        public static void ExpandUI(GroupListUI __instance, GroupCharaSelectUI groupCharaSelectUI)
        {
            var core = GameObject.Find("HomeScene/Canvas/Panel/CharaEdit/Group/SelectGroups/Panel/Filter And Sort");
            
            var ScrollView = new GameObject("ScrollView", typeof(RectTransform));
            ScrollView.transform.SetParent(core.transform, false);
            
            var ViewPort = new GameObject("ViewPort", typeof(RectTransform));
            ViewPort.transform.SetParent(ScrollView.transform, false);
            ViewPort.AddComponent<RectMask2D>();
            ViewPort.AddComponent<Image>().color = new Color(0, 0, 0, 0);
            
            var vpRectTransform = ViewPort.GetComponent<RectTransform>();
            
            var filter = GameObject.Find("HomeScene/Canvas/Panel/CharaEdit/Group/SelectGroups/Panel/Filter And Sort/Filter");
            filter.transform.SetParent(ViewPort.transform, false);
            
            var svScrollRect = ScrollView.AddComponent<ScrollRect>();
            svScrollRect.content = filter.GetComponent<RectTransform>();
            svScrollRect.viewport = vpRectTransform;
            svScrollRect.horizontal = true;
            svScrollRect.vertical = false;
            svScrollRect.scrollSensitivity = 40;
            
            var fitter = filter.AddComponent<ContentSizeFitter>();
            fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
            
            vpRectTransform.offsetMin = new Vector2(-80, 5);
            vpRectTransform.offsetMax = new Vector2(190, 50);

            var sRectTransform = ScrollView.GetComponent<RectTransform>();
            sRectTransform.offsetMin = new Vector2(-135, -71);
            sRectTransform.offsetMax = new Vector2(5.5f, 29);

            var hm = Singleton<Manager.HomeSceneManager>.Instance;
            var save = Singleton<Manager.Game>.Instance.saveData;
           
            var orig = GameObject.Find("HomeScene/Canvas/Panel/CharaEdit/Group/SelectGroups/Panel/Filter And Sort/ScrollView/ViewPort/Filter/tglFilter5");
            
            for (var i = 5; i < HS2_ExtraGroups.groupCount; i++)
            {
                var copy = Object.Instantiate(orig, orig.transform.parent);
                copy.name = "tglFilter" + (i + 1);
                
                var image = copy.GetComponentInChildren<Image>();
                var toggle = copy.GetComponentInChildren<Toggle>();

                var index = i;
                
                toggle.onValueChanged = new Toggle.ToggleEvent();
                toggle.onValueChanged.AddListener(delegate(bool on)
                {
                    image.enabled = !on;
                    
                    if (save.selectGroup == index)
                        return;

                    if (!on)
                        return;
                    
                    Utils.Sound.Play(SystemSE.ok_s);
                    
                    save.selectGroup = index;
                    hm.SetSelectGroupText(save.selectGroup);
                    
                    groupCharaSelectUI.ListCtrl.SelectInfoClear();
                    groupCharaSelectUI.ListCtrl.RefreshShown();
                    __instance.ListCtrl.SelectInfoClear();
                    __instance.Create();
                });

                toggle.OnPointerEnterAsObservable().Subscribe(delegate
                {
                    Utils.Sound.Play(SystemSE.sel);
                });
            }

            filter.GetComponent<HorizontalLayoutGroup>().enabled = true;
        }
        
        public static List<int> NewGroupsList()
        {
            var list = new List<int>();

            for (var i = 0; i < HS2_ExtraGroups.groupCount; i++)
                list.Add(i);

            return list;
        }
    }
}