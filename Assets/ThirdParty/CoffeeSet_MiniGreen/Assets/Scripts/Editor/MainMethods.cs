using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

using VRC.SDKBase;
using VRC.SDK3.Data;

using System;
using System.Reflection;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine.UIElements;

namespace VRCCoffeeSet
{
    public static class MainMethod
    {
        static public bool Initialize(string type)
        {
            // Get MainController in the scene
            var controllers = GameObject.FindObjectsOfType<MainController>();

            // Check MainController count
            if (controllers.Length == 0) return true;
            else if (controllers.Length > 1)
            {
                DebugLogError("There must be only one Coffee Set prefab in the scene");
                return false;
            }
            var controller = controllers[0];

            // Get Path
            var ms = MonoScript.FromMonoBehaviour(controller);
            var pathAsset = AssetDatabase.GetAssetPath(ms).Replace($"/Scripts/{ms.name}.cs", "");

            // unpack if controller is the prefab but not completely
            if (PrefabUtility.IsPartOfPrefabInstance(controller.gameObject))
                PrefabUtility.UnpackPrefabInstance(controller.gameObject, PrefabUnpackMode.OutermostRoot, InteractionMode.AutomatedAction);

            #region Audio Sources Update

            controller.audios = controller.GetComponentsInChildren<AudioSource>();
            foreach(AudioSource item in controller.GetComponentsInChildren<AudioSource>())
            {
                item.minDistance = 0;
                item.maxDistance = controller.m_soundRangeMax;
            }

            #endregion

            #region Machine

            List<Machine> machines = controller.GetComponentsInChildren<Machine>().ToList();
            List<Machine_Component> grinder = new List<Machine_Component>();
            List<Machine> machine = new List<Machine>();
            List<Machine_Component> machineF = new List<Machine_Component>();
            List<Machine_Component> machineP = new List<Machine_Component>();
            List<Machine> machineIce = new List<Machine>();

            int tempIndex = 0;
            for (int i = 0; i < machines.Count; i++)
            {
                EditorUtility.SetDirty(machines[i]);
                switch (machines[i].m_type)
                {
                    case TypeMachine.Grinder:
                        machines[i].m_index = (byte)grinder.Count;
                        grinder.Add(machines[i].GetComponentInChildren<Machine_Component>());
                        break;
                    case TypeMachine.Espresso:
                        machines[i].m_index = (byte)machine.Count;
                        machine.Add(machines[i]);
                        Machine_Component[] deposit = machines[i].GetComponentsInChildren<Machine_Component>();
                        foreach (Machine_Component script in deposit)
                        {
                            if (script.m_type == TypeMachineE.E_DF) machineF.Add(script);
                            else if (script.m_type == TypeMachineE.E_DP) machineP.Add(script);
                            else if (script.m_type == TypeMachineE.E_DC)
                            {
                                EditorUtility.SetDirty(script);
                                script.m_mode = (byte)tempIndex;
                                tempIndex++;
                            }
                        }
                        break;
                    case TypeMachine.Ice:
                        machines[i].m_index = (byte)machineIce.Count;
                        machineIce.Add(machines[i]);
                        break;
                }
            }
            controller.m_machine = machine.ToArray();
            controller.m_machineF = machineF.ToArray();
            controller.m_machineP = machineP.ToArray();

            #endregion

            #region Tool

            List<Tool> tools = controller.GetComponentsInChildren<Tool>().ToList();
            List<Tool> tampers = new List<Tool>();
            List<Tool> filters = new List<Tool>();
            List<Tool> pitchers = new List<Tool>();
            List<Tool> straws = new List<Tool>();
            List<Tool> others = new List<Tool>();
            foreach (Tool item in tools)
            {
                EditorUtility.SetDirty(item);
                item.m_controller = controller;
                item.m_animator = item.GetComponent<Animator>();
                item.m_pickup = (VRC_Pickup)item.GetComponentInChildren(typeof(VRC_Pickup));
                switch (item.m_type)
                {
                    case TypeTool.Tamper: tampers.Add(item);
                        break;
                    case TypeTool.Filter: filters.Add(item);
                        break;
                    case TypeTool.Pitcher: pitchers.Add(item);
                        break;
                    case TypeTool.Straw: straws.Add(item);
                        break;
                    case TypeTool.KnockBox: 
                        break;
                    default: others.Add(item);
                        break;
                }
            }
            controller.tool_tamper = tampers.AssignIndex();
            controller.tool_filter = filters.AssignIndex();
            controller.tool_pitcher = pitchers.AssignIndex();
            controller.tool_straw = straws.AssignIndex();
            controller.tool_other = others.ToArray();

            #endregion

            #region Cup, Plate

            List<Cup_Coffee> cups = controller.GetComponentsInChildren<Cup_Coffee>().ToList();
            List<Plate> plates = controller.GetComponentsInChildren<Plate>().ToList();
            List<Cup_Coffee> cupEspresso = new List<Cup_Coffee>();
            List<Cup_Coffee> cupCoffee = new List<Cup_Coffee>();
            List<Cup_Coffee> cupGlass = new List<Cup_Coffee>();
            List<Plate> plateEspresso = new List<Plate>();
            List<Plate> plateCoffee = new List<Plate>();

            foreach (Cup_Coffee item in cups)
            {
                EditorUtility.SetDirty(item);
                item.m_controller = controller;
                item.m_animator = item.GetComponent<Animator>();
                item.m_pickup = (VRC_Pickup)item.GetComponentInChildren(typeof(VRC_Pickup));
                switch (item.m_type)
                {
                    case TypeCup.Espresso: cupEspresso.Add(item);
                        break;
                    case TypeCup.Coffee: cupCoffee.Add(item);
                        break;
                    case TypeCup.Glass: cupGlass.Add(item);
                        break;
                }
            }
            foreach (Plate item in plates)
            {
                EditorUtility.SetDirty(item);
                item.m_controller = controller;
                switch (item.m_type)
                {
                    case TypeCup.Espresso: plateEspresso.Add(item);
                        break;
                    case TypeCup.Coffee: plateCoffee.Add(item);
                        break;
                }
            }
            controller.dish_espressoCup = cupEspresso.AssignIndex();
            controller.dish_coffeeCup = cupCoffee.AssignIndex();
            controller.dish_Glass = cupGlass.AssignIndex();
            controller.dish_espressoPlate = plateEspresso.AssignIndex();
            controller.dish_coffeePlate = plateCoffee.AssignIndex();
            // foreach (Cup_Coffee item in cups) UdonSharpEditorUtility.CopyProxyToUdon(item, ProxySerializationPolicy.All);

            #endregion

            // Assign materials from material group
            controller.AssignMaterialToField(AssetDatabase.LoadAssetAtPath<MaterialGroup>($"{pathAsset}/Materials/MatGroup_Main.asset"));

            #region Language
            
            controller.m_langJson = GetJsonsAtPath($"{pathAsset}/Languages");

            var pads = controller.GetComponentsInChildren<Pad>();
            var menuPlate = controller.GetComponentsInChildren<Menu_Plate>();
            var espressoMachine = controller.GetComponentsInChildren<Machine>().Where( machine => machine.m_type == TypeMachine.Espresso ).ToArray();
            
            // Destroy instantiated font objects
            foreach (var pad in pads) DestroyChildObjects(pad.m_canvas.transform, true);
            foreach (var item in menuPlate) DestroyChildObjects(item.m_canvas.transform, true);
            // instantiate font objects
            foreach (var json in controller.m_langJson)
            {
                var langFont = GetJsonValue(json, "langFont");
                foreach (var item in pads) InstantiateFontObject(item.m_canvas.transform, langFont);
                foreach (var item in menuPlate) InstantiateFontObject(item.m_canvas.transform, langFont);
                foreach (var item in espressoMachine) InstantiateFontObject(item.m_display.transform, langFont);
            }
            void InstantiateFontObject(Transform canvas, string fontName)
            {
                bool langDetect = false;
                foreach (Transform item2 in canvas) if (item2.name == fontName) langDetect = true;
                if (langDetect) return;;
                var go = GameObject.Instantiate(canvas.GetChild(0).gameObject, canvas);
                go.name = fontName;
                go.SetActive(false);
                var fontAsset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>($"{pathAsset}/Fonts/{fontName}.asset");
                var fontSelf = go.GetComponent<TextMeshProUGUI>();
                if (fontSelf != null) fontSelf.font = fontAsset;
                foreach (var item in go.GetComponentsInChildren<TextMeshProUGUI>(true))
                    if (!item.name.Contains("|IGNORE|")) item.font = fontAsset;
            }
            
            #endregion

            #region DLC

            if (controller.m_udonDLC[0]) Type.GetType("VRCCoffeeSet.Asset_Ade").GetMethod("Initialize").Invoke(null, new object[] {controller} );
            if (controller.m_udonDLC[1]) Type.GetType("VRCCoffeeSet.Asset_Boba").GetMethod("Initialize").Invoke(null, new object[] {controller} );

            #endregion

            DebugLog($"Initialized successfully on {type}");
            return true;
        }
        static public void AssignMaterialToField(this MainController controller, MaterialGroup matGroup)
        {
            FieldInfo fieldInfo;
            for (int i = 0; i < matGroup.m_matInfoList.Count; i++)
            {
                fieldInfo = controller.GetType().GetField(matGroup.m_matInfoList[i].name);
                if (fieldInfo == null) continue;
                if (fieldInfo.FieldType.IsArray) fieldInfo.SetValue(controller, matGroup.m_matInfoList[i].mat);
                else fieldInfo.SetValue(controller, matGroup.m_matInfoList[i].mat[0]);
            }
        }

