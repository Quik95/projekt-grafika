#version 150

in vec3 aPos;
in vec3 aNormal;
in vec2 aTexCoord;

out vec3 v_normal;
out vec3 v_position;
out vec2 v_texCoord;

uniform mat4 projection;
uniform mat4 view;
uniform mat4 model;

void main() {
    mat4 modelview = model * view;
    v_normal = transpose(inverse(mat3(modelview))) * aNormal;
    gl_Position = vec4(aPos, 1.0) * modelview * projection;
    v_position = gl_Position.xyz / gl_Position.w;
    v_texCoord = aTexCoord;
}