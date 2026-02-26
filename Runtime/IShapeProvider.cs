using Unity.Collections;
using Unity.Mathematics;

namespace CanvasPlus
{
	public interface IShapeProvider
	{
		/// <summary> Outputs shape's outline into <paramref name="line"/>. </summary>
		/// <param name="line"> The output list. </param>
		/// <param name="center"> The triangles fan center. </param>
		/// <param name="extents"> Rectangle's offsets from <paramref name="center"/>. </param>
		/// <param name="radius"> Radii of shape in order: left-top, top-right, right-bottom, bottom-left. </param>
		/// <param name="unitsPerPixel"> Length of one pixel. </param>
		void Generate(
			NativeList<ShapePoint> shape,
			in float2 center,
			in float2 size,
			float unitsPerPixel
		);
	}
}