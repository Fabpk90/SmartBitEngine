#shader vertex
#version 330 core

layout(location = 0) in vec2 in_position;
layout(location = 1) in vec2 in_texCoord;
layout(location = 2) in vec4 in_color;

uniform mat4 projection_matrix;

out vec4 color;
out vec2 texCoord;

void main()
{
    gl_Position = projection_matrix * vec4(in_position.xy, 0, 1);
    color = in_color;
	texCoord = in_texCoord;
}


#shader fragment
#version 330 core

uniform sampler2D FontTexture;

in vec4 color;
in vec2 texCoord;

out vec4 outputColor;

void main()
{
    outputColor = texture(FontTexture, texCoord) * color ;
}
