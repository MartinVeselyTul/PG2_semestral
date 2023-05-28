#version 410 core
layout (location = 0) in vec3 aPos;   // the position variable has attribute position 0
layout (location = 1) in vec3 aColor; // the color variable has attribute position 1
layout (location = 2) in vec2 aTexCoord; // the texture variable has attribute position 2
//layout (location = 3) in vec3 aNormal;
//atribut normála osvětlení a atribut textury

out vec3 ourColor; // output a color to the fragment shader
out vec2 TexCoord; // output a texture coordinate to the fragment shader

out vec3 Normal;
out vec3 FragPos;

uniform mat4 transform;
uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main()
{
    gl_Position = vec4(aPos, 1.0) * model * view * projection;
    ourColor = aColor; // set ourColor to the input color we got from the vertex data
    TexCoord = aTexCoord; // set the output texture coordinate to the input texture coordinate passed via vertex data
    //TexCoord = vec2(aTexCoord.x, 1.0 - aTexCoord.y);
    
    Normal = aColor * mat3(transpose(inverse(model)));
    FragPos = vec3(model * vec4(aPos, 1.0f));
}    