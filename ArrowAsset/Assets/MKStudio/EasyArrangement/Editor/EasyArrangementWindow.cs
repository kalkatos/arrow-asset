using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MKStudio.EditorLocalization;
using UnityEditor;
using UnityEngine;

namespace MKStudio.EasyArrangement
{
    public class EasyArrangementWindow : EditorWindow
    {
        private const string PrefKeyLastLanguageId = "MK_EA_LastLanguage";
        private const string PrefKeyShowRotationalGuides = "MK_EA_SRG";
        
        private readonly GUILayoutOption mainIconWidth = GUILayout.Width(30f);
        private readonly GUILayoutOption mainIconHeight = GUILayout.Height(30f);
        private readonly GUILayoutOption runIconHeight = GUILayout.Height(25f);
        
        private static Texture _logo;
        private static Texture _iconAlignXMin;
        private static Texture _iconAlignXMax;
        private static Texture _iconAlignXAvg;     
        private static Texture _iconAlignYMin;
        private static Texture _iconAlignYMax;
        private static Texture _iconAlignYAvg;
        private static Texture _iconAlignZMin;
        private static Texture _iconAlignZMax;
        private static Texture _iconAlignZAvg;
        private static Texture _iconDistributeX;
        private static Texture _iconDistributeY;
        private static Texture _iconDistributeZ;
        private static Texture _iconDistributeLine;
        private static Texture _iconProgressivePlacement;
        private static Texture _iconProgressiveRotation;
        private static Texture _iconRotationalArrangement;
        private static Texture _iconSettings;
        private static Texture _iconToggleRaGuides;
        private static MKLanguageSource _language;

        private static GUIContent _cBtnSettings;
        private static GUIContent _cLabelSelectedObject;
        private static GUIContent _cLabelDistributionHeader;
        private static GUIContent _cBtnDistX;
        private static GUIContent _cBtnDistY;
        private static GUIContent _cBtnDistZ;
        private static GUIContent _cBtnDistLine;
        private static GUIContent _cLabelAlignmentHeader;
        private static GUIContent _cBtnAlignXMin;
        private static GUIContent _cBtnAlignXMax;
        private static GUIContent _cBtnAlignXAvg;
        private static GUIContent _cBtnAlignYMin;
        private static GUIContent _cBtnAlignYMax;
        private static GUIContent _cBtnAlignYAvg;
        private static GUIContent _cBtnAlignZMin;
        private static GUIContent _cBtnAlignZMax;
        private static GUIContent _cBtnAlignZAvg;
        private static GUIContent _cLabelProgressivePlacementHeader;
        private static GUIContent _cLabelPpDelta;
        private static GUIContent _cDropdownPpActionOrder;
        private static GUIContent _cLabelSelectTwoObjects;
        private static GUIContent _cBtnSetToDelta;
        private static GUIContent _cLabelProgressiveRotationHeader;
        private static GUIContent _cLabelPrDelta;
        private static GUIContent _cLabelRotationalArrangementHeader;
        private static GUIContent _cVectorFieldRaRotationCenter;
        private static GUIContent _cSliderRaNormalAngle;
        private static GUIContent _cSliderRaTotalAngle;
        private static GUIContent _cToggleRaPreservePosition;
        private static GUIContent _cToggleRaApplyLocalRotation;
        private static GUIContent _cToggleRaInvert;
        private static GUIContent _cBtnRunPp;
        private static GUIContent _cBtnRunPr;
        private static GUIContent _cBtnRunRa;
        private static GUIContent _cBtnToggleRaGuides;
        private static GUIContent _cToggleRaAllowOver360;

        private static Material _logoMaterial;


        private Transform firstSelect;

        [SerializeField] private float deltaPositionX;
        [SerializeField] private float deltaPositionY;
        [SerializeField] private float deltaPositionZ;
        [SerializeField] bool enableDeltaPositionX = true;
        [SerializeField] bool enableDeltaPositionY = true;
        [SerializeField] bool enableDeltaPositionZ = true;
        [SerializeField] private ProgressivePlacementActionOrder progressivePlacementOrder = ProgressivePlacementActionOrder.Hierarchy;

        [SerializeField] private float deltaRotationX;
        [SerializeField] private float deltaRotationY;
        [SerializeField] private float deltaRotationZ;
        [SerializeField] bool enableDeltaRotationX = true;
        [SerializeField] bool enableDeltaRotationY = true;
        [SerializeField] bool enableDeltaRotationZ = true;
        [SerializeField] private Vector3 rotationCenter = Vector3.zero;

