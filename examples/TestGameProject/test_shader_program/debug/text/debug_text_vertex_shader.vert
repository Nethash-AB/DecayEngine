#version 440

layout(location = 0) uniform mat4 projection;
layout(location = 1) uniform mat4 view;
layout(location = 2) uniform mat4 model;

layout(location = 0) in vec3 vertex;
layout(location = 1) in vec2 textureUv;

layout(location = 0) out vec2 textureUvCoord;

void main()
{
    textureUvCoord = textureUv;
    gl_Position = (projection * view * model) * vec4(vertex, 1.0);
}
