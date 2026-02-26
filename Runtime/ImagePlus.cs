using TypeDropdown;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace CanvasPlus
{
	public class ImagePlus : MaskableGraphic
	{
		[SerializeReference, TypeDropdown] private IShapeProvider m_Shape;

		protected override void OnPopulateMesh(VertexHelper vh)
		{
			if (m_Shape == null)
				return; // TODO: fallback to default image generation

			var shape = new NativeList<ShapePoint>(Allocator.Temp);

			var rect = rectTransform.rect;
			var rectCenter = rect.center;
			var rectSize = rect.size;
			var pixelsPerUnit = math.rcp(transform.lossyScale.magnitude);

			m_Shape.Generate(shape, rectCenter, rectSize, pixelsPerUnit);

			vh.Clear();

			Vector4 mock = Vector4.zero;
			Vector4 half = new Vector4(.5f, .5f, .5f, .5f);

			int lineLengthFull = shape.Length;
			int lineLengthEven = lineLengthFull / 2;

			// fill
			{
				var fillColor = (Color32)color;
				vh.AddVert(new Vector3(rectCenter.x, rectCenter.y), fillColor, half, mock, mock, mock, mock, mock);
				for (int i = 0; i < lineLengthFull; i += 2)
				{
					var point = shape[i];
					vh.AddVert(point.position, fillColor, half, mock, mock, mock, point.normal, point.tangent);
				}

				for (int i = 1; i < lineLengthEven; ++i)
					vh.AddTriangle(0, i, i + 1);
				vh.AddTriangle(0, lineLengthEven, 1);
			}

			shape.Dispose();
		}
	}
}