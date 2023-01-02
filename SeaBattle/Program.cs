int DIRECTIONS = 4;

// Размер массива игрового поля - 10х10 и ограничение края единицами. Итоговый размер 12х12.
int[,] computerWorld = new int[12, 12];

int[][] squadron = {
	new int[] {0, 0, 0, 4, 0},
	new int[] {0, 0, 0, 3, 0}, new int[] {0, 0, 0, 3, 0},
	new int[] {0, 0, 0, 2, 0}, new int[] {0, 0, 0, 2, 0}, new int[] {0, 0, 0, 2, 0},
	new int[] {0, 0, 0, 1, 0}, new int[] {0, 0, 0, 1, 0}, new int[] {0, 0, 0, 1, 0}, new int[] {0, 0, 0, 1, 0}
};


array2dBorder(computerWorld);
// array2dToScreen(computerWorld);

int[,] stageStart = new int[12, 12];
Array.Copy(computerWorld, stageStart, computerWorld.Length);

System.Console.WriteLine("Empty cell in array:");
System.Console.WriteLine(countEmptyCells(stageStart));
Console.Clear();



foreach (int[] element in squadron)
{
    System.Console.WriteLine("888888888888");
	Console.WriteLine("Массив: [ " + string.Join(" | ", element) + " ]");

	ShipPlace(element, stageStart);
	FillCellsAroundShip(element, stageStart);
}

// // ---------------- Old-style place ship and output
// foreach (int[] element in squadron)
// {
//     System.Console.WriteLine("888888888888");
// 	Console.WriteLine("Массив: [ " + string.Join(" | ", element) + " ]");

// 	shipOnMap(element, stageStart);
// 	FillCellsAroundShip(element, stageStart);
// }

// foreach (int[] element in squadron)
// {
//     System.Console.WriteLine("77777777777777777777777777777777777");
// 	Console.WriteLine("Массив: [ " + string.Join(" | ", element) + " ]");
// }




// --------------------------- new-style SHIP PLACE
void ShipPlace(int[] ship, int[,] map)
{
	int[,] mapLocal = new int[12, 12];
	
	Array.Copy(map, mapLocal, map.Length);
	int emptyCells = countEmptyCells(mapLocal);

	while (emptyCells > 0)
	{
		int sucsess = -1;

		int randomCell = GetRandomFrom(1, emptyCells + 1);

		int xAnDy = randomCellXY(randomCell, mapLocal);

		int localX = xAnDy / 100;
		int localY = xAnDy % 100;

		int decks = ship[3];

		int[] directionArray = new int[DIRECTIONS];
		arrayUniqFill(directionArray);

		System.Console.WriteLine("DIRECTIONS");
		System.Console.WriteLine(DIRECTIONS);

		for (int i = 1; i <= DIRECTIONS; i++)
		{
			int[] dXdYupperLevel = GetDirection(directionArray[i - 1]);
			
			// получаем приращение координат от головы корабля
			int dXupperLevel = dXdYupperLevel[0];
			int dYupperLevel = dXdYupperLevel[1];

			for (int j = 1; j < decks; j++)
			{
				System.Console.WriteLine($"Point X: {localX + dXupperLevel * j}. Point Y: {localY + dYupperLevel * j} ");

				if (mapLocal[localY + dYupperLevel * j, localX + dXupperLevel * j] != 0)
				{
					System.Console.WriteLine("BREAK!!!");

					mapLocal[localY, localX] = 1;
					emptyCells--;

					sucsess = 0;
					
					break;
				}
				
				mapLocal[localY + dYupperLevel * j, localX + dXupperLevel * j] = 6;
				
			}

			if (sucsess == 0)
			{
				break;
			}

			// --------------------- ALL DECKS on MAP
			ship[0] = localX;
			ship[1] = localY;
			ship[2] = directionArray[i - 1];
			
			// ------- not used ever
			ship[4] = 1;
			System.Console.WriteLine("localX");
			System.Console.WriteLine(localX);
			System.Console.WriteLine("localY");
			System.Console.WriteLine(localY);


			array2dToScreen(mapLocal);
			return;

		}

		
	}
}
	

	


	

	