        [SerializeField] private float normalAngle = 0f;
        [SerializeField] private float rotateAroundTotalAngle = 360f;
        [SerializeField] private bool preservePositionAlongAxis = false;
        [SerializeField] private bool applyLocalRotation = true;
        [SerializeField] private bool invertRotationalArrangement = false;

        private static bool showRotationalArrangementGuides = true;

        private GUIStyle sceneLabelStyle;

        private List<Transform> _selectedTransforms;
        private Transform _transformA;
        private Transform _transformB;
        
        [MenuItem("GameObject/Easy Arrangement... %#g")]
        public static void Init()
        {
            EasyArrangementWindow window = (EasyArrangementWindow) EditorWindow.GetWindow(typeof(EasyArrangementWindow), true);
            window.maxSize = window.minSize = new Vector2(308, 630);

            window.titleContent = new GUIContent("Easy Arrangement");
            window.Show();
        }

        static void SetLanguage(int languageId)
        {
            _language.CurrentLanguageId = languageId;

            _cBtnSettings = new GUIContent(_iconSettings, _language.GetTranslation("Global_BtnSettings_Tip"));
            _cLabelSelectedObject = new GUIContent(_language.GetTranslation("Global_SelectedObject_Lb"));
            _cLabelDistributionHeader = new GUIContent(_language.GetTranslation("DIST_Header_Lb"));
            _cBtnDistX = new GUIContent(_iconDistributeX, _language.GetTranslation("DIST_X_Tip"));
            _cBtnDistY = new GUIContent(_iconDistributeY, _language.GetTranslation("DIST_Y_Tip"));
            _cBtnDistZ = new GUIContent(_iconDistributeZ, _language.GetTranslation("DIST_Z_Tip"));
            _cBtnDistLine = new GUIContent(_iconDistributeLine, _language.GetTranslation("DIST_Line_Tip"));
            _cLabelAlignmentHeader = new GUIContent(_language.GetTranslation("AL_Header_Lb"));
            _cBtnAlignXMin = new GUIContent(_iconAlignXMin, _language.GetTranslation("AL_XMin_Tip"));
            _cBtnAlignXMax = new GUIContent(_iconAlignXMax, _language.GetTranslation("AL_XMax_Tip"));
            _cBtnAlignXAvg = new GUIContent(_iconAlignXAvg, _language.GetTranslation("AL_XAvg_Tip"));
            _cBtnAlignYMin = new GUIContent(_iconAlignYMin, _language.GetTranslation("AL_YMin_Tip"));
            _cBtnAlignYMax = new GUIContent(_iconAlignYMax, _language.GetTranslation("AL_YMax_Tip"));
            _cBtnAlignYAvg = new GUIContent(_iconAlignYAvg, _language.GetTranslation("AL_YAvg_Tip"));
            _cBtnAlignZMin = new GUIContent(_iconAlignZMin, _language.GetTranslation("AL_ZMin_Tip"));
            _cBtnAlignZMax = new GUIContent(_iconAlignZMax, _language.GetTranslation("AL_ZMax_Tip"));
            _cBtnAlignZAvg = new GUIContent(_iconAlignZAvg, _language.GetTranslation("AL_ZAvg_Tip"));
            _cLabelProgressivePlacementHeader = new GUIContent(_language.GetTranslation("PP_Header_Lb"),
                _language.GetTranslation("PP_Header_Tip"));
            _cLabelPpDelta = new GUIContent(_language.GetTranslation("PP_Delta_Lb"));
            _cLabelSelectTwoObjects = new GUIContent(_language.GetTranslation("PP_SelectTwoObj_Lb"));
            _cBtnSetToDelta = new GUIContent(_language.GetTranslation("PP_WriteToDelta_Lb"), _language.GetTranslation("PP_WriteToDelta_Tip"));
            _cDropdownPpActionOrder = new GUIContent(_language.GetTranslation("PP_ActionOrder_Lb"),
                _language.GetTranslation("PP_ActionOrder_Tip"));
            _cLabelProgressiveRotationHeader = new GUIContent(_language.GetTranslation("PR_Header_Lb"),
                _language.GetTranslation("PR_Header_Tip"));
            _cLabelPrDelta = new GUIContent(_language.GetTranslation("PR_Delta_Lb"));
            _cLabelRotationalArrangementHeader = new GUIContent(_language.GetTranslation("RA_Header_Lb"),
                _language.GetTranslation("RA_Header_Tip"));
            _cVectorFieldRaRotationCenter = new GUIContent(_language.GetTranslation("RA_RotationCenter_Lb"),
                _language.GetTranslation("RA_RotationCenter_Tip"));
            _cSliderRaNormalAngle = new GUIContent(_language.GetTranslation("RA_NormalAngle_Lb"),
                _language.GetTranslation("RA_NormalAngle_Tip"));
            _cSliderRaTotalAngle = new GUIContent(_language.GetTranslation("RA_TotalAngle_Lb"),
                _language.GetTranslation("RA_TotalAngle_Tip"));
            _cToggleRaPreservePosition = new GUIContent(_language.GetTranslation("RA_PreservePositionAlongAxis_Lb"),
                _language.GetTranslation("RA_PreservePositionAlongAxis_Tip"));
            _cToggleRaApplyLocalRotation = new GUIContent(_language.GetTranslation("RA_ApplyLocalRotation_Lb"),
                _language.GetTranslation("RA_ApplyLocalRotation_Tip"));
            _cToggleRaInvert = new GUIContent(_language.GetTranslation("RA_Invert_Lb"));
            
            _cBtnRunPp = new GUIContent(_language.GetTranslation("Global_Btn_Execute"), _iconProgressivePlacement);
            _cBtnRunPr = new GUIContent(_language.GetTranslation("Global_Btn_Execute"), _iconProgressiveRotation);
            _cBtnRunRa = new GUIContent(_language.GetTranslation("Global_Btn_Execute"), _iconRotationalArrangement);
            _cBtnToggleRaGuides = new GUIContent(_iconToggleRaGuides, _language.GetTranslation("RA_ToggleGuides"));
            _cToggleRaAllowOver360 = new GUIContent(_language.GetTranslation("RA_AllowOver360_Lb"));

            EditorPrefs.SetInt(PrefKeyLastLanguageId, languageId);
        }
        
