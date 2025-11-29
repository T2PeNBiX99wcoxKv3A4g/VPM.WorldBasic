using System;
using UnityEngine;
using VRC.SDKBase;

namespace io.github.ykysnk.WorldBasic
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ColorHexAttribute : PropertyAttribute, IEditorOnly
    {
    }
}