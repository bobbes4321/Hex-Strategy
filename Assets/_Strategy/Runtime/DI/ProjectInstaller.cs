using _Strategy.Runtime.Board;
using _Strategy.Runtime.Deck;
using _Strategy.Runtime.Gameloop;
using _Strategy.Runtime.Util;
using Reflex.Core;
using Runtime.Messaging;
using UnityEngine;

public class ProjectInstaller : MonoBehaviour, IInstaller
{
    [SerializeField] private LevelSettings _levelSettings;
    [SerializeField] private HexView _hexViewPrefab;
    [SerializeField] private RaritySettings _raritySettings;
    
    public void InstallBindings(ContainerBuilder builder)
    {
        builder.AddSingleton(_levelSettings);
        builder.AddSingleton(_raritySettings);
        
        MonoBehaviourFactory<MessagingManager>.CreateAndRegister(builder);
        var levelManager = MonoBehaviourFactory<LevelManager>.CreateAndRegister(builder);
        MonoBehaviourFactory<Cheats>.CreateAndRegister(builder);

        builder.AddSingleton(levelManager.PlayerData);
        builder.AddSingleton(_hexViewPrefab);
    }
}