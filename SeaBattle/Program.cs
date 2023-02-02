// Количество направлений, по которым может располагаться корабль: 1 - от головы на север, далее - по часовой стрелке.
const int DIRECTIONS = 4;

// Определение границ игрового поля.
const int BORDER = 8;

// Символ пустой клетки для начального заполнения игрового поля
const int EMPTY = 0;

// Символ для заполнения сита поиска корабля
const int SIEVENUMBER = 1;

// Переменная сокрытия кораблей
bool HIDESHIP = false;

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

CreateMap(playerWorld, squadronPlayer);

// карта клеток, по которым имеет смысл делать выстрел (для игрока)
int[,] playerBitShotMap = new int[WORLDSIZE, WORLDSIZE];
array2dFillWithNumber(playerBitShotMap, SIEVENUMBER);
array2dBorder(playerBitShotMap, EMPTY);

// карта-решето (для игрока) - В основном цикле просто random
int[,] playerSieve = new int[WORLDSIZE, WORLDSIZE];
array2dFillWithNumber(playerSieve, SIEVENUMBER);
array2dBorder(playerSieve, EMPTY);

// карта клеток, по которым имеет смысл делать выстрел (для компьютера)
int[,] computerBitShotMap = new int[WORLDSIZE, WORLDSIZE];
array2dFillWithNumber(computerBitShotMap, SIEVENUMBER);
array2dBorder(computerBitShotMap, EMPTY);

// карта-решето для поиска текущего корабля
int[,] computerSieve = new int[WORLDSIZE, WORLDSIZE];
NewSieve(computerSieve, computerBitShotMap, squadronPlayer);


// Вывод поля игрока и запрос, в бесконечном цикле, на генерацию новой 
// расстановки кораблей для игрока (человека)
while (true)
{
	PrintSymbolMap(computerWorld, playerWorld, HIDESHIP);

	int stopGame = inputNumberPrompt("Generate new PLAYER map? (1 - yes, 0 - no)\n-----------------------------------------------------------");

	if (stopGame == 0)
	{
		break;
	}

	array2dFillWithNumber(playerWorld, EMPTY);

	CreateMap(playerWorld, squadronPlayer);
}
// ===============================================================================
// =================================== Конец раздела генерации игровых полей =====
// ===============================================================================


// ===============================================================================
// =================================== Подготовка и сам основной цикл  ===========
// ===============================================================================

bool playerTurn = true;

bool gameOver = false;

// Параметр для начальной генерации сита
int MAXDECKS = 4;

// Количество сделанных выстрелов
int playerShotsCount = 0;
int computerShotsCount = 0;
int overalTurnCount = 0;

// Бросаем монетку на очередность хода
int coinToss = GetRandomFrom(0, 1);
if (coinToss == 0)
{
	playerTurn = !playerTurn;
}

Console.WriteLine((playerTurn) ? "First shot — PLAYER" : "First shot — COMPUTER");


// =====================================================================
// ====================  MAIN LOOP  ====================================
// =====================================================================
while (true)
{
	overalTurnCount++;

	System.Console.WriteLine();
	System.Console.WriteLine("======================================================");
	System.Console.WriteLine();
	System.Console.WriteLine($"TURN {overalTurnCount}");

	if (playerTurn)
	{
		MainPlay(computerWorld, playerSieve, playerBitShotMap, squadronComputer, playerShotsCount);
	}

	if (!playerTurn)
	{
		MainPlay(playerWorld, computerSieve, computerBitShotMap, squadronPlayer, computerShotsCount);
	}

	if (gameOver)
	{
		break;
	}
}

// Финальные титры
System.Console.WriteLine();
System.Console.WriteLine("&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&");
System.Console.WriteLine("&&&&&&&                                            &&&&&&&");
Console.WriteLine((playerTurn) ?
						 "                        PLAYER WIN" : "                       COMPUTER WIN");
System.Console.WriteLine();
System.Console.WriteLine("&&&&&&&&&&&&&&           on  turn           &&&&&&&&&&&&&&");
System.Console.WriteLine();
System.Console.Write("                            ");
System.Console.WriteLine(overalTurnCount);
System.Console.WriteLine("&&&&&&&                                            &&&&&&&");
System.Console.WriteLine("&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&");

