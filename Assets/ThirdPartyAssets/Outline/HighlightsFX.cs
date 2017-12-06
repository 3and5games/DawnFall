using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;

[RequireComponent(typeof(Camera))]
public class HighlightsFX : MonoBehaviour 
{
	#region enums

	public enum HighlightType
	{
		Glow = 0,
		Solid = 1
	}

	public enum SortingType
	{
		Overlay = 3,
		DepthFiltered = 4,
	}

    public enum DepthInvertPass
    {
        StencilMapper = 5,
        StencilDrawer = 6
    }

	public enum FillType
	{
		Fill,
		Outline
	}
	public enum RTResolution
	{
		Quarter = 4,
		Half = 2,
		Full = 1
	}

    public enum BlurType
    {
        StandardGauss = 0,
        SgxGauss = 1,
    }

    public struct OutlineData
    {
        public Color color;
        public SortingType sortingType;
    }

    #endregion

    #region public vars

    [Header("Outline Settings")]

    public HighlightType m_selectionType = HighlightType.Glow;
	public FillType m_fillType = FillType.Outline;
	public RTResolution m_resolution = RTResolution.Full;
    [Range(0f, 1f)]
    public float m_controlValue = 0.5f;
    public CameraEvent BufferDrawEvent = CameraEvent.BeforeImageEffects;

    [Header("BlurOptimized Settings")]

    public BlurType blurType = BlurType.StandardGauss;
    [Range(0, 2)]
    public int downsample = 0;
    [Range(0.0f, 10.0f)]
    public float blurSize = 3.0f;
    [Range(1, 4)]
    public int blurIterations = 2;
    

    #endregion

    #region private field

    private CommandBuffer m_commandBuffer;

    private int m_highlightRTID, m_blurredRTID, m_temporaryRTID;

    private Dictionary<List<Renderer>, OutlineData> m_objectRenderers;
    private List<List<Renderer>> m_objectExcluders;

    private Material m_highlightMaterial, m_blurMaterial;		
    private Camera m_camera;

	private int m_RTWidth = 512;
	private int m_RTHeight = 512;

    private RenderTexture m_highlightRT, m_blurredRT, m_temporaryRT;

    #endregion

    public void AddRenderers(List<Renderer> renderers, Color col, SortingType sorting)
    {
        var data = new OutlineData() { color = col, sortingType = sorting };
        m_objectRenderers.Add(renderers, data);      
        RecreateCommandBuffer();
    }

    public void RemoveRenderers(List<Renderer> renderers)
    {
        m_objectRenderers.Remove(renderers);      
        RecreateCommandBuffer();
    }

    public void AddExcluders(List<Renderer> renderers)
    {
        m_objectExcluders.Add(renderers);
        RecreateCommandBuffer();
    }

    public void RemoveExcluders(List<Renderer> renderers)
    {
        m_objectExcluders.Remove(renderers);
        RecreateCommandBuffer();
    }

    public void ClearOutlineData()
    {
        m_objectRenderers.Clear();
        m_objectExcluders.Clear();
        RecreateCommandBuffer();
    }

    private void Awake()
	{
        m_objectRenderers = new Dictionary<List<Renderer>, OutlineData>();
        m_objectExcluders = new List<List<Renderer>>();

        m_commandBuffer = new CommandBuffer();
        m_commandBuffer.name = "HighlightFX Command Buffer";

        m_highlightRTID = Shader.PropertyToID("_HighlightRT");
        m_blurredRTID = Shader.PropertyToID("_BlurredRT");
        m_temporaryRTID = Shader.PropertyToID("_TemporaryRT");

        m_RTWidth = (int)(Screen.width / (float)m_resolution);
        m_RTHeight = (int)(Screen.height / (float)m_resolution);

        m_highlightMaterial = new Material(Shader.Find("Custom/Highlight"));
        m_blurMaterial = new Material(Shader.Find("Hidden/FastBlur"));

        m_camera = GetComponent<Camera>();
        m_camera.depthTextureMode = DepthTextureMode.Depth;
        m_camera.AddCommandBuffer(BufferDrawEvent, m_commandBuffer);
	}

