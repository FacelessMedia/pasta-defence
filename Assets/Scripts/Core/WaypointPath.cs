using UnityEngine;

namespace PastaDefence.Core
{
    public class WaypointPath : MonoBehaviour
    {
        [SerializeField] private Transform[] waypoints;
        [SerializeField] private Color gizmoColor = Color.yellow;

        public int WaypointCount => waypoints != null ? waypoints.Length : 0;

        public Vector3 GetWaypointPosition(int index)
        {
            if (waypoints == null || index < 0 || index >= waypoints.Length)
                return Vector3.zero;
            return waypoints[index].position;
        }

        public Transform GetWaypoint(int index)
        {
            if (waypoints == null || index < 0 || index >= waypoints.Length)
                return null;
            return waypoints[index];
        }

        public float GetTotalPathLength()
        {
            if (waypoints == null || waypoints.Length < 2) return 0f;

            float total = 0f;
            for (int i = 0; i < waypoints.Length - 1; i++)
            {
                total += Vector3.Distance(waypoints[i].position, waypoints[i + 1].position);
            }
            return total;
        }

        private void OnDrawGizmos()
        {
            if (waypoints == null || waypoints.Length < 2) return;

            Gizmos.color = gizmoColor;
            for (int i = 0; i < waypoints.Length - 1; i++)
            {
                if (waypoints[i] != null && waypoints[i + 1] != null)
                {
                    Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
                    Gizmos.DrawSphere(waypoints[i].position, 0.15f);
                }
            }

            if (waypoints[waypoints.Length - 1] != null)
                Gizmos.DrawSphere(waypoints[waypoints.Length - 1].position, 0.25f);
        }
    }
}