        static public void Reset(MainController_Editor editor)
        {
            // Reset Variables
            editor.controller.m_langJson = null;
            editor.controller.m_machine = null;
            editor.controller.m_machineF = null;
            editor.controller.m_machineP = null;
            editor.controller.tool_tamper = null;
            editor.controller.tool_filter = null;
            editor.controller.tool_pitcher = null;
            editor.controller.tool_straw = null;
            editor.controller.tool_other = null;
            editor.controller.dish_espressoCup = null;
            editor.controller.dish_espressoPlate = null;
            editor.controller.dish_coffeeCup = null;
            editor.controller.dish_coffeePlate = null;
            editor.controller.dish_Glass = null;

            // Reset Materials
            editor.controller.mat_Ring = null;
            editor.controller.mat_Straw = null;
            editor.controller.mat_Empty = null;
            editor.controller.mat_Water = null;
            editor.controller.mat_WaterIce = null;
            editor.controller.mat_Milk = null;
            editor.controller.mat_MilkIce = null;
            editor.controller.mat_Top = null;
            editor.controller.mat_Cream = null;
            editor.controller.mat_Syrup = null;
            editor.controller.mat_Espresso = null;
            editor.controller.mat_EspressoIce = null;
            editor.controller.mat_Americano = null;
            editor.controller.mat_AmericanoIce = null;
            editor.controller.mat_Latte = null;
            editor.controller.mat_LatteIce = null;
            editor.controller.mat_Cappuccino = null;
            editor.controller.mat_Mocha = null;
            editor.controller.mat_MochaArt = null;
            editor.controller.mat_MochaIce = null;
            editor.controller.mat_Macchiato = null;
            editor.controller.mat_MacchiatoIce = null;
            editor.controller.mat_Ade = null;
            editor.controller.mat_BobaPearls = null;
            
            // revert prefab changes
            var prefabs = editor.controller.GetComponentsInChildren<Transform>().Where( trans =>
                PrefabUtility.IsOutermostPrefabInstanceRoot(trans.gameObject) &&
                !PrefabUtility.IsAddedGameObjectOverride(trans.gameObject)
            ).ToArray();
            foreach (var item in prefabs) PrefabUtility.RevertPrefabInstance(item.gameObject, InteractionMode.UserAction);
            
            // DLC
            Type.GetType("VRCCoffeeSet.Asset_Ade")?.GetMethod("Reset")?.Invoke(null, new object[] {editor} );
            Type.GetType("VRCCoffeeSet.Asset_Boba")?.GetMethod("Reset")?.Invoke(null, new object[] {editor} );
            Type.GetType("VRCCoffeeSet.Asset_TEST")?.GetMethod("Reset")?.Invoke(null, new object[] {editor} );
            
            DebugLog("Sucessfully resetted.");
        }
    
