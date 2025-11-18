using io.github.ykysnk.WorldBasic.Udon;
using UnityEngine;

namespace Test
{
    public class Test : BasicUdonSharpBehaviour
    {
        [SerializeField] private string test;

        public override void Interact() => InteractCheck();

        protected override void OnChange()
        {
            test = "1234567890";
        }

        protected override void InteractAntiCheat()
        {
            Log($"Test {GetType().Name}");
            Synchronize();
        }

        protected override void AfterSynchronize(bool isOwner)
        {
            Log($"Test AfterSynchronize {isOwner}");
        }
    }
}