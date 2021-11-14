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
    float coeffs_x[4];
    float coeffs_y[4];
    float coeffs_z[4];
    float coeffs2_x[4];
    float coeffs2_y[4];
    float coeffs2_z[4];
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

   coeffs_x[0]=gl_in[0].gl_Position.x;
   coeffs_y[0]=gl_in[0].gl_Position.y;
   coeffs_z[0]=gl_in[0].gl_Position.z;
   coeffs_x[1]=gs_in[0].pos2.x;
   coeffs_y[1]=gs_in[0].pos2.y;
   coeffs_z[1]=gs_in[0].pos2.z;
   coeffs_x[2]=gs_in[0].pos3.x;
   coeffs_y[2]=gs_in[0].pos3.y;
   coeffs_z[2]=gs_in[0].pos3.z;
   coeffs_x[3]=gs_in[0].pos4.x;
   coeffs_y[3]=gs_in[0].pos4.y;
   coeffs_z[3]=gs_in[0].pos4.z;
   point1_0.x = DeKastilio(coeffs_x,t,vertices);
   point1_0.y = DeKastilio(coeffs_y,t,vertices);
   point1_0.z = DeKastilio(coeffs_z,t,vertices);
   point2_0.x = DeKastilio(coeffs_x,t2,vertices);
   point2_0.y = DeKastilio(coeffs_y,t2,vertices);
   point2_0.z = DeKastilio(coeffs_z,t2,vertices);
    
   coeffs_x[0]=gl_in[1].gl_Position.x;
   coeffs_y[0]=gl_in[1].gl_Position.y;
   coeffs_z[0]=gl_in[1].gl_Position.z;
   coeffs_x[1]=gs_in[1].pos2.x;
   coeffs_y[1]=gs_in[1].pos2.y;
   coeffs_z[1]=gs_in[1].pos2.z;
   coeffs_x[2]=gs_in[1].pos3.x;
   coeffs_y[2]=gs_in[1].pos3.y;
   coeffs_z[2]=gs_in[1].pos3.z;
   coeffs_x[3]=gs_in[1].pos4.x;
   coeffs_y[3]=gs_in[1].pos4.y;
   coeffs_z[3]=gs_in[1].pos4.z;
   point1_1.x = DeKastilio(coeffs_x,t,vertices);
   point1_1.y = DeKastilio(coeffs_y,t,vertices);
   point1_1.z = DeKastilio(coeffs_z,t,vertices);
   point2_1.x = DeKastilio(coeffs_x,t2,vertices);
   point2_1.y = DeKastilio(coeffs_y,t2,vertices);
   point2_1.z = DeKastilio(coeffs_z,t2,vertices);
       
   coeffs_x[0]=gl_in[2].gl_Position.x;
   coeffs_y[0]=gl_in[2].gl_Position.y;
   coeffs_z[0]=gl_in[2].gl_Position.z;
   coeffs_x[1]=gs_in[2].pos2.x;
   coeffs_y[1]=gs_in[2].pos2.y;
   coeffs_z[1]=gs_in[2].pos2.z;
   coeffs_x[2]=gs_in[2].pos3.x;
   coeffs_y[2]=gs_in[2].pos3.y;
   coeffs_z[2]=gs_in[2].pos3.z;
   coeffs_x[3]=gs_in[2].pos4.x;
   coeffs_y[3]=gs_in[2].pos4.y;
   coeffs_z[3]=gs_in[2].pos4.z;
   point1_2.x = DeKastilio(coeffs_x,t,vertices);
   point1_2.y = DeKastilio(coeffs_y,t,vertices);
   point1_2.z = DeKastilio(coeffs_z,t,vertices);
   point2_2.x = DeKastilio(coeffs_x,t2,vertices);
   point2_2.y = DeKastilio(coeffs_y,t2,vertices);
   point2_2.z = DeKastilio(coeffs_z,t2,vertices);
        
   coeffs_x[0]=gl_in[3].gl_Position.x;
   coeffs_y[0]=gl_in[3].gl_Position.y;
   coeffs_z[0]=gl_in[3].gl_Position.z;
   coeffs_x[1]=gs_in[3].pos2.x;
   coeffs_y[1]=gs_in[3].pos2.y;
   coeffs_z[1]=gs_in[3].pos2.z;
   coeffs_x[2]=gs_in[3].pos3.x;
   coeffs_y[2]=gs_in[3].pos3.y;
   coeffs_z[2]=gs_in[3].pos3.z;
   coeffs_x[3]=gs_in[3].pos4.x;
   coeffs_y[3]=gs_in[3].pos4.y;
   coeffs_z[3]=gs_in[3].pos4.z;
   point1_3.x = DeKastilio(coeffs_x,t,vertices);
   point1_3.y = DeKastilio(coeffs_y,t,vertices);
   point1_3.z = DeKastilio(coeffs_z,t,vertices);
   point2_3.x = DeKastilio(coeffs_x,t2,vertices);
   point2_3.y = DeKastilio(coeffs_y,t2,vertices);
   point2_3.z = DeKastilio(coeffs_z,t2,vertices);

    coeffs_x[0]=point1_0.x;
    coeffs_y[0]=point1_0.y;
    coeffs_z[0]=point1_0.z;
    coeffs_x[1]=point1_1.x;
    coeffs_y[1]=point1_1.y;
    coeffs_z[1]=point1_1.z;
    coeffs_x[2]=point1_2.x;
    coeffs_y[2]=point1_2.y;
    coeffs_z[2]=point1_2.z;
    coeffs_x[3]=point1_3.x;
    coeffs_y[3]=point1_3.y;
    coeffs_z[3]=point1_3.z;

    coeffs2_x[0]=point2_0.x;
    coeffs2_y[0]=point2_0.y;
    coeffs2_z[0]=point2_0.z;
    coeffs2_x[1]=point2_1.x;
    coeffs2_y[1]=point2_1.y;
    coeffs2_z[1]=point2_1.z;
    coeffs2_x[2]=point2_2.x;
    coeffs2_y[2]=point2_2.y;
    coeffs2_z[2]=point2_2.z;
    coeffs2_x[3]=point2_3.x;
    coeffs2_y[3]=point2_3.y;
    coeffs2_z[3]=point2_3.z;

    float move = (end - begin)/parts;
    t=begin;

    for(int i=0; i<=parts; i++)
    {
//        if(i == parts)
 if(i != parts)
        {
                gl_Position.x = DeKastilio(coeffs2_x,t + move,vertices);
        gl_Position.y = DeKastilio(coeffs2_y,t + move,vertices);
        gl_Position.z = DeKastilio(coeffs2_z,t + move,vertices);
        gl_Position.w=1.0;
        gl_Position = gl_Position * model *view * projection;
        EmitVertex();
        
//        gl_Position.x = DeKastilio(coeffs_x,t,vertices);
//        gl_Position.y = DeKastilio(coeffs_y,t,vertices);
//        gl_Position.z = DeKastilio(coeffs_z,t,vertices);
//        gl_Position.w=1.0;
//        gl_Position = gl_Position * model *view * projection;
//        EmitVertex();

        }


//        {
        gl_Position.x = DeKastilio(coeffs2_x,t,vertices);
        gl_Position.y = DeKastilio(coeffs2_y,t,vertices);
        gl_Position.z = DeKastilio(coeffs2_z,t,vertices);
        gl_Position.w=1.0;
        gl_Position =gl_Position * model *view * projection;
        EmitVertex();

        gl_Position.x = DeKastilio(coeffs_x,t,vertices);
        gl_Position.y = DeKastilio(coeffs_y,t,vertices);
        gl_Position.z = DeKastilio(coeffs_z,t,vertices);
        gl_Position.w=1.0;
        gl_Position = gl_Position * model *view * projection;
        EmitVertex();
//        }

        if(i != parts)
        {
                gl_Position.x = DeKastilio(coeffs_x,t + move,vertices);
        gl_Position.y = DeKastilio(coeffs_y,t + move,vertices);
        gl_Position.z = DeKastilio(coeffs_z,t + move,vertices);
        gl_Position.w=1.0;
        gl_Position = gl_Position * model *view * projection;
        EmitVertex();
        
//        gl_Position.x = DeKastilio(coeffs_x,t,vertices);
//        gl_Position.y = DeKastilio(coeffs_y,t,vertices);
//        gl_Position.z = DeKastilio(coeffs_z,t,vertices);
//        gl_Position.w=1.0;
//        gl_Position = gl_Position * model *view * projection;
//        EmitVertex();

        }

//        gl_Position.x = DeKastilio(coeffs2_x,t,vertices);
//        gl_Position.y = DeKastilio(coeffs2_y,t,vertices);
//        gl_Position.z = DeKastilio(coeffs2_z,t,vertices);
//        gl_Position.w=1.0;
//        gl_Position =gl_Position *model * view * projection;
//        EmitVertex();
    EndPrimitive();
        t+=move;
    }

//    gl_Position.x = DeKastilio(coeffs_x,t - move,vertices);
//    gl_Position.y = DeKastilio(coeffs_y,t - move,vertices);
//    gl_Position.z = DeKastilio(coeffs_z,t - move,vertices);
//    gl_Position.w=1.0;
//    gl_Position = gl_Position *model * view * projection;
//    EmitVertex();
//    if( gs_in[0].color.x == 0.12f)
//    {    gl_Position.x = DeKastilio(coeffs_x,begin,vertices);
//    gl_Position.y = DeKastilio(coeffs_y,begin,vertices);
//    gl_Position.z = DeKastilio(coeffs_z,begin,vertices);
//    gl_Position.w=1.0;
//    gl_Position = gl_Position *model * view * projection;
//    EmitVertex();
//    }
    EndPrimitive();
}    