        #region JSON Methods

        static public string[] GetJsonsAtPath(string path, bool addFileName = false, bool removeLangCode = false)
        {
            List<string> datas = new List<string>();
            var assets = AssetDatabase.FindAssets("t:TextAsset", new string[] {path})
            .Select(guid => AssetDatabase.LoadAssetAtPath<TextAsset>(AssetDatabase.GUIDToAssetPath(guid))).ToArray();
            foreach (var item in assets)
            {
                string data = Regex.Replace(item.text, @"^\s+", "", RegexOptions.Multiline); // Remove space at the beginning
                if (addFileName) AddJsonDataAtTop(ref data, "fileName", item.name);
                if (removeLangCode) RemoveJsonData(ref data, "langCode");
                // DebugLog(data);
                datas.Add(data);
            }
            return datas.ToArray();
        }
        static public string GetJsonByLangcode(string[] jsons, string langCode) => jsons.FirstOrDefault( json => GetJsonValue(json, "langCode") == langCode );
        static public string GetJsonValue(string json, string key)
        {
            var match = Regex.Match(json, $"\"{key}\"\\s*:\\s*\"([^\"]+)\"");
            if (match.Success) return match.Groups[1].Value;
            return null;
        }
        static public void AddJsonDataAtTop(ref string json, string key, string value)
        {
            var match = Regex.Match(json, @"^\s*\{");
            if (match.Success)
            {
                var modifiedJson = new StringBuilder(json);
                modifiedJson.Insert(match.Index + match.Length, $"\n\"{key}\":\"{value}\",");
                json = modifiedJson.ToString();
            }
        }
        static public void RemoveJsonData(ref string json, string key)
        {
            json = Regex.Replace(json, $"\"{key}\":\\s*\"\\w+\"\\s*,?", "", RegexOptions.Multiline);
            json = Regex.Replace(json, @"^\s+", "", RegexOptions.Multiline);
        }
        static public void MergeJson(ref string json, string json2)
        {
            // Json 파일 형식은 유지하면서 json의 아래에 json2 붙이기
            json = Regex.Replace(json, @"\}\s*$", "", RegexOptions.Multiline);
            // json 의 마지막 큰 따옴표(") 뒤에 쉼표(,) 붙이기
            json = Regex.Replace(json, "\"\\s*$", "\",", RegexOptions.Multiline);
            json += Regex.Replace(json2, @"^\s*\{", "", RegexOptions.Multiline);
        }
        
