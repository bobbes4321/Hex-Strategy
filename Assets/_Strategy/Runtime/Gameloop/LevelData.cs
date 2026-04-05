using System;
using System.Collections.Generic;
using System.Linq;
using _Strategy.Runtime.Board;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Strategy.Runtime.Gameloop
{
    [CreateAssetMenu(menuName = "Strategy/Create LevelData", fileName = "LevelData", order = 0)]
    public class LevelData : Settings.Settings
    {
        [TypeFilter("GetFilteredTypeList")] public IHexGenerator HexGenerator;
        public List<Piece.Piece> EnemyPieces;
    
        public IEnumerable<Type> GetFilteredTypeList()
        {
            return typeof(IHexGenerator).GetFilteredTypeList();
        }
    }

    public static class OdinHelpers
    {
        public static IEnumerable<Type> GetFilteredTypeList(this Type type)
        {
            return type.Assembly.GetTypes()
                .Where(x => !x.IsAbstract)
                .Where(x => !x.IsGenericTypeDefinition)
                .Where(type.IsAssignableFrom);
        }
    }
}