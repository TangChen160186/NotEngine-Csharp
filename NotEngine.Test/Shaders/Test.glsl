#shader vertex 
#version 460
layout(location = 0) in vec3 aPosition;
layout(location = 1) in vec2 aTexCoords;
uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;
out vec2 tex;
void main(void)
{
    gl_Position =  projection*  view * model * vec4(aPosition, 1.0);
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
//   outputColor = vec4(pow(texture(yourTexture, tex).rgb, vec3(1.0 / 2.2)),1);
     outputColor = texture(yourTexture, tex).rgba;
}