PrintSymbolMap(computerWorld, playerWorld, HIDESHIP);
// =====================================================================
// ====================  END of MAIN LOOP  =============================
// =====================================================================


// ----------------------- NEW UNIVERSAL MAIN PLAY FUNCTION
void MainPlay(int[,] map, int[,] sieve, int[,] bitmap, int[][] squadron, int turn)
{
	while (true)
	{
		turn++;

		// Вывод игрового поля !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
		System.Console.WriteLine();
		System.Console.WriteLine("------------------------------------------------------");

		PrintSymbolMap(computerWorld, playerWorld, HIDESHIP);

		int[] arrayFindAndCount = GetAndCountMaxIn2dArray(sieve);
		int numberToFind = arrayFindAndCount[0];
		int CountNumberToFind = arrayFindAndCount[1];

		int localCoordinates = GetCellToFire(sieve, numberToFind, CountNumberToFind);

		int localY = localCoordinates / 100;
		int localX = localCoordinates % 100;

		sieve[localY, localX] = 0;
		bitmap[localY, localX] = 0;

		Console.Write((playerTurn) ? $"Player Shot " : "COMPUTER Shot ");
		System.Console.WriteLine(turn);

		System.Console.Write("X ");
		System.Console.Write(localX);
		System.Console.Write(" Y ");
		System.Console.Write(localY);
		System.Console.Write(" ");

		int shootResult = map[localY, localX];


		// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
		// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! Останов между выстрелами !!!!!!!!!!!!!!!!!!!!!
		// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
		// Console.ReadLine();


		// Число для сдвига корабля. Четырехпалубник на карте имеет номер 10, последний однопалубник 19
		int shipShift = 10;

		// Выстрел в молоко
		if (shootResult < shipShift)
		{
			System.Console.WriteLine();
			System.Console.WriteLine("MISS");
			playerTurn = !playerTurn;
			break;
		}
		// Есть ПРОБИТИЕ!!!
		System.Console.WriteLine("   Есть ПРОБИТИЕ!!! * HIT!!!");

		// узнаем статус корабля, в который попали
		int shipUnderFire = shootResult - shipShift;

		int shipStatus = squadron[shipUnderFire][DIRECTIONS];


		// Если корабль потоплен
		if (shipStatus <= 1)
		{
			System.Console.WriteLine();
			System.Console.WriteLine("SHIP DESTROYED!!!");

			squadron[shipUnderFire][DIRECTIONS] = EMPTY;

			int newShipToFind = shipRemainMax(squadron);

			if (newShipToFind == -1)
			{
				gameOver = true;
				break;
			}

			System.Console.WriteLine();

			int[] currentShip = GetCurrentShip(squadron, shipUnderFire);

			// Зачистка решета от символов добивания корабля
			ChangeNumberIn2dArray(sieve, 2, EMPTY);

			FillCellsAroundShip(bitmap, currentShip, EMPTY, SIEVENUMBER);
			FillCellsAroundShip(sieve, currentShip, EMPTY, SIEVENUMBER);


			if (!playerTurn)
			{
				if (newShipToFind < MAXDECKS)
				{
					MAXDECKS = newShipToFind;

					NewSieve(computerSieve, computerBitShotMap, squadronPlayer);
				}
			}
			continue;
		}

		// Если корабль ранен - уменьшаем счетчик действующих палуб. 
		// DIRECTION - константа = 4, и в то же время номер живых палуб в массиве корабля. 
		// Зря так сделано, но работает.
		squadron[shipUnderFire][DIRECTIONS]--;

		FillCellsAroundWoundedDeckDiagonal(localX, localY, bitmap);
		FillCellsAroundWoundedDeckDiagonal(localX, localY, sieve);

		int numberToFire = 2; // Магическое число для последующего добивания корабля
		FillCellsAroundWoundedDeckCross(localX, localY, sieve, bitmap, numberToFire);

		continue;
	}
}


// --------------- Get current ship parameter
int[] GetCurrentShip(int[][] squadron, int ship)
{
	int shipArraySize = 5;
	int[] currentShip = new int[shipArraySize];

	for (int i = 0; i < shipArraySize; i++)
	{
		currentShip[i] = squadron[ship][i];
	}
	return currentShip;
}

