using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics.Vulkan;
using System;
using Buffer = OpenTK.Graphics.OpenGL.Buffer;

namespace NotEngine.Rendering.Opengl;

internal class GLDevice : GraphicsDevice
{
    public override bool IsDisposed { get; protected set; }

    public override void Dispose()
    {
        throw new NotImplementedException();
    }

    public override VertexBuffer CreateVertexBuffer(float[] data, params VertexBufferLayout[] layouts)
    {
        return new GLVertexBuffer(data, layouts);
    }

    public override IndexBuffer CreateIndexBuffer(uint[] data)
    {
        return new GLIndexBuffer(data);
    }

    public override VertexArray CreateVertexArray()
    {
        return new GLVertexArray();
    }

    public override UniformBuffer CreateUniformBuffer(int size, uint bindingPoint = 0)
    {
        return new GLUniformBuffer(size, bindingPoint);
    }

    public override ShaderStorageBuffer CreateShaderStorageBuffer(uint bindingIndex)
    {
        return new GLShaderStorageBuffer(bindingIndex);
    }

    public override ShaderProgram CreateShaderProgram(string shaderSource)
    {
        return new GLShaderProgram(shaderSource);
    }

    public override GraphicTexture2D CreateGraphicTexture2D(int width, int height, TextureInternalFormat format,
        bool genMipmap)
    {
        return new GlGraphicTexture2D(width, height, format, genMipmap);
    }

    public override FrameBuffer CreateFrameBuffer(int width, int height, GraphicTexture2D depthAttachment,
        GraphicTexture2D[]? colorAttachments)
    {
        return new GLFrameBuffer(width, height, depthAttachment, colorAttachments);
    }

    public override unsafe void ClearColorTarget(int index, float r, float g, float b, float a)
    {
        int frameBufferId;
        GL.GetIntegerv(GetPName.DrawFramebufferBinding, &frameBufferId);
        GL.ClearColor(r, g, b, a);
        GL.ClearNamedFramebufferf(frameBufferId, Buffer.Color, index, [r, g, b, a]);
    }

    public override unsafe void ClearDepthStencil(float depth, uint stencil)
    {
        int frameBufferId;
        GL.GetIntegerv(GetPName.DrawFramebufferBinding, &frameBufferId);
        GL.ClearNamedFramebufferf(frameBufferId, Buffer.Depth, 0, ref depth);
        GL.ClearNamedFramebufferui(frameBufferId, Buffer.Stencil, 0, ref stencil);
    }

    public override void ClearColorTarget(FrameBuffer frameBuffer, int index, float r, float g, float b, float a)
    {
        GLFrameBuffer glFrameBuffer = (frameBuffer as GLFrameBuffer)!;
        GL.ClearColor(r, g, b, a);
        GL.ClearNamedFramebufferf(glFrameBuffer.Id, Buffer.Color, index, [r, g, b, a]);
    }

    public override void ClearDepthStencil(FrameBuffer frameBuffer, float depth, uint stencil)
    {
        GLFrameBuffer glFrameBuffer = (frameBuffer as GLFrameBuffer)!;
        GL.ClearNamedFramebufferf(glFrameBuffer.Id, Buffer.Depth, 0, ref depth);
        GL.ClearNamedFramebufferui(glFrameBuffer.Id, Buffer.Stencil, 0, ref stencil);
    }

    public override void SetViewport(uint index, float x, float y, float width, float height)
    {
        GL.ViewportIndexedf(index, x, y, width, height);
    }

    public override void SetRasterizerState(RasterizerState state)
    {
        // Alpha to Coverage
        if (state.AlphaToMask)
            GL.Enable(EnableCap.SampleAlphaToCoverage);
        else
            GL.Disable(EnableCap.SampleAlphaToCoverage);

        // Blending
        if (state.Blendable)
        {
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc((BlendingFactor)state.SourceBlend, (BlendingFactor)state.DestinationBlend);
            GL.BlendEquation((BlendEquationMode)state.BlendOp);
        }
        else
        {
            GL.Disable(EnableCap.Blend);
        }

        // Color Mask
        GL.ColorMask(state.ColorWriteR, state.ColorWriteG, state.ColorWriteB, state.ColorWriteA);


        // Culling
        GL.CullFace((TriangleFace)state.Cull);

        // Depth Test
        if (state.DepthTest)
        {
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc((DepthFunction)state.ZTest);
        }
        else
        {
            GL.Disable(EnableCap.DepthTest);
        }

        // Depth Writing
        GL.DepthMask(state.ZWrite);

        // Depth Clipping
        if (state.ZClip)
            GL.Enable(EnableCap.DepthClamp);
        else
            GL.Disable(EnableCap.DepthClamp);

        // Polygon Offset
        if (state.OffsetFactor != 0.0f || state.OffsetUnits != 0.0f)
        {
            GL.Enable(EnableCap.PolygonOffsetFill);
            GL.PolygonOffset(state.OffsetFactor, state.OffsetUnits);
        }
        else
        {
            GL.Disable(EnableCap.PolygonOffsetFill);
        }

        // Stencil
        GL.Enable(EnableCap.StencilTest);
        GL.StencilFunc((StencilFunction)state.StencilFunc, state.StencilRef, state.StencilMask);
        GL.StencilOp((StencilOp)state.StencilFail, (StencilOp)state.StencilZFail, (StencilOp)state.StencilPass);
    }

    public override void DrawArrays(EPrimitiveType primitiveType, int first, int count)
    {
        GL.DrawArrays((PrimitiveType)primitiveType, first, count);
    }

    public override unsafe void DrawIndex(EPrimitiveType primitiveType, int indexCount, bool index32Bit, void* value)
    {
        GL.DrawElements((PrimitiveType)primitiveType, indexCount,
            index32Bit ? DrawElementsType.UnsignedInt : DrawElementsType.UnsignedShort, value);
    
    }

    public override byte[] ReadPixel(uint index,int x,int y,int width,int height)
    {
        byte[] pixelData = new byte[width * height * 4];
        GL.ReadBuffer(ReadBufferMode.ColorAttachment0 + index);
        GL.ReadPixels(x,y,width,height,PixelFormat.Rgba,PixelType.UnsignedByte, pixelData);
        return pixelData;
    }
}