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
    out vec4 outputColor;
    in vec2 tex;
    void main()
    {
        outputColor = texture(yourTexture, tex);
        float average = 0.2126 * outputColor.r + 0.7152 * outputColor.g + 0.0722 * outputColor.b;
        outputColor = vec4(average, average, average, 1.0);
    }