        void SetFirstSelectObject()
        {
            if (Selection.transforms.Length == 1)
            {
                firstSelect = Selection.transforms[0];
            }
            else if (Selection.transforms.Length == 0)
            {
                firstSelect = null;
            }
            else
            {
                if (firstSelect == null)
                {
                    firstSelect = Selection.transforms.Aggregate((t1, t2) => t1.GetSiblingIndex() > t2.GetSiblingIndex() ? t1 : t2);
                }
            }
        }

        private void OnSelectionChange()
        {
            _selectedTransforms.Clear();
            for (int i = 0; i < Selection.transforms.Length; i++)
            {
                _selectedTransforms.Add(Selection.transforms[i]);
            }

            _selectedTransforms.Sort((x, y) => x.GetSiblingIndex().CompareTo(y.GetSiblingIndex()));

            if (_selectedTransforms.Count >= 2)
            {
                _transformA = _selectedTransforms[0];
                _transformB = _selectedTransforms[1];
            }
            else if (_selectedTransforms.Count == 1)
            {
                _transformA = _selectedTransforms[0];
                _transformB = null;
            }
            else
            {
                _transformA = null;
                _transformB = null;
            }

            RepaintWindow();
        }

        private void RepaintWindow()
        {
            Repaint();
        }
  
        private void OnEnable()
        {
            Undo.undoRedoPerformed -= RepaintWindow;
            Undo.undoRedoPerformed += RepaintWindow;
            
#if UNITY_2019_2_OR_NEWER
            SceneView.duringSceneGui -= OnSceneGUI;
            SceneView.duringSceneGui += OnSceneGUI;
#else
            SceneView.onSceneGUIDelegate -= OnSceneGUI;
            SceneView.onSceneGUIDelegate += OnSceneGUI;
#endif
            string self = AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("EasyArrangement t:script")[0]);
            string path = Path.GetDirectoryName(Path.GetDirectoryName(self)) + "/EditorResources/";

            _logoMaterial = AssetDatabase.LoadAssetAtPath<Material>(path + "LogoMat.mat");


