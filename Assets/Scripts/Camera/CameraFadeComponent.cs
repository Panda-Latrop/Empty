using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFadeComponent : MonoBehaviour
{
	[Range(0.0f, 1.0f)]
	[SerializeField]
	protected float alpha = 1.0f;
	protected Material material;
	protected Color color = new Color(0, 0, 0, 0);
	protected int materialColorID;
	[Range(0, 3)]
	[SerializeField]
	protected int fadeState = 0;
	[SerializeField]
	protected float fadeSpeed = 1.0f;
	public bool IsFading => (fadeState != 0 && fadeState != 1);
	/* 0 - Tran
	 * 1 - Balck
	 * 2 - Out
	 * 3 - In
	 */

	protected void Awake()
	{
		materialColorID = Shader.PropertyToID("_Color");
		var shader = Shader.Find("Hidden/Internal-Colored");
		material = new Material(shader);
		material.hideFlags = HideFlags.HideAndDontSave;
		material.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
		material.SetInt("_ZWrite", 0);
		material.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.Always);
		material.SetColor(materialColorID, color);
	}
	public void Out(float fadeSpeed = 1.0f)
	{
		this.fadeSpeed = fadeSpeed;
		fadeState = 2;
	}
	public void In(float fadeSpeed = 1.0f)
	{
		this.fadeSpeed = fadeSpeed;
		this.fadeState = 3;
	}

    protected void Update()
    {

        switch (fadeState)
        {
			case 2:
				alpha += fadeSpeed * Time.deltaTime;
				if (alpha >= 1)
				{
					alpha = 1;
					fadeState = 1;
				}
				break;
			case 3:
				alpha -= fadeSpeed * Time.deltaTime;
				if (alpha <= 0)
				{
					alpha = 0;
					fadeState = 0;
				}
				break;
			default:
                break;
        }
	}
	public void OnPostRender()
	{
		if (fadeState != 0)
		{
			if (fadeState != 1)
			{
				color.a = alpha;
				material.SetColor(materialColorID, color);
			}
			GL.PushMatrix();
			GL.LoadOrtho();
			material.SetPass(0);
			GL.Begin(GL.QUADS);
			GL.Vertex3(0, 0, 0);
			GL.Vertex3(1, 0, 0);
			GL.Vertex3(1, 1, 0);
			GL.Vertex3(0, 1, 0);
			GL.End();
			GL.PopMatrix();
		}
	}
}
