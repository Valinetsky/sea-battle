int[] shipRemainPlayer = { 1, 2, 3, 4 };



// -------------------- Выбор решета для поиска корабля
int[] currentTargetShip(int[] squadron)
{
	int currentTarget = -1;

	while (true)
	{
		int localIndex = 0;
		if (squadron[localIndex] != 0)
		{
			currentTarget = squadron[localIndex];
			return currentTarget;
		}

		localIndex++;

		if (localIndex > squadron.Length)
		{
			return currentTarget;
		}
	}
}