using System.IO;
using System.Numerics;
using MessagePack;
using NotEngine.Assets;
using NotEngine.Editor.Utils;
using NotEngine.Rendering;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace NotEngine.Editor.Modules.ContentExplorer.Imports;

public static class TextureImporter
{
    public static void Import2D(string sourcePath, string targetPath, bool isCompressed, CompressFormat format, bool isSrgb,
        bool genMipMap)
    {
        using Image<Rgba32> baseImage = Image.Load<Rgba32>(sourcePath);

        Texture2D texture = null;
        try
        {
            var mipmaps = MipmapHelper.GenerateMipmaps(baseImage);
            texture = new Texture2D(
                baseImage.Width,
                baseImage.Height,
                isCompressed,
                format,
                isSrgb,
                genMipMap,
                genMipMap ? mipmaps : [mipmaps[0]]
            );
            var bytes = MessagePackSerializer.Serialize(texture);
            File.WriteAllBytes(Path.Combine(ProjectInfo.ProjectPath,targetPath, @$"{Path.GetFileNameWithoutExtension(sourcePath)}.2d"), bytes);
            RenderTexture2D(texture, Path.Combine(ProjectInfo.ThumbnailPath,texture.AssetId.ToString()));
            AssetMap.Instance.AddMetaData(texture.AssetId,
                new MetaData(texture.AssetId, Path.Combine(targetPath, @$"{Path.GetFileNameWithoutExtension(sourcePath)}.2d"),
                sourcePath));
            AssetMap.Instance.Save();
        }
        finally
        {
            texture?.Dispose();
        }
    }

    public static void RenderTexture2D(Texture2D texture,string savePath)
    {
        string shaderSrc = """

                           #shader vertex 
                           #version 460
                           layout(location = 0) in vec3 aPosition;
                           layout(location = 1) in vec2 aTexCoords;
                           out vec2 tex;
                           void main(void)
                           {
                               gl_Position = vec4(aPosition, 1.0);
                               tex = aTexCoords;
                           }

                           #shader fragment 
                           #version 460
                           #extension GL_ARB_bindless_texture: require
                           layout(bindless_sampler) uniform sampler2D yourTexture;
                           uniform int level;
                           out vec4 outputColor;
                           in vec2 tex;
                           void main()
                           {
                               outputColor = textureLod(yourTexture,tex,level);
                           }
                           """;
        GraphicTexture2D depthTexture2D = Graphics.Device.CreateGraphicTexture2D(texture.Width, texture.Height,
            TextureInternalFormat.Depth32fStencil8, false);

        GraphicTexture2D colorTexture2D = Graphics.Device.CreateGraphicTexture2D(texture.Width, texture.Height,
            TextureInternalFormat.RGBA8_UNORM, false);

        FrameBuffer frameBuffer =
            Graphics.Device.CreateFrameBuffer(texture.Width, texture.Height, depthTexture2D, [colorTexture2D]);
        frameBuffer.Bind();
        Graphics.Device.SetViewport(0, 0, 0, texture.Width, texture.Height);
        List<Vertex> vertices =
        [
            new() { Position = new Vector3(1f, 1f, 0.0f),TexCoords = new Vector2( 1.0f, 1.0f)},
            new() { Position = new Vector3(1f, -1f, 0.0f),TexCoords = new Vector2(1.0f, 0.0f) },
            new() { Position = new Vector3(-1f, -1f, 0.0f) ,TexCoords = new Vector2(0.0f, 0.0f)},
            new() { Position = new Vector3(-1f, 1f, 0.0f),TexCoords = new Vector2(0.0f, 1.0f ) }
        ];

        uint[] indices =
        [ // note that we start from 0!
            0, 1, 3,   // first triangle
            1, 2, 3    // second triangle
        ];
        Mesh mesh = new Mesh(vertices, indices, "");
        Shader shader = new Shader(shaderSrc);
        Material material = new Material(shader);
        material.RasterizerState.DepthTest = false;
        material.RasterizerState.DepthWriting = false;
        material.RasterizerState.Blendable = false;
        material.SetUniform("yourTexture", new AssetRef<Texture2D>(texture));
        if (!Directory.Exists(savePath))
            Directory.CreateDirectory(savePath);
        
        for (int i = 0; i < texture.MipLevels; i++)
        {
            material.SetUniform("level", i);
            Graphics.DrawMesh(mesh, material);
            var pixelData = Graphics.Device.ReadPixel(0, 0, 0, texture.Width, texture.Height);

            // Step 4: Convert pixel data to an Image<Rgba32> object
            using Image<Rgba32> image = Image.LoadPixelData<Rgba32>(pixelData, texture.Width, texture.Height);
            // ImageSharp loads the image data with the origin at the top-left corner, 
            // so we may need to flip the image vertically to match the framebuffer layout
            image.Mutate(x => x.Flip(FlipMode.Vertical));

            // Step 5: Save the image to a file 
            image.Save(Path.Combine(savePath,$"{i}.png")); // Save as PNG or specify format in file path like "filePath.png"
        }
        frameBuffer.Unbind();
        depthTexture2D.Dispose();
        colorTexture2D.Dispose();
        frameBuffer.Dispose();
        shader.Dispose();
        material.Dispose();
        mesh.Dispose();
    }
}