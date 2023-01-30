// Количество направлений, по которым может располагаться корабль: 1 - от головы на север, далее - по часовой стрелке.
int DIRECTIONS = 4;

// Определение границ игрового поля.
int FILLNUMBER = 8;

// Начальное количество палуб корабля для генерации сита
int MAXDECKS = 4;

// Символ пустой клетки для начального заполнения игрового поля
int EMPTY = 0;

// Символ для заполнения сита поиска корабля
int SIEVENUMBER = 1;

// Поле статуса в массиве корабля [4]
int STATUS = 4;

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


// Массив кораблей. [0] - x, [1] - y; [2] - direction; [3] - decks; [4] - status: n = 0 - destroyed, n = [3] — new ship, other n — wounded 
int[][] squadronComputer = {
	new int[] {0, 0, 0, 4, 4},
	new int[] {0, 0, 0, 3, 3}, new int[] {0, 0, 0, 3, 3},
	new int[] {0, 0, 0, 2, 2}, new int[] {0, 0, 0, 2, 2}, new int[] {0, 0, 0, 2, 2},
	new int[] {0, 0, 1, 1, 1}, new int[] {0, 0, 1, 1, 1}, new int[] {0, 0, 1, 1, 1}, new int[] {0, 0, 1, 1, 1}
};

int[][] squadronPlayer = {
	new int[] {0, 0, 0, 4, 4},
	new int[] {0, 0, 0, 3, 3}, new int[] {0, 0, 0, 3, 3},
	new int[] {0, 0, 0, 2, 2}, new int[] {0, 0, 0, 2, 2}, new int[] {0, 0, 0, 2, 2},
	new int[] {0, 0, 1, 1, 1}, new int[] {0, 0, 1, 1, 1}, new int[] {0, 0, 1, 1, 1}, new int[] {0, 0, 1, 1, 1}
};


CreateMap(computerWorld, squadronComputer);
System.Console.WriteLine("computerWorld is created");
System.Console.WriteLine();

CreateMap(playerWorld, squadronPlayer);
System.Console.WriteLine("playerWorld is created");

array2dToScreen(playerWorld);

// Вывод поля игрока и запрос, в бесконечном цикле, на генерацию новой 
// расстановки кораблей для игрока (человека)
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
// ===============================================================================
// =================================== Конец раздела генерации игровых полей =====
// ===============================================================================



// -------------------- карта клеток, по которым имеет смысл делать выстрел (для компьютера)
int[,] computerBitShotMap = new int[WORLDSIZE, WORLDSIZE];

// -------------------- Заполнение массива. Для заполнения массива, в конкретном, вырожденном, 
// -------------------- случае можно использовать решето без шага и сдвига  !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
array2dFillWithNumber(computerBitShotMap, SIEVENUMBER);
array2dBorder(computerBitShotMap, 0);


// -------------------- карта клеток, по которым имеет смысл делать выстрел (для человека)
int[,] playerBitShotMap = new int[WORLDSIZE, WORLDSIZE];
int[,] playerSieveMap = new int[WORLDSIZE, WORLDSIZE];

array2dFillWithNumber(playerBitShotMap, SIEVENUMBER);
array2dFillWithNumber(playerSieveMap, SIEVENUMBER);

array2dBorder(playerBitShotMap, 0);
array2dBorder(playerSieveMap, 0);


// -------------------- карта-решето для поиска текущего корабля
int[,] computerInitialSieve = new int[WORLDSIZE, WORLDSIZE];

// -------------------- генерация сита для компьютера
NewSieve(computerInitialSieve, computerBitShotMap, squadronPlayer);

array2dToScreen(computerInitialSieve);


// Генерация полей поиска для возможного выстрела
int MAXNUMBERTOFIND = 1;
int maxPlayerNumberToFind = 1;

// int localCoordinates = GetCellToFire(computerInitialSieve, MAXNUMBERTOFIND);

// int localYToFire = localCoordinates / 100;
// int localXToFire = localCoordinates % 100;

// System.Console.Write("localXToFire ");
// System.Console.WriteLine(localXToFire);
// System.Console.Write("localYToFire ");
// System.Console.WriteLine(localYToFire);



// ------------------------------------- MAIN CYCLE


// while (true)
// {
// 	if (playerTurn)
// 	{
// 		while (true)
// 		{
// 			maxPlayerTurn++;

