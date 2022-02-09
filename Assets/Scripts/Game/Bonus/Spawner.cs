using TSW;

using UnityEngine;

namespace Game.Bonus
{
	public class Spawner : TSW.Design.USingleton<Spawner>
	{
		[SerializeField]
		private GameObject _bonusPrefab;

		[SerializeField]
		private float _spawnAltitude = 200f;

		[SerializeField]
		private float _spawnDistance = 400f;
		private GameObject _nextBonusSlot;
		private Transform _player;
		private int _slotId = 0;

		public void ClearBonuses()
		{
			transform.DestroyAllChild();
		}

		public void StartSpawning(Transform player)
		{
			_player = player;
			_slotId = 0;
			PrepareNextBonus();
			TSW.OnDestroyNotifier.RegisterOnDestroy(player, OnDestroyPlayer);
		}

		public void OnDestroyPlayer(Transform player)
		{
			_player = null;
			_nextBonusSlot = null;
		}

		private void PrepareNextBonus()
		{
			_nextBonusSlot = GameObject.Find(LevelGen.Jobs.BonusSlotCreator.BonusSlotName(++_slotId));
		}

		private void OnDestroy()
		{
			TSW.OnDestroyNotifier.UnregisterOnDestroy(_player, OnDestroyPlayer);
		}

		private void Update()
		{
			if (_nextBonusSlot != null && Mathf.Abs(Vector3.Distance(_nextBonusSlot.transform.position, _player.position)) < _spawnDistance)
			{
				//				_nextBonusSlot.SetActive(false);
				GameObject bonus = GameObject.Instantiate(_bonusPrefab);
				Vector3 pos = _nextBonusSlot.transform.position;
				bonus.transform.SetParent(transform);
				pos.y = _spawnAltitude;
				bonus.transform.position = pos;
				_nextBonusSlot = null;
				PrepareNextBonus();
			}
		}
	}
}
