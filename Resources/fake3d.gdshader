// Hey this is Hei! This shader "fakes" a 3D-camera perspective on CanvasItems.
// License: MIT

shader_type canvas_item;

// Camera FOV
uniform float fov : hint_range(1, 179) = 90;
uniform bool cull_back = true;
uniform float y_rot : hint_range(-180, 180) = 0.0;
uniform float x_rot : hint_range(-180, 180) = 0.0;
// At 0, the image retains its size when unrotated.
// At 1, the image is resized so that it can do a full
// rotation without clipping inside its rect.
uniform float inset : hint_range(0, 1) = 0.0;

// 溶解shader
uniform sampler2D dissolve_texture : source_color;
uniform float dissolve_value : hint_range(0,1);
uniform float burn_size: hint_range(0.0, 1.0, 0.01);
uniform vec4 burn_color: source_color;
// Consider changing this to a uniform and changing it from code

uniform int effect_type: hint_range(0,2);

varying flat vec2 o;
varying vec3 p;

//uniform float rayOffsetX:hint_range(0,2);
//uniform float rayOffsetY:hint_range(0,2);
uniform sampler2D in_color;
uniform float scaleArg:hint_range(0.0, 16.0, 0.1) = 4;
// x轴旋转对全息效果的影响因子
uniform float x_rot_scale:hint_range(0.0, 360, 1) = 10;
// y轴旋转对全息效果的影响因子
uniform float y_rot_scale:hint_range(0.0, 360, 1) = 10;
// 负片效果影响因子
uniform float negative:hint_range(0.0, 1, 0.1) = 0.1;
uniform vec4 negative_color:source_color;

//const float PI = 3.14159;

// RGB转HSL
vec4 hsl(vec4 c)
{
    float low = min(c.r, min(c.g, c.b));
    float high = max(c.r, max(c.g, c.b));
    float delta = high - low;
    float sum = high + low;
    vec4 res = vec4(0,0,0.5*sum,c.a);
    if (abs(delta) < 0.01) {
        return res;
    }
    res.y = (res.z < .5) ? delta / sum : delta / (2.0 - sum);
    if (abs(high - c.r) < 0.01)
    res.x = (c.g - c.b) / delta;
    else if (abs(high - c.g) < 0.01)
    res.x = (c.b - c.r) / delta + 2.0;
    else
    res.x = (c.r - c.g) / delta + 4.0;

    res.x = mod(res.x / 6.0, 1);
    return res;
}

float hue(float s, float t, float h)
{
    float hs = mod(h, 1.) * 6.;
    if (hs < 1.) return (t-s) * hs + s;
    if (hs < 3.) return t;
    if (hs < 4.) return (t-s) * (4.-hs) + s;
    return s;
}

vec4 RGB(vec4 c)
{
    if (c.y < 0.0001) return vec4(c.zzz, c.a);

    float t = (c.z < .5) ? c.y*c.z + c.z : -c.y*c.z + (c.y + c.z);
    float s= 2. * c.z - t;
    return vec4(hue(s,t,c.x+1./3.), hue(s,t,c.x), hue(s,t,c.x - 1./3.), c.w);
}

// Creates rotation matrix
void vertex(){
    float sin_b = sin(y_rot / 180.0 * PI);
    float cos_b = cos(y_rot / 180.0 * PI);
    float sin_c = sin(x_rot / 180.0 * PI);
    float cos_c = cos(x_rot / 180.0 * PI);

    mat3 inv_rot_mat;
    inv_rot_mat[0][0] = cos_b;
    inv_rot_mat[0][1] = 0.0;
    inv_rot_mat[0][2] = -sin_b;

    inv_rot_mat[1][0] = sin_b * sin_c;
    inv_rot_mat[1][1] = cos_c;
    inv_rot_mat[1][2] = cos_b * sin_c;

    inv_rot_mat[2][0] = sin_b * cos_c;
    inv_rot_mat[2][1] = -sin_c;
    inv_rot_mat[2][2] = cos_b * cos_c;


    float t = tan(fov / 360.0 * PI);
    p = inv_rot_mat * vec3((UV - 0.5), 0.5 / t);
    float v = (0.5 / t) + 0.5;
    p.xy *= v * inv_rot_mat[2].z;
    o = v * inv_rot_mat[2].xy;

    VERTEX += (UV - 0.5) / TEXTURE_PIXEL_SIZE * t * (1.0 - inset);
}

