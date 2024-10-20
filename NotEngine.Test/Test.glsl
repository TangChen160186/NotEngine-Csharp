 #shader vertex 
#version 460
layout(location = 0) in vec3 aPosition;
layout(location = 1) in vec3 aColor;
layout(location = 2) in vec2 aTexCoords;
out vec2 tex;
out vec3 color;
void main(void)
{
gl_Position = vec4(aPosition, 1.0);
tex = aTexCoords;
color = aColor;
}

#shader fragment 
#version 460
#extension GL_ARB_bindless_texture: require
layout(std140,binding = 1) uniform UBO{
vec3 c;
}ubo;
                              
layout(std430,binding = 1) buffer SSBO{
vec3 c;
}ssbo;
layout(bindless_sampler) uniform sampler2D yourTexture;
out vec4 outputColor;
in vec2 tex;
in vec3 color;
void main()
{
outputColor = texture(yourTexture,tex);
}