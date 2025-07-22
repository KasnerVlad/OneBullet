using System.Collections.Generic;
using UnityEngine;

namespace Project.Scripts.Reflector
{
    public class ReflectorMoveSurfaces : MonoBehaviour
    {
        [SerializeField] private List<BoxCollider> _surfaces;
        public List<BoxCollider> Surfaces { get => _surfaces; set => _surfaces = value; }
    }
}