// 			int localCoordinates = GetCellToFire(playerBitShotMap, maxPlayerNumberToFind);

// 			int localYToFire = localCoordinates / 100;
// 			int localXToFire = localCoordinates % 100;

// 			System.Console.Write("localXToFire ");
// 			System.Console.WriteLine(localXToFire);
// 			System.Console.Write("localYToFire ");
// 			System.Console.WriteLine(localYToFire);

// 			playerBitShotMap[localYToFire, localXToFire] = 0;

// 			int shootResult = FireResult(localXToFire, localYToFire, squadronComputer);

// 			// Выстрел в молоко
// 			if (shootResult == -1)
// 			{
// 				playerTurn = !playerTurn;
// 				break;
// 			}

// 			// int shipSize = squadronComputer[shootResult][3];
// 			int shipStatus = squadronComputer[shootResult][4];

// 			// Если корабль потоплен
// 			if (shipStatus == 0)
// 			{
// 				maxPlayerNumberToFind = 1;
// 				FillCellsAroundShip(squadronComputer[shootResult], playerBitShotMap, 0);
// 				computerShips--;
// 				if (computerShips == 0)
// 				{
// 					GameOver;
// 				}
// 				continue;
// 			}

// 			// Если корабль ранен
// 			maxPlayerNumberToFind = 2;

// 			FillCellsAroundWoundedDeckDiagonal(localXToFire, localYToFire, playerBitShotMap);
// 			FillCellsAroundWoundedDeckDiagonal(localXToFire, localYToFire, playerSieveMap);

// 			FillCellsAroundWoundedDeckCross(localXToFire, localYToFire, playerBitShotMap, playerSieveMap, maxPlayerNumberToFind);
// 			continue;

// 		}
// 	}

// 	if (!playerTurn)
// 	{
// 		while (true)
// 		{
// 			maxComputerTurn++;

// 			int localCoordinates = GetCellToFire(computerBitShotMap, MAXNUMBERTOFIN);

// 			int localYToFire = localCoordinates / 100;
// 			int localXToFire = localCoordinates % 100;

// 			System.Console.Write("localXToFire ");
// 			System.Console.WriteLine(localXToFire);
// 			System.Console.Write("localYToFire ");
// 			System.Console.WriteLine(localYToFire);

// 			computerBitShotMap[localYToFire, localXToFire] = 0;

// 			int shootResult = FireResult(localXToFire, localYToFire, squadronPlayer);

// 			// Выстрел в молоко
// 			if (shootResult == -1)
// 			{
// 				playerTurn = !playerTurn;
// 				break;
// 			}

// 			// int shipSize = squadronComputer[shootResult][3];
// 			int shipStatus = squadronComputer[shootResult][4];

// 			// Если корабль потоплен
// 			if (shipStatus == 0)
// 			{

// 				maxPlayerNumberToFind = 1;
// 				FillCellsAroundShip(squadronPlayer[shootResult], computerBitShotMap, 0);
// 				playerShips--;
// 				if (computerShips == 0)
// 				{
// 					GameOver;
// 				}

// 				// _____________________________________________________________________________
// 				// _____________________________________________________________________________
// 				// _____________________________________________________________________________
// 				// Сделать проверку и при надобности сгенерировать новое сито!!!
// 				// 20.01.23

// 				continue;
// 			}

// 			// Если корабль ранен
// 			MAXNUMBERTOFIND = 2;

// 			FillCellsAroundWoundedDeckDiagonal(localXToFire, localYToFire, computerBitShotMap);
// 			FillCellsAroundWoundedDeckDiagonal(localXToFire, localYToFire, computerInitialSieve);

// 			FillCellsAroundWoundedDeckCross(localXToFire, localYToFire, computerBitShotMap, playerSieveMap, maxPlayerNumberToFind);
// 			continue;

// 		}
// 	}
// }


bool playerTurn = true;

// Бросаем монетку на очередность хода
int coinToss = GetRandomFrom(0, 1);
if (coinToss == 0)
{
	playerTurn = !playerTurn;
}

int playerShip = 4;
int computerShip = 4;

int maxPlayerTurn = 0;
int maxComputerTurn = 0;



