shader_type canvas_item;

uniform vec4 edge_color: source_color;
uniform float threshold : hint_range(0.0, 1.0) = 0.01; // 阈值，调整以识别黑色的严重程度

void fragment() {
    vec2 p1 = vec2(.015, .015);
    vec2 p2 = vec2(.985, .985);
    vec2 p3 = vec2(.015, .985);
    vec2 p4 = vec2(.985, .015);

	//vec2 uv = FRAGCOORD.xy / TEXTURE_PIXEL_SIZE.xy;
	vec2 uv = UV;
    vec4 c2 = texture(TEXTURE, abs(sin(uv + TIME/10.)));
    
    float d1 = step(p1.x,uv.x)*step(uv.x,p4.x)*abs(uv.y-p1.y)+
        step(uv.x,p1.x)*distance(uv,p1)+step(p4.x,uv.x)*distance(uv,p4);
    d1 = min(step(p3.x,uv.x)*step(uv.x,p2.x)*abs(uv.y-p2.y)+
        step(uv.x,p3.x)*distance(uv,p3)+step(p2.x,uv.x)*distance(uv,p2),d1);
    d1 = min(step(p1.y,uv.y)*step(uv.y,p3.y)*abs(uv.x-p1.x)+
        step(uv.y,p1.y)*distance(uv,p1)+step(p3.y,uv.y)*distance(uv,p3),d1);
    d1 = min(step(p4.y,uv.y)*step(uv.y,p2.y)*abs(uv.x-p2.x)+
        step(uv.y,p4.y)*distance(uv,p4)+step(p2.y,uv.y)*distance(uv,p2),d1);
        
    float f1 = .01 / abs(d1 + c2.r/100.);
    vec4 c = vec4(f1 * edge_color.xyz, 1.0);
	float is_black = step(threshold, c.r+c.g+c.b);
	c.a = is_black;
	//c.rgb = mix(c.xyz,vec3(0.0),is_black);
	COLOR = c;
	//COLOR = c2;
}
