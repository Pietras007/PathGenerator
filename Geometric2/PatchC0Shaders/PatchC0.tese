#version 400

layout(quads, equal_spacing, cw) in;
uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

in vec2 tes_texCoord[];
out vec2 textureCoord;

vec4 DeCasteljau(vec4 coeffs_t[4], float t)
{
	coeffs_t[0] = (1 - t) * coeffs_t[0] + t * coeffs_t[1];
	coeffs_t[1] = (1 - t) * coeffs_t[1] + t * coeffs_t[2];
	coeffs_t[2] = (1 - t) * coeffs_t[2] + t * coeffs_t[3];
	coeffs_t[0] = (1 - t) * coeffs_t[0] + t * coeffs_t[1];
	coeffs_t[1] = (1 - t) * coeffs_t[1] + t * coeffs_t[2];
	coeffs_t[0] = (1 - t) * coeffs_t[0] + t * coeffs_t[1];
	return coeffs_t[0];
}

void main( )
{
	float u = gl_TessCoord.x;
	float v = gl_TessCoord.y;

	vec4 p00 = gl_in[0].gl_Position;
	vec4 p10 = gl_in[1].gl_Position;
	vec4 p20 = gl_in[2].gl_Position;
	vec4 p30 = gl_in[3].gl_Position;
	vec4 p01 = gl_in[4].gl_Position;
	vec4 p11 = gl_in[5].gl_Position;
	vec4 p21 = gl_in[6].gl_Position;
	vec4 p31 = gl_in[7].gl_Position;
	vec4 p02 = gl_in[8].gl_Position;
	vec4 p12 = gl_in[9].gl_Position;
	vec4 p22 = gl_in[10].gl_Position;
	vec4 p32 = gl_in[11].gl_Position;
	vec4 p03 = gl_in[12].gl_Position;
	vec4 p13 = gl_in[13].gl_Position;
	vec4 p23 = gl_in[14].gl_Position;
	vec4 p33 = gl_in[15].gl_Position;

	vec4 coeffs[4];
	coeffs[0]= p00;
	coeffs[1]= p10;
	coeffs[2]= p20;
	coeffs[3]= p30;
	vec4 coeffs1 = DeCasteljau(coeffs,v);

	coeffs[0]= p01;
	coeffs[1]= p11;
	coeffs[2]= p21;
	coeffs[3]= p31;
	vec4 coeffs2 = DeCasteljau(coeffs,v);

	coeffs[0]= p02;
	coeffs[1]= p12;
	coeffs[2]= p22;
	coeffs[3]= p32;
	vec4 coeffs3 = DeCasteljau(coeffs,v);

	coeffs[0]= p03;
	coeffs[1]= p13;
	coeffs[2]= p23;
	coeffs[3]= p33;
	vec4 coeffs4 = DeCasteljau(coeffs,v);

	coeffs[0]= coeffs1;
	coeffs[1]= coeffs2;
	coeffs[2]= coeffs3;
	coeffs[3]= coeffs4;

	vec2 tmp1 = mix(tes_texCoord[0], tes_texCoord[3], v);
	vec2 tmp2 = mix(tes_texCoord[12], tes_texCoord[15], v);
	textureCoord = mix(tmp1, tmp2, u);

	vec4 position = vec4(DeCasteljau(coeffs,u).xyz, 1.0f);
	gl_Position = position * model * view * projection;
}