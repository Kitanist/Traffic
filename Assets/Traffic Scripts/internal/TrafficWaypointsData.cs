using System.Collections.Generic;
using UnityEngine;

namespace Gley.TrafficSystem.Internal
{
    /// <summary>
    /// Stores all available waypoints for current scene.
    /// </summary>
    public class TrafficWaypointsData : MonoBehaviour
    {
        public TrafficWaypoint[] _allTrafficWaypoints;
        public List<int> DisabledWaypoints;
        internal TrafficWaypoint[] AllTrafficWaypoints
        {
            get
            {
                return _allTrafficWaypoints;
            }
        }
        private void Start()
        {
            Invoke(nameof(DisableTempDis),1f);
        }
        public void DisableTempDis()
        {
            foreach (var waypoint in DisabledWaypoints)
            {
                API.DisableSingleWaypoint(waypoint,true);
            }
           
        }
        public void SetTrafficWaypoints(TrafficWaypoint[] waypoints)
        {
            
            _allTrafficWaypoints = waypoints;
            Debug.Log("TrafikWaypointleri editlendi adresi býrakýyorum");
            foreach (var item in _allTrafficWaypoints)
            {
                if (item.TemporaryDisabled)
                {
                    Debug.Log($"{item.BenDisable}  waypoint Name : {item.Name}");
                }
            }
        }


        public void AssignZipperGiveWay()
        {
            for (int i = 0; i < _allTrafficWaypoints.Length; i++)
            {
                if (_allTrafficWaypoints[i].ZipperGiveWay)
                {
                    var prevs = _allTrafficWaypoints[i].Prev;
                    for (int j = 0; j < prevs.Length; j++)
                    {
                        _allTrafficWaypoints[prevs[j]].GiveWay = true;
                    }
                }
            }
        }


        internal bool IsValid(out string error)
        {
            error = string.Empty;
            if (_allTrafficWaypoints == null)
            {
                error = TrafficSystemErrors.NullWaypointData;
                return false;
            }

            if (_allTrafficWaypoints.Length <= 0)
            {
                error = TrafficSystemErrors.NoWaypointsFound;
                return false;
            }

            return true;
        }
    }
}