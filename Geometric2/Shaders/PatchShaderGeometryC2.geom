#version 330 core
layout (lines_adjacency) in;
layout (line_strip, max_vertices = 256) out;

uniform mat4 projection;
uniform mat4 view;
uniform mat4 model;


in VS_OUT {
    vec3 color;
    vec3 pos2;
    vec3 pos3;
    vec3 pos4;
} gs_in[4];


vec3 DeBoor(float t, vec3 B0_, vec3 B1_, vec3 B2_, vec3 B3_) 
{
	float T0 = -1.0f;
	float T1 = 0.0f;
    float T2 = 1.0f;
    float T3 = 2.0f;
    float T4 = 3.0f;
	float Tm1 = -2.0f;

	float A1 = T2 - t;
	float A2 = T3 - t;
	float A3 = T4 - t;
	float B1 = t - T1;
	float B2 = t - T0;
	float B3 = t - Tm1;

	float N1 = 1;
	float N2 = 0;
	float N3 = 0;
	float N4 = 0;

	float saved = 0.0f;
	float term = 0.0f;

	term = N1/(A1+B1);
	N1 = saved + A1*term;
	saved = B1 * term;

	N2 = saved;
	saved = 0.0f;

	term = N1/(A1+B2);
	N1 = saved + A1*term;
	saved = B2 * term;

	term = N2/(A2+B1);
	N2 = saved + A2*term;
	saved = B1 * term;

	N3 = saved;
	saved = 0.0f;

	term = N1/(A1+B3);
	N1 = saved + A1*term;
	saved = B3 * term;

	term = N2/(A2+B2);
	N2 = saved + A2*term;
	saved = B2 * term;

	term = N3/(A3+B1);
	N3 = saved + A3*term;
	saved = B1 * term;

	N4 = saved;

	return N1*B0_+N2*B1_+N3*B2_+N4*B3_;
}
  
float DeKastilio(float vert[4], float t, int degree)
{
    for (int i = 0; i < degree; i++)
	{
		for (int j = 0; j < degree - i - 1; j++)
		{
			vert[j] = (1 - t) * vert[j] + t * vert[j + 1];
		}
	}

	return vert[0];
}

void main() {    
    //fColor=vec3(0.7f, 0.7f, 0.7f);//gs_in[0].color;
    float begin = gs_in[2].color.x;
    float end = gs_in[2].color.y;
    float parts = gs_in[2].color.z;

    float t =  gs_in[1].color.x;
    float t2 = gs_in[1].color.y;
    int vertices =4;

    vec3 point1_0;
    vec3 point1_1;
    vec3 point1_2;
    vec3 point1_3;
    vec3 point2_0;
    vec3 point2_1;
    vec3 point2_2;
    vec3 point2_3;

    point1_0 = vec3(gl_in[0].gl_Position);
    point1_1 = vec3(gl_in[1].gl_Position);
    point1_2 = vec3(gl_in[2].gl_Position);
    point1_3 = vec3(gl_in[3].gl_Position);


   
   point1_0 = DeBoor(t ,vec3(gl_in[0].gl_Position),vec3(gs_in[0].pos2),vec3(gs_in[0].pos3),vec3(gs_in[0].pos4));
   point2_0 = DeBoor(t2,vec3(gl_in[0].gl_Position),vec3(gs_in[0].pos2),vec3(gs_in[0].pos3),vec3(gs_in[0].pos4));
   
   point1_1 = DeBoor(t ,vec3(gl_in[1].gl_Position),vec3(gs_in[1].pos2),vec3(gs_in[1].pos3),vec3(gs_in[1].pos4));
   point2_1 = DeBoor(t2,vec3(gl_in[1].gl_Position),vec3(gs_in[1].pos2),vec3(gs_in[1].pos3),vec3(gs_in[1].pos4));
   
   point1_2 = DeBoor(t ,vec3(gl_in[2].gl_Position),vec3(gs_in[2].pos2),vec3(gs_in[2].pos3),vec3(gs_in[2].pos4));
   point2_2 = DeBoor(t2,vec3(gl_in[2].gl_Position),vec3(gs_in[2].pos2),vec3(gs_in[2].pos3),vec3(gs_in[2].pos4));
   
   point1_3 = DeBoor(t ,vec3(gl_in[3].gl_Position),vec3(gs_in[3].pos2),vec3(gs_in[3].pos3),vec3(gs_in[3].pos4));
   point2_3 = DeBoor(t2,vec3(gl_in[3].gl_Position),vec3(gs_in[3].pos2),vec3(gs_in[3].pos3),vec3(gs_in[3].pos4));
    
    
    float move = (end - begin)/parts;
    t=begin;
    for(int i=0; i<=parts;++i)
    {
		if(i != parts)
        {
			gl_Position = vec4(DeBoor(t + move,point1_0,point1_1,point1_2,point1_3),1.0f);
			gl_Position = gl_Position * model * view * projection;
			EmitVertex();
		}

        gl_Position = vec4(DeBoor(t,point1_0,point1_1,point1_2,point1_3),1.0f);
        gl_Position = gl_Position * model * view * projection;
        EmitVertex();

        gl_Position = vec4(DeBoor(t,point2_0,point2_1,point2_2,point2_3),1.0f);
        gl_Position = gl_Position  * model * view * projection;
        EmitVertex();

		if(i != parts)
        {
			gl_Position = vec4(DeBoor(t + move,point2_0,point2_1,point2_2,point2_3),1.0f);
			gl_Position = gl_Position  * model * view * projection;
			EmitVertex();
		}
		
		EndPrimitive();

        t+=move;
    }
    EndPrimitive();
}    