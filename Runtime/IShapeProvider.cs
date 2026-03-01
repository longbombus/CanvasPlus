namespace CanvasPlus
{
	public interface IShapeProvider
	{
		/// <summary> Generates the shape of figure. </summary>
		/// <param name="figure"> The output list. </param>
		/// <param name="unitsPerPixel"> Length of one pixel. </param>
		void Generate(ref Figure figure, float unitsPerPixel);
	}
}