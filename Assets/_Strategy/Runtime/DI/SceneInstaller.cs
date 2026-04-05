using _Strategy.Runtime.Board;
using _Strategy.Runtime.Deck;
using _Strategy.Runtime.Gameloop;
using Cysharp.Threading.Tasks;
using Reflex.Core;
using UnityEngine;

public class SceneInstaller : MonoBehaviour, IInstaller
{
    public void InstallBindings(ContainerBuilder builder)
    {
        builder.AddSingleton(_ => DI.Resolve<LevelManager>().CurrentLevel.HexGenerator);
        builder.AddSingleton(_ => DI.Resolve<LevelManager>().CurrentLevel.EnemyPieces);
        builder.AddSingleton(_ => DI.Resolve<LevelManager>().PlayerData);
        MonoBehaviourFactory<HexHighlightManager>.CreateAndRegister(builder, false);
        MonoBehaviourFactory<Board>.CreateAndRegister(builder, false);
        MonoBehaviourFactory<GameManager>.CreateAndRegister(builder, false);
        builder.AddSingleton(FindAnyObjectByType<Deck>());
        builder.AddSingleton(FindAnyObjectByType<UpgradeSelection>());
        builder.AddSingleton(FindAnyObjectByType<Hand>());
        builder.AddSingleton(FindAnyObjectByType<AP>());
        builder.AddSingleton(FindAnyObjectByType<EnemyAI>());
        builder.AddSingleton(FindAnyObjectByType<HexBlockManager>());
    }
}