while (true)
{
	System.Console.Write("Turn ");
	System.Console.WriteLine(playerTurn);
	Console.ReadLine();

	if (playerTurn)
	{
		maxPlayerTurn = MainPlay(playerBitShotMap, playerSieveMap, squadronComputer, playerShip, maxPlayerTurn);
	}

	if (!playerTurn)
	{
		maxComputerTurn = MainPlay(computerBitShotMap, computerInitialSieve, squadronPlayer, computerShip, maxComputerTurn);
	}

}


// ----------------------- NEW UNIVERSAL MAIN PLAY FUNCTION
int MainPlay(int[,] map, int[,] sieve, int[][] squadron, int currentDecksToFind, int turn)
{
	int numberToFind = MaxIn2dArray(map);

	while (true)
	{
		turn++;

		// Вывод игрового поля !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
		System.Console.WriteLine();
		System.Console.WriteLine("Players Map");
		ReadyToPrintMap(playerBitShotMap, squadronComputer);

		System.Console.WriteLine();
		System.Console.WriteLine("Computers Map");
		ReadyToPrintMap(computerBitShotMap, squadronPlayer);

		int localCoordinates = GetCellToFire(sieve, numberToFind);

		int localY = localCoordinates / 100;
		int localX = localCoordinates % 100;

		System.Console.Write("localX ");
		System.Console.WriteLine(localX);
		System.Console.Write("localY ");
		System.Console.WriteLine(localY);

		map[localY, localX] = 0;
		sieve[localY, localX] = 0;

		int shootResult = FireResult(localX, localY, squadron);

		// Выстрел в молоко
		if (shootResult == -1)
		{
			playerTurn = !playerTurn;
			return turn;
		}

		// узнаем статус корабля, к который попали
		int shipStatus = squadron[shootResult][STATUS];

		// Если корабль потоплен
		if (shipStatus == 0)
		{
			int newShipToFind = shipRemainMax(squadron);

			if (newShipToFind == -1)
			{
				GameOver(turn);
				return -1;
			}

			numberToFind = 1;

			FillCellsAroundShip(squadron[shootResult], map, 0);
			FillCellsAroundShip(squadron[shootResult], sieve, 0);

			if (!playerTurn)
			{
				if (newShipToFind < MAXDECKS)
				{
					MAXDECKS = newShipToFind;

					NewSieve(computerInitialSieve, computerBitShotMap, squadronPlayer);
				}
			}
			continue;
		}

		// Если корабль ранен
		numberToFind = 2;

		FillCellsAroundWoundedDeckDiagonal(localX, localY, map);
		FillCellsAroundWoundedDeckDiagonal(localX, localY, sieve);

		FillCellsAroundWoundedDeckCross(localX, localY, map, sieve, numberToFind);
		continue;

	}
}






// --------------- MaxIn2dArray
int MaxIn2dArray(int[,] map)
{
	int maxValue = 0;

	foreach (int cell in map)
	{
		maxValue = maxValue < cell ? cell : maxValue;
	}
	Console.WriteLine("наибольшее число в этом двухмерном массиве : " + maxValue);
	return maxValue;
}


// -------------------- Функция генерации нового решета
void NewSieve(int[,] sieve, int[,] bitmap, int[][] squadron)
{
	// -------------------- поиск и выбор числа палуб самого большого живого корабля в моменте
	int decksForRandomChoice = shipRemainMax(squadron);

	// -------------------- случайный сдвиг для текущего сита. (Сито — это сито. Для просеивания. Никаких Dart-вёдер.)
	int shiftRandom = GetRandomFrom(0, decksForRandomChoice - 1);

	sieveGenerate(sieve, bitmap, shiftRandom, SIEVENUMBER, decksForRandomChoice);


	// // -------------------- поиск и выбор числа палуб самого большого живого корабля в моменте
	// int decksForRandomChoice = currentTargetShip(shipRemainMax(squadronPlayer));


	// // -------------------- случайный сдвиг для текущего сита. (Сито — это сито. Для просеивания. Никаких Dart-вёдер.)
	// int shiftRandom = GetRandomFrom(0, decksForRandomChoice - 1);
}

// --------------- shipRemainMax
int shipRemainMax(int[][] squadron)
{
	foreach (var ship in squadron)
	{
		if (ship[4] != 0)
		{
			return ship[3];
		}
	}
	return -1;
}

