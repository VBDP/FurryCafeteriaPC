// ============================================================================
// iRoleSystem v5.6 - iAreaZoneEditor
// ============================================================================

#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ishiruhii.iRoleSystem.Editor
{
    using ishiruhii.iRoleSystem.Core;

    [CustomEditor(typeof(iAreaZone))]
    public class iAreaZoneEditor : UnityEditor.Editor
    {
        private static readonly Color ColorFloor    = new Color(0.2f, 0.8f, 1f,  0.13f);
        private static readonly Color ColorWall     = new Color(0.2f, 0.8f, 1f,  0.09f);
        private static readonly Color ColorEdge     = new Color(0.2f, 0.8f, 1f,  0.90f);
        private static readonly Color ColorEdgeTop  = new Color(0.5f, 1.0f, 1f,  0.90f);
        private static readonly Color ColorEdgeVert = new Color(0.2f, 0.8f, 1f,  0.50f);
        private static readonly Color ColorPoint    = new Color(0.1f, 0.9f, 1f,  1.00f);
        private static readonly Color ColorPointSel = new Color(1.0f, 0.8f, 0.1f, 1.00f);
        private static readonly Color ColorAdd      = new Color(0.2f, 1.0f, 0.4f, 1.00f);
        private static readonly Color ColorDelete   = new Color(1.0f, 0.3f, 0.3f, 1.00f);
        private static readonly Color ColorArrow    = new Color(1.0f, 1.0f, 0.3f, 0.90f);
        private static readonly Color ColorClosed   = new Color(0.2f, 1.0f, 0.5f, 1.00f);
        private static readonly Color ColorOpen     = new Color(1.0f, 0.5f, 0.1f, 1.00f);

        private int  _selectedIndex = -1;
        private bool _addMode       = false;
        private bool _deleteMode    = false;

        // FIX LAG: Propiedades cacheadas en OnEnable en vez de buscarlas cada frame
        private SerializedProperty _pointsProp;
        private SerializedProperty _heightProp;
        private SerializedProperty _floorProp;
        private SerializedProperty _closedProp;

        private void OnEnable()
        {
            _selectedIndex = -1;
            _pointsProp = serializedObject.FindProperty("points");
            _heightProp = serializedObject.FindProperty("height");
            _floorProp  = serializedObject.FindProperty("floorOffset");
            _closedProp = serializedObject.FindProperty("closed");
        }

        // ── Inspector ─────────────────────────────────────────────────────────

        public override void OnInspectorGUI()
        {
            iAreaZone zone = (iAreaZone)target;
            serializedObject.Update();

            DrawHeader(zone.closed);
            EditorGUILayout.Space(6);

            // FIX LAG: Usar propiedades cacheadas (FindProperty fue movido a OnEnable)
            SerializedProperty pointsProp = _pointsProp;
            SerializedProperty heightProp = _heightProp;
            SerializedProperty floorProp  = _floorProp;
            SerializedProperty closedProp = _closedProp;

            int n = pointsProp.arraySize;

            // Estado
            EditorGUILayout.LabelField("Estado de la zona", EditorStyles.boldLabel);
            string estadoTxt = zone.closed
                ? $"CERRADA  ({n} puntos)"
                : (n < 3 ? $"Abierta - necesitas al menos 3 puntos (tienes {n})" : $"Abierta - {n} puntos listos para cerrar");
            EditorGUILayout.LabelField(estadoTxt, new GUIStyle(EditorStyles.boldLabel)
                { normal = { textColor = zone.closed ? ColorClosed : (n >= 3 ? ColorOpen : Color.gray) } });

            EditorGUILayout.Space(4);

            // Boton cerrar / reabrir
            using (new EditorGUILayout.HorizontalScope())
            {
                if (!zone.closed)
                {
                    GUI.enabled = n >= 3;
                    Color prev = GUI.backgroundColor;
                    GUI.backgroundColor = n >= 3 ? ColorClosed : Color.gray;
                    if (GUILayout.Button("Cerrar zona  (conectar ultimo con primero)", GUILayout.Height(32)))
                    {
                        Undo.RecordObject(zone, "Cerrar iAreaZone");
                        closedProp.boolValue = true;
                        _addMode = _deleteMode = false;
                        serializedObject.ApplyModifiedProperties();
                        EditorUtility.SetDirty(zone);
                        SceneView.RepaintAll();
                    }
                    GUI.backgroundColor = prev;
                    GUI.enabled = true;
                }
                else
                {
                    Color prev = GUI.backgroundColor;
                    GUI.backgroundColor = ColorOpen;
                    if (GUILayout.Button("Reabrir zona  (seguir editando puntos)", GUILayout.Height(32)))
                    {
                        Undo.RecordObject(zone, "Reabrir iAreaZone");
                        closedProp.boolValue = false;
                        serializedObject.ApplyModifiedProperties();
                        EditorUtility.SetDirty(zone);
                        SceneView.RepaintAll();
                    }
                    GUI.backgroundColor = prev;
                }
            }

            if (!zone.closed && n < 3)
                EditorGUILayout.HelpBox("Necesitas al menos 3 puntos para cerrar la zona.", MessageType.Warning);

            EditorGUILayout.Space(8);

            // Geometria
            EditorGUILayout.LabelField("Geometria", EditorStyles.boldLabel);
            using (new EditorGUI.IndentLevelScope())
            {
                EditorGUILayout.PropertyField(heightProp, new GUIContent("Altura (m)"));
                EditorGUILayout.PropertyField(floorProp,  new GUIContent("Offset de suelo"));
            }

            EditorGUILayout.Space(6);

            // Herramientas edicion
            if (!zone.closed)
            {
                EditorGUILayout.LabelField("Edicion de puntos", EditorStyles.boldLabel);

                if (n > 0)
                {
                    string insertAfter = (_selectedIndex >= 0 && _selectedIndex < n)
                        ? $"despues de P{_selectedIndex}"
                        : $"despues de P{n - 1} (ultimo)";
                    EditorGUILayout.HelpBox($"Insercion: {insertAfter}", MessageType.None);
                }

                using (new EditorGUILayout.HorizontalScope())
                {
                    Color prev = GUI.backgroundColor;
                    GUI.backgroundColor = _addMode ? ColorAdd : Color.white;
                    if (GUILayout.Button("+ Anadir punto", GUILayout.Height(26)))
                    { _addMode = !_addMode; _deleteMode = false; }

                    GUI.backgroundColor = _deleteMode ? ColorDelete : Color.white;
                    if (GUILayout.Button("X Eliminar punto", GUILayout.Height(26)))
                    { _deleteMode = !_deleteMode; _addMode = false; }
                    GUI.backgroundColor = prev;
                }

                if (_addMode)
                    EditorGUILayout.HelpBox("Haz clic en la Scene View para insertar un punto despues del seleccionado.", MessageType.Info);
                if (_deleteMode)
                    EditorGUILayout.HelpBox("Haz clic sobre un punto en la Scene View para eliminarlo.", MessageType.Warning);

                EditorGUILayout.Space(4);
            }
            else
            {
                _addMode = _deleteMode = false;
                EditorGUILayout.HelpBox("Zona cerrada. Pulsa 'Reabrir zona' para editar los puntos.", MessageType.Info);
            }

            // Lista de puntos
            EditorGUILayout.LabelField($"Puntos ({n})", EditorStyles.boldLabel);
            if (n == 0)
            {
                EditorGUILayout.HelpBox("Sin puntos. Activa modo Anadir y haz clic en la Scene View.", MessageType.Info);
            }
            else
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    for (int i = 0; i < n; i++)
                    {
                        bool isSel = (_selectedIndex == i);
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            Color bg = GUI.backgroundColor;
                            GUI.backgroundColor = isSel ? ColorPointSel : Color.white;

                            string label = i == 0     ? "P0 (inicio)" :
                                           i == n - 1 ? (zone.closed ? $"P{i}  ->  P0" : $"P{i} (ultimo)") :
                                                        $"P{i}  ->  P{i+1}";

                            EditorGUILayout.PropertyField(pointsProp.GetArrayElementAtIndex(i), new GUIContent(label));
                            GUI.backgroundColor = bg;

                            GUI.enabled = !zone.closed;
                            Color bc = GUI.backgroundColor;
                            GUI.backgroundColor = isSel ? ColorPointSel : Color.white;
                            if (GUILayout.Button("o", GUILayout.Width(22)))
                            { _selectedIndex = isSel ? -1 : i; SceneView.RepaintAll(); }
                            GUI.backgroundColor = bc;

                            GUI.enabled = !zone.closed && n > 0;
                            if (GUILayout.Button("X", GUILayout.Width(22)))
                            {
                                Undo.RecordObject(zone, "Eliminar punto iAreaZone");
                                pointsProp.DeleteArrayElementAtIndex(i);
                                serializedObject.ApplyModifiedProperties();
                                if (_selectedIndex >= pointsProp.arraySize) _selectedIndex = pointsProp.arraySize - 1;
                                EditorUtility.SetDirty(zone);
                                SceneView.RepaintAll();
                                break;
                            }
                            GUI.enabled = true;
                        }
                    }
                }
            }

            EditorGUILayout.Space(6);
            EditorGUILayout.HelpBox("Las flechas amarillas muestran el orden del poligono en el suelo.", MessageType.None);

            // FIX LAG: SceneView.RepaintAll() dentro de OnInspectorGUI se ejecuta cada frame del inspector,
            // causando lag masivo. Solo forzamos repaint de la SceneView cuando realmente hay cambios.
            if (serializedObject.ApplyModifiedProperties())
                SceneView.RepaintAll();
        }

        // ── Scene View ────────────────────────────────────────────────────────

        private void OnSceneGUI()
        {
            iAreaZone zone = (iAreaZone)target;
            if (zone == null) return;

            Transform tr = zone.transform;
            Event     e  = Event.current;

            if (_addMode || _deleteMode)
                if (e.type == EventType.Layout)
                    HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

            int n = (zone.points != null) ? zone.points.Length : 0;

            if (n >= 2) DrawZoneGeometry(zone, tr, n);
            else if (n == 1)
            {
                Vector3 w = tr.TransformPoint(new Vector3(zone.points[0].x, zone.floorOffset, zone.points[0].z));
                Handles.color = ColorPoint;
                float sz = HandleUtility.GetHandleSize(w) * 0.10f;
                Handles.SphereHandleCap(0, w, Quaternion.identity, sz, EventType.Repaint);
            }

            if (n > 0) DrawPointHandles(zone, tr, n, e);
            if (_addMode && !zone.closed) HandleAddMode(zone, tr, n, e);
        }

        // ── Geometria 3D ──────────────────────────────────────────────────────

        private void DrawZoneGeometry(iAreaZone zone, Transform tr, int n)
        {
            float h  = zone.height;
            float fy = zone.floorOffset;

            Vector3[] wFloor = new Vector3[n];
            Vector3[] wRoof  = new Vector3[n];

            for (int i = 0; i < n; i++)
            {
                Vector3 p = zone.points[i];
                wFloor[i] = tr.TransformPoint(new Vector3(p.x, fy,     p.z));
                wRoof[i]  = tr.TransformPoint(new Vector3(p.x, fy + h, p.z));
            }

            int segs = zone.closed ? n : n - 1;

            // ── Paredes (quads entre pares de puntos adyacentes) ──────────────
            for (int i = 0; i < segs; i++)
            {
                int nx = (i + 1) % n;

                // Borde inferior
                Handles.color = ColorEdge;
                Handles.DrawLine(wFloor[i], wFloor[nx], 2f);

                // Borde superior
                Handles.color = ColorEdgeTop;
                Handles.DrawLine(wRoof[i], wRoof[nx], 2f);

                // Relleno de la pared (2 triangulos por quad)
                Handles.color = ColorWall;
                Handles.DrawAAConvexPolygon(wFloor[i], wFloor[nx], wRoof[nx]);
                Handles.DrawAAConvexPolygon(wFloor[i], wRoof[nx],  wRoof[i]);
            }

            // Linea discontinua de previsualizacion del cierre cuando abierta
            if (!zone.closed)
            {
                Handles.color = new Color(0.2f, 0.8f, 1f, 0.25f);
                Handles.DrawDottedLine(wFloor[n - 1], wFloor[0], 6f);
                Handles.DrawDottedLine(wRoof[n - 1],  wRoof[0],  6f);
            }

            // ── Aristas verticales en cada punto ──────────────────────────────
            Handles.color = ColorEdgeVert;
            for (int i = 0; i < n; i++)
                Handles.DrawLine(wFloor[i], wRoof[i], 1.5f);

            // ── Suelo y techo: triangulacion ear-clipping ─────────────────────
            if (zone.closed)
            {
                int[] tris = EarClip(zone.points, n);
                if (tris != null)
                {
                    for (int t = 0; t < tris.Length; t += 3)
                    {
                        int a = tris[t], b = tris[t+1], c = tris[t+2];

                        Handles.color = ColorFloor;
                        Handles.DrawAAConvexPolygon(wFloor[a], wFloor[b], wFloor[c]);
                        Handles.DrawAAConvexPolygon(wRoof[a], wRoof[b], wRoof[c]);
                    }
                }
            }

            // ── Flechas de direccion en suelo ─────────────────────────────────
            Handles.color = ColorArrow;
            for (int i = 0; i < segs; i++)
            {
                int     nx  = (i + 1) % n;
                Vector3 mid = (wFloor[i] + wFloor[nx]) * 0.5f;
                Vector3 dir = (wFloor[nx] - wFloor[i]).normalized;
                float   sz  = HandleUtility.GetHandleSize(mid) * 0.10f;
                if (dir.sqrMagnitude > 0.001f)
                    Handles.ArrowHandleCap(0, mid - dir * sz * 0.5f,
                        Quaternion.LookRotation(dir), sz, EventType.Repaint);
            }

            // Etiqueta
            string estado = zone.closed ? "CERRADA" : "abierta";
            Handles.Label(zone.GetWorldCenter(),
                $"  iAreaZone [{estado}]  h:{h:F1}m  ({n}pts)",
                new GUIStyle(EditorStyles.boldLabel)
                {
                    normal   = { textColor = zone.closed ? ColorClosed : ColorOpen },
                    fontSize = 10
                });
        }

        // ── Ear-clipping triangulation ────────────────────────────────────────
        // Triangula un poligono 2D (XZ) y devuelve indices de triangulos.

        // ── Ear-clipping con deteccion automatica de orientacion ─────────────

        private float PolygonArea2D(Vector3[] pts, int n)
        {
            // Shoelace formula — positivo = CCW, negativo = CW (en XZ)
            float area = 0f;
            for (int i = 0; i < n; i++)
            {
                int j = (i + 1) % n;
                area += pts[i].x * pts[j].z;
                area -= pts[j].x * pts[i].z;
            }
            return area * 0.5f;
        }

        private int[] EarClip(Vector3[] pts, int n)
        {
            if (n < 3) return null;

            // Detectar orientacion: si CW invertir la lista de indices para
            // que el algoritmo siempre trabaje en CCW
            bool isCCW = PolygonArea2D(pts, n) > 0f;

            List<int> indices = new List<int>();
            if (isCCW)
                for (int i = 0; i < n; i++) indices.Add(i);
            else
                for (int i = n - 1; i >= 0; i--) indices.Add(i);

            List<int> result = new List<int>();
            int safety = n * n + 10;
            int idx    = 0;

            while (indices.Count > 3 && safety-- > 0)
            {
                int count = indices.Count;
                int prev  = indices[(idx - 1 + count) % count];
                int curr  = indices[idx];
                int next  = indices[(idx + 1) % count];

                Vector2 a = new Vector2(pts[prev].x, pts[prev].z);
                Vector2 b = new Vector2(pts[curr].x, pts[curr].z);
                Vector2 c = new Vector2(pts[next].x, pts[next].z);

                if (IsEar(a, b, c, pts, indices, prev, curr, next))
                {
                    result.Add(prev);
                    result.Add(curr);
                    result.Add(next);
                    indices.RemoveAt(idx);
                    if (idx >= indices.Count) idx = 0;
                }
                else
                {
                    idx = (idx + 1) % indices.Count;
                }
            }

            if (indices.Count == 3)
            {
                result.Add(indices[0]);
                result.Add(indices[1]);
                result.Add(indices[2]);
            }

            return result.ToArray();
        }

        private bool IsEar(Vector2 a, Vector2 b, Vector2 c,
                           Vector3[] pts, List<int> indices, int pi, int ci, int ni)
        {
            // El triangulo debe ser CCW (cross > 0)
            if (Cross2D(a, b, c) <= 0f) return false;

            // Ningun otro vertice del poligono debe estar dentro del triangulo
            foreach (int idx in indices)
            {
                if (idx == pi || idx == ci || idx == ni) continue;
                Vector2 p = new Vector2(pts[idx].x, pts[idx].z);
                if (PointInTriangle2D(p, a, b, c)) return false;
            }
            return true;
        }

        private float Cross2D(Vector2 o, Vector2 a, Vector2 b)
            => (a.x - o.x) * (b.y - o.y) - (a.y - o.y) * (b.x - o.x);

        private bool PointInTriangle2D(Vector2 p, Vector2 a, Vector2 b, Vector2 c)
        {
            float d1 = Cross2D(p, a, b);
            float d2 = Cross2D(p, b, c);
            float d3 = Cross2D(p, c, a);
            bool hasNeg = (d1 < 0) || (d2 < 0) || (d3 < 0);
            bool hasPos = (d1 > 0) || (d2 > 0) || (d3 > 0);
            return !(hasNeg && hasPos);
        }

        // ── Point handles ─────────────────────────────────────────────────────

        private void DrawPointHandles(iAreaZone zone, Transform tr, int n, Event e)
        {
            float fy = zone.floorOffset;
            float sz = HandleUtility.GetHandleSize(tr.position) * 0.08f;

            for (int i = 0; i < n; i++)
            {
                Vector3 p     = zone.points[i];
                Vector3 world = tr.TransformPoint(new Vector3(p.x, fy, p.z));
                bool    isSel = (_selectedIndex == i);

                Handles.color = _deleteMode ? ColorDelete : (isSel ? ColorPointSel : ColorPoint);

                string lbl = i == 0 ? "P0" : (i == n-1 && zone.closed) ? $"P{i}->P0" : $"P{i}";
                Handles.Label(world + Vector3.up * (sz * 2.2f), $" {lbl}",
                    new GUIStyle(EditorStyles.miniLabel)
                    { normal = { textColor = isSel ? ColorPointSel : ColorPoint } });

                if (_deleteMode && !zone.closed)
                {
                    if (Handles.Button(world, Quaternion.identity, sz, sz * 1.5f, Handles.SphereHandleCap))
                    {
                        Undo.RecordObject(zone, "Eliminar punto iAreaZone");
                        var list = new List<Vector3>(zone.points);
                        list.RemoveAt(i);
                        zone.points = list.ToArray();
                        if (_selectedIndex >= zone.points.Length) _selectedIndex = zone.points.Length - 1;
                        EditorUtility.SetDirty(zone);
                        Repaint();
                        break;
                    }
                }
                else
                {
                    if (Handles.Button(world, Quaternion.identity, sz, sz * 1.5f, Handles.SphereHandleCap))
                    {
                        _selectedIndex = isSel ? -1 : i;
                        Repaint();
                    }

                    if (isSel && !zone.closed)
                    {
                        EditorGUI.BeginChangeCheck();
                        Vector3 nw = Handles.PositionHandle(world, Quaternion.identity);
                        if (EditorGUI.EndChangeCheck())
                        {
                            Undo.RecordObject(zone, "Mover punto iAreaZone");
                            Vector3 nl = tr.InverseTransformPoint(nw);
                            zone.points[i] = new Vector3(nl.x, zone.points[i].y, nl.z);
                            EditorUtility.SetDirty(zone);
                        }
                    }
                }
            }
        }

        // ── Add mode ──────────────────────────────────────────────────────────

        private void HandleAddMode(iAreaZone zone, Transform tr, int n, Event e)
        {
            float fy = zone.floorOffset;
            Plane pl = new Plane(tr.up, tr.TransformPoint(new Vector3(0, fy, 0)));

            if (e.type == EventType.MouseDown && e.button == 0 && !e.alt)
            {
                Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
                if (pl.Raycast(ray, out float dist))
                {
                    Vector3 hit   = ray.GetPoint(dist);
                    Vector3 local = tr.InverseTransformPoint(hit);
                    Vector3 newPt = new Vector3(local.x, 0f, local.z);

                    Undo.RecordObject(zone, "Anadir punto iAreaZone");
                    var list = new List<Vector3>(zone.points ?? new Vector3[0]);

                    int insertAfter = (n == 0) ? -1 :
                                     (_selectedIndex >= 0 && _selectedIndex < n) ? _selectedIndex : n - 1;
                    int insertAt = insertAfter + 1;
                    list.Insert(insertAt, newPt);

                    zone.points    = list.ToArray();
                    _selectedIndex = insertAt;
                    EditorUtility.SetDirty(zone);
                    Repaint();
                    e.Use();
                }
            }

            // Preview
            Ray r = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            if (pl.Raycast(r, out float d))
            {
                Vector3 preview = r.GetPoint(d);
                float   s       = HandleUtility.GetHandleSize(preview) * 0.08f;
                Handles.color   = ColorAdd;
                Handles.SphereHandleCap(0, preview, Quaternion.identity, s, EventType.Repaint);

                if (n > 0)
                {
                    int fromIdx = (_selectedIndex >= 0 && _selectedIndex < n) ? _selectedIndex : n - 1;
                    int toIdx   = (fromIdx + 1) % n;
                    Vector3 fp  = zone.points[fromIdx];
                    Vector3 tp  = zone.points[toIdx];
                    Vector3 wF  = tr.TransformPoint(new Vector3(fp.x, fy, fp.z));
                    Vector3 wT  = tr.TransformPoint(new Vector3(tp.x, fy, tp.z));

                    Handles.color = ColorAdd;
                    Handles.DrawDottedLine(wF, preview, 4f);
                    Handles.color = new Color(0.2f, 1f, 0.4f, 0.4f);
                    Handles.DrawDottedLine(preview, wT, 4f);
                }
            }

            // FIX LAG: SceneView.RepaintAll() aquí forzaba repaint en cada movimiento del ratón.
            // En modo Add el preview ya usa EventType.Repaint de Handles; Unity repinta solo cuando
            // hay interacción real. No es necesario forzarlo manualmente.
        }

        // ── Header ────────────────────────────────────────────────────────────

        private void DrawHeader(bool closed)
        {
            Rect r = EditorGUILayout.GetControlRect(false, 36);
            EditorGUI.DrawRect(r, closed ? new Color(0.1f, 0.5f, 0.2f, 0.35f) : new Color(0.1f, 0.5f, 0.7f, 0.25f));
            GUI.Label(r, closed ? "  iAreaZone  -  ZONA CERRADA" : "  iAreaZone  -  Zona Poligonal 3D",
                new GUIStyle(EditorStyles.boldLabel)
                {
                    fontSize  = 13,
                    alignment = TextAnchor.MiddleLeft,
                    normal    = { textColor = closed ? ColorClosed : new Color(0.6f, 1f, 1f) }
                });
        }

        // OnEnable ya definido arriba con la caché de propiedades
        private void OnDisable() { _addMode = false; _deleteMode = false; }
    }
}
#endif
