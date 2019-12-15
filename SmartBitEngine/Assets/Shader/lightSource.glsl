#shader vertex
#version 330 core

layout(location = 0) in vec3 _position;

uniform mat4 transform;
uniform mat4 view;
uniform mat4 projection;

uniform vec3 color;

out vec3 fragColor;

void main() {
    gl_Position = projection * view * transform * vec4(_position, 1.0f);
    fragColor = color;
}


#shader fragment
#version 330 core

in vec3 fragColor;

out vec4 aColor;

void main()
{    
    aColor = vec4(fragColor, 1.0f);
}