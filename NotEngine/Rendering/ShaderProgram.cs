using System.Numerics;

namespace NotEngine.Rendering;

public enum EUniformType
{
    Bool,
    Int,
    Float,
    V2,
    V3,
    V4,
    Texture2D,
}

public class UniformInfo
{
    public EUniformType Type;
    public string Name;
    public int Location;
    public object? Value;
}

public abstract class ShaderProgram : IDisposable
{
    private const string VERTEX_TOKEN = "#shader vertex";
    private const string FRAGMENT_TOKEN = "#shader fragment";
    public abstract bool IsDisposed { get; protected set; }
    public abstract void Dispose();
    public abstract void Bind();
    public abstract void UnBind();

    public abstract void GetUniform(string name, out int res);
    public abstract void GetUniform(string name, out float res);
    public abstract void GetUniform(string name, out double res);
    public abstract void GetUniform(string name, out Vector2 res);
    public abstract void GetUniform(string name, out Vector3 res);
    public abstract void GetUniform(string name, out Vector4 res);
    public abstract void GetUniform(string name, out Matrix4x4 res);


    public abstract void SetUniform(string name, bool value);
    public abstract void SetUniform(string name, int value);
    public abstract void SetUniform(string name, uint value);
    public abstract void SetUniform(string name, float value);

    public abstract void SetUniform(string name, Vector2 value);
    public abstract void SetUniform(string name, Vector3 value);
    public abstract void SetUniform(string name, Vector4 value);
    public abstract void SetUniform(string name, Matrix4x4 value);
    public abstract void SetTextureHandle(string name, ulong textureHandle);

    public abstract List<UniformInfo> QueryUniforms();
    protected void ParseShader(string source, out string vertexShader, out string fragmentShader)
    {
        // 查找vertex和fragment shader的起始位置
        int vertexPos = source.IndexOf(VERTEX_TOKEN, StringComparison.OrdinalIgnoreCase);
        int fragmentPos = source.IndexOf(FRAGMENT_TOKEN, StringComparison.OrdinalIgnoreCase);

        if (vertexPos != -1 && fragmentPos != -1)
        {
            // 提取vertex shader
            vertexShader = source.Substring(vertexPos + VERTEX_TOKEN.Length, fragmentPos - vertexPos - VERTEX_TOKEN.Length);

            // 提取fragment shader
            fragmentShader = source.Substring(fragmentPos + FRAGMENT_TOKEN.Length);
        }
        else
        {
            vertexShader = "";
            fragmentShader = "";
        }
    }


}