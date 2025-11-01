// #if !COMPILER_UDONSHARP && UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.Events;
using VRC.Udon;
using VRC.SDK3.Data;
using TMPro;

namespace VRCCoffeeSet
{
    static public class Asset_Ade
    {
        const string NAME_PATH = "Assets_Ade";
        const string NAME_OBJ = "Objects_Ade";
        const string NAME_CUPGLASS = "CupGlass_Ade";
        const string NAME_UI_RECIPEBTN = "Button_Ade";
        const string NAME_UI_RECIPESEC = "Section_Ade";
        const string PATH_MATGROUP = "/" + NAME_PATH  + "/Materials/MatGroup_Ade.Asset";
        const string PATH_OBJ = "/" + NAME_PATH  + "/Prefabs/" + NAME_OBJ + ".prefab";
        const string PATH_CUPGLASS = "/" + NAME_PATH  + "/Prefabs/" + NAME_CUPGLASS + ".prefab";
        const string PATH_UI_RECIPEBTN = "/" + NAME_PATH  + "/Prefabs/" + NAME_UI_RECIPEBTN + ".prefab";
        const string PATH_UI_RECIPESEC = "/" + NAME_PATH  + "/Prefabs/" + NAME_UI_RECIPESEC + ".prefab";
        const string PATH_LANG = "/" + NAME_PATH + "/Languages";

