inline float st0(float x)
{
    return saturate(x / fwidth(x) + 0.5);
}

inline float2 pr(float2 p, float o, float ro, int r)
{
    p = float2(atan2(p.y, p.x), length(p));
    p.x = fmod(p.x - ro, UNITY_TWO_PI / float(r)) - UNITY_PI / float(r); // Use fmod for modulus operation
    p = float2(p.y * sin(p.x), p.y * cos(p.x)) - float2(0.0, o);
    return p;
}

inline float cd(float2 uv)
{
    return max(min(max(min(max(length(uv + float2(0.2, 0.0)) - 0.42, -uv.x - 0.225 + uv.y * 0.1), length(uv) - 0.22), -uv.y), -length(uv + float2(0.3, -0.12)) + 0.32), -uv.x - 0.225 + uv.y * 0.1), length(uv) - 0.22);
}

inline float p(float t)
{
    return smoothstep(1.0, 0.0, pow(frac(t), 3.0)) - floor(t);
}

// Add the C function
void C(out float3 c, float2 u, float2 o)
{
    c = lerp(lerp(float3(0.72, 0.67, 0.79), float3(0.54, 0.5, 0.59), st0(-length(u * 2.0 - 1.0) + 0.075)), float3(0.72, 0.67, 0.79), 1.0 * (0.5 + 0.5 * sin(length(u * 20.0 - 10.0) * 20.0 - _Time.y * 5.0 * 1.0)) * saturate(0.003 / abs(min(min(min(max(abs(frac(length(u * 20.0 - 10.0)) - 0.5) / 20.0) - 0.002, -length(u * 2.0 - 1.0) + 0.07), cd(pr(u * 2.0 - 1.0, 0.075, -UNITY_HALF_PI - 1.0 * UNITY_TWO_PI + 0.0, 3) * 12.0) / 12.0), cd(pr(u * 2.0 - 1.0, 0.125, UNITY_HALF_PI + 1.0 * UNITY_TWO_PI + 0.0, 3) * 12.0) / 12.0), length(u * 2.0 - 1.0) - 0.021)))));
}