// --------------- Fill Cells Around Wounded Deck
void FillCellsAroundWoundedDeckDiagonal(int localX, int localY, int[,] map)
{
	map[localX - 1, localY - 1] = 0;
	map[localX + 1, localY - 1] = 0;
	map[localX - 1, localY + 1] = 0;
	map[localX + 1, localY + 1] = 0;
}

void FillCellsAroundWoundedDeckCross(int localX, int localY, int[,] map, int[,] bitMap, int fillNumber)
{
	map[localX, localY - 1] = fillNumber * bitMap[localX, localY - 1];
	map[localX - 1, localY] = fillNumber * bitMap[localX - 1, localY];
	map[localX + 1, localY] = fillNumber * bitMap[localX + 1, localY];
	map[localX, localY + 1] = fillNumber * bitMap[localX, localY + 1];
}



// ------------------- FireResult
int FireResult(int localX, int localY, int[][] localSquadron)
{
	int count = 0;

	foreach (var ship in localSquadron)
	{

		if (ship[4] == 0)
		{
			continue;
		}

		int shipHeadX = ship[0];
		int shipHeadY = ship[1];
		int shipDirection = ship[2];
		int shipDecks = ship[3];

		// ------------------------------------------- Этот блок повторяется в функции ShipPlace. Надо будет переписать!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
		int[] dXdYupperLevel = GetDirection(shipDirection);

		// получаем приращение координат от головы корабля
		int dXupperLevel = dXdYupperLevel[0];
		int dYupperLevel = dXdYupperLevel[1];

		// получаем координату последней палубы корабля
		int shipTailX = localX + (shipDecks - 1) * dXupperLevel;
		int shipTailY = localY + (shipDecks - 1) * dYupperLevel;

		// ------------------------------ Проверка на попадание по кораблю
		// Если мы внутри корабля по координате X
		if (localX == shipHeadX
				// И внутри корабля по координате Y
				&& (localY <= Math.Max(shipHeadY, shipTailY)
				&& localY >= Math.Min(shipHeadY, shipTailY))
				||
			// Или наоборот
			localY == shipHeadY
				&& (localX <= Math.Max(shipHeadX, shipTailX)
				&& localX >= Math.Min(shipHeadX, shipTailX)))
		// И при этом — корабль не потоплен

		// Ерунда. Мы по определению не можем стрелять по потопленному кораблю!
		// || ship[4] != 0)
		{
			ship[4]--;
			return count;
		}
		count++;
	}
	return -1;
}


// -------------------- GameOver
void GameOver(int number)
{
	System.Console.WriteLine("GAME OVER");
	return;
}





// ------------------------- GetCellToFire
int GetCellToFire(int[,] sieve, int numberToFind)
{
	int numberOfCellsToFire = countCellsWithNumber(sieve, numberToFind);
	System.Console.Write("numberOfCellsToFire ");
	System.Console.WriteLine(numberOfCellsToFire);

	int randomCellToFire = GetRandomFrom(1, numberOfCellsToFire);
	System.Console.Write("randomCellToFire ");
	System.Console.WriteLine(randomCellToFire);

	int xAnDyToFire = randomCellXY(randomCellToFire, sieve, numberToFind);

	return xAnDyToFire;
}

// -------------------- Выбор решета для поиска корабля
int currentTargetShip(int[] squadron)
{
	int currentTarget = -1;

	int localIndex = 0;

	while (true)
	{

		if (squadron[localIndex] != 0)
		{
			currentTarget = MAXDECKS - localIndex;
			return currentTarget;
		}

		localIndex++;

		if (localIndex > squadron.Length)
		{
			return currentTarget;
		}
	}
}

// -------------------- Сито для поиска текущего корабля
void sieveGenerate(int[,] sieve, int[,] shotThroughMap, int shiftRandom, int fillNumber, int decks)
{
	shiftRandom = shiftRandom % decks;
	int localX = 1 + shiftRandom;
	int localY = 1;

	int boardBorder = 2;

	int boardWidth = sieve.GetLength(1) - boardBorder;
	// System.Console.WriteLine("boardWidth");
	// System.Console.WriteLine(boardWidth);
	int boardHight = sieve.GetLength(0) - boardBorder;
	// System.Console.WriteLine("boardHight");
	// System.Console.WriteLine(boardHight);

	while (true)
	{
		sieve[localY, localX] = fillNumber * shotThroughMap[localY, localX];

		localX = localX + decks;

		if (localX > boardWidth)
		{

			localX = (localY + shiftRandom) % decks + 1;

			localY++;

			if (localY > boardHight)
			{
				break;
			}
		}
	}
}

