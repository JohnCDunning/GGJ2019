using UnityEngine;
using System.Collections.Generic;

namespace EXPToolkit
{
    /// <summary>
    /// Creates a path from transforms which can then be traverse using a follow path script
    /// </summary>
    public class TransformPath : MonoBehaviour
    {
        public List<TransformPathNode> _PathNodes = new List<TransformPathNode>();
       
        public float _TotalLength;

      
        void Start()
        {
            if (_PathNodes.Count > 1)
                UpdateNodes();
            else
                AddNode(transform);
        }

        public Transform AddNode(Vector3 pos)
        {
            TransformPathNode node = new GameObject().AddComponent<TransformPathNode>();
            node.transform.position = pos;
            _PathNodes.Add(node);

            UpdateNodes();

            return node.transform;
        }

        public void AddNode(Transform tform)
        {
            TransformPathNode node = tform.gameObject.AddComponent<TransformPathNode>();            
            _PathNodes.Add(node);

            UpdateNodes();
        }

        public bool CheckDistanceAdd(Vector3 pos, float distCutoff)
        {
            return Vector3.Distance(pos, _PathNodes[_PathNodes.Count - 1].transform.position) > distCutoff;
        }

        void UpdateNodes()
        {
            if (_PathNodes.Count < 2) return;

            float distance = 0;
            _PathNodes[0].m_Distance = 0;

            for (int i = 1; i < _PathNodes.Count; i++)
            {
                float dist = Vector3.Distance(_PathNodes[i].transform.position, _PathNodes[i - 1].transform.position);
                distance += dist;
                _PathNodes[i].m_Distance = distance;
            }

            _TotalLength = distance;

            for (int i = 0; i < _PathNodes.Count; i++)
            {
                _PathNodes[i].m_NormValue = _PathNodes[i].m_Distance / _TotalLength;
            }
        }


        public Vector3 GetPositionFromNorm(float norm)
        {
            float distance = norm * _TotalLength;
            return GetPosition(distance);
        }

        public Vector3 GetPosition(float dist)
        {
            float distance = Mathf.Clamp(dist, 0, _TotalLength);
            Vector3 pos = Vector3.zero;

            for (int i = 0; i < _PathNodes.Count - 1; i++)
            {
                if (_PathNodes[i].m_Distance <= distance && _PathNodes[i + 1].m_Distance >= distance)
                {
                    float lerp = distance.ScaleTo01(_PathNodes[i].m_Distance, _PathNodes[i + 1].m_Distance);
                    pos = Vector3.Lerp(_PathNodes[i].transform.position, _PathNodes[i + 1].transform.position, lerp);
                }
            }

            return pos;
        }


        void OnDrawGizmos()
        {
            if (_PathNodes.Count > 1)
            {
                for (int i = 0; i < _PathNodes.Count - 1; i++)
                {
                    Gizmos.DrawLine(_PathNodes[i].transform.position, _PathNodes[i + 1].transform.position);
                }
            }
        }
    }
}


