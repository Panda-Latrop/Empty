using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility
{

    public static Vector3 ClampRotation(Vector3 vector, float min, float max)
    {
        vector.x = Mathf.Clamp(vector.x, min, max);
        vector.y = ClampRotation(vector.y);
        vector.z = ClampRotation(vector.z);
        return vector;
    }
	public static Vector3 ClampRotation(Vector3 vector)
	{
		vector.x = ClampRotation(vector.x);
		vector.y = ClampRotation(vector.y);
		vector.z = ClampRotation(vector.z);
		return vector;
	}
	public static float ClampRotation(float angle)
    {
        if (angle > 180.0f)
            angle -= 360.0f;
        else if (angle <= -180.0f)
            angle += 360.0f;
        return angle;
    }

	public static Vector3 CharacterRotateToDirection(Vector3 rotation, Vector3 direction, float displacement)
    {
		float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
		rotation.y = Rotate(rotation.y, angle, displacement);
		return rotation;
    }


	public static float Rotate(float from, float to, float displacement)
    {
		if (from != to)
		{
			if (from - to > 180f || from - to < -180f)
			{
				if (from > 0f && from < 180f)
				{
					from += displacement;
					if (from >= 180f)
					{
						from -= 360.0f;
						if (from >= to)
							from = to;
					}
				}
				else if(from < 0f && from >= -180f)
				{
					from -= displacement;
					if (from <= -180f)
					{
						from += 360.0f;
						if (from <= to)
							from = to;
					}
				}
			}
			else
			{
				if (from < to)
				{
					from += displacement;
					if (from >= to)
						from = to;
				}
				else
				{
					from -= displacement;
					if (from <= to)
						from = to;
				}
			}
		}
		return from;
	}
}
