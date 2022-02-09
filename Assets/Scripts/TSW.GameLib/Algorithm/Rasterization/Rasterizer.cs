using TSW.Struct;

using UnityEngine;

/// <summary>
/// Using http://joshbeam.com/articles/triangle_rasterization/
/// </summary>

namespace TSW.Algorithm.Rasterization
{
	public class Rasterizer
	{
		protected static void DrawSpan(Span span, int y, float[] data, int size)
		{
			int xdiff = span.X2 - span.X1;
			if (xdiff == 0)
			{
				return;
			}

			float factor = 0.0f;
			float factorStep = 1.0f / xdiff;

			// draw each pixel in the span
			for (int x = span.X1; x < span.X2; x++)
			{
				SetPixel(x, y, data, size);
				factor += factorStep;
			}
		}

		protected static void DrawSpansBetweenEdges(Edge e1, Edge e2, float[] data, int size)
		{
			// calculate difference between the y coordinates
			// of the first edge and return if 0
			int e1zdiff = e1.end.z - e1.begin.z;
			if (e1zdiff == 0.0f)
			{
				return;
			}

			// calculate difference between the y coordinates
			// of the second edge and return if 0
			int e2zdiff = e2.end.z - e2.begin.z;
			if (e2zdiff == 0.0f)
			{
				return;
			}

			// calculate differences between the x coordinates
			// and colors of the points of the edges
			int e1xdiff = e1.end.x - e1.begin.x;
			int e2xdiff = e2.end.x - e2.begin.x;

			// calculate factors to use for interpolation
			// with the edges and the step values to increase
			// them by after drawing each span
			float factor1 = (e2.begin.z - e1.begin.z) / (float)e1zdiff;
			float factorStep1 = 1 / (float)e1zdiff;
			float factor2 = 0.0f;
			float factorStep2 = 1 / (float)e2zdiff;

			// loop through the lines between the edges and draw spans
			for (int z = e2.begin.z; z < e2.end.z; z++)
			{
				// create and draw span
				Span span = new Span(
					Mathf.RoundToInt(e1.begin.x + e1xdiff * factor1),
					Mathf.RoundToInt(e2.begin.x + e2xdiff * factor2));
				DrawSpan(span, z, data, size);

				// increase factors
				factor1 += factorStep1;
				factor2 += factorStep2;
			}
		}

		public static void DrawPolygon(Int2[] vertices, float[] data, int size)
		{
			if (vertices.Length < 3)
			{
				return;
			}
			for (int i = 0; i <= vertices.Length - 3; ++i)
			{
				DrawTriangle(vertices[0 + i], vertices[1 + i], vertices[2 + i], data, size);
			}
		}

		public static void DrawTriangle(Int2 a, Int2 b, Int2 c, float[] data, int size)
		{
			// create edges for the triangle
			Edge[] edges = {
				new Edge(a, b),
				new Edge(b, c),
				new Edge(c, a)
			};

			float maxLength = 0f;
			int longEdge = 0;

			// find edge with the greatest length in the y axis
			for (int i = 0; i < 3; i++)
			{
				int length = edges[i].end.z - edges[i].begin.z;
				if (length > maxLength)
				{
					maxLength = length;
					longEdge = i;
				}
			}

			int shortEdge1 = (longEdge + 1) % 3;
			int shortEdge2 = (longEdge + 2) % 3;

			// draw spans between edges; the long edge can be drawn
			// with the shorter edges to draw the full triangle
			DrawSpansBetweenEdges(edges[longEdge], edges[shortEdge1], data, size);
			DrawSpansBetweenEdges(edges[longEdge], edges[shortEdge2], data, size);
		}

		public static void DrawLine(Int2 begin, Int2 end, float[] data, int size)
		{
			int xdiff = (end.x - begin.x);
			int zdiff = (end.z - begin.z);

			if (xdiff == 0 && zdiff == 0)
			{
				SetPixel(begin.x, begin.z, data, size);
				return;
			}

			if (System.Math.Abs(xdiff) > System.Math.Abs(zdiff))
			{
				int xmin, xmax;

				// set xmin to the lower x value given
				// and xmax to the higher value
				if (begin.x < end.x)
				{
					xmin = begin.x;
					xmax = end.x;
				}
				else
				{
					xmin = end.x;
					xmax = begin.x;
				}

				// draw line in terms of y slope
				float slope = zdiff / (float)xdiff;
				for (int x = xmin; x <= xmax; ++x)
				{
					int z = Mathf.RoundToInt(begin.z + ((x - begin.x) * slope));
					SetPixel(x, z, data, size);
				}
			}
			else
			{
				int zmin, zmax;

				// set ymin to the lower y value given
				// and ymax to the higher value
				if (begin.z < end.z)
				{
					zmin = begin.z;
					zmax = end.z;
				}
				else
				{
					zmin = end.z;
					zmax = begin.z;
				}

				// draw line in terms of x slope
				float slope = xdiff / zdiff;
				for (int z = zmin; z <= zmax; ++z)
				{
					int x = Mathf.RoundToInt(begin.x + ((z - begin.z) * slope));
					SetPixel(x, z, data, size);
				}
			}
		}

		private static void SetPixel(int x, int z, float[] data, int size)
		{
			if (x < 0.0f || z < 0.0f)
			{
				return;
			}
			if (x >= size || z >= size)
			{
				return;
			}
			data[z * size + x] = 1f;
		}
	}
}