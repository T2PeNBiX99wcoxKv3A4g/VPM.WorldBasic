using System;
using UnityEngine;
using VRC.SDKBase;

namespace io.github.ykysnk.WorldBasic
{
    [AttributeUsage(AttributeTargets.Field)]
    public class UniqueIDAttribute : PropertyAttribute, IEditorOnly
    {
    }
}