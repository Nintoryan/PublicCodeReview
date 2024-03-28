using System;
using Random = UnityEngine.Random;

namespace Data.Scripts.Audio
{
	[Serializable]
	public struct RangedFloat
	{
		public float MinValue;
		public float MaxValue;

		public float GetRandomValue()
		{
			return Random.Range(MinValue, MaxValue);
		}
	}
}