            if (EditorGUIUtility.isProSkin)
            {
                _logo = AssetDatabase.LoadAssetAtPath<Texture>(path + "LogoLight.png");

                _iconAlignXMin = AssetDatabase.LoadAssetAtPath<Texture>(path + "AlignXMin.png");
                _iconAlignXMax = AssetDatabase.LoadAssetAtPath<Texture>(path + "AlignXMax.png");
                _iconAlignXAvg = AssetDatabase.LoadAssetAtPath<Texture>(path + "AlignXAvg.png");
                _iconAlignYMin = AssetDatabase.LoadAssetAtPath<Texture>(path + "AlignYMin.png");
                _iconAlignYMax = AssetDatabase.LoadAssetAtPath<Texture>(path + "AlignYMax.png");
                _iconAlignYAvg = AssetDatabase.LoadAssetAtPath<Texture>(path + "AlignYAvg.png");
                _iconAlignZMin = AssetDatabase.LoadAssetAtPath<Texture>(path + "AlignZMin.png");
                _iconAlignZMax = AssetDatabase.LoadAssetAtPath<Texture>(path + "AlignZMax.png");
                _iconAlignZAvg = AssetDatabase.LoadAssetAtPath<Texture>(path + "AlignZAvg.png");
                _iconDistributeX = AssetDatabase.LoadAssetAtPath<Texture>(path + "DisX.png");
                _iconDistributeY = AssetDatabase.LoadAssetAtPath<Texture>(path + "DisY.png");
                _iconDistributeZ = AssetDatabase.LoadAssetAtPath<Texture>(path + "DisZ.png");
                _iconDistributeLine = AssetDatabase.LoadAssetAtPath<Texture>(path + "DisLine.png");
                _iconProgressivePlacement = AssetDatabase.LoadAssetAtPath<Texture>(path + "ProgPlacement.png");
                _iconProgressiveRotation = AssetDatabase.LoadAssetAtPath<Texture>(path + "ProgRotation.png");
                _iconRotationalArrangement = AssetDatabase.LoadAssetAtPath<Texture>(path + "RotArrangement.png");
                _iconSettings = AssetDatabase.LoadAssetAtPath<Texture>(path + "IconSettings.png");
                _iconToggleRaGuides = AssetDatabase.LoadAssetAtPath<Texture>(path + "Eye.png");
                _language = AssetDatabase.LoadAssetAtPath<MKLanguageSource>(path + "Languages.asset");
            }
            else
            {
                _logo = AssetDatabase.LoadAssetAtPath<Texture>(path + "LogoDark.png");

                _iconAlignXMin = AssetDatabase.LoadAssetAtPath<Texture>(path + "AlignXMinDark.png");
                _iconAlignXMax = AssetDatabase.LoadAssetAtPath<Texture>(path + "AlignXMaxDark.png");
                _iconAlignXAvg = AssetDatabase.LoadAssetAtPath<Texture>(path + "AlignXAvgDark.png");
                _iconAlignYMin = AssetDatabase.LoadAssetAtPath<Texture>(path + "AlignYMinDark.png");
                _iconAlignYMax = AssetDatabase.LoadAssetAtPath<Texture>(path + "AlignYMaxDark.png");
                _iconAlignYAvg = AssetDatabase.LoadAssetAtPath<Texture>(path + "AlignYAvgDark.png");
                _iconAlignZMin = AssetDatabase.LoadAssetAtPath<Texture>(path + "AlignZMinDark.png");
                _iconAlignZMax = AssetDatabase.LoadAssetAtPath<Texture>(path + "AlignZMaxDark.png");
                _iconAlignZAvg = AssetDatabase.LoadAssetAtPath<Texture>(path + "AlignZAvgDark.png");
                _iconDistributeX = AssetDatabase.LoadAssetAtPath<Texture>(path + "DisXDark.png");
                _iconDistributeY = AssetDatabase.LoadAssetAtPath<Texture>(path + "DisYDark.png");
                _iconDistributeZ = AssetDatabase.LoadAssetAtPath<Texture>(path + "DisZDark.png");
                _iconDistributeLine = AssetDatabase.LoadAssetAtPath<Texture>(path + "DisLineDark.png");
                _iconProgressivePlacement = AssetDatabase.LoadAssetAtPath<Texture>(path + "ProgPlacementDark.png");
                _iconProgressiveRotation = AssetDatabase.LoadAssetAtPath<Texture>(path + "ProgRotationDark.png");
                _iconRotationalArrangement = AssetDatabase.LoadAssetAtPath<Texture>(path + "RotArrangementDark.png");
                _iconToggleRaGuides = AssetDatabase.LoadAssetAtPath<Texture>(path + "EyeDark.png");
                _iconSettings = AssetDatabase.LoadAssetAtPath<Texture>(path + "IconSettingsDark.png");
            }

            _language = AssetDatabase.LoadAssetAtPath<MKLanguageSource>(path + "Languages.asset");


            _language.Initialize();

            if (EditorPrefs.HasKey(PrefKeyLastLanguageId))
            {
                SetLanguage(EditorPrefs.GetInt(PrefKeyLastLanguageId));
            }
            else
            {
                SetLanguage(0);
            }
            
