//Specify the version.
#version 330 core

//Input values to our shader.
layout(location = 0) in vec3 v_position;

//All the source goes here.
void main()
{
    //Set the position of the vertex.
    gl_Position = vec4(v_position, 1.0);
}