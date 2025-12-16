using System.Collections.Generic;
using UnityEngine;

public static class EnemyManager
{
    private static readonly List<Enemy> _activeEnemies = new List<Enemy>();

    public static void Register(Enemy enemy)
    {
        if (!_activeEnemies.Contains(enemy))
        {
            _activeEnemies.Add(enemy);
        }
    }

    public static void Unregister(Enemy enemy)
    {
        _activeEnemies.Remove(enemy);
    }

    public static Enemy GetNearest(Vector3 position, float maxRange)
    {
        Enemy nearest = null;
        float minDstSqr = maxRange * maxRange;

        foreach (Enemy enemy in _activeEnemies)
        {
            float dstSqr = (enemy.transform.position - position).sqrMagnitude;
            if (dstSqr < minDstSqr)
            {
                minDstSqr = dstSqr;
                nearest = enemy;
            }
        }

        return nearest;
    }
}