            if (EditorPrefs.HasKey(PrefKeyShowRotationalGuides))
            {
                showRotationalArrangementGuides = EditorPrefs.GetBool(PrefKeyShowRotationalGuides);
            }
            else
            {
                showRotationalArrangementGuides = true;
                EditorPrefs.SetBool(PrefKeyShowRotationalGuides, true);
            }
            
            _selectedTransforms = new List<Transform>();
        }
        
        void OnDisable()
        {
#if UNITY_2019_2_OR_NEWER
            SceneView.duringSceneGui -= OnSceneGUI;
#else
            SceneView.onSceneGUIDelegate -= OnSceneGUI;
#endif
            Undo.undoRedoPerformed -= RepaintWindow;
        }

        private void OnGUI()
        {
            Undo.RecordObject(this, "Easy Arrangement");

            SetFirstSelectObject();
            
            float headerHeight = 20f;
            if (_logo)
            {
                EditorGUI.DrawPreviewTexture(new Rect(0f, 0f, _logo.width, _logo.height), _logo, _logoMaterial);
                headerHeight = _logo.height;
            }
            else
            {
                EditorGUI.LabelField(new Rect(2f, 2f, EditorGUIUtility.currentViewWidth, 20f),  "Logo is missing.");
            }

            if (GUI.Button(new Rect(EditorGUIUtility.currentViewWidth - 28f, 3f, 25f, 25f), _cBtnSettings, EditorStyles.miniButton))
            {
                GenericMenu menu = new GenericMenu();
                
                menu.AddItem(new GUIContent("English"), _language.CurrentLanguageId == 0, () => { SetLanguage(0); });
                menu.AddItem(new GUIContent("简体中文"), _language.CurrentLanguageId == 1, () => { SetLanguage(1); });
                
                menu.AddSeparator("");
                
                menu.AddDisabledItem(new GUIContent("Easy Arrangement v1.1"));
                menu.AddItem(new GUIContent(_language.GetTranslation("Global_MenuItem_Support")), false, () =>
                {
                    Application.OpenURL("mailto:lx84@outlook.com");
                });
                
                menu.AddItem(new GUIContent(_language.GetTranslation("Global_MenuItem_MoreAssets")), false, () =>
                {
                    Application.OpenURL("https://assetstore.unity.com/publishers/34940");
                });

                menu.ShowAsContext();
            }

            EditorGUILayout.LabelField("", GUILayout.Height(headerHeight));
            
            DrawTransformManipulationTab();
            
            EditorGUILayout.LabelField(_cLabelSelectedObject.text + ": " + Selection.transforms.Length, EditorStyles.centeredGreyMiniLabel);
        }
        
        void OnSceneGUI(SceneView sceneView)
        {
            if (SceneView.lastActiveSceneView != null)
            {
                if (Event.current.type == EventType.Repaint)
                {
                    SetFirstSelectObject();

                    if (Selection.transforms.Length > 0 && showRotationalArrangementGuides)
                    {
                        float radius = (firstSelect.transform.position - rotationCenter).magnitude;
                        //rotate arrange
                        if (radius > 0)
                        {
                            Handles.color = Color.yellow;
                            var r = firstSelect.position - rotationCenter;
                            var cross = Vector3.Cross(Vector3.down, r) == Vector3.zero ? Vector3.Cross(Vector3.right, r) : Vector3.Cross(Vector3.down, r);
                            var normal = Quaternion.AngleAxis(normalAngle, r) * cross;
                            Handles.ArrowHandleCap(0, rotationCenter, Quaternion.LookRotation(normal), 2f, EventType.Repaint);
                            DrawColoredLabelHandle(rotationCenter,
                                rotateAroundTotalAngle != 360f ? _language.GetTranslation("SceneView_RA_CenterArc") + rotateAroundTotalAngle.ToString() :
                                    _language.GetTranslation("SceneView_RA_CenterFull") + rotateAroundTotalAngle.ToString(), Color.white);
                            Handles.color = Color.white;
                            Handles.DrawDottedLine(rotationCenter, firstSelect.position, 5f);
                            DrawColoredLabelHandle(0.5f * (firstSelect.transform.position + rotationCenter), "R = " + radius.ToString(), Color.white);
                            Handles.color = rotateAroundTotalAngle < 360f
                                ? new Color(1f, .66f, .23f, .1f)
                                : (rotateAroundTotalAngle == 360f ? new Color(0.4f, 0.7f, 0.4f, 0.1f) : new Color(0.639f, 0.149f, 0.424f, 0.1f));
                            Handles.DrawSolidArc(rotationCenter, normal, r, invertRotationalArrangement ? -rotateAroundTotalAngle : rotateAroundTotalAngle,
                                r.magnitude);
                        }
                        else
                        {
                            DrawColoredLabelHandle(rotationCenter, _language.GetTranslation("SceneView_RadiusWarning"), Color.red);
                        }
                    }

                    //base 
                    if (Selection.transforms.Length > 1)
                    {
                        var firstPos = firstSelect.position;
                        DrawColoredLabelHandle(firstPos, _language.GetTranslation("SceneView_BaseObject") + firstSelect.gameObject.name, Color.green);
                    }
                }

                SceneView.lastActiveSceneView.Repaint();
            }
        }