// -------------------- Подготовка и вывод игровых полей компьютера и игрока
void PrepareMap(int[,] mapPlayer, int[,] sievePlayer, bool showShip)
{

}


// -------------------- Вывод символьного игрового поля
// void PrintMap(int[,] map, int emptyNumber, int ship)
void PrintMap(int[,] map)
{
	System.Console.WriteLine(" \t  1 2 3 4 5 6 7 8 9 10 \t  1 2 3 4 5 6 7 8 9 10");
	for (int i = 0; i < map.GetLength(0); i++)
	{
		if (i < 1 || i > map.GetLength(0) - 2)
		{
			System.Console.Write(" ");
			System.Console.Write("\t");
		}

		if (i > 0 && i < map.GetLength(0) - 1)
		{
			System.Console.Write(i);
			System.Console.Write("\t");
		}

		for (int j = 0; j < map.GetLength(1); j++)
		{
			if (map[j, i] == FILLNUMBER)
			{
				System.Console.Write("*");
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
				System.Console.Write("@");
				System.Console.Write(" ");
			}
		}
		System.Console.WriteLine();
	}
}

// // -------------------- MapState
// int[,] MapState(int[,] map, int[][] squadron)
// {

// }


// -------------------- Создание игрового поля для игрока или компьютера
void CreateMap(int[,] map, int[][] squadron)
{
	array2dBorder(map, FILLNUMBER);

	int[,] stageStart = new int[map.GetLength(0), map.GetLength(0)];
	Array.Copy(map, stageStart, map.Length);

	foreach (int[] element in squadron)
	{
		ShipPlace(element, stageStart);
		FillCellsAroundShip(element, stageStart, SIEVENUMBER);
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

	// array2dToScreen(mapLocal);
	PrintMap(mapLocal);
}

// --------------------------- New Year 2023 - SHIP PLACE
void ShipPlace(int[] ship, int[,] map)
{
	int decks = ship[3];
	int[,] mapLocal = new int[map.GetLength(0), map.GetLength(0)];

	Array.Copy(map, mapLocal, map.Length);

	int emptyCells = countCellsWithNumber(mapLocal, EMPTY);

	while (emptyCells > 0)
	{
		int randomCell = GetRandomFrom(1, emptyCells);
		System.Console.Write("randomCell ");
		System.Console.WriteLine(randomCell);

		int xAnDy = randomCellXY(randomCell, mapLocal, EMPTY);

		int localY = xAnDy / 100;
		int localX = xAnDy % 100;

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
			// ------------ commented on 170123 !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

			// System.Console.Write("Direction ");
			// System.Console.Write(item);
			// System.Console.Write(" | tailX ");
			// System.Console.Write(tailX);
			// System.Console.Write(" tailY ");
			// System.Console.Write(tailY);
			// System.Console.Write(" | ");
			// System.Console.Write(ship[3]);
			// System.Console.WriteLine();

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
void FillCellsAroundShip(int[] ship, int[,] map, int fillNumber)
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
				map[j, i] = fillNumber;
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
int randomCellXY(int number, int[,] arr, int numberToFind)
{
	int count = 0;
	int localxAnDy = 0;
	for (int i = 1; i < arr.GetLength(0) - 1; i++)
	{
		for (int j = 1; j < arr.GetLength(1) - 1; j++)
		{
			if (arr[i, j] == numberToFind)
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

// ------------ count of cells contain NUMBER ---
int countCellsWithNumber(int[,] arr, int numberToFind)
{
	int count = 0;
	for (int i = 1; i < arr.GetLength(0) - 1; i++)
	{
		for (int j = 1; j < arr.GetLength(1) - 1; j++)
		{
			if (arr[i, j] == numberToFind)
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

// -------------- 2d-Array fill with number ---
void array2dFillWithNumber(int[,] arr2d, int number)
{
	for (int i = 0; i < arr2d.GetLength(0); i++)
	{
		for (int j = 0; j < arr2d.GetLength(1); j++)
		{
			arr2d[i, j] = number;
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