#version 420 core

layout(location = 0) in vec3 aPosition;
layout(location = 1) in vec3 aNormal;
layout(location = 2) in vec2 aTexCoord;

out vec2 texCoord;
out vec3 normal;
out vec3 v_position;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main(void)
{
    gl_Position = vec4(aPosition, 1.0) * model * view * projection;
    v_position = gl_Position.xyz / gl_Position.w;
    texCoord = aTexCoord;
    normal = transpose(inverse(mat3(model))) * aNormal;
}