        void DrawTransformManipulationTab()
        {
            EditorGUILayout.LabelField(_cLabelDistributionHeader);
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            //distribute x
            GUI.enabled = Selection.transforms.Length > 2;
            if (GUILayout.Button(_cBtnDistX, mainIconWidth, mainIconHeight))
            {
                EasyArrangementUtilities.DistributeX();
            }

            //distribute y
            GUI.enabled = Selection.transforms.Length > 2;
            if (GUILayout.Button(_cBtnDistY, mainIconWidth, mainIconHeight))
            {
                EasyArrangementUtilities.DistributeY();
            }

            //distribute z
            GUI.enabled = Selection.transforms.Length > 2;
            if (GUILayout.Button(_cBtnDistZ, mainIconWidth, mainIconHeight))
            {
                EasyArrangementUtilities.DistributeZ();
            }

            //distribute along the line
            GUI.enabled = Selection.transforms.Length > 2;
            if (GUILayout.Button(_cBtnDistLine, mainIconWidth, mainIconHeight))
            {
                EasyArrangementUtilities.DistributeAlongLine();
            }

            GUI.enabled = true;

            EditorGUILayout.EndHorizontal();


            EditorGUILayout.Space();
            EditorGUILayout.LabelField(_cLabelAlignmentHeader);

            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);

            //Align Y centers - highest
            GUI.enabled = Selection.transforms.Length > 1;
            if (GUILayout.Button(_cBtnAlignYMax, mainIconWidth, mainIconHeight))
            {
                EasyArrangementUtilities.AlignYCentersToTop();
            }

            //Align Y centers - lowest
            GUI.enabled = Selection.transforms.Length > 1;
            if (GUILayout.Button(_cBtnAlignYMin, mainIconWidth, mainIconHeight))
            {
                EasyArrangementUtilities.AlignYCentersToBottom();
            }

            //Align Y centers - average
            GUI.enabled = Selection.transforms.Length > 1;
            if (GUILayout.Button(_cBtnAlignYAvg, mainIconWidth, mainIconHeight))
            {
                EasyArrangementUtilities.AlignYCentersToAverage();
            }

            GUI.enabled = true;

            //Align X centers - leftmost
            GUI.enabled = Selection.transforms.Length > 1;
            if (GUILayout.Button(_cBtnAlignXMin, mainIconWidth, mainIconHeight))
            {
                EasyArrangementUtilities.AlignXCentersToLeftBound();
            }

            //Align X centers - rightmost
            GUI.enabled = Selection.transforms.Length > 1;
            if (GUILayout.Button(_cBtnAlignXMax, mainIconWidth, mainIconHeight))
            {
                EasyArrangementUtilities.AlignXCentersToRightBound();
            }

            //Align X centers - average
            GUI.enabled = Selection.transforms.Length > 1;
            if (GUILayout.Button(_cBtnAlignXAvg, mainIconWidth, mainIconHeight))
            {
                EasyArrangementUtilities.AlignXCentersToAverage();
            }

            GUI.enabled = true;

            //Align Z centers - front
            GUI.enabled = Selection.transforms.Length > 1;
            if (GUILayout.Button(_cBtnAlignZMax, mainIconWidth, mainIconHeight))
            {
                EasyArrangementUtilities.AlignZCentersToForwardBound();
            }

            //Align Z centers - back
            GUI.enabled = Selection.transforms.Length > 1;
            if (GUILayout.Button(_cBtnAlignZMin, mainIconWidth, mainIconHeight))
            {
                EasyArrangementUtilities.AlignZCentersToBackBound();
            }

            //Align Z centers - average
            GUI.enabled = Selection.transforms.Length > 1;
            if (GUILayout.Button(_cBtnAlignZAvg, mainIconWidth, mainIconHeight))
            {
                EasyArrangementUtilities.AlignZCentersToAverage();
            }

