using System.Runtime.CompilerServices;
using TypeDropdown;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace CanvasPlus
{
	public class ImagePlus : MaskableGraphic
	{
		private static readonly Vector4 Mock4 = Vector4.zero;

		[SerializeReference, TypeDropdown] private IShapeProvider m_Shape;
		[SerializeField] private StrokeStyle[] m_Strokes;
		[SerializeField] private bool m_SoftEdge;

		protected override void OnPopulateMesh(VertexHelper vh)
		{
			if (m_Shape == null)
				return; // TODO: fallback to default image generation

			var rect = rectTransform.rect;
			var unitsPerPixel = math.rcp(canvas ? canvas.scaleFactor : 1);

			var figure = GenerateFigure(rect.center, rect.size, unitsPerPixel);

			vh.Clear();

			int lineLengthFull = figure.shape.Length;
			int lineLengthEven = lineLengthFull / 2;

			var offsets = new NativeArray<float>(lineLengthEven, Allocator.Temp);

			AddShapeFill(vh, figure, offsets, out bool loopsCountOdd);

			foreach (var stroke in figure.strokes)
			{
				for (int i = 0; i < offsets.Length; ++i)
					offsets[i] += unitsPerPixel;

				var strokeSlice = stroke.Slice();
				var strokeColorsSlice = strokeSlice.SliceWithStride<Color32>(StrokePoint.ColorOffset);

				UpdateOffsets(offsets, unitsPerPixel);
				AddShapeLoop(vh, figure.shape, strokeColorsSlice, offsets, ref loopsCountOdd);
				UpdateOffsets(offsets, strokeSlice.SliceWithStride<float>(StrokePoint.WidthOffset), loopsCountOdd);
				AddShapeLoop(vh, figure.shape, strokeColorsSlice, offsets, ref loopsCountOdd);
			}

			if (m_SoftEdge && vh.currentVertCount > 0)
			{
				NativeSlice<Color32> colorSlice;

				if (figure.strokes.Length > 0)
				{
					var lastStroke = figure.strokes[figure.strokes.Length - 1];
					colorSlice = lastStroke.Slice().SliceWithStride<Color32>(StrokePoint.ColorOffset);
				}
				else
				{
					var shapeSlice = figure.shape.AsArray().Slice();
					colorSlice = shapeSlice.SliceWithStride<Color32>(ShapePoint.ColorOffset);
				}

				UpdateOffsets(offsets, unitsPerPixel);
				AddShapeLoop(vh, figure.shape, colorSlice, offsets, ref loopsCountOdd, true);
			}

			figure.Dispose();
			offsets.Dispose();
		}

		private Figure GenerateFigure(float2 rectCenter, float2 rectSize, float unitsPerPixel)
		{
			var figure = new Figure(rectCenter, rectSize, Allocator.Temp);

			m_Shape.Generate(ref figure, unitsPerPixel);

			for (var i = 0; i < m_Strokes.Length; ++i)
			{
				var stroke = m_Strokes[i];
				stroke.width?.Generate(ref figure, i);
				stroke.color?.Generate(ref figure, i);
			}

			return figure;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void AddShapeFill(VertexHelper vh, in Figure figure, NativeArray<float> offsets, out bool odd)
		{
			var fillColor = (Color32)color;

			if (fillColor.a > 0)
			{
				vh.AddVert(new Vector3(figure.center.x, figure.center.y), fillColor, Mock4, Mock4, Mock4, Mock4, UIVertex.simpleVert.normal, UIVertex.simpleVert.tangent);
				AddLoopVertices(vh, figure.shape, fillColor, offsets, false);

				var shapeLengthHalf = figure.shape.Length / 2;
				for (int i = 1; i < shapeLengthHalf; ++i)
					vh.AddTriangle(0, i, i + 1);
				vh.AddTriangle(0, shapeLengthHalf, 1);
			}
			else if (figure.strokes.Length > 0)
			{
				var firstStroke = figure.strokes[0];
				var firstStrokeColorsSlice = firstStroke.Slice().SliceWithStride<Color32>(StrokePoint.ColorOffset);
				AddLoopVerticesZeroAlpha(vh, figure.shape, firstStrokeColorsSlice, offsets, false);
			}

			odd = true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void AddShapeLoop(VertexHelper vh, in NativeList<ShapePoint> shape, NativeSlice<Color32> colors, NativeArray<float> offsets, ref bool odd, bool zeroColorAlpha = false)
		{
			int loopBegin = vh.currentVertCount;
			if (zeroColorAlpha)
				AddLoopVerticesZeroAlpha(vh, shape, colors, offsets, odd);
			else
				AddLoopVertices(vh, shape, colors, offsets, odd);

			AddLoopIndices(vh, loopBegin, shape.Length / 2, odd);

			odd = !odd;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void UpdateOffsets(NativeArray<float> offsets, float delta)
		{
			for (int i = 0; i < offsets.Length; ++i)
				offsets[i] += delta;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void UpdateOffsets(NativeArray<float> offsets, NativeSlice<float> deltas, bool odd)
		{
			for (int offsetIndex = 0, deltaIndex = odd ? 1 : 0; offsetIndex < offsets.Length; ++offsetIndex, deltaIndex += 2)
				offsets[offsetIndex] += deltas[deltaIndex];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void AddLoopVertices(VertexHelper vh, NativeList<ShapePoint> shape, Color32 color, NativeArray<float> offsets, bool odd)
		{
			for (int i = odd ? 1 : 0; i < shape.Length; i += 2)
			{
				var shapePoint = shape[i];

				vh.AddVert(
					shapePoint.GetVertexPosition(offsets[i >> 1]),
					color,
					Mock4,
					Mock4,
					Mock4,
					Mock4,
					shapePoint.GetVertexNormal(),
					shapePoint.GetVertexTangent()
				);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void AddLoopVertices(VertexHelper vh, NativeList<ShapePoint> shape, NativeSlice<Color32> colors, NativeArray<float> offsets, bool odd)
		{
			for (int i = odd ? 1 : 0; i < shape.Length; i += 2)
			{
				var shapePoint = shape[i];

				vh.AddVert(
					shapePoint.GetVertexPosition(offsets[i >> 1]),
					colors[i],
					Mock4,
					Mock4,
					Mock4,
					Mock4,
					shapePoint.GetVertexNormal(),
					shapePoint.GetVertexTangent()
				);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void AddLoopVerticesZeroAlpha(VertexHelper vh, NativeList<ShapePoint> shape, NativeSlice<Color32> colors, NativeArray<float> offsets, bool odd)
		{
			for (int i = odd ? 1 : 0; i < shape.Length; i += 2)
			{
				var shapePoint = shape[i];
				var color = colors[i];
				color.a = 0;

				vh.AddVert(
					shapePoint.GetVertexPosition(offsets[i >> 1]),
					color,
					Mock4,
					Mock4,
					Mock4,
					Mock4,
					shapePoint.GetVertexNormal(),
					shapePoint.GetVertexTangent()
				);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void AddLoopIndices(VertexHelper vh, int loopBegin, int loopLength, bool odd)
		{
			if (odd)
			{
				for (int i = 1; i < loopLength; ++i)
				{
					int currLoopIndex = loopBegin + i;
					int prevLoopIndex = currLoopIndex - loopLength;
					vh.AddTriangle(prevLoopIndex, prevLoopIndex - 1, currLoopIndex - 1);
					vh.AddTriangle(currLoopIndex - 1, currLoopIndex, prevLoopIndex);
				}

				int currLoopIndexLast = loopBegin + loopLength - 1;
				int prevLoopIndexFirst = loopBegin - loopLength;
				vh.AddTriangle(prevLoopIndexFirst, loopBegin - 1, currLoopIndexLast);
				vh.AddTriangle(currLoopIndexLast, loopBegin, prevLoopIndexFirst);
			}
			else
			{
				for (int i = 1; i < loopLength; ++i)
				{
					int currLoopIndex = loopBegin + i;
					int prevLoopIndex = currLoopIndex - loopLength;
					vh.AddTriangle(prevLoopIndex, prevLoopIndex - 1, currLoopIndex);
					vh.AddTriangle(currLoopIndex - 1, currLoopIndex, prevLoopIndex - 1);
				}

				int currLoopIndexLast = loopBegin + loopLength - 1;
				int prevLoopIndexFirst = loopBegin - loopLength;
				vh.AddTriangle(prevLoopIndexFirst, loopBegin - 1, loopBegin);
				vh.AddTriangle(currLoopIndexLast, loopBegin, loopBegin - 1);
			}
		}
	}
}