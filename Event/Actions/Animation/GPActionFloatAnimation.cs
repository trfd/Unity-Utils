using UnityEngine;
using System.Collections;

namespace Utils.Event
{
	[System.Serializable]
	[GPActionAlias("Animation/Float")]
	public class GPActionFloatAnimation : GPAction 
	{
		#region Public Members

		public Component _component;

		public DataMemberWrapper _member;

		public float _duration;

		public RandomAnimationCurve _curve;

		#endregion

		#region GPAction Override

		protected void OnTrigger()
		{

		}

		#endregion
	}

}