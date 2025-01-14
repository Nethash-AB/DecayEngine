#version 440

layout (location = 0) in vec2 vertex;
layout (location = 1) in vec2 textureUv;

layout(location = 7) uniform mat4 view;
layout(location = 8) uniform mat4 projection;

layout(location = 0) out vec2 textureUvCoord;
layout(location = 1) out vec3 envMapCoord;

void main()
{
    textureUvCoord = textureUv;

    mat4 viewRotationMatrix = mat4(transpose(mat3(view)));
    envMapCoord = -(projection * viewRotationMatrix * vec4(-vertex.x, -vertex.y, vec2(1.0))).xyz;

    gl_Position = vec4(vertex.x, vertex.y, 0.0, 1.0);
}