// --------------- Fill Cells Around Wounded Deck
void FillCellsAroundWoundedDeckDiagonal(int localX, int localY, int[,] map)
{
	map[localY - 1, localX - 1] = 0;
	map[localY + 1, localX - 1] = 0;
	map[localY - 1, localX + 1] = 0;
	map[localY + 1, localX + 1] = 0;
}

void FillCellsAroundWoundedDeckCross(int localX, int localY, int[,] map, int[,] bitMap, int fillNumber)
{
	map[localY, localX - 1] = fillNumber * bitMap[localY, localX - 1];
	map[localY - 1, localX] = fillNumber * bitMap[localY - 1, localX];
	map[localY + 1, localX] = fillNumber * bitMap[localY + 1, localX];
	map[localY, localX + 1] = fillNumber * bitMap[localY, localX + 1];
}

// ------------------------- GetCellToFire
int GetCellToFire(int[,] sieve, int numberToFind, int count)
{
	int randomCellToFire = GetRandomFrom(1, count);

	int xAnDyToFire = randomCellXY(randomCellToFire, sieve, numberToFind);

	return xAnDyToFire;
}

// --------------------- Вывод символьных полей игрока и компьютера
void PrintSymbolMap(int[,] computerMap, int[,] playerMap, bool hideShip)
{
	System.Console.WriteLine();
	System.Console.WriteLine("         computer Map \t\t\t player Map");
	System.Console.WriteLine();
	System.Console.WriteLine("     1 2 3 4 5 6 7 8 9 10 \t     1 2 3 4 5 6 7 8 9 10");

	for (int i = 0; i < computerMap.GetLength(0); i++)
	{
		symbolY(i);

		for (int j = 0; j < computerMap.GetLength(1); j++)
		{
			string symbol = GetSymbol(computerMap[i, j], playerBitShotMap[i, j]);
			System.Console.Write($"{symbol} ");
		}

		System.Console.Write("\t");
		symbolY(i);

		for (int k = 0; k < playerMap.GetLength(1); k++)
		{
			string symbol = GetSymbol(playerMap[i, k], computerBitShotMap[i, k]);
			System.Console.Write($"{symbol} ");
		}

		System.Console.WriteLine();
	}
	System.Console.WriteLine();
}

// ------------------- Очистка карты от мусора
void ChangeNumberIn2dArray(int[,] map, int findNumber, int changeNumber)
{
	for (int i = 0; i < map.GetLength(0); i++)
	{
		for (int j = 0; j < map.GetLength(1); j++)
		{
			if (map[i, j] == findNumber)
			{
				map[i, j] = changeNumber;
			}
		}
	}
}

// --------------- GetAndCountMaxIn2dArray
int[] GetAndCountMaxIn2dArray(int[,] map)
{
	int maxValue = 0;

	foreach (int cell in map)
	{
		maxValue = cell;
		break;
	}

	int count = 0;

	foreach (int cell in map)
	{
		if (maxValue == cell)
		{
			count++;
		}

		if (maxValue < cell)
		{
			maxValue = cell;
			count = 1;
		}
	}

	int[] arr = new int[2];
	arr[0] = maxValue;
	arr[1] = count;

	return arr;
}

// --------------- Вывод координаты Y в символьное поле
void symbolY(int i)
{
	if (i == 0 || i == 11)
	{
		System.Console.Write("   ");
	}

	if (i > 0 && i < 11 && i != 10)
	{
		System.Console.Write(i);
		System.Console.Write("  ");
	}

	if (i == 10)
	{
		System.Console.Write(i);
		System.Console.Write(" ");
	}
}

// --------------- Получение символа карты
string GetSymbol(int number, int shot)
{
	if (shot == 1)
	{
		if (number == 0 || number == 1)
		{
			return " ";
		}

		if (number == 8)
		{
			return "M";
		}

		if (number > 8)
		{
			return "@";
		}
	}

	if (shot == 0)
	{
		if (number == 0 || number == 1)
		{
			return ".";
		}

		if (number == 8)
		{
			return "M";
		}

		if (number > 8)
		{
			return "X";
		}
	}


	// Здесь ЧТО-ТО случается!!!! Надо когда-нибудь разобраться!!! (02.02.2023)
	System.Console.WriteLine("Epic fail!!!");
	return "F";
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
	}
}