// --------------------------- fill cell around ship
void FillCellsAroundShip(int[] ship, int[,] map)
{
	int headDeckX = ship[0];
	int headDeckY = ship[1];

	int[] dXdY = GetDirection(ship[2]);
	int dX = dXdY[0];
	int dY = dXdY[1];
	System.Console.WriteLine("dX");
	System.Console.WriteLine(dX);
	System.Console.WriteLine("dY");
	System.Console.WriteLine(dY);

	for (int decks = 1; decks <= ship[3]; decks++)
	{
		for (int i = headDeckX - 1; i <= headDeckX + 1; i++)
		{
			for (int j = headDeckY - 1; j <= headDeckY + 1; j++)
			{
				// j - Y, i - X
				map[j,i] = 1;
			}
		}
		headDeckX = headDeckX + dX;
		headDeckY = headDeckY + dY;
	}
	array2dToScreen(map);
}

// System.Console.WriteLine(shipOnMap(squadron[0], stageStart));

// System.Console.WriteLine("++++++++++++++++++++++++++");
// array2dToScreen(stageStart);

// // ---------------- Trying place ship on map ---------
// int shipOnMap(int[] arrShip, int[,] arrMap)
// {
// 	int[,] arrLocalMap = new int[12, 12];
// 	Array.Copy(arrMap, arrLocalMap, arrMap.Length);

// 	int sucsess = -1;

// 	int decks = arrShip[3];
// 	int emptyCells = countEmptyCells(arrLocalMap);

// 	if (emptyCells < decks)
// 	{
// 		sucsess = 0;
// 		return sucsess;
// 	}

	
// 	int randomCell = GetRandomFrom(1, emptyCells + 1);

// 	// получаем координаты X и Y выбранной выше случайной ячейки
// 	int xAnDy = randomCellXY(randomCell, arrMap);

// 	int newX = xAnDy / 100;
// 	int newY = xAnDy % 100;

// 	System.Console.WriteLine("xAnDy");
// 	System.Console.WriteLine(xAnDy);
// 	System.Console.WriteLine("--------");

// 	System.Console.WriteLine(decks);
// 	System.Console.Write(newX);
// 	System.Console.Write(" ");
// 	System.Console.Write(newY);
// 	System.Console.WriteLine();

// 	// Успешно помещаем голову корабля в пустую клетку.
// 	arrLocalMap[newY, newX] = 7;

// 	// создаем массив случайных направлений
// 	int[] probeArray = new int[DIRECTIONS];
// 	arrayUniqFill(probeArray);

// 	for (int i = 1; i <= DIRECTIONS; i++)
// 	{
// 		System.Console.Write("...... ");
// 		System.Console.Write(i);
// 		System.Console.Write(" .....");
// 		System.Console.WriteLine();

// 		int[] dXdYupperLevel = GetDirection(probeArray[i - 1]);
// 		System.Console.Write("Direction: ");
// 		System.Console.Write(probeArray[i - 1]);
// 		System.Console.WriteLine();

// 		// получаем приращение координат от головы корабля
// 		int dXupperLevel = dXdYupperLevel[0];
// 		int dYupperLevel = dXdYupperLevel[1];

// 		for (int j = 1; j < decks; j++)
// 		{
// 			// System.Console.Write("Point: ");
// 			// System.Console.WriteLine(arrMap[newY + dYupperLevel * j, newX + dXupperLevel * j]);
// 			System.Console.WriteLine($"Point X: {newX + dXupperLevel * j}. Point Y: {newY + dYupperLevel * j} ");

// 			if (arrLocalMap[newY + dYupperLevel * j, newX + dXupperLevel * j] != 0)
// 			{
// 				System.Console.WriteLine("BREAK!!!");
// 				Array.Copy(arrMap, arrLocalMap, arrMap.Length);
// 				arrLocalMap[newY, newX] = 7;
// 				sucsess = 0;
// 				break;
// 			}
// 			sucsess = 1;
// 			arrLocalMap[newY + dYupperLevel * j, newX + dXupperLevel * j] = 6;
// 			arrShip[0] = newX;
// 			arrShip[1] = newY;
// 			arrShip[2] = probeArray[i - 1];
// 		}

