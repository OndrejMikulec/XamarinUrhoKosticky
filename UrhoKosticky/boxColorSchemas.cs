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

