/*
The MIT License (MIT)

Copyright (c) 2015 Ondrej Mikulec
o.mikulec@seznam.cz
Vsetin, Czech Republic

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

	The above copyright notice and this permission notice shall be included in all
	copies or substantial portions of the Software.

	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
	AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
	OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
	SOFTWARE.
*/


using System;
using System.Collections;
using System.Collections.Generic;

namespace UrhoKosticky
{
	public static class BoxColorSchemas
	{


		public static List<int[]> boxes0(int positionX, int positionY)
		{
			List<int[]> boxes0 = new List<int[]> () {
				new int[] { 0+positionX, 0+positionY, 0 },
				new int[] { 1+positionX, 0+positionY, 0 },
				new int[] { 2+positionX, 0+positionY, 0 },
				new int[] { 0+positionX, 1+positionY, 0 },
				new int[] { 2+positionX, 1+positionY, 0 },
				new int[] { 0+positionX, 2+positionY, 0 },
				new int[] { 2+positionX, 2+positionY, 0 },
				new int[] { 0+positionX, 3+positionY, 0 },
				new int[] { 2+positionX, 3+positionY, 0 },
				new int[] { 0+positionX, 4+positionY, 0 },
				new int[] { 1+positionX, 4+positionY, 0 },
				new int[] { 2+positionX, 4+positionY, 0 }

			};
			return boxes0;
		}

		public static List<int[]> boxes1(int positionX, int positionY)
		{
			List<int[]> boxes1 = new List<int[]> (){
				new int[] {0+positionX,0+positionY,0},
				new int[] {0+positionX,1+positionY,0},
				new int[] {0+positionX,2+positionY,0},
				new int[] {0+positionX,3+positionY,0},
				new int[] {0+positionX,4+positionY,0}

			};
			return boxes1;
		}

		public static List<int[]> boxes2(int positionX, int positionY)
		{
			List<int[]> boxes2 = new List<int[]> (){
				new int[] {0+positionX,0+positionY,0},
				new int[] {1+positionX,0+positionY,0},
				new int[] {2+positionX,0+positionY,0},
				new int[] {0+positionX,1+positionY,0},
				new int[] {0+positionX,2+positionY,0},
				new int[] {1+positionX,2+positionY,0},
				new int[] {2+positionX,2+positionY,0},
				new int[] {2+positionX,3+positionY,0},
				new int[] {0+positionX,4+positionY,0},
				new int[] {1+positionX,4+positionY,0},
				new int[] {2+positionX,4+positionY,0}
			};
			return boxes2;
		}

		public static List<int[]> boxes3(int positionX, int positionY)
		{
			List<int[]> boxes3 = new List<int[]> (){
				new int[] {0+positionX,0+positionY,0},
				new int[] {1+positionX,0+positionY,0},
				new int[] {2+positionX,0+positionY,0},
				new int[] {2+positionX,1+positionY,0},
				new int[] {0+positionX,2+positionY,0},
				new int[] {1+positionX,2+positionY,0},
				new int[] {2+positionX,2+positionY,0},
				new int[] {2+positionX,3+positionY,0},
				new int[] {0+positionX,4+positionY,0},
				new int[] {1+positionX,4+positionY,0},
				new int[] {2+positionX,4+positionY,0}
			};
			return boxes3;
		}



		public static List<int[]> boxes5(int positionX, int positionY)
		{
			List<int[]> boxes5 = new List<int[]> (){
				new int[] {0+positionX,0+positionY,0},
				new int[] {1+positionX,0+positionY,0},
				new int[] {2+positionX,0+positionY,0},
				new int[] {2+positionX,1+positionY,0},
				new int[] {0+positionX,2+positionY,0},
				new int[] {1+positionX,2+positionY,0},
				new int[] {2+positionX,2+positionY,0},
				new int[] {0+positionX,3+positionY,0},
				new int[] {0+positionX,4+positionY,0},
				new int[] {1+positionX,4+positionY,0},
				new int[] {2+positionX,4+positionY,0}

			};
			return boxes5;
		}

		public static List<int[]> boxes6(int positionX, int positionY)
		{
			List<int[]> boxes5 = new List<int[]> (){
				new int[] {0+positionX,0+positionY,0},
				new int[] {1+positionX,0+positionY,0},
				new int[] {2+positionX,0+positionY,0},
				new int[] {2+positionX,1+positionY,0},
				new int[] {0+positionX,1+positionY,0},
				new int[] {0+positionX,2+positionY,0},
				new int[] {1+positionX,2+positionY,0},
				new int[] {2+positionX,2+positionY,0},
				new int[] {0+positionX,3+positionY,0},
				new int[] {0+positionX,4+positionY,0},
				new int[] {1+positionX,4+positionY,0},
				new int[] {2+positionX,4+positionY,0}

			};
			return boxes5;
		}
	}
}

