using UnityEngine;
using System.Collections;

public class CameraNavigation : MonoBehaviour {
	
	public float touchSensitivityX = 0.2F;
	public float touchSensitivityY = 0.2F;
	public float touchSensitivityMove = 5.0F;
	public float mouseSensitivityX = 8F;
	public float mouseSensitivityY = 8F;
	public float speed = 1000F;
	public float speedBoost = 4F;

	float mHdg = 0F;
	float mPitch = 0F;

	void Update()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer) // IPhone input.
		{
			if(Input.touchCount > 0)
			{
				Touch touch = Input.touches[0];
			
				if(touch.phase == TouchPhase.Moved)
				{
					if(Input.touchCount == 1) // One finger.
					{
						// Change the camera orientation based on the finger move.
						ChangeHeading(touch.deltaPosition.x * touchSensitivityX);
						ChangePitch(touch.deltaPosition.y * touchSensitivityY);
					}
					else
					if(Input.touchCount == 2) // Two fingers.
					{
						MoveForwards((touch.deltaPosition.x+touch.deltaPosition.y) * touchSensitivityMove);
					}
				}
			}
		}
		else // Desktop input.
		{
			// Mouse look...
			if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
			{
				float deltaX = Input.GetAxis("Mouse X") * mouseSensitivityX;
				float deltaY = Input.GetAxis("Mouse Y") * mouseSensitivityY;
				
				if (Input.GetMouseButton(1)) // Strafe and up-down.
				{
					Strafe(2F * deltaX);
					ChangeHeight(2F * deltaY);
				}
				else
				{
					if (Input.GetMouseButton(0)) // Mouse look.
					{
						ChangeHeading(deltaX);
						ChangePitch(-deltaY);
					}
				}
			}
			
			float increase_speed = 1F;
			if(Input.GetKey("right shift") || Input.GetKey("left shift"))
				increase_speed = speedBoost;

            // Move by keys...
            float z = Input.GetAxis("Vertical") * Time.deltaTime * speed;
            if (z != 0F)
            {
                MoveForwards(z * increase_speed);
            }
            float x = Input.GetAxis("Horizontal") * Time.deltaTime * speed;
            if (x != 0F)
            {
                Strafe(x * increase_speed);
            }

        }
    }

	void MoveForwards(float aVal)
	{
		Vector3 fwd = transform.forward;
		transform.position += aVal * fwd;
	}

	void Strafe(float aVal)
	{
		transform.position += aVal * transform.right;
	}

	void ChangeHeight(float aVal)
	{
		transform.position += aVal * Vector3.up;
	}

	void ChangeHeading(float aVal)
	{
		mHdg += aVal;
		WrapAngle(ref mHdg);
		transform.localEulerAngles = new Vector3(mPitch, mHdg, 0);
	}

	void ChangePitch(float aVal)
	{
		mPitch += aVal;
		WrapAngle(ref mPitch);
		transform.localEulerAngles = new Vector3(mPitch, mHdg, 0);
	}

	public static void WrapAngle(ref float angle)
	{
		if (angle < -360F)
			angle += 360F;
		if (angle > 360F)
			angle -= 360F;
	}
}