        #endregion

        static public void DebugLog(string str) => Debug.Log("<color=#99ff00>Coffee Set</color> " + str);
        static public void DebugLogError(string str) => Debug.LogError("<color=#99ff00>Coffee Set</color> " + str);

        static public Tool[] AssignIndex(this List<Tool> list)
        {
            for (int i = 0; i < list.Count; i++) list[i].m_index = (byte)i;
            return list.ToArray();
        }
        static public Cup_Coffee[] AssignIndex(this List<Cup_Coffee> list)
        {
            for (byte i = 0; i < list.Count; i++) list[i].m_index = i;
            return list.ToArray();
        }
        static public Plate[] AssignIndex(this List<Plate> list)
        {
            for (byte i = 0; i < list.Count; i++) list[i].m_index = i;
            return list.ToArray();
        }
        static public Transform CreateObject(string _name, Vector3 _pos, Vector3 _rot, Transform _parent = null)
        {
            Transform tempTrans = new GameObject().transform;
            tempTrans.name = _name;
            if (_parent) tempTrans.SetParent(_parent);
            tempTrans.position = _pos;
            tempTrans.eulerAngles = _rot;
            return tempTrans;
        }
        static public GameObject[] FindObjectsByName(Transform transform, string name, bool contains = false)
        {
            List<GameObject> list = new List<GameObject>();
            if (contains) foreach (Transform item in transform) if (item.name.Contains(name)) list.Add(item.gameObject);
            else foreach (Transform item2 in transform) if (item2.name == name) list.Add(item2.gameObject);
            return list.ToArray();
        }
        static public void DestroyChildObjects(Transform transform, bool exceptFirst = false)
        {
            List<GameObject> list = new List<GameObject>();
            foreach (Transform item in transform) list.Add(item.gameObject);
            for (int i = exceptFirst ? 1:0; i < list.Count; i++) GameObject.DestroyImmediate(list[i]);
        }
    }
}