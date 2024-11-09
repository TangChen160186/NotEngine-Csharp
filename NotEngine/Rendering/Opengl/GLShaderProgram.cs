using System.Numerics;
using OpenTK.Graphics.OpenGL;

namespace NotEngine.Rendering.Opengl;

internal sealed class GLShaderProgram : ShaderProgram
{
    internal int Id { get; private set; }

    public override bool IsDisposed { get; protected set; }

    private readonly Dictionary<string, int> _uniformLocationCache = [];

    internal GLShaderProgram(string shaderSource)
    {
        ParseShader(shaderSource, out var vertexShaderSource, out var fragmentShaderSource);
        Id = CreateProgram(vertexShaderSource, fragmentShaderSource);
    }

    public override void Bind() => GL.UseProgram(Id);
    public override void UnBind() => GL.UseProgram(0);

    #region Get Set Uniform

    public override void GetUniform(string name, out int res) =>
        GL.GetUniformi(Id, GetUniformLocation(name), out res);

    public override void GetUniform(string name, out float res) =>
        GL.GetUniformf(Id, GetUniformLocation(name), out res);

    public override void GetUniform(string name, out double res) =>
        GL.GetUniformd(Id, GetUniformLocation(name), out res);


    public override unsafe void GetUniform(string name, out Vector2 res)
    {
        Vector2 temp;
        GL.GetUniformfv(Id, GetUniformLocation(name), (float*)&temp);
        res = temp;
    }

    public override unsafe void GetUniform(string name, out Vector3 res)
    {
        Vector3 temp;
        GL.GetUniformfv(Id, GetUniformLocation(name), (float*)&temp);
        res = temp;
    }

    public override unsafe void GetUniform(string name, out Vector4 res)
    {
        Vector4 temp;
        GL.GetUniformfv(Id, GetUniformLocation(name), (float*)&temp);
        res = temp;
    }

    public override unsafe void GetUniform(string name, out Matrix4x4 res)
    {
        Matrix4x4 temp;
        GL.GetUniformfv(Id, GetUniformLocation(name), (float*)&temp);
        res = temp;
    }
    public override void SetUniform(string name, bool value) => GL.ProgramUniform1i(Id, GetUniformLocation(name), value ? 1 : 0);

    public override void SetUniform(string name, int value) => GL.ProgramUniform1i(Id, GetUniformLocation(name), value);

    public override void SetUniform(string name, uint value) => GL.ProgramUniform1ui(Id, GetUniformLocation(name), value);


    public override void SetUniform(string name, float value) => GL.ProgramUniform1f(Id, GetUniformLocation(name), value);

    public override unsafe void SetUniform(string name, Vector2 value) =>
        GL.ProgramUniform2f(Id, GetUniformLocation(name), value[0], value[1]);


    public override unsafe void SetUniform(string name, Vector3 value) =>
        GL.ProgramUniform3f(Id, GetUniformLocation(name), value[0], value[1], value[2]);


    public override void SetUniform(string name, Vector4 value) =>
        GL.ProgramUniform4f(Id, GetUniformLocation(name), value[0], value[1], value[2], value[3]);

    public override void SetUniform(string name, Matrix4x4 value) =>
        GL.ProgramUniformMatrix4f(Id, GetUniformLocation(name), 1, false, ref value);

    public override void SetTextureHandle(string name, ulong textureHandle)
    {
        GL.ARB.UniformHandleui64ARB(GetUniformLocation(name), textureHandle);
    }

