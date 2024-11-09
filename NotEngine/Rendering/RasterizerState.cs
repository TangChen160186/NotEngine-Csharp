namespace NotEngine.Rendering;


public class RasterizerState
{
    public bool AlphaToMask { get; set; } = false;
    public bool Blendable { get; set; } = false;
    public EBlendingFactor SourceBlend { get; set; } = EBlendingFactor.One;
    public EBlendingFactor DestinationBlend { get; set; } = EBlendingFactor.Zero;
    public EBlendEquationMode BlendOp { get; set; } = EBlendEquationMode.FuncAdd;

    public bool ColorWriteR { get; set; } = true;
    public bool ColorWriteG { get; set; } = true;
    public bool ColorWriteB { get; set; } = true;
    public bool ColorWriteA { get; set; } = true;

    public ETriangleFace Cull { get; set; } = ETriangleFace.Back;

    public bool DepthTest { get; set; } = true;
    public EDepthFunction ZTest { get; set; } = EDepthFunction.Lequal;
    public bool ZWrite { get; set; } = true;
    public bool ZClip { get; set; } = true;

    public bool DepthWriting { get; set; } = true;

    public float OffsetFactor { get; set; } = 0.0f;
    public float OffsetUnits { get; set; } = 0.0f;

    public EStencilFunction StencilFunc { get; set; } = EStencilFunction.Always;
    public int StencilRef { get; set; } = 0;
    public uint StencilMask { get; set; } = 0xFF;
    public EStencilOp StencilFail { get; set; } = EStencilOp.Keep;
    public EStencilOp StencilZFail { get; set; } = EStencilOp.Keep;
    public EStencilOp StencilPass { get; set; } = EStencilOp.Keep;
}