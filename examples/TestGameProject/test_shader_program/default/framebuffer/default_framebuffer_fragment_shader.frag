#version 440

layout(location = 0, binding = 10) uniform sampler2D screenTexture;

layout(location = 0) in vec2 textureUvCoord;

layout(location = 0) out vec4 color;

void main()
{
    color = texture(screenTexture, textureUvCoord);
}
