void URPLightningFunction_float (float3 ObjPos, out float3 Color, out float3 Direction, out float ShadowAttenuation)
{
   #ifdef LIGHTWEIGHT_LIGHTING_INCLUDED
   
      //Actual light data from the pipeline
      Light light = GetMainLight(GetShadowCoord(GetVertexPositionInputs(ObjPos)));
      Color = light.color;
      Direction = light.direction;
      ShadowAttenuation = light.shadowAttenuation;
      
   #else
   
      //Hardcoded data, used for the preview shader inside the graph
      //where light functions are not available
      Color = float3(1, 1, 1);
      Direction = float3(-0.5, 0.5, -0.5);
      ShadowAttenuation = 0.4;
      
   #endif
}