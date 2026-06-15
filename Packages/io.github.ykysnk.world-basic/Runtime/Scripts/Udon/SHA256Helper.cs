using System;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using UdonSharp;
#if !COMPILER_UDONSHARP && UNITY_EDITOR
using UnityEngine.SceneManagement;
using UnityEditor.Build.Reporting;
using UnityEditor.Build;
#endif

namespace io.github.ykysnk.WorldBasic.Udon
{
#if !COMPILER_UDONSHARP && UNITY_EDITOR
    public partial class SHA256Helper : IProcessSceneWithReport
    {
        public int callbackOrder => -100;

        public void OnProcessScene(Scene scene, BuildReport report)
        {
            var helpers = FindObjectsOfType<SHA256Helper>();
            var needHelpers = FindObjectsOfType<UdonSharpBehaviour>(true).OfType<ISHA256Helper>().ToArray();
            if (needHelpers.Length > 0)
            {
                if (helpers.Length < 1)
                    throw new Exception("No SHA256Helper found in the scene.");
                if (helpers.Length > 1)
                    throw new Exception("More than one SHA256Helper found in the scene.");
            }

            foreach (var need in needHelpers)
                need.SHA256Helper = helpers.First();
        }
    }
#endif

    [PublicAPI]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public partial class SHA256Helper : UdonSharpBehaviour
    {
        private readonly uint[] _h =
        {
            0x6a09e667, 0xbb67ae85, 0x3c6ef372, 0xa54ff53a, 0x510e527f, 0x9b05688c, 0x1f83d9ab, 0x5be0cd19
        };

        private readonly uint[] _k =
        {
            0x428a2f98, 0x71374491, 0xb5c0fbcf, 0xe9b5dba5, 0x3956c25b, 0x59f111f1, 0x923f82a4, 0xab1c5ed5, 0xd807aa98,
            0x12835b01, 0x243185be, 0x550c7dc3, 0x72be5d74, 0x80deb1fe, 0x9bdc06a7, 0xc19bf174, 0xe49b69c1, 0xefbe4786,
            0x0fc19dc6, 0x240ca1cc, 0x2de92c6f, 0x4a7484aa, 0x5cb0a9dc, 0x76f988da, 0x983e5152, 0xa831c66d, 0xb00327c8,
            0xbf597fc7, 0xc6e00bf3, 0xd5a79147, 0x06ca6351, 0x14292967, 0x27b70a85, 0x2e1b2138, 0x4d2c6dfc, 0x53380d13,
            0x650a7354, 0x766a0abb, 0x81c2c92e, 0x92722c85, 0xa2bfe8a1, 0xa81a664b, 0xc24b8b70, 0xc76c51a3, 0xd192e819,
            0xd6990624, 0xf40e3585, 0x106aa070, 0x19a4c116, 0x1e376c08, 0x2748774c, 0x34b0bcb5, 0x391c0cb3, 0x4ed8aa4a,
            0x5b9cca4f, 0x682e6ff3, 0x748f82ee, 0x78a5636f, 0x84c87814, 0x8cc70208, 0x90befffa, 0xa4506ceb, 0xbef9a3f7,
            0xc67178f2
        };

        public string ComputeSHA256(string input)
        {
            var inputBytes = Encoding.UTF8.GetBytes(input);
            var hashBytes = ComputeHash(inputBytes);

            var result = "";
            foreach (var t in hashBytes)
                result += t.ToString("x2");

            return result;
        }

        public byte[] ComputeHash(byte[] message)
        {
            var msgLen = message.Length;
            var bitLen = (long)msgLen * 8;

            var padLen = msgLen % 64 < 56 ? 64 - msgLen % 64 : 128 - msgLen % 64;
            var paddedMessage = new byte[msgLen + padLen];
            Array.Copy(message, paddedMessage, msgLen);

            paddedMessage[msgLen] = 0x80;

            for (var i = 0; i < 8; i++)
                paddedMessage[paddedMessage.Length - 1 - i] = (byte)(bitLen >> i * 8 & 0xFF);

            var currentH = new uint[8];
            Array.Copy(_h, currentH, 8);

            var w = new uint[64];

            for (var chunk = 0; chunk < paddedMessage.Length / 64; chunk++)
            {
                var offset = chunk * 64;

                for (var t = 0; t < 16; t++)
                {
                    var idx = offset + t * 4;
                    w[t] = (uint)paddedMessage[idx] << 24 |
                           (uint)paddedMessage[idx + 1] << 16 |
                           (uint)paddedMessage[idx + 2] << 8 |
                           paddedMessage[idx + 3];
                }

                for (var t = 16; t < 64; t++)
                {
                    var s0 = RotateRight(w[t - 15], 7) ^ RotateRight(w[t - 15], 18) ^ w[t - 15] >> 3;
                    var s1 = RotateRight(w[t - 2], 17) ^ RotateRight(w[t - 2], 19) ^ w[t - 2] >> 10;
                    w[t] = w[t - 16] + s0 + w[t - 7] + s1;
                }

                var a = currentH[0];
                var b = currentH[1];
                var c = currentH[2];
                var d = currentH[3];
                var e = currentH[4];
                var f = currentH[5];
                var g = currentH[6];
                var h = currentH[7];

                for (var t = 0; t < 64; t++)
                {
                    var s1 = RotateRight(e, 6) ^ RotateRight(e, 11) ^ RotateRight(e, 25);
                    var ch = e & f ^ ~e & g;
                    var temp1 = h + s1 + ch + _k[t] + w[t];
                    var s0 = RotateRight(a, 2) ^ RotateRight(a, 13) ^ RotateRight(a, 22);
                    var maj = a & b ^ a & c ^ b & c;
                    var temp2 = s0 + maj;

                    h = g;
                    g = f;
                    f = e;
                    e = d + temp1;
                    d = c;
                    c = b;
                    b = a;
                    a = temp1 + temp2;
                }

                currentH[0] += a;
                currentH[1] += b;
                currentH[2] += c;
                currentH[3] += d;
                currentH[4] += e;
                currentH[5] += f;
                currentH[6] += g;
                currentH[7] += h;
            }

            var result = new byte[32];
            for (var i = 0; i < 8; i++)
            {
                result[i * 4] = (byte)(currentH[i] >> 24 & 0xFF);
                result[i * 4 + 1] = (byte)(currentH[i] >> 16 & 0xFF);
                result[i * 4 + 2] = (byte)(currentH[i] >> 8 & 0xFF);
                result[i * 4 + 3] = (byte)(currentH[i] & 0xFF);
            }

            return result;
        }

        private static uint RotateRight(uint value, int count) => value >> count | value << 32 - count;
    }
}