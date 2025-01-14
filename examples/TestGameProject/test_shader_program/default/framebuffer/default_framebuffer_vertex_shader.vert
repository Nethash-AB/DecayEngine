#version 440

layout (location = 0) in vec2 vertex;
layout (location = 1) in vec2 textureUv;

layout(location = 0) out vec2 textureUvCoord;

void main()
{
    textureUvCoord = textureUv;
    gl_Position = vec4(vertex.x, vertex.y, 0.0, 1.0);
}
