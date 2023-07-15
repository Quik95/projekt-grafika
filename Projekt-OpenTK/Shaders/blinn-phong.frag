#version 420 core

in vec3 v_normal;
in vec3 v_position;
in vec2 v_texCoord;

out vec4 color;

uniform sampler2D texture0;

uniform vec3 ambient_color;
uniform vec3 diffuse_color;
uniform vec3 specular_color;

void main() {
    vec3 texel = vec3(texture(texture0, v_texCoord));
    vec3 u_light = vec3(-100.0, 100.0, 50.0);
    
    float diffuse = max(dot(normalize(v_normal), normalize(u_light)), 0.0);

    vec3 camera_dir = normalize(-v_position);
    vec3 half_direction = normalize(normalize(u_light) + camera_dir);
    float specular = pow(max(dot(half_direction, normalize(v_normal)), 0.0), 16.0);

    vec3 ambient = ambient_color * texel;
    vec3 diffuse_final = diffuse_color * diffuse;
    vec3 specular_final = specular_color * specular;
    color = vec4(ambient + diffuse_final + specular_final, 1.0);
}