using System.Collections;
using UnityEngine;

namespace Util
{
    public interface ICoroutineRunner
    {
        Coroutine StartCoroutine(IEnumerator coroutine);
    }
}