
using UnityEditor;
using UnityEngine;

namespace HashStudiosSwimmingSystem.Scripts
{
    [CustomEditor(typeof(U_HashStudiosSwimmingSystem_Main))]
    public class Editor_HashStudiosSwimmingSystem : Editor
    {
        public Texture2D texture;
        private U_HashStudiosSwimmingSystem_Main mainScript;

        private bool isFoldoutOpen_generalsettings;

        private bool isFoldoutOpen_customizationsettings;

        private bool isFoldoutOpen_advancedsettings;

        public void OnEnable()
        {
            mainScript = (U_HashStudiosSwimmingSystem_Main)target;
        }

        public override void OnInspectorGUI()
        {
            Color32 hashGrey = new Color32(190, 190, 190, 255);
            Color32 hashBlue = new Color32(21, 146, 163, 255);
            GUI.color = Color.white;

            GUIStyle Title = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 20,
                fontStyle = FontStyle.Bold
            };

            GUIStyle Header = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 16,
                fontStyle = FontStyle.Bold
            };

            GUIStyle Header_It = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 16,
                fontStyle = FontStyle.Italic
            };

            GUIStyle LeftHeader = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleLeft,
                fontSize = 14,
                fontStyle = FontStyle.Bold
            };

            GUIStyle BoldText = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleLeft,
                fontSize = 11,
                fontStyle = FontStyle.Bold
            };
            BoldText.normal.textColor = hashBlue;

            GUIStyle Text = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleLeft,
                fontSize = 11,
                fontStyle = FontStyle.Normal
            };

            GUIStyle SmallText = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleLeft,
                fontSize = 10,
                fontStyle = FontStyle.Normal
            };
            SmallText.normal.textColor = Color.gray;

            GUIStyle SmallRedText = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleLeft,
                fontSize = 10,
                fontStyle = FontStyle.Normal
            };
            SmallRedText.normal.textColor = new Color32(200, 120, 120, 255);

            GUIStyle SmallTextCenter = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 12,
                fontStyle = FontStyle.Normal
            };
            SmallTextCenter.normal.textColor = hashGrey;

            EditorGUI.DrawPreviewTexture(new Rect(2, 2, EditorGUIUtility.currentViewWidth, EditorGUIUtility.currentViewWidth / 5), texture);
            GUILayout.Label("", GUILayout.Height(EditorGUIUtility.currentViewWidth / 5), GUILayout.Width(EditorGUIUtility.currentViewWidth));
            GUILayout.Label("", GUILayout.Height(15), GUILayout.Width(EditorGUIUtility.currentViewWidth));

            GUILayout.Label("Hash Studios Swimming System", Header, GUILayout.Width(EditorGUIUtility.currentViewWidth));
            GUILayout.Label("", GUILayout.Height(5), GUILayout.Width(EditorGUIUtility.currentViewWidth));

            GUILayout.Label("An advanced and customizable Udon", SmallTextCenter, GUILayout.Width(EditorGUIUtility.currentViewWidth));

            GUILayout.Label("swimming prefab for Unity", SmallTextCenter, GUILayout.Width(EditorGUIUtility.currentViewWidth));

            GUILayout.Label("", GUILayout.Height(20), GUILayout.Width(EditorGUIUtility.currentViewWidth));

            GUILayout.Label("高度でカスタマイズ可能な", SmallTextCenter, GUILayout.Width(EditorGUIUtility.currentViewWidth));

            GUILayout.Label("Unity向けのUdon泳法プレハブ", SmallTextCenter, GUILayout.Width(EditorGUIUtility.currentViewWidth));

            GUILayout.Label("", GUILayout.Height(20), GUILayout.Width(EditorGUIUtility.currentViewWidth));

            Rect rect10 = EditorGUILayout.GetControlRect(false, 1);
            rect10.height = 1;
            EditorGUI.DrawRect(rect10, new Color(0.5f, 0.5f, 0.5f, 1));
            GUILayout.Label("", GUILayout.Height(20), GUILayout.Width(EditorGUIUtility.currentViewWidth));

            GUILayout.Label("Need Help?", LeftHeader, GUILayout.Width(350));
            GUILayout.Label("View our documentation!", SmallText, GUILayout.Width(350));

            if (GUILayout.Button("View Documentation", GUILayout.Width(265)))
            {
                Application.OpenURL("https://hash-studios-llc.github.io/DevelopmentDocumentation/");
            }

            GUILayout.Label("", GUILayout.Height(10), GUILayout.Width(EditorGUIUtility.currentViewWidth));

            if (GUILayout.Button("Join Our Discord", GUILayout.Width(265)))
            {
                Application.OpenURL("https://discord.gg/78EnuECcY4");
            }

            GUILayout.Label("", GUILayout.Height(10), GUILayout.Width(EditorGUIUtility.currentViewWidth));

            if (GUILayout.Button("Join Our Patreon", GUILayout.Width(265)))
            {
                Application.OpenURL("https://www.patreon.com/HashStudiosLLC");
            }

            GUILayout.Label("", GUILayout.Height(20), GUILayout.Width(EditorGUIUtility.currentViewWidth));

            Rect rect11 = EditorGUILayout.GetControlRect(false, 1);
            rect11.height = 1;
            EditorGUI.DrawRect(rect11, new Color(0.5f, 0.5f, 0.5f, 1));
            GUILayout.Label("", GUILayout.Height(20), GUILayout.Width(EditorGUIUtility.currentViewWidth));

            GUILayout.Label("General Settings / 一般設定", LeftHeader, GUILayout.Width(350));

            GUILayout.Label("General configurations that control the overall behavior of the swimming system.", SmallText, GUILayout.Width(350));

            GUILayout.Label("水泳システム全体の動作を制御する一般設定です。", SmallText, GUILayout.Width(350));

            string foldoutText_generalsettings = isFoldoutOpen_generalsettings ? "Hide List / リストを隠す" : "Show List / リストを表示";

            if (GUILayout.Button(foldoutText_generalsettings, GUILayout.Width(265)))
            {
                isFoldoutOpen_generalsettings = !isFoldoutOpen_generalsettings;
            }

            if (isFoldoutOpen_generalsettings)
            {
                GUILayout.Label("", GUILayout.Height(20), GUILayout.Width(EditorGUIUtility.currentViewWidth));

                GUILayout.Label("Underwater Walk Speed / 水中歩行速度", BoldText);

                GUILayout.Label("Controls how fast the player can walk while underwater.", SmallText);

                GUILayout.Label("水中でのプレイヤーの歩行速度を制御します。", SmallText);

                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("underWaterWalkSpeed", GUILayout.Width(265));

                mainScript.underWaterWalkSpeed = EditorGUILayout.FloatField(mainScript.underWaterWalkSpeed, GUILayout.Width(200));

                GUILayout.EndHorizontal();
                GUILayout.Label("", GUILayout.Height(20), GUILayout.Width(EditorGUIUtility.currentViewWidth));

                GUILayout.Label("Underwater Run Speed / 水中走行速度", BoldText);

                GUILayout.Label("Adjusts the speed at which the player can run underwater.", SmallText);

                GUILayout.Label("水中でのプレイヤーの走行速度を調整します。", SmallText);

                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("underWaterRunSpeed", GUILayout.Width(265));

                mainScript.underWaterRunSpeed = EditorGUILayout.FloatField(mainScript.underWaterRunSpeed, GUILayout.Width(200));

                GUILayout.EndHorizontal();
                GUILayout.Label("", GUILayout.Height(20), GUILayout.Width(EditorGUIUtility.currentViewWidth));

                GUILayout.Label("Underwater Strafe Speed / 水中横移動速度", BoldText);

                GUILayout.Label("Sets the speed for moving sideways underwater.", SmallText);

                GUILayout.Label("水中での横方向への移動速度を設定します。", SmallText);

                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("underWaterStrafeSpeed", GUILayout.Width(265));

                mainScript.underWaterStrafeSpeed = EditorGUILayout.FloatField(mainScript.underWaterStrafeSpeed, GUILayout.Width(200));

                GUILayout.EndHorizontal();
                GUILayout.Label("", GUILayout.Height(20), GUILayout.Width(EditorGUIUtility.currentViewWidth));

                GUILayout.Label("Underwater Jump Impulse / 水中ジャンプの衝動", BoldText);

                GUILayout.Label("Controls the upward force applied when the player jumps underwater.", SmallText);

                GUILayout.Label("水中でジャンプする際の上昇力を制御します。", SmallText);

                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("underWaterJumpImpulse", GUILayout.Width(265));

                mainScript.underWaterJumpImpulse = EditorGUILayout.FloatField(mainScript.underWaterJumpImpulse, GUILayout.Width(200));

                GUILayout.EndHorizontal();
                GUILayout.Label("", GUILayout.Height(20), GUILayout.Width(EditorGUIUtility.currentViewWidth));

                GUILayout.Label("Underwater Gravity Strength / 水中の重力強度", BoldText);

                GUILayout.Label("Adjusts the gravitational pull on the player while underwater.", SmallText);

                GUILayout.Label("水中でのプレイヤーへの重力の強さを調整します。", SmallText);

                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("underWaterGravity", GUILayout.Width(265));

                mainScript.underWaterGravity = EditorGUILayout.FloatField(mainScript.underWaterGravity, GUILayout.Width(200));

                GUILayout.EndHorizontal();
                GUILayout.Label("", GUILayout.Height(20), GUILayout.Width(EditorGUIUtility.currentViewWidth));

                GUILayout.Label("Swimming Speed Modifier / 泳ぎ速度の修正値", BoldText);

                GUILayout.Label("Modifies the player's swimming speed for smoother movement.", SmallText);

                GUILayout.Label("プレイヤーのスムーズな動きのために泳ぐ速度を修正します。", SmallText);

                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("swimmingVelocityMod", GUILayout.Width(265));

                mainScript.swimmingVelocityMod = EditorGUILayout.FloatField(mainScript.swimmingVelocityMod, GUILayout.Width(200));

                GUILayout.EndHorizontal();
                GUILayout.Label("", GUILayout.Height(20), GUILayout.Width(EditorGUIUtility.currentViewWidth));

                GUILayout.Label("Vertical Swimming Speed Modifier / 垂直泳ぎ速度の修正値", BoldText);

                GUILayout.Label("Adjusts how fast the player can swim upwards or downwards.", SmallText);

                GUILayout.Label("上下方向への泳ぎ速度を調整します。", SmallText);

                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("swimmingVelocityVerticalMod", GUILayout.Width(265));

                mainScript.swimmingVelocityVerticalMod = EditorGUILayout.FloatField(mainScript.swimmingVelocityVerticalMod, GUILayout.Width(200));

                GUILayout.EndHorizontal();
                GUILayout.Label("", GUILayout.Height(20), GUILayout.Width(EditorGUIUtility.currentViewWidth));

                GUILayout.Label("Water Jump Modifier / 水ジャンプの修正値", BoldText);

                GUILayout.Label("Enhances the player's ability to jump out of the water.", SmallText);

                GUILayout.Label("プレイヤーの水中からのジャンプ力を強化します。", SmallText);

                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("waterJumpMod", GUILayout.Width(265));

                mainScript.waterJumpMod = EditorGUILayout.FloatField(mainScript.waterJumpMod, GUILayout.Width(200));

                GUILayout.EndHorizontal();
                GUILayout.Label("", GUILayout.Height(20), GUILayout.Width(EditorGUIUtility.currentViewWidth));

            }
            GUILayout.Label("", GUILayout.Height(20), GUILayout.Width(EditorGUIUtility.currentViewWidth));
            Rect rect13 = EditorGUILayout.GetControlRect(false, 1);
            rect13.height = 1;
            EditorGUI.DrawRect(rect13, new Color(0.5f, 0.5f, 0.5f, 1));
            GUILayout.Label("", GUILayout.Height(20), GUILayout.Width(EditorGUIUtility.currentViewWidth));

            GUILayout.Label("Customization Settings / カスタマイズ設定", LeftHeader, GUILayout.Width(350));

            GUILayout.Label("Customization options to modify the appearance and feel of underwater effects.", SmallText, GUILayout.Width(350));

            GUILayout.Label("水中の効果の見た目や感覚を調整するためのカスタマイズオプションです。", SmallText, GUILayout.Width(350));

            string foldoutText_customizationsettings = isFoldoutOpen_customizationsettings ? "Hide List / リストを隠す" : "Show List / リストを表示";

            if (GUILayout.Button(foldoutText_customizationsettings, GUILayout.Width(265)))
            {
                isFoldoutOpen_customizationsettings = !isFoldoutOpen_customizationsettings;
            }

            if (isFoldoutOpen_customizationsettings)
            {
                GUILayout.Label("", GUILayout.Height(20), GUILayout.Width(EditorGUIUtility.currentViewWidth));

                GUILayout.Label("Underwater Visual Effect / 水中ビジュアルエフェクト", BoldText);

                GUILayout.Label("Defines the material or shader used to display underwater visuals.", SmallText);

                GUILayout.Label("水中の視覚効果として使用されるマテリアルまたはシェーダーを定義します。", SmallText);

                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("underwaterVisualEffect", GUILayout.Width(265));

                // Handle custom type
                mainScript.underwaterVisualEffect = (Material)EditorGUILayout.ObjectField(mainScript.underwaterVisualEffect, typeof(Material), false);

                GUILayout.EndHorizontal();
                GUILayout.Label("", GUILayout.Height(20), GUILayout.Width(EditorGUIUtility.currentViewWidth));

                GUILayout.Label("Water Entry Sound / 水の入水音", BoldText);

                GUILayout.Label("Plays a sound effect when the player enters the water.", SmallText);

                GUILayout.Label("プレイヤーが水に入る際に再生される音声効果です。", SmallText);

                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("waterEnterAudio", GUILayout.Width(265));

                // Handle custom type
                if (mainScript.waterEnterAudio != null)
                {
                    mainScript.waterEnterAudio.clip =
                        (AudioClip)EditorGUILayout.ObjectField(mainScript.waterEnterAudio.clip, typeof(AudioClip), false);
                }

                GUILayout.EndHorizontal();
                GUILayout.Label("", GUILayout.Height(20), GUILayout.Width(EditorGUIUtility.currentViewWidth));

                GUILayout.Label("Underwater Ambience Sound / 水中の環境音", BoldText);

                GUILayout.Label("Provides ambient sound while the player is submerged.", SmallText);

                GUILayout.Label("プレイヤーが水中にいる間の環境音を提供します。", SmallText);

                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("underWaterAudio", GUILayout.Width(265));

                // Handle custom type
                if (mainScript.underWaterAudio != null)
                {
                    mainScript.underWaterAudio.clip =
                        (AudioClip)EditorGUILayout.ObjectField(mainScript.underWaterAudio.clip, typeof(AudioClip), false);
                }

                GUILayout.EndHorizontal();
                GUILayout.Label("", GUILayout.Height(20), GUILayout.Width(EditorGUIUtility.currentViewWidth));

            }
            GUILayout.Label("", GUILayout.Height(20), GUILayout.Width(EditorGUIUtility.currentViewWidth));
            Rect rect14 = EditorGUILayout.GetControlRect(false, 1);
            rect14.height = 1;
            EditorGUI.DrawRect(rect14, new Color(0.5f, 0.5f, 0.5f, 1));
            GUILayout.Label("", GUILayout.Height(20), GUILayout.Width(EditorGUIUtility.currentViewWidth));

            GUILayout.Label("Advanced Settings / 高度な設定", LeftHeader, GUILayout.Width(350));

            GUILayout.Label("Advanced parameters that offer detailed control over the system’s behavior.", SmallText, GUILayout.Width(350));

            GUILayout.Label("システムの動作を詳細に制御するための高度なパラメーターです。", SmallText, GUILayout.Width(350));

            string foldoutText_advancedsettings = isFoldoutOpen_advancedsettings ? "Hide List / リストを隠す" : "Show List / リストを表示";

            if (GUILayout.Button(foldoutText_advancedsettings, GUILayout.Width(265)))
            {
                isFoldoutOpen_advancedsettings = !isFoldoutOpen_advancedsettings;
            }

            if (isFoldoutOpen_advancedsettings)
            {
                GUILayout.Label("", GUILayout.Height(20), GUILayout.Width(EditorGUIUtility.currentViewWidth));

                GUILayout.Label("Debug Mode Toggle / デバッグモードの切り替え", BoldText);

                GUILayout.Label("Allows real-time testing and adjustment of swimming parameters. Without this enabled,", SmallText);
                GUILayout.Label("you won’t see player or prefab values update in real-time when testing in Unity.", SmallText);
                GUILayout.Label("Use with caution, as it may cause unexpected issues.", SmallText);

                GUILayout.Label("水泳のパラメーターをリアルタイムでテストおよび調整できます。有効にしないと、Unityで", SmallText);
                GUILayout.Label("テスト中にプレイヤーやプレハブの値がリアルタイムで更新されません。注意して使用してくだ", SmallText);
                GUILayout.Label("さい。不具合が発生する可能性があります。", SmallText);

                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("debugMode", GUILayout.Width(265));

                mainScript.debugMode = EditorGUILayout.Toggle(mainScript.debugMode, GUILayout.Width(200));

                GUILayout.EndHorizontal();
                GUILayout.Label("", GUILayout.Height(20), GUILayout.Width(EditorGUIUtility.currentViewWidth));

            }
            GUILayout.Label("", GUILayout.Height(20), GUILayout.Width(EditorGUIUtility.currentViewWidth));
            Rect rect15 = EditorGUILayout.GetControlRect(false, 1);
            rect15.height = 1;
            EditorGUI.DrawRect(rect15, new Color(0.5f, 0.5f, 0.5f, 1));
            GUILayout.Label("", GUILayout.Height(20), GUILayout.Width(EditorGUIUtility.currentViewWidth));

        }
    }
}
