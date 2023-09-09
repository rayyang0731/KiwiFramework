using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace KiwiFramework.Runtime.UI.Layer
{
	[RequireComponent(typeof(Canvas)),
	 RequireComponent(typeof(GraphicRaycaster)),
	AddComponentMenu("KiwiUI/RootCanvas")]
	public class RootCanvas : UIBehaviour
	{
		public Canvas Canvas { get; private set; }
		public GraphicRaycaster GraphicRaycaster { get; private set; }

		protected override void Start()
		{
			Canvas           = GetComponent<Canvas>();
			GraphicRaycaster = GetComponent<GraphicRaycaster>();
		}
	}
}