// 		if (sucsess == 0)
// 		{
// 			continue;
// 		}
// 		break;
// 	}
// 	if (sucsess == 1)
// 	{
// 		array2dToScreen(arrLocalMap);
// 	}
// 	return sucsess;
// }


// ------------ Get direction -----------
int[] GetDirection(int number)
{
	int[] dXdY = new int[2];
	int dX = 0;
	int dY = 0;

	if (number == 1)
	{
		dY = -1;
	}

	if (number == 2)
	{
		dX = 1;
	}

	if (number == 3)
	{
		dY = 1;
	}

	if (number == 4)
	{
		dX = -1;
	}
	dXdY[0] = dX;
	dXdY[1] = dY;

	return dXdY;
}

// ------------ Get random cell X, Y ----
int randomCellXY(int number, int[,] arr)
{
	int count = 0;
	int localxAnDy = 0;
	for (int i = 1; i < arr.GetLength(0) - 1; i++)
	{
		for (int j = 1; j < arr.GetLength(1) - 1; j++)
		{
			if (arr[i, j] == 0)
			{
				count++;
				if (count == number)
				{
					localxAnDy = i * 100 + j;
					return localxAnDy;
				}
			}
		}
	}
	return localxAnDy;

}

// ------------ count of empty cells ---
int countEmptyCells(int[,] arr)
{
	int count = 0;
	for (int i = 1; i < arr.GetLength(0) - 1; i++)
	{
		for (int j = 1; j < arr.GetLength(1) - 1; j++)
		{
			if (arr[i, j] == 0)
			{
				count++;
			}
		}
	}
	return count;
}

// --------- square border in array fill with 1 -----
void array2dBorder(int[,] arr2d)
{
	for (int i = 0; i < arr2d.GetLength(0); i++)
	{
		arr2d[i, 0] = 1;
		arr2d[i, arr2d.GetLength(1) - 1] = 1;
	}

	for (int i = 0; i < arr2d.GetLength(0); i++)
	{
		for (int j = 1; j < arr2d.GetLength(1) - 1; j++)
		{
			if (i == 0 || i == arr2d.GetLength(0) - 1)
			{
				arr2d[i, j] = 1;
			}
		}
	}
}

// -------------- 2d-Array to screen ---
void array2dToScreen(int[,] arr2d)
{
	for (int i = 0; i < arr2d.GetLength(0); i++)
	{
		for (int j = 0; j < arr2d.GetLength(1); j++)
		{
			System.Console.Write($"{arr2d[i, j]} ");
		}
		System.Console.WriteLine();
	}
}

// ------------------- fill ARRAY
int[] arrayFill(int[] arr, int left, int right)
{
	for (int i = 0; i < arr.Length; i++)
	{
		arr[i] = GetRandomFrom(left, right);
	}
	System.Console.WriteLine("=====================");
	Console.WriteLine("Массив: [ " + string.Join(" | ", arr) + " ]");

	return arr;
}

// ------------------ fill Arr with unique numbers
void arrayUniqFill(int[] arr)
{
	int tempRandom;
	for (int i = 0; i < arr.Length; i++)
	{
		tempRandom = GetRandomFrom(1, arr.Length);

		if (arr.Contains(tempRandom))
		{
			i--;
		}

		if (!arr.Contains(tempRandom))
		{
			arr[i] = tempRandom;
		}

	}
	System.Console.WriteLine("=====================");
	Console.WriteLine("Массив: [ " + string.Join(" | ", arr) + " ]");
}

// --------------------- RANDOM NUMBER from - to -------------------
int GetRandomFrom(int bottom, int top)
{
	Random rnd = new Random();
	int result = rnd.Next(bottom, top + 1);
	return result;
}