            GUI.enabled = true;

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            //Progressive placement - Position
            EditorGUILayout.LabelField(_cLabelProgressivePlacementHeader);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            GUI.enabled = Selection.transforms.Length > 1;
            EditorGUILayout.LabelField(_cLabelPpDelta);
            EditorGUILayout.BeginHorizontal();
            EditorGUIUtility.labelWidth = 10f;
            GUI.enabled = enableDeltaPositionX && Selection.transforms.Length > 1;
            deltaPositionX = EditorGUILayout.FloatField("X", deltaPositionX);
            GUI.enabled = Selection.transforms.Length > 1;
            enableDeltaPositionX = EditorGUILayout.Toggle(GUIContent.none, enableDeltaPositionX, GUILayout.Width(26f));
            GUI.enabled = enableDeltaPositionY && Selection.transforms.Length > 1;
            deltaPositionY = EditorGUILayout.FloatField("Y", deltaPositionY);
            GUI.enabled = Selection.transforms.Length > 1;
            enableDeltaPositionY = EditorGUILayout.Toggle(GUIContent.none, enableDeltaPositionY, GUILayout.Width(26f));
            GUI.enabled = enableDeltaPositionZ && Selection.transforms.Length > 1;
            deltaPositionZ = EditorGUILayout.FloatField("Z", deltaPositionZ);
            GUI.enabled = Selection.transforms.Length > 1;
            enableDeltaPositionZ = EditorGUILayout.Toggle(GUIContent.none, enableDeltaPositionZ, GUILayout.Width(26f));
            EditorGUIUtility.labelWidth = 0f;
            EditorGUILayout.EndHorizontal();

            GUI.enabled = (enableDeltaPositionX || enableDeltaPositionY || enableDeltaPositionZ) && Selection.transforms.Length > 1;

            progressivePlacementOrder = (ProgressivePlacementActionOrder) EditorGUILayout.EnumPopup(_cDropdownPpActionOrder, progressivePlacementOrder);

            EditorGUILayout.BeginHorizontal();
            if (_transformA != null && _transformB != null)
            {
                Vector3 delta = _transformB.position - _transformA.position;
                EditorGUILayout.LabelField(string.Format("dX: {0}, dY: {1}, dZ: {2}", delta.x, delta.y, delta.z), EditorStyles.miniLabel);
            }
            else
            {
                EditorGUILayout.LabelField(_cLabelSelectTwoObjects, EditorStyles.miniLabel);
            }

