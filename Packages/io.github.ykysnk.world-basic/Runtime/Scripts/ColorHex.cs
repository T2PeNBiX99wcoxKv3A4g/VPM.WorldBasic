using System;
using UnityEngine;

namespace io.github.ykysnk.WorldBasic
{
    [Serializable]
    public struct ColorHex
    {
        public Color color;
        public string hex;

        public override string ToString() => $"Color: {color}, Hex: {hex}";

        public static implicit operator string(ColorHex colorHex) => colorHex.hex;
        public static implicit operator Color(ColorHex colorHex) => colorHex.color;

        public static implicit operator ColorHex(string hex)
        {
            var getColor = ColorUtility.TryParseHtmlString(hex, out var color) ? color : Color.white;
            var getHex = $"#{ColorUtility.ToHtmlStringRGBA(getColor)}";

            return new()
            {
                color = getColor,
                hex = getHex
            };
        }

        public static implicit operator ColorHex(Color color) => new()
        {
            color = color,
            hex = $"#{ColorUtility.ToHtmlStringRGBA(color)}"
        };
    }
}