        static public void Install(MainController_Editor editor)
        {
            // Find DLC object
            var objDLC = editor.controller.transform.GetComponentInChildren<Object_Ade>();
            // Find cup objects
            List<Cup_Coffee> cupGlass = new List<Cup_Coffee>();
            foreach (Cup_Coffee cup in editor.controller.transform.GetComponentsInChildren<Cup_Coffee>()) if (cup.m_type == TypeCup.Glass) cupGlass.Add(cup);

            // Install
            if (!objDLC)
            {
                // Load assets
                Object prefabDLC = AssetDatabase.LoadAssetAtPath(editor.pathRoot + PATH_OBJ, typeof(Object));
                Object prefabCup = AssetDatabase.LoadAssetAtPath(editor.pathRoot + PATH_CUPGLASS, typeof(Object));
                // Check validation
                if (!prefabDLC || !prefabCup)
                {
                    MainMethod.DebugLogError("The Ade DLC file is not correct. Please re-import the DLC Prefab.");
                    return;
                }
                // Instantiate objects
                var objNew = PrefabUtility.InstantiatePrefab(prefabDLC, editor.controller.transform) as GameObject;
                PrefabUtility.UnpackPrefabInstance(objNew, PrefabUnpackMode.OutermostRoot, InteractionMode.AutomatedAction);
                foreach (Cup_Coffee cup in cupGlass)
                {
                    GameObject[] obj = MainMethod.FindObjectsByName(cup.transform, NAME_CUPGLASS, true);
                    if (obj.Length > 1)
                    {
                        for (int i = 1; i < obj.Length; i++) Object.DestroyImmediate(obj[i]);
                        continue;
                    }
                    else if (obj.Length > 0) continue;
                    GameObject go = PrefabUtility.InstantiatePrefab(
                        AssetDatabase.LoadAssetAtPath<GameObject>(editor.pathRoot + PATH_CUPGLASS), cup.transform
                    ) as GameObject;
                    cup.m_particle[0] = go.GetComponentInChildren<ParticleSystem>();
                }
                // Set Materials
                editor.controller.AssignMaterialToField(AssetDatabase.LoadAssetAtPath<MaterialGroup>(editor.pathRoot + PATH_MATGROUP));
                // Set menu plate
                foreach (var item in editor.controller.GetComponentsInChildren<Menu_Plate>())
                {
                    TextMeshProUGUI textAde = GetMenuAdeText(item);
                    if (!textAde) continue;
                    EditorUtility.SetDirty(textAde);
                    var lang = MainMethod.GetJsonByLangcode(MainMethod.GetJsonsAtPath($"{editor.pathRoot}{PATH_LANG}"), "en");
                    textAde.text = MainMethod.GetJsonValue(lang, "lemon") + "\n" +
                    MainMethod.GetJsonValue(lang, "greenGrape") + "\n" +
                    MainMethod.GetJsonValue(lang, "grapefruit") + "\n" +
                    MainMethod.GetJsonValue(lang, "strawberry");
                }
                // Set pad
                foreach (var item in editor.controller.transform.GetComponentsInChildren<Pad>())
                {
                    // Button
                    var go = PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(editor.pathRoot + PATH_UI_RECIPEBTN), item.m_recipeBtnContainer) as GameObject;
                    go.name = NAME_UI_RECIPEBTN;
                    go.transform.SetSiblingIndex(1);
                    var goBtn = go.GetComponentInChildren<Button>();
                    var methodDelegate = System.Delegate.CreateDelegate(
                        typeof(UnityAction<string>), item.m_textHolder,
                        item.m_textHolder.GetType().GetProperty("text").GetSetMethod()
                    );
                    UnityEventTools.AddStringPersistentListener(goBtn.onClick, (UnityAction<string>)methodDelegate, "Ade_Ade");
                    UnityEventTools.AddStringPersistentListener(goBtn.onClick, ((UdonBehaviour)item.GetComponent(typeof(UdonBehaviour))).SendCustomEvent, "BtnSectionNext");
                    // Section
                    go = PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(editor.pathRoot + PATH_UI_RECIPESEC), item.m_recipeSectionContainer) as GameObject;
                    go.name = NAME_UI_RECIPESEC;
                    go.transform.SetSiblingIndex(item.m_recipeSectionContainer.childCount - 2);
                    goBtn = go.GetComponentInChildren<Button>();
                    methodDelegate = System.Delegate.CreateDelegate(
                        typeof(UnityAction<string>), item.m_textHolder,
                        item.m_textHolder.GetType().GetProperty("text").GetSetMethod()
                    );
                    UnityEventTools.AddStringPersistentListener(goBtn.onClick, (UnityAction<string>)methodDelegate, "0");
                    UnityEventTools.AddStringPersistentListener(goBtn.onClick, ((UdonBehaviour)item.GetComponent(typeof(UdonBehaviour))).SendCustomEvent, "BtnSectionBack");
                }
                // Set parameters
                editor.controller.m_udonDLC[0] = (UdonBehaviour)objNew.GetComponent(typeof(UdonBehaviour));
                MainMethod.DebugLog("DLC Ade has been installed.");
            }
            // Uninstall
            else
            {
                // Destroy objects
                Object.DestroyImmediate(objDLC.gameObject);
                foreach (Cup_Coffee cup in cupGlass)
                {
                    GameObject[] obj = MainMethod.FindObjectsByName(cup.transform, NAME_CUPGLASS, true);
                    foreach (var item in obj) Object.DestroyImmediate(item);
                }
                // Set menu plate
                foreach (var item in editor.controller.transform.GetComponentsInChildren<Menu_Plate>())
                {
                    var lang = MainMethod.GetJsonByLangcode(MainMethod.GetJsonsAtPath($"{editor.pathRoot}/Assets/Languages"), "en");
                    GetMenuAdeText(item).text = MainMethod.GetJsonValue(lang, "needDLC");
                }
                // Set pad
                foreach (var item in editor.controller.transform.GetComponentsInChildren<Pad>())
                {
                    for (int i = 0; i < item.m_recipeBtnContainer.childCount; i++)
                        if (item.m_recipeBtnContainer.GetChild(i).name == NAME_UI_RECIPEBTN)
                            Object.DestroyImmediate(item.m_recipeBtnContainer.GetChild(i).gameObject);
                    for (int i = 0; i < item.m_recipeSectionContainer.childCount; i++)
                        if (item.m_recipeSectionContainer.GetChild(i).name == NAME_UI_RECIPESEC)
                            Object.DestroyImmediate(item.m_recipeSectionContainer.GetChild(i).gameObject);
                }
                // Set parameters
                editor.controller.mat_Ade = null;
                editor.controller.m_udonDLC[0] = null;
                MainMethod.DebugLog("DLC Ade has been uninstalled.");
            }
        }
        static public void Reset(MainController_Editor editor)
        {
            foreach (var item in editor.controller.transform.GetComponentsInChildren<Object_Ade>()) Object.DestroyImmediate(item.gameObject);
            editor.controller.m_udonDLC[0] = null;
        }
        static public void Initialize(MainController controller)
        {
            var ms = MonoScript.FromMonoBehaviour(controller);
            var pathAsset = AssetDatabase.GetAssetPath(ms).Replace($"/Assets/Scripts/{ms.name}.cs", "");

            // Language
            string[] jsons = MainMethod.GetJsonsAtPath(pathAsset + PATH_LANG);
            for (int i = 0; i < controller.m_langJson.Length; i++)
            {
                var temp = MainMethod.GetJsonByLangcode(jsons, MainMethod.GetJsonValue(controller.m_langJson[i], "langCode"));
                if (temp == null) continue;
                MainMethod.RemoveJsonData(ref temp, "langCode");
                MainMethod.MergeJson(ref controller.m_langJson[i], temp);
            }
        }

        static TextMeshProUGUI GetMenuAdeText(Menu_Plate menu)
        {
            foreach (var item in menu.GetComponentsInChildren<TextMeshProUGUI>(true))
                if (item.name == "|DLCAde|menuAde") return item;
            return null;
        }
    }
}
