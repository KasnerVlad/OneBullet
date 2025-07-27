using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace Project.Scripts.Custom
{
    [Serializable]
    public class LoggerData
    {
        [Serializable]
        public class Vector3List
        {
            
            public List<Vector3> values;

            public void Add(Vector3 value)
            {
                values.Add(value);
            }

            public Vector3List(List<Vector3> values)
            {
                this.values = values;
            }
            public Vector3List()
            {
                values = new List<Vector3>();
            }
            public static implicit operator List<Vector3>(Vector3List value)
            {
                return value.values;
            }
            public static implicit operator Vector3List( List<Vector3> value)
            {
                return new Vector3List(value);
            }
        }

        public List<float> keys = new List<float>();
        public List<Vector3List> values = new List<Vector3List>();

        public void Add(float time, List<Vector3> point)
        {
            
            Debug.Log("added");
            keys.Add(time);
            values.Add(point);
        }
    }
    
}