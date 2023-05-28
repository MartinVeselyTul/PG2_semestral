#version 330 core

struct DirectionalLight {
    vec3 direction;
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;

};

struct PointLight {
    vec3 position;
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
    float constant;
    float linear;
    float quadratic;
};

struct Material {
    sampler2D diffuseMap;
    sampler2D specularMap;
    sampler2D normalMap;
    float shininess;
};

in vec2 TexCoord;
in vec3 FragPos;
in vec3 Normal;
in vec3 VertexPosition;


out vec4 color;


uniform vec3 viewPos;
uniform DirectionalLight directionalLight;
uniform PointLight light;
uniform Material material;

vec3 CalcDirLight(DirectionalLight dl, vec3 normal, vec3 viewDir, vec3 objectColor, vec3 specularColor);
vec3 CalcPointLight(PointLight l, vec3 normal, vec3 viewDir, vec3 objectColor, vec3 specularColor);
mat3 cotangentFrame(vec3 N, vec3 p, vec2 uv);
vec3 applyNormalMap(vec3 N, vec3 V, vec2 texcoord);

void main()
{
    //vec4 objectColor = mix(texture(ourTexture1, TexCoord), texture(ourTexture2, TexCoord), 0.2);
    vec3 objectColor =  texture(material.diffuseMap, TexCoord).xyz;
    vec3 specularColor = texture(material.specularMap, TexCoord).xzy;


    vec3 vertexToEye = normalize(-VertexPosition);


    vec3 norm = normalize(applyNormalMap(Normal, vertexToEye, TexCoord));

    vec3 viewDir = normalize(viewPos - FragPos);

    vec3 result = CalcDirLight(directionalLight, norm, viewDir, objectColor, specularColor);
    result+=CalcPointLight(light, norm, viewDir, objectColor, specularColor);

    color = vec4(result, 1.0f);

}

vec3 CalcDirLight(DirectionalLight dl, vec3 normal, vec3 viewDir, vec3 objectColor, vec3 specularColor)
{
    vec3 ambient = dl.ambient * objectColor;

    float diff = max(dot(normal, dl.direction), 0.0);
    vec3 diffuse = dl.diffuse * (diff * objectColor);

    vec3 halfwayDir = normalize(dl.direction + viewDir);
    float spec = pow(max(dot(normal, halfwayDir), 0.0), material.shininess);
    vec3 specular = dl.specular * (spec * specularColor);

    return (ambient + diffuse + specular);
}

vec3 CalcPointLight(PointLight l, vec3 normal, vec3 viewDir, vec3 objectColor, vec3 specularColor)
{
    vec3 lightDir = normalize(l.position - FragPos);

    float distance    = length(l.position - FragPos);
    float attenuation = 1.0f / (l.constant + l.linear * distance + l.quadratic * (distance * distance));

    vec3 ambient = l.ambient * objectColor * attenuation;

    float diff = max(dot(normal, lightDir), 0.0);
    vec3 diffuse = l.diffuse * (diff * objectColor) * attenuation;

    vec3 halfwayDir = normalize(lightDir + viewDir);
    float spec = pow(max(dot(normal, halfwayDir), 0.0), material.shininess) * attenuation;
    vec3 specular = l.specular * (spec * specularColor);

    return (ambient + diffuse + specular);
}

mat3 cotangentFrame(vec3 N, vec3 p, vec2 uv)
{
    // get edge vectors of the pixel triangle
    vec3 dp1 = dFdx(p);
    vec3 dp2 = dFdy(p);
    vec2 duv1 = dFdx(uv);
    vec2 duv2 = dFdy(uv);

    // solve the linear system
    vec3 dp2perp = cross(dp2, N);
    vec3 dp1perp = cross(N, dp1);
    vec3 T = dp2perp * duv1.x + dp1perp * duv2.x;
    vec3 B = dp2perp * duv1.y + dp1perp * duv2.y;

    // construct a scale-invariant frame 
    float invmax = inversesqrt( max( dot(T,T), dot(B,B) ) );
    return mat3( T * invmax, B * invmax, N );
}

vec3 applyNormalMap(vec3 N, vec3 V, vec2 texcoord)
{
    // assume N, the interpolated vertex normal and 
    // V, the view vector (vertex to eye)
    vec3 map = texture2D( material.normalMap, texcoord ).xyz;
    //#ifdef WITH_NORMALMAP_UNSIGNED
    map = map * 255./127. - 128./127.;
    //#endif
    //#ifdef WITH_NORMALMAP_2CHANNEL
    //  map.z = sqrt( 1. - dot( map.xy, map.xy ) );
    //#endif
    //#ifdef WITH_NORMALMAP_GREEN_UP
    //  map.y = -map.y;
    //#endif
    mat3 TBN = cotangentFrame( N, -V, texcoord );
    return normalize( TBN * map );
}