    private void RecreateCommandBuffer()
    {
        m_commandBuffer.Clear();

        if (m_objectRenderers.Count == 0)
            return;

        // initialization

        m_commandBuffer.GetTemporaryRT(m_highlightRTID, m_RTWidth, m_RTHeight, 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32);
        m_commandBuffer.SetRenderTarget(m_highlightRTID, BuiltinRenderTextureType.CurrentActive);
        m_commandBuffer.ClearRenderTarget(false, true, Color.clear);

        // rendering into texture

        foreach (var collection in m_objectRenderers)
        {
            m_commandBuffer.SetGlobalColor("_Color", collection.Value.color);
            foreach (var render in collection.Key)
            {
                m_commandBuffer.DrawRenderer(render, m_highlightMaterial, 0, (int)collection.Value.sortingType);
            }
        }

        // excluding from texture 

        m_commandBuffer.SetGlobalColor("_Color", Color.clear);
        foreach (var collection in m_objectExcluders)
        {         
            foreach (var render in collection)
            {
                m_commandBuffer.DrawRenderer(render, m_highlightMaterial, 0, (int) SortingType.Overlay);
            }
        }

        // Bluring texture

        float widthMod = 1.0f / (1.0f * (1 << downsample));

        int rtW = m_RTWidth >> downsample;
        int rtH = m_RTHeight >> downsample;
   
        m_commandBuffer.GetTemporaryRT(m_blurredRTID, rtW, rtH, 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32);
        m_commandBuffer.GetTemporaryRT(m_temporaryRTID, rtW, rtH, 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32);

        m_commandBuffer.Blit(m_highlightRTID, m_temporaryRTID, m_blurMaterial, 0);

        var passOffs = blurType == BlurType.StandardGauss ? 0 : 2;

        for (int i = 0; i < blurIterations; i++)
        {
            float iterationOffs = (i * 1.0f);
            var blurHorizParam = blurSize * widthMod + iterationOffs;
            var blurVertParam = -blurSize * widthMod - iterationOffs;

            m_commandBuffer.SetGlobalVector("_Parameter", new Vector4(blurHorizParam, blurVertParam));

            m_commandBuffer.Blit(m_temporaryRTID, m_blurredRTID, m_blurMaterial, 1 + passOffs);
            m_commandBuffer.Blit(m_blurredRTID, m_temporaryRTID, m_blurMaterial, 2 + passOffs);
        }

        // occlusion

        if (m_fillType == FillType.Outline)
        {
            // Excluding the original image from the blurred image, leaving out the areal alone
            m_commandBuffer.SetGlobalTexture("_SecondaryTex", m_highlightRTID);
            m_commandBuffer.Blit(m_temporaryRTID, m_blurredRTID, m_highlightMaterial, 2);

            m_commandBuffer.SetGlobalTexture("_SecondaryTex", m_blurredRTID);
        }
        else
        {
            m_commandBuffer.SetGlobalTexture("_SecondaryTex", m_temporaryRTID);
        }

        // back buffer
        m_commandBuffer.Blit(BuiltinRenderTextureType.CameraTarget, m_highlightRTID);

        // overlay
        m_commandBuffer.SetGlobalFloat("_ControlValue", m_controlValue);
        m_commandBuffer.Blit(m_highlightRTID, BuiltinRenderTextureType.CameraTarget, m_highlightMaterial, (int)m_selectionType);

        m_commandBuffer.ReleaseTemporaryRT(m_temporaryRTID);
        m_commandBuffer.ReleaseTemporaryRT(m_blurredRTID);
        m_commandBuffer.ReleaseTemporaryRT(m_highlightRTID);
    }
}
