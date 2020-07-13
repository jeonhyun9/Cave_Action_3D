using UnityEngine;

namespace UnityStandardAssets.Utility
{
	public class SmoothFollow : MonoBehaviour
	{
		public float speed = 50;
		[SerializeField]
		private Transform target;
		[SerializeField]
		private float distance = 10.0f;
		[SerializeField]
		private float height = 5.0f;

		private float rotationDamping = 0f;
		private float heightDamping = 3.0f;

		void Start() 
		{

		}

		void LateUpdate()
		{
			if (!target)
				return;

			//일부 상태일 때는 카메라가 돌아가지 않게 한다.
			if (PlayerInput.Instance.state == PlayerInput.PlayerState.START_CASTING
				|| PlayerInput.Instance.state == PlayerInput.PlayerState.END_CASTING
				|| PlayerInput.Instance.state == PlayerInput.PlayerState.HIT
				|| PlayerInput.Instance.state == PlayerInput.PlayerState.DIE
				|| PlayerInput.Instance.state == PlayerInput.PlayerState.ROLL)
			{
				rotationDamping = 0f;
            }
			else{ rotationDamping = 3f; }

			var wantedRotationAngle = target.eulerAngles.y;
			var wantedHeight = target.position.y + height;

			var currentRotationAngle = transform.eulerAngles.y;
			var currentHeight = transform.position.y;

			currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);
			
			currentHeight = Mathf.Lerp(currentHeight, wantedHeight, heightDamping * Time.deltaTime);

			var currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);

			transform.position = target.position;
			transform.position -= currentRotation * Vector3.forward * distance;

			transform.position = new Vector3(transform.position.x, currentHeight, transform.position.z);

			transform.LookAt(target);
			
			transform.eulerAngles += new Vector3(-30, 0, 0);
		}
	}
}