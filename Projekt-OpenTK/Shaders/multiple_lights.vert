#version 330 core
in vec3 aPos;
in vec3 aNormal;
in vec2 aTexCoords;

out vec3 FragPos;
out vec3 Normal;
out vec2 TexCoords;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main()
{
    gl_Position = vec4(aPos, 1.0) * model * view * projection;
    FragPos = gl_Position.xyz / gl_Position.w;
    TexCoords = aTexCoords;
    Normal = transpose(inverse(mat3(model))) * aNormal;
}
