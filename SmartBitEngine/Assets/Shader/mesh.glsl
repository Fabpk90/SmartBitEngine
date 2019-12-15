#shader vertex
#version 330 core

layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec2 aTexCoords;

out vec2 TexCoords;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

out vec3 Normal;
out vec3 FragPos;

void main()
{
    TexCoords = aTexCoords;    
    gl_Position = projection * view * model * vec4(aPos, 1.0f);
    
    Normal = aNormal ; //mat3(transpose((model)));
    FragPos = vec3(model * vec4(aPos, 1.0f));
}

#shader fragment
#version 330 core

struct Material
{
    sampler2D texture_diffuse1;
    sampler2D texture_specular1;
    
    float shininess;
};

struct Light
{
    vec3 lightColor; //The color of the light.
    vec3 lightPos; //The position of the light.
};

out vec4 FragColor;

in vec2 TexCoords;
in vec3 Normal;
in vec3 FragPos;

uniform vec3 viewPos;//The position of the view and/or of the player.

uniform Light light;
uniform Material material;


void main()
{    
    vec3 ambient = light.lightColor * texture(material.texture_diffuse1, TexCoords).rgb;

    vec3 norm = normalize(Normal);
    vec3 lightDir = normalize(light.lightPos - FragPos);  
    float diff = max(dot(norm, lightDir), 0.0f);
    vec3 diffuse = diff * light.lightColor * texture(material.texture_diffuse1, TexCoords).rgb;
    
    float specularStrength = 0.5f;
    vec3 viewDir = normalize(viewPos - FragPos);
    vec3 reflectDir = reflect(lightDir, norm); 
    //float spec = pow(max(dot(viewDir, reflectDir), 0.0f), material.shininess);*/
    
    /*Using a fast alternative to the phong model by Christophe Schlick (Graphics Gems 4)*/
    
    float dotDir = dot(viewDir, reflectDir);
    float spec = dotDir / (material.shininess - (material.shininess * dotDir) + dotDir);
    
    vec3 specular = specularStrength * spec * vec3(texture(material.texture_specular1, TexCoords));

    FragColor = vec4(diffuse + specular + ambient, 1.0f);// * texture(material.texture_diffuse1, TexCoords);
}