    #endregion
    public override List<UniformInfo> QueryUniforms()
    {
        List<UniformInfo> uniforms = [];
        GL.GetProgrami(Id, ProgramProperty.ActiveUniforms, out var numActiveUniforms);
        for (uint uniformIndex = 0; uniformIndex < numActiveUniforms; ++uniformIndex)
        {
            GL.GetActiveUniform(Id, uniformIndex, 256, out var length,
                out var size, out var uniformType,
                out var name);
            switch (uniformType)
            {
                case UniformType.Bool:
                    GetUniform(name, out int b);
                    uniforms.Add(new UniformInfo()
                    {
                        Type = EUniformType.Bool,
                        Name = name,
                        Value = b != 0,
                        Location = GetUniformLocation(name)
                    });
                    break;
                case UniformType.Int:
                    GetUniform(name, out int i);
                    uniforms.Add(new UniformInfo()
                    {
                        Type = EUniformType.Int,
                        Name = name,
                        Value = i,
                        Location = GetUniformLocation(name)
                    });
                    break;
                case UniformType.Float:
                    GetUniform(name, out float f);
                    uniforms.Add(new UniformInfo()
                    {
                        Type = EUniformType.Float,
                        Name = name,
                        Value = f,
                        Location = GetUniformLocation(name)
                    });
                    break;
                case UniformType.FloatVec2:
                    GetUniform(name, out Vector2 v2);
                    uniforms.Add(new UniformInfo()
                    {
                        Type = EUniformType.V2,
                        Name = name,
                        Value = v2,
                        Location = GetUniformLocation(name)
                    });
                    break;
                case UniformType.FloatVec3:
                    GetUniform(name, out Vector3 v3);
                    uniforms.Add(new UniformInfo()
                    {
                        Type = EUniformType.V3,
                        Name = name,
                        Value = v3,
                        Location = GetUniformLocation(name)
                    });
                    break;
                case UniformType.FloatVec4:
                    GetUniform(name, out Vector4 v4);
                    uniforms.Add(new UniformInfo()
                    {
                        Type = EUniformType.V4,
                        Name = name,
                        Value = v4,
                        Location = GetUniformLocation(name)
                    });
                    break;
                case UniformType.Sampler2d:
                    uniforms.Add(new UniformInfo()
                    {
                        Type = EUniformType.Texture2D,
                        Name = name,
                        Value = null,
                        Location = GetUniformLocation(name)
                    });
                    break;
                default:
                    break;
            }
        }

        return uniforms;
    }

    private int GetUniformLocation(string name)
    {
        if (_uniformLocationCache.TryGetValue(name, out var uniformLocation))
            return uniformLocation;

        var location = GL.GetUniformLocation(Id, name);
        //if (location == -1)
        //    throw new Exception("Failed to lookup variable: " + name);

        return _uniformLocationCache[name] = location;
    }

    private static int CreateProgram(string vertexShader, string fragmentShader)
    {
        int program = GL.CreateProgram();
        int vs = CompileShader(ShaderType.VertexShader, vertexShader);
        int fs = CompileShader(ShaderType.FragmentShader, fragmentShader);
        GL.AttachShader(program, vs);
        GL.AttachShader(program, fs);
        GL.LinkProgram(program);

        GL.GetProgrami(program, ProgramProperty.LinkStatus, out int linkStatus);

        if (linkStatus == 0)
        {
            GL.GetProgramInfoLog(program, out string errorLog);
            throw new Exception("Failed to link shaders " + Environment.NewLine + errorLog);
        }
        GL.ValidateProgram(program);
        GL.DeleteShader(vs);
        GL.DeleteShader(fs);
        return program;

    }
    private static int CompileShader(ShaderType type, string source)
    {
        int shaderHandle = GL.CreateShader(type);
        GL.ShaderSource(shaderHandle, source);
        GL.CompileShader(shaderHandle);

        GL.GetShaderi(shaderHandle, ShaderParameterName.CompileStatus, out int compileStatus);
        if (compileStatus == 0)
        {
            GL.GetShaderInfoLog(shaderHandle, out var errorLog);
            throw new Exception("Failed to compile shader: " + Environment.NewLine + errorLog);
        }
        return shaderHandle;
    }

    public override void Dispose()
    {
        if (!IsDisposed && Id != 0)
        {
            GL.DeleteShader(Id);
            IsDisposed = true;
            Id = 0;
        }
    }
}