void fragment(){
    if (cull_back && p.z <= 0.0) discard;
    vec2 uv = (p.xy / p.z).xy - o;
    COLOR = texture(TEXTURE, uv + 0.5);
	
	ivec2 ts = textureSize(TEXTURE,0);
	vec2 tex_size = vec2(ts);

    // 根据effect_type应用不同效果
    if(effect_type == 1){  // 全息效果
		//vec4 tex = texture(TEXTURE, UV);
		vec4 tex = texture(in_color, UV);
		float low = min(tex.r, min(tex.g, tex.b));
		float high = max(tex.r, max(tex.g, tex.b));
		float delta = high - low;
		float sf = 1.0-max(0, 0.05*(1.1-delta));
		vec4 HSL = hsl(vec4(tex.r*sf, tex.g*sf, tex.b, tex.a));

		float t = TIME * 5.0 + 2003.;
		vec2 floored_uv = (floor((uv*tex_size.xy)))/max(tex_size.x, tex_size.y);
    	vec2 uv_scaled_centered = (floored_uv - 0.5) * 2.3 * max(tex_size.x, tex_size.y) / scaleArg;

		vec2 field_part1 = uv_scaled_centered + 50.*vec2(sin(-t / 143.6340), cos(-t / 99.4324));
		vec2 field_part2 = uv_scaled_centered + 50.*vec2(cos( t / 53.1532),  cos( t / 61.4532));
		vec2 field_part3 = uv_scaled_centered + 50.*vec2(sin(-t / 87.53218), sin(-t / 49.0000));

	    float field = (1.+ (
	        cos(length(field_part1) / 19.483) + sin(length(field_part2) / 33.155) * cos(field_part2.y / 15.73) +
	        cos(length(field_part3) / 27.193) * sin(field_part3.x / 21.92) ))/2.;
		//float res = 0.5 + 0.5*cos((rayOffsetX) * 2.612 + (field - 0.5) * 3.14);
		float res = 0.5 + 0.5*cos((x_rot/x_rot_scale) * 2.612 + (field - 0.5) * 3.14);

		//HSL.x = HSL.x + res + rayOffsetY*0.04;
		HSL.x = HSL.x + res + y_rot/y_rot_scale*0.04;
		HSL.y = min(0.6, HSL.y+0.5);

        COLOR.rgb += RGB(HSL).rgb;  // 叠加全息效果
    } else if(effect_type == 2){  // 负片效果
		vec4 tex = texture(in_color, UV);
		//vec2 ng_uv = uv * 2.;
//
	    float low = min(tex.r, min(tex.g, tex.b));
	    float high = max(tex.r, max(tex.g, tex.b));
	    float delta = high - low - 0.1;
//
	    //float fac = 0.8 + 0.9 * sin(11.0 * ng_uv.x + 4.32 * ng_uv.y + tex.r * 12.0 + cos(tex.r * 5.3 + ng_uv.y * 4.2 - ng_uv.x * 4.0));
	    //float fac2 = 0.5 + 0.5 * sin(8.0 * ng_uv.x + 2.32 * ng_uv.y + tex.r * 5.0 - cos(tex.r * 2.3 + ng_uv.x * 8.2));
	    //float fac3 = 0.5 + 0.5 * sin(10.0 * ng_uv.x + 5.32 * ng_uv.y + tex.r * 6.111 + sin(tex.r * 5.3 + ng_uv.y * 3.2));
	    //float fac4 = 0.5 + 0.5 * sin(3.0 * ng_uv.x + 2.32 * ng_uv.y + tex.r * 8.111 + sin(tex.r * 1.3 + ng_uv.y * 11.2));
	    //float fac5 = sin(0.9 * 16.0 * ng_uv.x + 5.32 * ng_uv.y + tex.r * 12.0 + cos(tex.r * 5.3 + ng_uv.y * 4.2 - ng_uv.x * 4.0));
//
	    //float maxfac = 0.7 * max(max(fac, max(fac2, max(fac3, 0.0))) + (fac + fac2 + fac3 * fac4), 0.0);
//
	    //tex.rgb = tex.rgb * 0.5 + vec3(0.4, 0.4, 0.8);
//
	    //tex.r = tex.r - delta + delta * maxfac * (0.7 + fac5 * 0.27) - 0.1;
	    //tex.g = tex.g - delta + delta * maxfac * (0.7 - fac5 * 0.27) - 0.1;
	    //tex.b = tex.b - delta + delta * maxfac * 0.7 - 0.1;
	    //tex.a = tex.a * (0.5 * max(min(1.0, max(0.0, 0.3 * max(low * 0.2, delta) + min(max(maxfac * 0.1, 0.0), 0.4))), 0.0) + 0.15 * maxfac * (0.1 + delta));
	//
        //COLOR.rgb *= tex.rgb;
		
		vec4 SAT = hsl(tex);
		if (abs(negative)>0.01) {
			SAT.b = 1.0-SAT.b;
		}
		SAT.r = .2-SAT.r;
		tex = RGB(SAT) + .8*vec4(.31,.39,.41,0.);
		
		float change = x_rot / 12.;
		
		float fac = 0.8 + 0.9*sin(11.*uv.x+4.32*uv.y + change*12. + cos(change*5.3 + uv.y*4.2 - uv.x*4.));
	    float fac2 = 0.5 + 0.5*sin(8.*uv.x+2.32*uv.y +change*5. - cos(change*2.3 + uv.x*8.2));
	    float fac3 = 0.5 + 0.5*sin(10.*uv.x+5.32*uv.y + change*6.111 + sin(change*5.3 + uv.y*3.2));
	    float fac4 = 0.5 + 0.5*sin(3.*uv.x+2.32*uv.y + change*8.111 + sin(change*1.3 + uv.y*11.2));
	    float fac5 = sin(0.9*16.*uv.x+5.32*uv.y + change*12. + cos(change*5.3 + uv.y*4.2 - uv.x*4.));
	
		float maxfac = 0.7*max(max(fac, max(fac2, max(fac3,0.0))) + (fac+fac2+fac3*fac4), 0);
		
		tex.r = tex.r-delta + delta*maxfac*(0.7 + fac5*0.27) - 0.1;
	    tex.g = tex.g-delta + delta*maxfac*(0.7 - fac5*0.27) - 0.1;
	    tex.b = tex.b-delta + delta*maxfac*0.7 - 0.1;
	    tex.a = tex.a*(0.5*max(min(1., max(0.,0.3*max(low*0.2, delta)+ min(max(maxfac*0.1,0.), 0.4)) ), 0.) + 0.15*maxfac*(0.1+delta));
		
		tex.rgb = tex.rgb * 0.5 + negative_color.rgb;
		COLOR.rgb *= tex.rgb;
    }

    vec4 noise_texture = texture(dissolve_texture, UV);

    // This is needed to avoid keeping a small burn_color dot with dissolve being 0 or 1
    // is there another way to do it?
    float burn_size_step = burn_size * step(0.001, dissolve_value) * step(dissolve_value, 0.999);
    float threshold = smoothstep(noise_texture.x-burn_size_step, noise_texture.x, dissolve_value);
    float border = smoothstep(noise_texture.x, noise_texture.x + burn_size_step, dissolve_value);

    COLOR.a *= step(max(abs(uv.x), abs(uv.y)), 0.5) * threshold;
    COLOR.rgb = mix(burn_color.rgb, COLOR.rgb, border);
}