// Количество направлений, по которым может располагаться корабль: 1 - от головы на север, далее - по часовой стрелке.
int DIRECTIONS = 4;

// Определение границ игрового поля.
int FILLNUMBER = 8;

// Размер массива игрового поля - 10х10 и ограничение края единицами. Итоговый размер 12х12.
// Не очень хороший пример для тестирования, но должен срабатывать вплоть до помещения всех однопалубных кораблей.
// В итеративном прогоне от четырехпалубника до последнего однопалубника на изначатьно пустом поле, матрица заполнения, не позволяющая встать кораблям ближе,
// чем возможно по правилам игры, равна 3 х (decks + 2). Она не даст хвосту следующего по списку корабля встать на невозможное место. 
int WORLDSIZE = 10 + 1 + 1;

int[,] computerWorld = new int[WORLDSIZE, WORLDSIZE]
										// // Массив для тестового прогона
										// 										{
										// 											{ 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
										// 											{ 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1},
										// 											{ 1, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 1},
										// 											{ 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1},
										// 											{ 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
										// 											{ 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
										// 											{ 1, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 1},
										// 											{ 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1},
										// 											{ 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
										// 											{ 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1},
										// 											{ 1, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 1},
										// 											{ 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1}
										// 										}
										;
int[,] playerWorld = new int[WORLDSIZE, WORLDSIZE];


// Массив кораблей. [0] - x, [1] - y; [2] - direction; [3] - decks; [4] - status: 0 - destroyed, 1 - wounded, 2 - initial (new ship, ready to begin fight)
int[][] squadronComputer = {
	new int[] {0, 0, 0, 4, 2},
	new int[] {0, 0, 0, 3, 2}, new int[] {0, 0, 0, 3, 2},
	new int[] {0, 0, 0, 2, 2}, new int[] {0, 0, 0, 2, 2}, new int[] {0, 0, 0, 2, 2},
	new int[] {0, 0, 1, 1, 1}, new int[] {0, 0, 1, 1, 1}, new int[] {0, 0, 1, 1, 1}, new int[] {0, 0, 1, 1, 1}
};

int[][] squadronPlayer = {
	new int[] {0, 0, 0, 4, 2},
	new int[] {0, 0, 0, 3, 2}, new int[] {0, 0, 0, 3, 2},
	new int[] {0, 0, 0, 2, 2}, new int[] {0, 0, 0, 2, 2}, new int[] {0, 0, 0, 2, 2},
	new int[] {0, 0, 1, 1, 1}, new int[] {0, 0, 1, 1, 1}, new int[] {0, 0, 1, 1, 1}, new int[] {0, 0, 1, 1, 1}
};


CreateMap(computerWorld, squadronComputer);
System.Console.WriteLine("computerWorld is created");
System.Console.WriteLine();

CreateMap(playerWorld, squadronPlayer);
System.Console.WriteLine("playerWorld is created");

array2dToScreen(playerWorld);

// PrintMap(playerWorld, squadronPlayer);

while (true)
{
	int stopGame = inputNumberPrompt("Generate new map? (1 - yes, 0 - no)");
		
		if (stopGame == 0)
		{
			break;
		}
		CreateMap(playerWorld, squadronPlayer);
		System.Console.WriteLine("playerWorld is created");

		ReadyToPrintMap(playerWorld, squadronPlayer);

		
}

// -------------------- Вывод символьного игрового поля
void PrintMap(int[,] map)
{
	for (int i = 0; i < map.GetLength(0); i++)
	{
		if (i > 1 || i < map.GetLength(0) - 1)
		{
			System.Console.Write(i);
			System.Console.Write("\t");
		}
		for (int j = 0; j < map.GetLength(1); j++)
		{
			if (map[j, i] == FILLNUMBER)
			{
				System.Console.Write("•");
				System.Console.Write(" ");
			}

			if (map[j, i] == 0)
			{
				System.Console.Write(" ");
				System.Console.Write(" ");
			}

			if (map[j, i] != 0 && map[j, i] != FILLNUMBER)
			{
				// System.Console.Write("@");
				System.Console.Write(map[j, i]);
				System.Console.Write(" ");
			}
		}
		System.Console.WriteLine();
	}
}

// -------------------- Создание игрового поля для игрока или компьютера
void CreateMap(int[,] map, int[][] squadron)
{
	array2dBorder(map, FILLNUMBER);

	int[,] stageStart = new int[map.GetLength(0), map.GetLength(0)];
	Array.Copy(map, stageStart, map.Length);

	foreach (int[] element in squadron)
	{
		ShipPlace(element, stageStart);
		FillCellsAroundShip(element, stageStart);
	}
	System.Console.WriteLine("All ships placed on map");

};

// --------------------------- map with ships PRINT
void ReadyToPrintMap(int[,] map, int[][] squadron)
{
	int[,] mapLocal = new int[map.GetLength(0), map.GetLength(0)];
	Array.Copy(map, mapLocal, map.Length);

	array2dBorder(mapLocal, FILLNUMBER);

	foreach (int[] ship in squadron)
	{
		int localX = ship[0];
		int localY = ship[1];
		int direction = ship[2];
		int decks = ship[3];
		
		mapLocal[localY, localX] = 7;
		
		if (decks > 1)
		{
			int[] dXdYupperLevel = GetDirection(direction);

			// получаем приращение координат от головы корабля
			int dXupperLevel = dXdYupperLevel[0];
			int dYupperLevel = dXdYupperLevel[1];

			for (int decksIndex = 1; decksIndex < decks; decksIndex++)
			{
				mapLocal[localY + decksIndex * dYupperLevel, localX + decksIndex * dXupperLevel] = decks;
			}				
		}
	}
	System.Console.WriteLine();
	System.Console.WriteLine("Map is ready");
	// array2dToScreen(mapLocal);
	PrintMap(mapLocal);
}

