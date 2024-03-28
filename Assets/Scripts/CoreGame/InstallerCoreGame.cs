using CoreGame.SO;
using SO;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using VillageGame.UI.Panels.Warnings;

namespace CoreGame
{
    public class InstallerCoreGame : LifetimeScope
    {
        [Space, Header("Scriptable objects")]
        [SerializeField] private LevelDatabase _levelDatabase;
        [SerializeField] private VFXConfig _vfxConfig;
        [SerializeField] private CutSceneConfig _cutSceneConfig;

        protected override void Configure(IContainerBuilder builder)
        {
            BindScriptableObjects(builder);
            builder.RegisterComponentInHierarchy<LevelStorage>().AsSelf();
            builder.RegisterComponentInHierarchy<EnergyWarningUIPanel>().AsSelf();
        }

        private void BindScriptableObjects(IContainerBuilder builder)
        {
            builder.RegisterInstance(_levelDatabase);
            builder.RegisterInstance(_vfxConfig);
            builder.RegisterInstance(_cutSceneConfig);
        }
    }
}
