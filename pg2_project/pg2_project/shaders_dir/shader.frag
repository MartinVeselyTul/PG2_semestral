#version 410 core

out vec4 FragColor;

in vec3 ourColor;
in vec2 TexCoord;
in vec3 Normal;
in vec3 FragPos;

uniform sampler2D ourTexture;

//uniform vec3 lightColor;
//uniform vec3 lightPos;

void main()
{
    //vec3 normal = normalize(Normal);
    //vec3 lightDir = normalize(lightPos - FragPos);
    
    //float diff = max(dot(normal, lightDir), 0.0f);
    //FragColor = texture(ourTexture, TexCoord);
    vec4 texColor = texture(ourTexture, TexCoord);
    FragColor = vec4(texColor.rgb * ourColor, texColor.a);
    
    //FragColor = texture(ourTexture, TexCoord) * lightColor * diff;
}