// --------------------------- New Year 2023 - SHIP PLACE
void ShipPlace(int[] ship, int[,] map)
{
	int decks = ship[3];
	int[,] mapLocal = new int[map.GetLength(0), map.GetLength(0)];

	Array.Copy(map, mapLocal, map.Length);

	int emptyCells = countEmptyCells(mapLocal);

	while (emptyCells > 0)
	{
		int randomCell = GetRandomFrom(1, emptyCells);
		System.Console.Write("randomCell ");
		System.Console.WriteLine(randomCell);

		int xAnDy = randomCellXY(randomCell, mapLocal);

		int localY = xAnDy / 100;
		int localX = xAnDy % 100;

		// emptyCells and decks OUTPUT
		System.Console.Write("emptyCells ");
		System.Console.Write(emptyCells);
		System.Console.Write(" localX ");
		System.Console.Write(localX);
		System.Console.Write(" localY ");
		System.Console.Write(localY);
		System.Console.Write(" | ");
		System.Console.Write(ship[3]);
		System.Console.WriteLine();

		mapLocal[localY, localX] = 7;
		array2dToScreen(mapLocal);

		int[] directionArray = new int[DIRECTIONS];
		arrayUniqFill(directionArray);

		// // ------------------- DIRECTIONS Array output
		// //output style:  [8, 1, 8, 8, 4, 8, 6, 8, 8, 8]
		// Console.WriteLine("[{0}]", string.Join(", ", directionArray));
		// System.Console.WriteLine();


		// После генерации массива из четырех случайных направлений 
		// итерируемся по ним в попытке поставить корабль на игровое поле
		foreach (int item in directionArray)
		{
			int[] dXdYupperLevel = GetDirection(item);

			// получаем приращение координат от головы корабля
			int dXupperLevel = dXdYupperLevel[0];
			int dYupperLevel = dXdYupperLevel[1];

			// получаем координату последней палубы корабля
			int tailX = localX + (decks - 1) * dXupperLevel;
			int tailY = localY + (decks - 1) * dYupperLevel;

			// ------------ OUTPUT x and y tail
			System.Console.Write("Direction ");
			System.Console.Write(item);
			System.Console.Write(" | tailX ");
			System.Console.Write(tailX);
			System.Console.Write(" tailY ");
			System.Console.Write(tailY);
			System.Console.Write(" | ");
			System.Console.Write(ship[3]);
			System.Console.WriteLine();

			// decks == 1 - видимо, избыточная проверка для однопалубника
			// Далее: если хвост корабля в пределах игрового поля, и эта клетка свободна - Ура. Ship on map
			if (
				decks == 1 ||
				tailX > 0 &&
				tailX < mapLocal.GetLength(0) - 1 &&
				tailY > 0 &&
				tailY < mapLocal.GetLength(1) - 1 &&
				mapLocal[tailY, tailX] == 0
				)
			{
				System.Console.WriteLine("Ship on map");
				System.Console.WriteLine();
				ship[0] = localX;
				ship[1] = localY;
				ship[2] = item;
				return;
			}

			System.Console.WriteLine("Ship cannot placed on map. New place need to find");
			continue;

		}

		emptyCells--;

	}

	System.Console.WriteLine("Epic fail!!!");

}


// --------------------------- fill cell around ship
void FillCellsAroundShip(int[] ship, int[,] map)
{
	int headDeckX = ship[0];
	int headDeckY = ship[1];

	int[] dXdY = GetDirection(ship[2]);
	int dX = dXdY[0];
	int dY = dXdY[1];

	for (int decks = 1; decks <= ship[3]; decks++)
	{
		for (int i = headDeckX - 1; i <= headDeckX + 1; i++)
		{
			for (int j = headDeckY - 1; j <= headDeckY + 1; j++)
			{
				// j - Y, i - X
				map[j, i] = 1;
			}
		}
		headDeckX = headDeckX + dX;
		headDeckY = headDeckY + dY;
	}
}

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

// --------- square border in array fill with NUMBER -----
void array2dBorder(int[,] arr2d, int number)
{
	for (int i = 0; i < arr2d.GetLength(0); i++)
	{
		arr2d[i, 0] = number;
		arr2d[i, arr2d.GetLength(1) - 1] = number;
	}

	for (int i = 0; i < arr2d.GetLength(0); i++)
	{
		for (int j = 1; j < arr2d.GetLength(1) - 1; j++)
		{
			if (i == 0 || i == arr2d.GetLength(0) - 1)
			{
				arr2d[i, j] = number;
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
	System.Console.WriteLine();
}

// ------------------- fill ARRAY random numbers [left, right] 
int[] arrayFill(int[] arr, int left, int right)
{
	for (int i = 0; i < arr.Length; i++)
	{
		arr[i] = GetRandomFrom(left, right);
	}

	return arr;
}

// ------------------ fill Arr with unique numbers from 1 to array.lenght
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
}

// --------------------- RANDOM NUMBER from - to -------------------
int GetRandomFrom(int bottom, int top)
{
	Random rnd = new Random();
	int result = rnd.Next(bottom, top + 1);
	return result;
}

// ------------------------ safe input int number
int inputNumberPrompt(string str)
{
	int number;
	string text;

	while (true)
	{
		Console.Write($"{str} ");
		text = Console.ReadLine();
		if (int.TryParse(text, out number))
		{
			break;
		}
		Console.WriteLine("Не удалось распознать число, попробуйте еще раз.");
	}
	return number;
}