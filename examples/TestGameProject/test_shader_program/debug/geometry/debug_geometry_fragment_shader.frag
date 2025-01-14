#version 440

layout(location = 3, binding = 0) uniform sampler2D diffuse;
layout(location = 4, binding = 1) uniform sampler2D normal;
layout(location = 5) uniform vec4 fragColor;

layout(location = 0) out vec4 color;

void main()
{
    color = fragColor;
}
