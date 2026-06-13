// ============================================================================
// iRoleSystem v5.6 - iAreaZone
// Author: ishiruhii
// ============================================================================

using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace ishiruhii.iRoleSystem.Core
{
    [AddComponentMenu("iRoleSystem/Zones/iAreaZone")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class iAreaZone : UdonSharpBehaviour
    {
        [Header("Zona Poligonal")]
        [Tooltip("Puntos que definen el poligono de suelo (en espacio local).")]
        public Vector3[] points;

        [Tooltip("La zona esta cerrada (el ultimo punto conecta con el primero).")]
        public bool closed = false;

        [Tooltip("Altura de la zona (extrusion hacia arriba desde cada punto).")]
        [Min(0.1f)]
        public float height = 3f;

        [Tooltip("Offset vertical del suelo de la zona respecto al transform.")]
        public float floorOffset = 0f;

        // Margen vertical extra para absorber imprecisiones del avatar (pies vs centro)
        // y pequeñas diferencias de altura entre el suelo real y el punto definido.
        private const float VerticalTolerance = 0.35f;

        // Epsilon para evitar falsos negativos en puntos justo sobre el borde del poligono
        private const float EdgeEpsilon = 0.001f;

        // ─────────────────────────────────────────────────────────────────────

        public bool ContainsPoint(Vector3 worldPos)
        {
            if (!closed || points == null || points.Length < 3) return false;

            // Convertir a espacio local del transform del AreaZone.
            // Usamos InverseTransformPoint que tiene en cuenta rotacion y escala.
            Vector3 local = transform.InverseTransformPoint(worldPos);

            // Check vertical con tolerancia:
            // - Por abajo: el jugador puede estar hasta VerticalTolerance por debajo del suelo
            //   (GetPosition devuelve los pies, puede haber una pequeña diferencia de altura)
            // - Por arriba: igual, margen de VerticalTolerance sobre el techo
            float floorY = floorOffset - VerticalTolerance;
            float ceilY  = floorOffset + height + VerticalTolerance;

            if (local.y < floorY || local.y > ceilY) return false;

            // Check 2D en el plano XZ del espacio local
            return PointInPolygonXZ(local.x, local.z);
        }

        public bool ContainsLocalPlayer()
        {
            VRCPlayerApi player = Networking.LocalPlayer;
            if (player == null) return false;

            // Usamos GetPosition (pies del avatar).
            // Tambien comprobamos un punto a media altura del avatar (~0.9m)
            // para cubrir casos donde los pies estan justo en el limite.
            Vector3 feet  = player.GetPosition();
            Vector3 waist = feet + Vector3.up * 0.9f;

            return ContainsPoint(feet) || ContainsPoint(waist);
        }

        // Ray-casting algorithm (Jordan curve theorem) con epsilon en bordes
        private bool PointInPolygonXZ(float px, float pz)
        {
            int  n      = points.Length;
            bool inside = false;
            int  j      = n - 1;

            for (int i = 0; i < n; i++)
            {
                float xi = points[i].x, zi = points[i].z;
                float xj = points[j].x, zj = points[j].z;

                // Si el punto esta exactamente sobre un vertice o borde,
                // desplazamos ligeramente para evitar falsos negativos
                float testPz = pz;
                if (Mathf.Abs(zi - pz) < EdgeEpsilon) testPz += EdgeEpsilon;
                if (Mathf.Abs(zj - pz) < EdgeEpsilon) testPz += EdgeEpsilon;

                bool intersect = ((zi > testPz) != (zj > testPz)) &&
                                 (px < (xj - xi) * (testPz - zi) / (zj - zi) + xi);
                if (intersect) inside = !inside;

                j = i;
            }
            return inside;
        }

        public Vector3 GetWorldCenter()
        {
            if (points == null || points.Length == 0)
                return transform.position;

            Vector3 sum = Vector3.zero;
            foreach (Vector3 p in points) sum += p;
            Vector3 localCenter = sum / points.Length;
            localCenter.y += floorOffset + height * 0.5f;
            return transform.TransformPoint(localCenter);
        }
    }
}
