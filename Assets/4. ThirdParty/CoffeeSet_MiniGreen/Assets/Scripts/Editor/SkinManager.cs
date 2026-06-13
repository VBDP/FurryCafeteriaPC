using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace VRCCoffeeSet
{
    public enum SkinsMachine
    {
        Default, WhiteWood
    }

    public enum SkinsCupEspresso
    {
        Default
    }
    public enum SkinsCupCoffee
    {
        Default, Mug
    }
    public enum SkinsCupGlass
    {
        Default
    }

    static public class SkinManager
    {
        #region Change

        static public string ChangeMachine(Machine machine, SkinsMachine skin)
        {
            string pathRoot = GetAssetPath(machine.gameObject);
            GameObject objTarget = null;
            Type tempType = null;

            switch (skin)
            {
                case SkinsMachine.Default:
                    if (machine.m_type == TypeMachine.Grinder)
                        objTarget = LoadAssetObj($"{pathRoot}/Assets/Prefabs/Machine_Grinder.prefab");
                    else if (machine.m_type == TypeMachine.Espresso)
                        objTarget = LoadAssetObj($"{pathRoot}/Assets/Prefabs/Machine_Espresso.prefab");
                    else if (machine.m_type == TypeMachine.Ice)
                        objTarget = LoadAssetObj($"{pathRoot}/Assets/Prefabs/Machine_Ice.prefab");
                    break;
                case SkinsMachine.WhiteWood:
                    tempType = Type.GetType("VRCCoffeeSet.Skin_WhiteWood");
                    if (tempType == null) return "You don't have WhiteWood Skin DLC.";
                    if (machine.m_type == TypeMachine.Grinder)
                        objTarget = LoadAssetObj($"{pathRoot}{tempType.GetField("PATH_G").GetValue("PATH_G") as string}");
                    else if (machine.m_type == TypeMachine.Espresso)
                        objTarget = LoadAssetObj($"{pathRoot}{tempType.GetField("PATH_ME").GetValue("PATH_ME") as string}");
                    else if (machine.m_type == TypeMachine.Ice)
                        objTarget = LoadAssetObj($"{pathRoot}{tempType.GetField("PATH_I").GetValue("PATH_I") as string}");
                    break;
            }
            if (!objTarget) return "ERROR";
            else
            {
                ReplaceObject(machine.gameObject, objTarget);
                return null;
            }
        }

        static public GameObject ChangeCup(Cup_Coffee cup, int indexSkin)
        {
            string pathRoot = GetAssetPath(cup.gameObject);

            GameObject objTarget = null;

            if (cup.m_type == TypeCup.Espresso) switch ((SkinsCupEspresso)indexSkin)
            {
                case SkinsCupEspresso.Default: objTarget = LoadAssetObj($"{pathRoot}/Assets/Prefabs/CupEspresso.prefab");
                    break;
            }
            else if (cup.m_type == TypeCup.Coffee) switch ((SkinsCupCoffee)indexSkin)
            {
                case SkinsCupCoffee.Default: objTarget = LoadAssetObj($"{pathRoot}/Assets/Prefabs/CupCoffee.prefab");
                    break;
                case SkinsCupCoffee.Mug: objTarget = LoadAssetObj($"{pathRoot}/Assets/Prefabs/CupMug.prefab");
                    break;
            }
            else if (cup.m_type == TypeCup.Glass) switch ((SkinsCupGlass)indexSkin)
            {
                case SkinsCupGlass.Default: objTarget = LoadAssetObj($"{pathRoot}/Assets/Prefabs/CupGlass.prefab");
                    break;
            }
            if (!objTarget) return null;
            else
            {
                return ReplaceObject(cup.gameObject, objTarget);
            }
        }

        static public string ChangeTool(Tool tool, int indexSkin)
        {
            string pathRoot = GetAssetPath(tool.gameObject);
            GameObject objTarget = null;
            Type tempType = null;

            switch (tool.m_type)
            {
                case TypeTool.Filter:
                    SkinsMachine skinFilter = (SkinsMachine)indexSkin;
                    if (skinFilter == SkinsMachine.Default)
                        objTarget = LoadAssetObj($"{pathRoot}/Assets/Prefabs/Portafilter.prefab");
                    else if (skinFilter == SkinsMachine.WhiteWood)
                    {
                        tempType = Type.GetType("VRCCoffeeSet.Skin_WhiteWood");
                        if (tempType == null) return "You don't have WhiteWood Skin DLC.";
                        objTarget = LoadAssetObj($"{pathRoot}{tempType.GetField("PATH_PF").GetValue("PATH_PF") as string}");
                    }
                    break;
                case TypeTool.KnockBox:
                    SkinsMachine skinKnockBox = (SkinsMachine)indexSkin;
                    if (skinKnockBox == SkinsMachine.Default)
                        objTarget = LoadAssetObj($"{pathRoot}/Assets/Prefabs/KnockBox.prefab");
                    else if (skinKnockBox == SkinsMachine.WhiteWood)
                    {
                        tempType = Type.GetType("VRCCoffeeSet.Skin_WhiteWood");
                        if (tempType == null) return "You don't have WhiteWood Skin DLC.";
                        objTarget = LoadAssetObj($"{pathRoot}{tempType.GetField("PATH_KB").GetValue("PATH_KB") as string}");
                    }
                    break;
                case TypeTool.Tamper:
                    SkinsMachine skinTamper = (SkinsMachine)indexSkin;
                    if (skinTamper == SkinsMachine.Default)
                        objTarget = LoadAssetObj($"{pathRoot}/Assets/Prefabs/Tamper.prefab");
                    else if (skinTamper == SkinsMachine.WhiteWood)
                    {
                        tempType = Type.GetType("VRCCoffeeSet.Skin_WhiteWood");
                        if (tempType == null) return "You don't have WhiteWood Skin DLC.";
                        objTarget = LoadAssetObj($"{pathRoot}{tempType.GetField("PATH_T").GetValue("PATH_T") as string}");
                    }
                    break;
            }
            if (!objTarget) return "ERROR";
            else
            {
                ReplaceObject(tool.gameObject, objTarget);
                return null;
            }
        }

        #endregion

        #region Skin

        static public SkinsMachine GetSkinMachine(Machine machine)
        {
            string pathRoot = GetAssetPath(machine.gameObject);
            GameObject objThis = PrefabUtility.GetCorrespondingObjectFromOriginalSource<GameObject>(machine.gameObject);
            SkinsMachine skin = SkinsMachine.Default;
            Type typeWW = Type.GetType("VRCCoffeeSet.Skin_WhiteWood");

            switch (machine.m_type)
            {
                case TypeMachine.Grinder:
                    if (CompareObject(objThis, $"{pathRoot}/Assets/Models/Machine_Grinder.fbx")) return SkinsMachine.Default;
                    if (typeWW != null)
                        if (CompareObject(objThis, $"{pathRoot}{typeWW.GetField("PATH_Gfbx").GetValue("PATH_Gfbx") as string}")) return SkinsMachine.WhiteWood;
                    break;
                case TypeMachine.Espresso:
                    if (CompareObject(objThis, $"{pathRoot}/Assets/Models/Machine_Espresso.fbx")) return SkinsMachine.Default;
                    if (typeWW != null)
                        if (CompareObject(objThis, $"{pathRoot}{typeWW.GetField("PATH_MEfbx").GetValue("PATH_MEfbx") as string}")) return SkinsMachine.WhiteWood;
                    break;
                case TypeMachine.Ice:
                    if (CompareObject(objThis, $"{pathRoot}/Assets/Models/Machine_Ice.fbx")) return SkinsMachine.Default;
                    if (typeWW != null)
                        if (CompareObject(objThis, $"{pathRoot}{typeWW.GetField("PATH_Ifbx").GetValue("PATH_Ifbx") as string}")) return SkinsMachine.WhiteWood;
                    break;
            }
            return skin;
        }

        static public string[] GetSkinList_Machine()
        {
            List<string> listSkin = new List<string>{"Default"};
            if (Type.GetType("VRCCoffeeSet.Skin_WhiteWood") != null) listSkin.Add("White Wood");
            return listSkin.ToArray();
        }

        static public int GetSkinCup(Cup_Coffee cup)
        {
            string pathRoot = GetAssetPath(cup.gameObject);
            GameObject thisObj = PrefabUtility.GetCorrespondingObjectFromOriginalSource<GameObject>(cup.gameObject);
            int tempSkinIndex = 0;

            switch (cup.m_type)
            {
                case TypeCup.Coffee:
                    if (CompareObject(thisObj, $"{pathRoot}/Assets/Prefabs/CupCoffee.prefab")) tempSkinIndex = (int)SkinsCupCoffee.Default;
                    else if (CompareObject(thisObj, $"{pathRoot}/Assets/Prefabs/CupMug.prefab")) tempSkinIndex = (int)SkinsCupCoffee.Mug;
                    break;
            }

            return tempSkinIndex;
        }

        static public string[] GetSkinList_Cup(TypeCup type)
        {
            List<string> listSkin = new List<string>{"Default"};
            switch (type)
            {
                case TypeCup.Espresso:
                    break;
                case TypeCup.Coffee:
                    listSkin.Add("Mug");
                    break;
                case TypeCup.Glass:
                    break;
            }
            return listSkin.ToArray();
        }

        static public int GetSkinTool(Tool tool)
        {
            string pathRoot = GetAssetPath(tool.gameObject);
            GameObject objThis = PrefabUtility.GetCorrespondingObjectFromOriginalSource<GameObject>(tool.gameObject);
            int indexSkin = 0;
            Type tempType = null;

            switch (tool.m_type)
            {
                case TypeTool.Filter:
                    if (CompareObject(objThis, $"{pathRoot}/Assets/Models/Portafilter.fbx")) return (int)SkinsMachine.Default;
                    tempType = Type.GetType("VRCCoffeeSet.Skin_WhiteWood");
                    if (tempType != null)
                        if (CompareObject(objThis, $"{pathRoot}{tempType.GetField("PATH_PFfbx").GetValue("PATH_PFfbx") as string}")) return (int)SkinsMachine.WhiteWood;
                    break;
                case TypeTool.KnockBox:
                    if (CompareObject(objThis, $"{pathRoot}/Assets/Models/KnockBox.fbx")) return (int)SkinsMachine.Default;
                    tempType = Type.GetType("VRCCoffeeSet.Skin_WhiteWood");
                    if (tempType != null)
                        if (CompareObject(objThis, $"{pathRoot}{tempType.GetField("PATH_KBfbx").GetValue("PATH_KBfbx") as string}")) return (int)SkinsMachine.WhiteWood;
                    break;
                case TypeTool.Tamper:
                    if (CompareObject(objThis, $"{pathRoot}/Assets/Models/Tamper.fbx")) return (int)SkinsMachine.Default;
                    tempType = Type.GetType("VRCCoffeeSet.Skin_WhiteWood");
                    if (tempType != null)
                        if (CompareObject(objThis, $"{pathRoot}{tempType.GetField("PATH_Tfbx").GetValue("PATH_Tfbx") as string}")) return (int)SkinsMachine.WhiteWood;
                    break;
            }

            return indexSkin;
        }

        #endregion

        #region Color

        static public Color[] GetColorCup(Cup_Coffee cup, Renderer renderer)
        {
            Color[] color = new Color[0];

            switch (cup.m_type)
            {
                case TypeCup.Coffee:
                    SkinsCupCoffee skin = (SkinsCupCoffee)GetSkinCup(cup);
                    if (skin == SkinsCupCoffee.Mug)
                    {
                        Array.Resize(ref color, 1);
                        color[0] = renderer.sharedMaterials[0].color;
                    }
                    break;
                default: break;
            }
            return color;
        }
        static public void SetColor(Renderer renderer, int indexMat, Color color)
        {
            Material[] tempMat = renderer.sharedMaterials;
            if (tempMat[indexMat].name != renderer.transform.parent.name)
            {
                tempMat[indexMat] = UnityEngine.Object.Instantiate( tempMat[indexMat] );
                tempMat[indexMat].name = renderer.transform.parent.name;
            }
            tempMat[indexMat].color = color;
            renderer.sharedMaterials = tempMat;
        }

        #endregion

        #region Function

        static private string GetAssetPath(GameObject obj)
        {
            var controller = obj.transform.GetComponentInParent<MainController>();
            if (!controller) return null;
            var ms = MonoScript.FromMonoBehaviour(controller);
            return AssetDatabase.GetAssetPath(ms).Replace($"/Assets/Scripts/{ms.name}.cs", "");
        }

        static private GameObject LoadAssetObj(string path)
        {
            return AssetDatabase.LoadAssetAtPath<GameObject>(path);
        }

        static private bool CompareObject(GameObject obj, string path)
        {
            if (obj == LoadAssetObj(path)) return true;
            else return false;
        }

        static private GameObject ReplaceObject(GameObject before, GameObject after)
        {
            var objTarget = PrefabUtility.InstantiatePrefab(after, before.transform.parent);
            objTarget.name = before.name;
            
            ((GameObject)objTarget).transform.SetPositionAndRotation(before.transform.position, before.transform.rotation);
            ((GameObject)objTarget).transform.localScale = before.transform.localScale;
            ((GameObject)objTarget).transform.SetSiblingIndex( before.transform.GetSiblingIndex() );

            UnityEngine.Object.DestroyImmediate(before.gameObject);
            Selection.activeObject = objTarget;
            // EditorApplication.delayCall += delegate
            // {
            //     Selection.activeObject = null;
            //     EditorApplication.delayCall += () => Selection.activeObject = objTarget;
            // };
            return (GameObject)objTarget;

            // EditorApplication.delayCall += () => Selection.activeObject = objTarget;
            // Assembly.GetAssembly(typeof(Editor)).GetType("UnityEditor.InspectorWindow").GetMethod("RepaintAllInspectors", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, null);
            // foreach (var item in Resources.FindObjectsOfTypeAll<Editor>())
            // {
            //     Debug.Log(item.GetType());
            // }
        }

        #endregion

    }
}