            if (GUILayout.Button(_cBtnSetToDelta, EditorStyles.miniButton, GUILayout.Width(90f)))
            {
                Vector3 delta = _transformB.position - _transformA.position;
                deltaPositionX = delta.x;
                deltaPositionY = delta.y;
                deltaPositionZ = delta.z;
            }

            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button(_cBtnRunPp, runIconHeight))
            {
                EasyArrangementUtilities.ProgressivePlacement(firstSelect, progressivePlacementOrder, deltaPositionX, deltaPositionY, deltaPositionZ, enableDeltaPositionX,
                    enableDeltaPositionY, enableDeltaPositionZ);
            }

            GUI.enabled = true;

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();

            //progressive rotation
            EditorGUILayout.LabelField(_cLabelProgressiveRotationHeader);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUI.enabled = Selection.transforms.Length > 1;
            EditorGUILayout.LabelField(_cLabelPrDelta);
            EditorGUILayout.BeginHorizontal();
            EditorGUIUtility.labelWidth = 10f;
            GUI.enabled = enableDeltaRotationX && Selection.transforms.Length > 1;
            deltaRotationX = EditorGUILayout.FloatField("X", deltaRotationX);
            GUI.enabled = Selection.transforms.Length > 1;
            enableDeltaRotationX = EditorGUILayout.Toggle(GUIContent.none, enableDeltaRotationX, GUILayout.Width(26f));
            GUI.enabled = enableDeltaRotationY && Selection.transforms.Length > 1;
            deltaRotationY = EditorGUILayout.FloatField("Y", deltaRotationY);
            GUI.enabled = Selection.transforms.Length > 1;
            enableDeltaRotationY = EditorGUILayout.Toggle(GUIContent.none, enableDeltaRotationY, GUILayout.Width(26f));
            GUI.enabled = enableDeltaRotationZ && Selection.transforms.Length > 1;
            deltaRotationZ = EditorGUILayout.FloatField("Z", deltaRotationZ);
            GUI.enabled = Selection.transforms.Length > 1;
            enableDeltaRotationZ = EditorGUILayout.Toggle(GUIContent.none, enableDeltaRotationZ, GUILayout.Width(26f));
            EditorGUIUtility.labelWidth = 0f;
            EditorGUILayout.EndHorizontal();

            GUI.enabled = (enableDeltaRotationX || enableDeltaRotationY || enableDeltaRotationZ) && Selection.transforms.Length > 1;

            if (GUILayout.Button(_cBtnRunPr, runIconHeight))
            {
                EasyArrangementUtilities.ProgressiveRotation(firstSelect, deltaRotationX, deltaRotationY, deltaRotationZ, enableDeltaRotationX, enableDeltaRotationY,
                    enableDeltaRotationZ);
            }

            GUI.enabled = true;
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();

            //Rotate arrange around axis
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(_cLabelRotationalArrangementHeader);
            if (GUILayout.Button(_cBtnToggleRaGuides, EditorStyles.label, GUILayout.Width(20f), GUILayout.Height(20f)))
            {
                showRotationalArrangementGuides = !showRotationalArrangementGuides;
                EditorPrefs.SetBool(PrefKeyShowRotationalGuides, showRotationalArrangementGuides);
            }

            var rct = GUILayoutUtility.GetLastRect();

            if (showRotationalArrangementGuides)
            {
                EditorGUI.DrawRect(new Rect(rct.x + 16f, rct.y + 13f, 3f, 3f), new Color(0.859f, 0.988f, 0.616f));
            }
            
            rotationCenterLocator = (Transform) EditorGUI.ObjectField(new Rect(rct.x - 4, rct.y + 25, 20f, 18f),
                rotationCenterLocator, typeof(Transform), true);
            if (rotationCenterLocator != null)
            {
                rotationCenter = rotationCenterLocator.position;
                rotationCenterLocator = null;
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);


            rotationCenter = EditorGUILayout.Vector3Field(_cVectorFieldRaRotationCenter, rotationCenter);
            GUI.enabled = Selection.transforms.Length > 0;

            float originalW = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 120f;

            normalAngle = EditorGUILayout.Slider(_cSliderRaNormalAngle, normalAngle, 0f, 360f);

            EditorGUILayout.BeginHorizontal();
            rotateAroundTotalAngle = allowArcOver360 ? EditorGUILayout.FloatField(_cSliderRaTotalAngle, rotateAroundTotalAngle) : 
                EditorGUILayout.Slider(_cSliderRaTotalAngle, rotateAroundTotalAngle, 0f, 360f);

            if (GUILayout.Button("...", EditorStyles.miniButton, GUILayout.Width(22f)))
            {
                GenericMenu menu = new GenericMenu();

                string[] options = {"30", "45", "60", "90", "120", "180", "270", "360"};
                for (int i = 0; i < options.Length; i++)
                {
                    var selected = float.Parse(options[i]);
                    menu.AddItem(new GUIContent(options[i]), false, () => { rotateAroundTotalAngle = selected; });
                }

                menu.ShowAsContext();
            }

            EditorGUILayout.EndHorizontal();
            allowArcOver360 = EditorGUILayout.Toggle(_cToggleRaAllowOver360, allowArcOver360);
            preservePositionAlongAxis = EditorGUILayout.Toggle(_cToggleRaPreservePosition, preservePositionAlongAxis);
            applyLocalRotation = EditorGUILayout.Toggle(_cToggleRaApplyLocalRotation, applyLocalRotation);
            invertRotationalArrangement = EditorGUILayout.Toggle(_cToggleRaInvert, invertRotationalArrangement);

            EditorGUIUtility.labelWidth = originalW;

            GUI.enabled = Selection.transforms.Length > 1 && (firstSelect.position - rotationCenter).sqrMagnitude > 0;

            if (GUILayout.Button(_cBtnRunRa, runIconHeight))
            {
                EasyArrangementUtilities.RotationalArrangement(firstSelect, rotationCenter, normalAngle, rotateAroundTotalAngle, preservePositionAlongAxis,
                    invertRotationalArrangement, applyLocalRotation);
            }

            GUI.enabled = true;

            EditorGUILayout.EndVertical();
        }

        private Transform rotationCenterLocator;
        private bool allowArcOver360 = false;

        void DrawColoredLabelHandle(Vector3 location, string content, Color color)
        {
            if (sceneLabelStyle == null)
            {
                sceneLabelStyle = new GUIStyle(GUI.skin.GetStyle("Label")); 
            }  

            FontStyle oStyle = sceneLabelStyle.fontStyle;
            Color oColor = sceneLabelStyle.normal.textColor;
            sceneLabelStyle.normal.textColor = color;
            Handles.Label(location, content, sceneLabelStyle);
            sceneLabelStyle.fontStyle = oStyle;
            sceneLabelStyle.normal.textColor = oColor;
        }
    }
}
