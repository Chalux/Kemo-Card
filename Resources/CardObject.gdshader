shader_type canvas_item;

/*~ RAINBOW TEXTURE ~*/

// hsv2rgb source 
// https://gist.github.com/983/e170a24ae8eba2cd174f
vec3 hsv2rgb(vec3 c)
{
    vec4 K = vec4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

#define RAINBOW_REPETITIONS 2.5
#define RAINBOW_OFFSET -0.4

#define PI 3.14159265
#define TWO_PI (2.0 * PI)

#define MASK_OFFSET radians(-15.0)

/*
 The goal is to create a rainbow texture that has rainbow on the left,
 and black on the right.
 
 We can create a repeating rainbow pattern with hsv.
 We just need to then mult by some mask to gradually fade out the right.
*/
void fragment()
{
    // The x coord will act as our Hue in hsv
    //float x = fragCoord.x / iResolution.x;
	float x = UV.x;
    // Stretch out the hue so it repeats multiple times across the texture
    // desaturate the color to have a lighter tone
    vec3 col = hsv2rgb(vec3(x * RAINBOW_REPETITIONS + RAINBOW_OFFSET, 1.0, 1.));
    //col += 0.8;
    // this is our mask, maybe not the best way, but it got the job done
    float s = sin(x * TWO_PI + MASK_OFFSET);
    
    // clamp the texture to just exist 0 ... 1
    COLOR = vec4(clamp(col * s, 0., 1.), 1.0);
}