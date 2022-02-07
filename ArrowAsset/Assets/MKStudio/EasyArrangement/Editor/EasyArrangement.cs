using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace MKStudio.EasyArrangement
{
    public static class EasyArrangementUtilities
    {
        private static List<Transform> _operationTransforms = new List<Transform>();
        private static double _lastCallTime;

        [MenuItem("GameObject/Easy Arrangement/Distribute X", false, 12)]
        public static void DistributeX()
        {
            if (Selection.transforms.Length <= 2)
            {
                return;
            }

            _operationTransforms.Clear();

            for (int i = 0; i < Selection.transforms.Length; i++)
            {
                _operationTransforms.Add(Selection.transforms[i]);
            }

            _operationTransforms.Sort((x, y) => x.position.x.CompareTo(y.position.x));

            float t0 = _operationTransforms[0].position.x;
            float tn = _operationTransforms[_operationTransforms.Count - 1].transform.position.x;
            var array = _operationTransforms.ToArray();
            Undo.RecordObjects(array, "Distribute X");
            for (int i = 1; i < _operationTransforms.Count - 1; i++)
            {
                var pos = _operationTransforms[i].position;
                _operationTransforms[i].position = new Vector3((t0 + i * (tn - t0) / (_operationTransforms.Count - 1)), pos.y, pos.z);
            }
        }

        [MenuItem("GameObject/Easy Arrangement/Distribute Y", false, 12)]
        public static void DistributeY()
        {
            if (Selection.transforms.Length <= 2)
            {
                return;
            }

            _operationTransforms.Clear();

            for (int i = 0; i < Selection.transforms.Length; i++)
            {
                _operationTransforms.Add(Selection.transforms[i]);
            }

            _operationTransforms.Sort((x, y) => x.position.y.CompareTo(y.position.y));

            float t0 = _operationTransforms[0].position.y;
            float tn = _operationTransforms[_operationTransforms.Count - 1].transform.position.y;
            var array = _operationTransforms.ToArray();
            Undo.RecordObjects(array, "Distribute Y");
            for (int i = 1; i < _operationTransforms.Count - 1; i++)
            {
                var pos = _operationTransforms[i].position;
                _operationTransforms[i].position = new Vector3(pos.x, (t0 + i * (tn - t0) / (_operationTransforms.Count - 1)), pos.z);
            }
        }

        [MenuItem("GameObject/Easy Arrangement/Distribute Z", false, 12)]
        public static void DistributeZ()
        {
            if (Selection.transforms.Length <= 2)
            {
                return;
            }

            _operationTransforms.Clear();

            for (int i = 0; i < Selection.transforms.Length; i++)
            {
                _operationTransforms.Add(Selection.transforms[i]);
            }

            _operationTransforms.Sort((x, y) => x.position.z.CompareTo(y.position.z));

            float t0 = _operationTransforms[0].position.z;
            float tn = _operationTransforms[_operationTransforms.Count - 1].transform.position.z;
            var array = _operationTransforms.ToArray();
            Undo.RecordObjects(array, "Distribute Z");
            for (int i = 1; i < _operationTransforms.Count - 1; i++)
            {
                var pos = _operationTransforms[i].position;
                _operationTransforms[i].position = new Vector3(pos.x, pos.y, (t0 + i * (tn - t0) / (_operationTransforms.Count - 1)));
            }
        }

        [MenuItem("GameObject/Easy Arrangement/Distribute Along Line", false, 12)]
        public static void DistributeAlongLine()
        {
            if (Selection.transforms.Length <= 2)
            {
                return;
            }

            _operationTransforms.Clear();

            for (int i = 0; i < Selection.transforms.Length; i++)
            {
                _operationTransforms.Add(Selection.transforms[i]);
            }

            _operationTransforms.Sort((x, y) => x.GetSiblingIndex().CompareTo(y.GetSiblingIndex()));

            var line = _operationTransforms[_operationTransforms.Count - 1].position - _operationTransforms[0].position;
            float deltaX = line.x / (_operationTransforms.Count - 1);
            float deltaY = line.y / (_operationTransforms.Count - 1);
            float deltaZ = line.z / (_operationTransforms.Count - 1);
            
            var start = _operationTransforms[0].position;

            var array = _operationTransforms.ToArray();
            Undo.RecordObjects(array, "Distribute Along Line");
            for (int i = 1; i < _operationTransforms.Count - 1; i++)
            {
                _operationTransforms[i].position = new Vector3(start.x + i * deltaX, start.y + i * deltaY, start.z + i * deltaZ);
            }
        }

       
        [MenuItem("GameObject/Easy Arrangement/Align to Left", false, 12)]
        public static void AlignXCentersToLeftBound()
        {
            if (Selection.transforms.Length < 2)
            {
                return;
            }

            _operationTransforms.Clear();

            for (int i = 0; i < Selection.transforms.Length; i++)
            {
                _operationTransforms.Add(Selection.transforms[i]);
            }

            float leftmost = (from obj in _operationTransforms select obj.position.x).Min();

            var array = _operationTransforms.ToArray();
            Undo.RecordObjects(array, "Align to Left");
            for (int i = 0; i < _operationTransforms.Count; i++)
            {
                var pos = _operationTransforms[i].position;
                _operationTransforms[i].position = new Vector3(leftmost, pos.y, pos.z);
            }
        }

        [MenuItem("GameObject/Easy Arrangement/Align to Right", false, 12)]
        public static void AlignXCentersToRightBound()
        {
            if (Selection.transforms.Length < 2)
            {
                return;
            }

            _operationTransforms.Clear();

            for (int i = 0; i < Selection.transforms.Length; i++)
            {
                _operationTransforms.Add(Selection.transforms[i]);
            }

            float rightmost = (from obj in _operationTransforms select obj.position.x).Max();

            var array = _operationTransforms.ToArray();
            Undo.RecordObjects(array, "Align to Right");
            for (int i = 0; i < _operationTransforms.Count; i++)
            {
                var pos = _operationTransforms[i].position;
                _operationTransforms[i].position = new Vector3(rightmost, pos.y, pos.z);
            }
        }

        public static void AlignXCentersToAverage()
        {
            if (Selection.transforms.Length < 2)
            {
                return;
            }

            _operationTransforms.Clear();
            float sum = 0f;

            for (int i = 0; i < Selection.transforms.Length; i++)
            {
                _operationTransforms.Add(Selection.transforms[i]);
                sum += Selection.transforms[i].position.x;
            }

            float avg = sum / Selection.transforms.Length;

            var array = _operationTransforms.ToArray();
            Undo.RecordObjects(array, "Align X to Average");
            for (int i = 0; i < _operationTransforms.Count; i++)
            {
                var pos = _operationTransforms[i].position;
                _operationTransforms[i].position = new Vector3(avg, pos.y, pos.z);
            }
        }
        
         [MenuItem("GameObject/Easy Arrangement/Align to Top", false, 12)]
        public static void AlignYCentersToTop()
        {
            if (Selection.transforms.Length < 2)
            {
                return;
            }


            _operationTransforms.Clear();

            for (int i = 0; i < Selection.transforms.Length; i++)
            {
                _operationTransforms.Add(Selection.transforms[i]);
            }

            float highest = (from obj in _operationTransforms select obj.position.y).Max();

            var array = _operationTransforms.ToArray();
            Undo.RecordObjects(array, "Align to Top");
            for (int i = 0; i < _operationTransforms.Count; i++)
            {
                var pos = _operationTransforms[i].position;
                _operationTransforms[i].position = new Vector3(pos.x, highest, pos.z);
            }
        }

        [MenuItem("GameObject/Easy Arrangement/Align To Bottom", false, 12)]
        public static void AlignYCentersToBottom()
        {
            if (Selection.transforms.Length < 2)
            {
                return;
            }

            _operationTransforms.Clear();

            for (int i = 0; i < Selection.transforms.Length; i++)
            {
                _operationTransforms.Add(Selection.transforms[i]);
            }

            float highest = (from obj in _operationTransforms select obj.position.y).Min();

            var array = _operationTransforms.ToArray();
            Undo.RecordObjects(array, "Align to Bottom");
            for (int i = 0; i < _operationTransforms.Count; i++)
            {
                var pos = _operationTransforms[i].position;
                _operationTransforms[i].position = new Vector3(pos.x, highest, pos.z);
            }
        }

        public static void AlignYCentersToAverage()
        {
            if (Selection.transforms.Length < 2)
            {
                return;
            }

            _operationTransforms.Clear();
            float sum = 0f;

            for (int i = 0; i < Selection.transforms.Length; i++)
            {
                _operationTransforms.Add(Selection.transforms[i]);
                sum += Selection.transforms[i].position.y;
            }

            float avg = sum / Selection.transforms.Length;

            var array = _operationTransforms.ToArray();
            Undo.RecordObjects(array, "Align Y to Average");
            for (int i = 0; i < _operationTransforms.Count; i++)
            {
                var pos = _operationTransforms[i].position;
                _operationTransforms[i].position = new Vector3(pos.x, avg, pos.z);
            }
        }


        [MenuItem("GameObject/Easy Arrangement/Align to Forward", false, 12)]
        public static void AlignZCentersToForwardBound()
        {
            if (Selection.transforms.Length < 2)
            {
                return;
            }

            _operationTransforms.Clear();

            for (int i = 0; i < Selection.transforms.Length; i++)
            {
                _operationTransforms.Add(Selection.transforms[i]);
            }

            float front = (from obj in _operationTransforms select obj.position.z).Max();

            var array = _operationTransforms.ToArray();
            Undo.RecordObjects(array, "Align to Forward");
            for (int i = 0; i < _operationTransforms.Count; i++)
            {
                var pos = _operationTransforms[i].position;
                _operationTransforms[i].position = new Vector3(pos.x, pos.y, front);
            }
        }

        [MenuItem("GameObject/Easy Arrangement/Align to Back", false, 12)]
        public static void AlignZCentersToBackBound()
        {
            if (Selection.transforms.Length < 2)
            {
                return;
            }

            _operationTransforms.Clear();

            for (int i = 0; i < Selection.transforms.Length; i++)
            {
                _operationTransforms.Add(Selection.transforms[i]);
            }

            float back = (from obj in _operationTransforms select obj.position.z).Min();

            var array = _operationTransforms.ToArray();
            Undo.RecordObjects(array, "Align to Back");
            for (int i = 0; i < _operationTransforms.Count; i++)
            {
                var pos = _operationTransforms[i].position;
                _operationTransforms[i].position = new Vector3(pos.x, pos.y, back);
            }
        }

        public static void AlignZCentersToAverage()
        {
            if (Selection.transforms.Length < 2)
            {
                return;
            }

            _operationTransforms.Clear();
            float sum = 0f;

            for (int i = 0; i < Selection.transforms.Length; i++)
            {
                _operationTransforms.Add(Selection.transforms[i]);
                sum += Selection.transforms[i].position.z;
            }

            float avg = sum / Selection.transforms.Length;

            var array = _operationTransforms.ToArray();
            Undo.RecordObjects(array, "Align Z to Average");
            for (int i = 0; i < _operationTransforms.Count; i++)
            {
                var pos = _operationTransforms[i].position;
                _operationTransforms[i].position = new Vector3(pos.x, pos.y, avg);
            }
        }

        public static void ProgressivePlacement(Transform firstSelectedObject, ProgressivePlacementActionOrder order, float deltaX, float deltaY, float deltaZ, bool enableX,
            bool enableY, bool enableZ)
        {
            if (Selection.transforms.Length < 2)
            {
                return;
            }

            _operationTransforms.Clear();
            for (int i = 0; i < Selection.transforms.Length; i++)
            {
                if (Selection.transforms[i] != firstSelectedObject)
                {
                    _operationTransforms.Add(Selection.transforms[i]);
                }
            }

            switch (order)
            {
                case ProgressivePlacementActionOrder.Hierarchy:
                    _operationTransforms.Sort((x, y) => x.GetSiblingIndex().CompareTo(y.GetSiblingIndex()));
                    break;
                case ProgressivePlacementActionOrder.AxisX:
                    if (deltaX > 0)
                    {
                        _operationTransforms.Sort((x, y) => x.transform.position.x.CompareTo(y.transform.position.x));
                    }
                    else
                    {
                        _operationTransforms.Sort((x, y) => y.transform.position.x.CompareTo(x.transform.position.x));
                    }

                    break;
                case ProgressivePlacementActionOrder.AxisY:
                    if (deltaY > 0)
                    {
                        _operationTransforms.Sort((x, y) => x.transform.position.y.CompareTo(y.transform.position.y));
                    }
                    else
                    {
                        _operationTransforms.Sort((x, y) => y.transform.position.y.CompareTo(x.transform.position.y));
                    }

                    break;
                case ProgressivePlacementActionOrder.AxisZ:
                    if (deltaZ > 0)
                    {
                        _operationTransforms.Sort((x, y) => x.transform.position.z.CompareTo(y.transform.position.z));
                    }
                    else
                    {
                        _operationTransforms.Sort((x, y) => y.transform.position.z.CompareTo(x.transform.position.z));
                    }

                    break;
            }

            var array = _operationTransforms.ToArray();
            Undo.RecordObjects(array, "Progressive Placement");
            for (int i = 0; i < _operationTransforms.Count; i++)
            {
                Vector3 first = firstSelectedObject.position;
                _operationTransforms[i].transform.position = new Vector3(
                    enableX ? first.x + (i + 1) * deltaX : _operationTransforms[i].transform.position.x,
                    enableY ? first.y + (i + 1) * deltaY : _operationTransforms[i].transform.position.y,
                    enableZ ? first.z + (i + 1) * deltaZ : _operationTransforms[i].transform.position.z);
            }
        }

        public static void ProgressiveRotation(Transform firstSelectedObject, float deltaX, float deltaY, float deltaZ, bool enableX, bool enableY, bool enableZ)
        {
            if (Selection.transforms.Length < 2)
            {
                return;
            }

            _operationTransforms.Clear();
            for (int i = 0; i < Selection.transforms.Length; i++)
            {
                if (Selection.transforms[i] != firstSelectedObject)
                {
                    _operationTransforms.Add(Selection.transforms[i]);
                }
            }

            _operationTransforms.Sort((x, y) => x.GetSiblingIndex().CompareTo(y.GetSiblingIndex()));

            var array = _operationTransforms.ToArray();
            Undo.RecordObjects(array, "Progressive Rotation");
            for (int i = 0; i < _operationTransforms.Count; i++)
            {
                Quaternion first = firstSelectedObject.localRotation;
                _operationTransforms[i].transform.localRotation = Quaternion.Euler(
                    enableX ? first.eulerAngles.x + (i + 1) * deltaX : _operationTransforms[i].transform.localRotation.eulerAngles.x,
                    enableY ? first.eulerAngles.y + (i + 1) * deltaY : _operationTransforms[i].transform.localRotation.eulerAngles.y,
                    enableZ ? first.eulerAngles.z + (i + 1) * deltaZ : _operationTransforms[i].transform.localRotation.eulerAngles.z);
            }
        }

        public static void RotationalArrangement(Transform firstSelectedObject, Vector3 rotationCenter, float normalAngle, float totalAngle, bool preservePositionAlongAxis,
            bool invert, bool applyLocalRotation)
        {
            if (!(Selection.transforms.Length > 1 && (firstSelectedObject.position - rotationCenter).sqrMagnitude > 0))
            {
                return;
            }

            _operationTransforms.Clear();
            for (int i = 0; i < Selection.transforms.Length; i++)
            {
                if (Selection.transforms[i] != firstSelectedObject)
                {
                    _operationTransforms.Add(Selection.transforms[i]);
                }
            }

            _operationTransforms.Sort((x, y) => x.GetSiblingIndex().CompareTo(y.GetSiblingIndex()));

            var firstPos = firstSelectedObject.position;
            Vector3 firstDirection = firstPos - rotationCenter;
            var r = firstSelectedObject.position - rotationCenter;
            var cross = Vector3.Cross(Vector3.down, r) == Vector3.zero ? Vector3.Cross(Vector3.right, r) : Vector3.Cross(Vector3.down, r);
            var normal = Quaternion.AngleAxis(normalAngle, r) * cross;

            float deltaDegree = totalAngle != 360f ? totalAngle / _operationTransforms.Count : totalAngle / (_operationTransforms.Count + 1f);
            
            var array = _operationTransforms.ToArray();
            Undo.RecordObjects(array, "Rotational Arrangement");
            for (int i = 0; i < _operationTransforms.Count; i++)
            {
                if (preservePositionAlongAxis)
                {
                    var longEdge = _operationTransforms[i].position - rotationCenter;
                    var projection = Vector3.Project(longEdge, normal);
                    var projectionEnd = projection + rotationCenter;
                    var rotated = Quaternion.AngleAxis((i + 1) * (invert ? -deltaDegree : deltaDegree), normal) * (firstSelectedObject.position - rotationCenter);
                    var endPoint = rotated + projectionEnd;
                    _operationTransforms[i].position = endPoint;
                }
                else
                {
                    var rotated = Quaternion.AngleAxis((i + 1) * (invert ? -deltaDegree : deltaDegree), normal) * firstDirection;
                    var endPoint = rotated + rotationCenter;
                    _operationTransforms[i].position = endPoint;
                }

                if (applyLocalRotation)
                {
                    _operationTransforms[i].localRotation = firstSelectedObject.localRotation;
                    _operationTransforms[i].Rotate(normal, (i + 1) * (invert ? -deltaDegree : deltaDegree), Space.World);
                }
            }
        }
        
        [MenuItem("GameObject/Easy Arrangement/Group In New GameObject", false, 12)]
        public static void GroupInNewGameObject()
        {
            if (Selection.transforms.Length == 0)
            {
                return;
            }

            if (Math.Abs(EditorApplication.timeSinceStartup - _lastCallTime) < 0.1)
            {
                return;
            }

            var parent = Selection.transforms[0].parent;
            GameObject obj = new GameObject("GameObject");
            obj.transform.SetParent(parent);
            for (int i = 0; i < Selection.transforms.Length; i++)
            {
                Selection.transforms[i].SetParent(obj.transform, true);
            }
            EditorGUIUtility.PingObject(Selection.transforms[0]);

            _lastCallTime = EditorApplication.timeSinceStartup;
        }

        [MenuItem("GameObject/Easy Arrangement/Open Easy Arrangement Window...", false, 12)]
        public static void OpenMainWindow()
        {
            EasyArrangementWindow.Init();
        }
    }

    public enum ProgressivePlacementActionOrder
    {
        Hierarchy = 0,
        AxisX,
        AxisY,
        AxisZ,
    }
}
