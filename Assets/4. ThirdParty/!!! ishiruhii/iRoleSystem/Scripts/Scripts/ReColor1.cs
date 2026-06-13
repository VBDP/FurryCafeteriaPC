// ============================================================
//  ReColor1.cs  —  Script de color de jerarquia
//  Ubicacion: Assets/TuProyecto/Scripts/
//  Requiere: iHC_Core.cs en la misma carpeta (o cualquier carpeta del proyecto)
//  NO coloques este archivo dentro de una carpeta Editor
//  Generado con: ishiruhii > iTools > iHierarchyEditor
// ============================================================

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace ishiruhii.iTools
{
    [InitializeOnLoad]
    public static class ReColor1
    {
        // ITOOLS_RULES_BEGIN
        private static readonly iHC_Rule[] rules = new iHC_Rule[]
        {
            new iHC_Rule {
                objectName      = "iRoleSystem Pro",
                backgroundColor = new Color(0.132076f, 0.132076f, 0.132076f, 1.000000f),
                textColor       = new Color(0.000000f, 0.986470f, 1.000000f, 1.000000f),
                labelMode       = iHC_LabelMode.AdditionalLabel,
                scopeMode       = iHC_ScopeMode.AffectsObject,
                customLabel     = "Main",
                iconPath        = "Assets/!!! ishiruhii/iHiearchyPro/Sprites/box2-heart-fill.png",
                iconTint        = new Color(0.496226f, 0.978978f, 1.000000f, 1.000000f)
            },
            new iHC_Rule {
                objectName      = "iRoleCore",
                backgroundColor = new Color(0.177359f, 0.177359f, 0.177359f, 1.000000f),
                textColor       = new Color(0.941788f, 1.000000f, 0.000000f, 1.000000f),
                labelMode       = iHC_LabelMode.AdditionalLabel,
                scopeMode       = iHC_ScopeMode.AffectsObject,
                customLabel     = "Nucleo",
                iconPath        = "Assets/!!! ishiruhii/iHiearchyPro/Sprites/x-diamond.png",
                iconTint        = new Color(1.000000f, 0.966349f, 0.503774f, 1.000000f)
            },
            new iHC_Rule {
                objectName      = "iRoleDatabase",
                backgroundColor = new Color(0.177359f, 0.177359f, 0.177359f, 1.000000f),
                textColor       = new Color(0.941788f, 1.000000f, 0.000000f, 1.000000f),
                labelMode       = iHC_LabelMode.AdditionalLabel,
                scopeMode       = iHC_ScopeMode.AffectsObject,
                customLabel     = "Rank Names",
                iconPath        = "Assets/!!! ishiruhii/iHiearchyPro/Sprites/database-fill.png",
                iconTint        = new Color(1.000000f, 0.966349f, 0.503774f, 1.000000f)
            },
            new iHC_Rule {
                objectName      = "iRoleManager",
                backgroundColor = new Color(0.177359f, 0.177359f, 0.177359f, 1.000000f),
                textColor       = new Color(0.941788f, 1.000000f, 0.000000f, 1.000000f),
                labelMode       = iHC_LabelMode.AdditionalLabel,
                scopeMode       = iHC_ScopeMode.AffectsObject,
                customLabel     = "Nucleo",
                iconPath        = "Assets/!!! ishiruhii/iHiearchyPro/Sprites/x-diamond.png",
                iconTint        = new Color(1.000000f, 0.966349f, 0.503774f, 1.000000f)
            },
            new iHC_Rule {
                objectName      = "MODULOS",
                backgroundColor = new Color(0.133333f, 0.133333f, 0.133333f, 1.000000f),
                textColor       = new Color(1.000000f, 0.000000f, 0.671160f, 1.000000f),
                labelMode       = iHC_LabelMode.AdditionalLabel,
                scopeMode       = iHC_ScopeMode.AffectsObject,
                customLabel     = "System Modules",
                iconPath        = "Assets/!!! ishiruhii/iHiearchyPro/Sprites/diagram-3.png",
                iconTint        = new Color(1.000000f, 0.501961f, 0.806107f, 1.000000f)
            },
            new iHC_Rule {
                objectName      = "iRankDisplay",
                backgroundColor = new Color(0.000390f, 0.215094f, 0.000000f, 1.000000f),
                textColor       = new Color(0.000000f, 1.000000f, 0.026534f, 1.000000f),
                labelMode       = iHC_LabelMode.AdditionalLabel,
                scopeMode       = iHC_ScopeMode.AffectsObject,
                customLabel     = "Module (Not Copy)",
                iconPath        = "Assets/!!! ishiruhii/iHiearchyPro/Sprites/vinyl.png",
                iconTint        = new Color(0.035849f, 1.000000f, 0.199286f, 1.000000f)
            },
            new iHC_Rule {
                objectName      = "iRankDisplay",
                backgroundColor = new Color(0.000000f, 0.094340f, 0.000000f, 1.000000f),
                textColor       = new Color(0.000000f, 1.000000f, 0.026534f, 1.000000f),
                labelMode       = iHC_LabelMode.AdditionalLabel,
                scopeMode       = iHC_ScopeMode.AffectsFirstChildren,
                customLabel     = "Config (Allow Copy)",
                iconPath        = "Assets/!!! ishiruhii/iHiearchyPro/Sprites/textarea.png",
                iconTint        = new Color(0.035849f, 1.000000f, 0.199286f, 1.000000f)
            },
            new iHC_Rule {
                objectName      = "iObjects",
                backgroundColor = new Color(0.000390f, 0.215094f, 0.000000f, 1.000000f),
                textColor       = new Color(0.000000f, 1.000000f, 0.026534f, 1.000000f),
                labelMode       = iHC_LabelMode.AdditionalLabel,
                scopeMode       = iHC_ScopeMode.AffectsObject,
                customLabel     = "Module (Allow Copy)",
                iconPath        = "Assets/!!! ishiruhii/iHiearchyPro/Sprites/vinyl.png",
                iconTint        = new Color(0.035849f, 1.000000f, 0.199286f, 1.000000f)
            },
            new iHC_Rule {
                objectName      = "iRolePrivateZone",
                backgroundColor = new Color(0.000390f, 0.215094f, 0.000000f, 1.000000f),
                textColor       = new Color(0.000000f, 1.000000f, 0.026534f, 1.000000f),
                labelMode       = iHC_LabelMode.AdditionalLabel,
                scopeMode       = iHC_ScopeMode.AffectsObject,
                customLabel     = "Module (Allow Copy)",
                iconPath        = "Assets/!!! ishiruhii/iHiearchyPro/Sprites/vinyl.png",
                iconTint        = new Color(0.035849f, 1.000000f, 0.199286f, 1.000000f)
            },
            new iHC_Rule {
                objectName      = "TP-Point",
                backgroundColor = new Color(0.141177f, 0.141177f, 0.141177f, 1.000000f),
                textColor       = new Color(1.000000f, 0.917321f, 0.000000f, 1.000000f),
                labelMode       = iHC_LabelMode.AdditionalLabel,
                scopeMode       = iHC_ScopeMode.AffectsObject,
                customLabel     = "(Move to Exit Point)",
                iconPath        = "Assets/!!! ishiruhii/iHiearchyPro/Sprites/geo-alt-fill.png",
                iconTint        = new Color(1.000000f, 0.968859f, 0.526415f, 1.000000f)
            },
            new iHC_Rule {
                objectName      = "Areas",
                backgroundColor = new Color(0.139623f, 0.139623f, 0.139623f, 1.000000f),
                textColor       = new Color(0.784176f, 0.000000f, 1.000000f, 1.000000f),
                labelMode       = iHC_LabelMode.AdditionalLabel,
                scopeMode       = iHC_ScopeMode.AffectsObject,
                customLabel     = "",
                iconPath        = "Assets/!!! ishiruhii/iHiearchyPro/Sprites/globe-central-south-asia-fill.png",
                iconTint        = new Color(0.959919f, 0.000000f, 1.000000f, 1.000000f)
            },
            new iHC_Rule {
                objectName      = "iAreaZone",
                backgroundColor = new Color(0.139623f, 0.139623f, 0.139623f, 1.000000f),
                textColor       = new Color(0.384314f, 1.000000f, 0.436051f, 1.000000f),
                labelMode       = iHC_LabelMode.AdditionalLabel,
                scopeMode       = iHC_ScopeMode.AffectsObject,
                customLabel     = "(Allow Copy)",
                iconPath        = "Assets/!!! ishiruhii/iHiearchyPro/Sprites/textarea.png",
                iconTint        = new Color(0.000000f, 0.924528f, 0.078665f, 1.000000f)
            },
            new iHC_Rule {
                objectName      = "iJail",
                backgroundColor = new Color(0.109434f, 0.109434f, 0.109434f, 1.000000f),
                textColor       = new Color(0.784176f, 0.000000f, 1.000000f, 1.000000f),
                labelMode       = iHC_LabelMode.AdditionalLabel,
                scopeMode       = iHC_ScopeMode.AffectsObject,
                customLabel     = "(Not Copy)",
                iconPath        = "Assets/!!! ishiruhii/iHiearchyPro/Sprites/globe-central-south-asia-fill.png",
                iconTint        = new Color(0.959919f, 0.000000f, 1.000000f, 1.000000f)
            },
            new iHC_Rule {
                objectName      = "iJail",
                backgroundColor = new Color(0.124528f, 0.124528f, 0.124528f, 1.000000f),
                textColor       = new Color(0.830777f, 0.380085f, 0.954717f, 1.000000f),
                labelMode       = iHC_LabelMode.AdditionalLabel,
                scopeMode       = iHC_ScopeMode.AffectsAllChildren,
                customLabel     = "JailZone",
                iconPath        = "Assets/!!! ishiruhii/iHiearchyPro/Sprites/exclamation-diamond-fill.png",
                iconTint        = new Color(0.960784f, 0.000000f, 1.000000f, 1.000000f)
            },
            new iHC_Rule {
                objectName      = "iPrison",
                backgroundColor = new Color(0.000390f, 0.215094f, 0.000000f, 1.000000f),
                textColor       = new Color(0.000000f, 1.000000f, 0.026534f, 1.000000f),
                labelMode       = iHC_LabelMode.AdditionalLabel,
                scopeMode       = iHC_ScopeMode.AffectsObject,
                customLabel     = "Module (Not Copy)",
                iconPath        = "Assets/!!! ishiruhii/iHiearchyPro/Sprites/vinyl.png",
                iconTint        = new Color(0.035849f, 1.000000f, 0.199286f, 1.000000f)
            },
            new iHC_Rule {
                objectName      = "iNameGetRole",
                backgroundColor = new Color(0.000390f, 0.215094f, 0.000000f, 1.000000f),
                textColor       = new Color(0.000000f, 1.000000f, 0.026534f, 1.000000f),
                labelMode       = iHC_LabelMode.AdditionalLabel,
                scopeMode       = iHC_ScopeMode.AffectsObject,
                customLabel     = "Module (Not Copy)",
                iconPath        = "Assets/!!! ishiruhii/iHiearchyPro/Sprites/vinyl.png",
                iconTint        = new Color(0.035849f, 1.000000f, 0.199286f, 1.000000f)
            },
            new iHC_Rule {
                objectName      = "RoleSelectionCanvas",
                backgroundColor = new Color(0.000000f, 0.260377f, 0.184624f, 1.000000f),
                textColor       = new Color(0.000000f, 1.000000f, 0.808568f, 1.000000f),
                labelMode       = iHC_LabelMode.AdditionalLabel,
                scopeMode       = iHC_ScopeMode.AffectsObject,
                customLabel     = "(Not Copy)",
                iconPath        = "Assets/!!! ishiruhii/iHiearchyPro/Sprites/textarea.png",
                iconTint        = new Color(1.000000f, 1.000000f, 1.000000f, 1.000000f)
            },
            new iHC_Rule {
                objectName      = "iNameGetRole",
                backgroundColor = new Color(0.378418f, 0.000000f, 0.524528f, 1.000000f),
                textColor       = new Color(0.998040f, 0.835849f, 1.000000f, 1.000000f),
                labelMode       = iHC_LabelMode.AdditionalLabel,
                scopeMode       = iHC_ScopeMode.AffectsFirstChildren,
                customLabel     = "Key Assingers",
                iconPath        = "Assets/!!! ishiruhii/iHiearchyPro/Sprites/diagram-3.png",
                iconTint        = new Color(1.000000f, 1.000000f, 1.000000f, 1.000000f)
            },
            new iHC_Rule {
                objectName      = "RoleSelectionCanvas",
                backgroundColor = new Color(0.000000f, 0.260377f, 0.184624f, 1.000000f),
                textColor       = new Color(0.000000f, 1.000000f, 0.808568f, 1.000000f),
                labelMode       = iHC_LabelMode.AdditionalLabel,
                scopeMode       = iHC_ScopeMode.AffectsAllChildren,
                customLabel     = "(Not Copy)",
                iconPath        = "Assets/!!! ishiruhii/iHiearchyPro/Sprites/textarea.png",
                iconTint        = new Color(1.000000f, 1.000000f, 1.000000f, 1.000000f)
            },
            new iHC_Rule {
                objectName      = "iRoleZoneGrant",
                backgroundColor = new Color(0.000390f, 0.215094f, 0.000000f, 1.000000f),
                textColor       = new Color(0.000000f, 1.000000f, 0.026534f, 1.000000f),
                labelMode       = iHC_LabelMode.AdditionalLabel,
                scopeMode       = iHC_ScopeMode.AffectsObject,
                customLabel     = "Module (Allow Copy)",
                iconPath        = "Assets/!!! ishiruhii/iHiearchyPro/Sprites/vinyl.png",
                iconTint        = new Color(0.035849f, 1.000000f, 0.199286f, 1.000000f)
            },
            new iHC_Rule {
                objectName      = "iAutoRole",
                backgroundColor = new Color(0.000390f, 0.215094f, 0.000000f, 1.000000f),
                textColor       = new Color(0.000000f, 1.000000f, 0.026534f, 1.000000f),
                labelMode       = iHC_LabelMode.AdditionalLabel,
                scopeMode       = iHC_ScopeMode.AffectsObject,
                customLabel     = "Module (Not Copy)",
                iconPath        = "Assets/!!! ishiruhii/iHiearchyPro/Sprites/vinyl.png",
                iconTint        = new Color(0.035849f, 1.000000f, 0.199286f, 1.000000f)
            },
            new iHC_Rule {
                objectName      = "iRoleAdminPanel",
                backgroundColor = new Color(0.000390f, 0.215094f, 0.000000f, 1.000000f),
                textColor       = new Color(0.000000f, 1.000000f, 0.026534f, 1.000000f),
                labelMode       = iHC_LabelMode.AdditionalLabel,
                scopeMode       = iHC_ScopeMode.AffectsObject,
                customLabel     = "Module (Not Copy)",
                iconPath        = "Assets/!!! ishiruhii/iHiearchyPro/Sprites/vinyl.png",
                iconTint        = new Color(0.035849f, 1.000000f, 0.199286f, 1.000000f)
            },
            new iHC_Rule {
                objectName      = "AdminRoleCanvas",
                backgroundColor = new Color(0.000000f, 0.260377f, 0.184624f, 1.000000f),
                textColor       = new Color(0.000000f, 1.000000f, 0.808568f, 1.000000f),
                labelMode       = iHC_LabelMode.AdditionalLabel,
                scopeMode       = iHC_ScopeMode.AffectsObject,
                customLabel     = "(Not Copy)",
                iconPath        = "Assets/!!! ishiruhii/iHiearchyPro/Sprites/textarea.png",
                iconTint        = new Color(1.000000f, 1.000000f, 1.000000f, 1.000000f)
            },
            new iHC_Rule {
                objectName      = "AdminRoleCanvas",
                backgroundColor = new Color(0.000000f, 0.260377f, 0.184624f, 1.000000f),
                textColor       = new Color(0.000000f, 1.000000f, 0.808568f, 1.000000f),
                labelMode       = iHC_LabelMode.AdditionalLabel,
                scopeMode       = iHC_ScopeMode.AffectsAllChildren,
                customLabel     = "(Not Copy)",
                iconPath        = "Assets/!!! ishiruhii/iHiearchyPro/Sprites/textarea.png",
                iconTint        = new Color(1.000000f, 1.000000f, 1.000000f, 1.000000f)
            },
            new iHC_Rule {
                objectName      = "iLanguageSystem",
                backgroundColor = new Color(0.101887f, 0.101887f, 0.101887f, 1.000000f),
                textColor       = new Color(1.000000f, 0.926526f, 0.000000f, 1.000000f),
                labelMode       = iHC_LabelMode.AdditionalLabel,
                scopeMode       = iHC_ScopeMode.AffectsObject,
                customLabel     = "Addon (Not Copy)",
                iconPath        = "Assets/!!! ishiruhii/iHiearchyPro/Sprites/globe-central-south-asia-fill.png",
                iconTint        = new Color(1.000000f, 0.359215f, 0.000000f, 1.000000f)
            },
            new iHC_Rule {
                objectName      = "iRolePickup",
                backgroundColor = new Color(0.000390f, 0.215094f, 0.000000f, 1.000000f),
                textColor       = new Color(0.000000f, 1.000000f, 0.026534f, 1.000000f),
                labelMode       = iHC_LabelMode.AdditionalLabel,
                scopeMode       = iHC_ScopeMode.AffectsObject,
                customLabel     = "Module (Testing Phase)",
                iconPath        = "Assets/!!! ishiruhii/iHiearchyPro/Sprites/exclamation-diamond-fill.png",
                iconTint        = new Color(1.000000f, 0.797041f, 0.035294f, 1.000000f)
            },
        };
        // ITOOLS_RULES_END

        static ReColor1()
        {
            EditorApplication.hierarchyWindowItemOnGUI -= OnHierarchyGUI;
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
        }

        private static void OnHierarchyGUI(int instanceID, Rect selectionRect)
        {
            var obj = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (obj == null) return;

            var activeRules = (iHC_Preview.key == nameof(ReColor1) && iHC_Preview.rules != null)
                ? iHC_Preview.rules
                : rules;

            foreach (var rule in activeRules)
            {
                if (!iHC_Painter.MatchesScope(obj, rule)) continue;
                iHC_Painter.Paint(obj, selectionRect, rule);
                break;
            }
        }
    }
}
#endif
