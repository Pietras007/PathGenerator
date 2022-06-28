#version 400
uniform vec3 fragmentColor;
uniform int showTrimmed;
uniform int trimmedPart;
uniform sampler2D heightMap;

in vec2 textureCoord;
void main()
{
    float shouldColor = texture(heightMap, textureCoord).x;
    if(showTrimmed == 1) {
//        if(textureCoord.x > 0.5){
//            gl_FragColor = vec4(vec3(0,1,0), 1.0);
//        }
//        if(textureCoord.y > 0.5){
//            gl_FragColor = vec4(vec3(0,0,1), 1.0);
//        }
        if(shouldColor < 0.5f && trimmedPart == 0) {
            discard;
        }
        if(shouldColor > 0.5f && trimmedPart == 1) {
            discard;
        }
    }
    else {
        gl_FragColor = vec4(fragmentColor, 1.0);
    }
}