// -------------------- Функция генерации нового сита
void NewSieve(int[,] sieve, int[,] bitmap, int[][] squadron)
{
	// поиск и выбор числа палуб самого большого живого корабля в моменте
	int decksForRandomChoice = shipRemainMax(squadron);

	// случайный сдвиг для текущего сита. (Сито — это сито. Для просеивания. Никаких Dart-вёдер.)
	int shiftRandom = GetRandomFrom(0, decksForRandomChoice - 1);

	sieveGenerate(sieve, bitmap, shiftRandom, SIEVENUMBER, decksForRandomChoice);
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

// -------------------- Сито для поиска текущего корабля
void sieveGenerate(int[,] sieve, int[,] shotThroughMap, int shiftRandom, int fillNumber, int decks)
{
	shiftRandom = shiftRandom % decks;

	int localX = 1 + shiftRandom;
	int localY = 1;

	// Края карты: 1 - слева, 1 - справа. Всего: 2.
	int boardBorder = 2;

	int boardWidth = sieve.GetLength(1) - boardBorder;

	int boardHight = sieve.GetLength(0) - boardBorder;

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

// -------------------- Создание игрового поля для игрока или компьютера
void CreateMap(int[,] map, int[][] squadron)
{
	array2dBorder(map, BORDER);

	// индекс корабля
	int shipIndex = 10;

	foreach (int[] element in squadron)
	{
		ShipPlace(map, element, shipIndex);
		FillCellsAroundShip(map, element, SIEVENUMBER, EMPTY);

		shipIndex++;
	}

	ChangeNumberIn2dArray(map, SIEVENUMBER, EMPTY);
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

// --------------------------- fill cell around ship
void FillCellsAroundShip(int[,] map, int[] ship, int fillNumber, int numberToChange)
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
				if (map[j, i] == numberToChange)
				{
					map[j, i] = fillNumber;
				}
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

// --------------------------- Функция заполнения клеток кораблей их номерами
void ShipNumbersOnMap(int[,] map, int[] ship, int shipIndex)
{
	int localX = ship[0];
	int localY = ship[1];
	int direction = ship[2];
	int decks = ship[3];

	map[localY, localX] = shipIndex;

	if (decks > 1)
	{
		int[] dXdYupperLevel = GetDirection(direction);

		// получаем приращение координат от головы корабля
		int dXupperLevel = dXdYupperLevel[0];
		int dYupperLevel = dXdYupperLevel[1];

		for (int decksIndex = 1; decksIndex < decks; decksIndex++)
		{
			map[localY + decksIndex * dYupperLevel, localX + decksIndex * dXupperLevel] = shipIndex;
		}
	}
}

// --------------------------- New Year 2023 - SHIP PLACE
void ShipPlace(int[,] map, int[] ship, int shipIndex)
{
	int decks = ship[3];

	int emptyCells = countCellsWithNumber(map, EMPTY);

	while (emptyCells > 0)
	{
		int randomCell = GetRandomFrom(1, emptyCells);

		int xAnDy = randomCellXY(randomCell, map, EMPTY);

		int localY = xAnDy / 100;
		int localX = xAnDy % 100;

		map[localY, localX] = 7;

		int[] directionArray = new int[DIRECTIONS];
		arrayUniqFill(directionArray);

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

			// Если корабль однопалубный или хвост корабля в пределах игрового поля, 
			// и эта клетка свободна - Ура. Ship on map
			if (
				decks == 1 ||
				tailX > 0 &&
				tailX < map.GetLength(0) - 1 &&
				tailY > 0 &&
				tailY < map.GetLength(1) - 1 &&
				map[tailY, tailX] == 0
				)
			{
				ship[0] = localX;
				ship[1] = localY;
				ship[2] = item;

				ShipNumbersOnMap(map, ship, shipIndex);
				return;
			}
			continue;
		}
		emptyCells--;
	}
	// Такого не бывает! Но если вдруг корабль не поместился на карту:
	System.Console.WriteLine("Epic fail!!!");
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

// --------------------- RANDOM NUMBER from - to -------------------
int GetRandomFrom(int bottom, int top)
{
	Random rnd = new Random();
	int result = rnd.Next(bottom, top + 1);
	return result;
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
	// Интересно, что будет делать вызывавшая функция с этим результатом?
	return localxAnDy;
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