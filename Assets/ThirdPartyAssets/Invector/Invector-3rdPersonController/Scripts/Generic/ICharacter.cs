using UnityEngine;
using System.Collections;

namespace Invector
{
    public interface ICharacter
    {
        void ResetRagdoll();
        void EnableRagdoll();
        void RagdollGettingUp();
    }
}