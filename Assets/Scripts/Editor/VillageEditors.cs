using SO.Data;
using SO.Data.Characters;
using UnityEditor;
using UnityEngine;
using VillageGame.Data.Types;
using VillageGame.Services.Buildings;
using VillageGame.Services.CutScenes;
using VillageGame.Services.Snowdrifts;
using VillageGame.UI.Elements;

[CustomEditor(typeof(TileAreasService))]
public class TileAreasServiceEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TileAreasService service = (TileAreasService)target;
        
        if (GUILayout.Button("Analyze Builds Tilemap"))
        {
            service.ClearBuildsAreas();
            service.AnalyzeSquredTileAreas(service.Tilemap_main, service.GetTile(TileType.Building),
                service.BuildingTileAreas, new Vector2Int(2, 2));
        }

        if (GUILayout.Button("Analyze Decorations Tilemap"))
        {
            service.ClearDecorationAreas();
            service.AnalyzeTilemap(service.Tilemap_main, service.GetTile(TileType.Decoration),
                service.DecorationTileAreas, new Vector2Int(2, 2));
        }

        if (GUILayout.Button("Analyze Character spawn Tilemap"))
        {
            service.ClearCharacterSpawnAreas();
            service.AnalyzeSquredTileAreas(service.Tilemap_characterSpawn, service.GetTile(TileType.CharacterSpawn),
                service.CharacterSpawnAreas, new Vector2Int(1, 1));
        }
    }
}

[CustomEditor(typeof(ConstructionSiteService))]
public class ConstructionSiteServiceEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        ConstructionSiteService service = (ConstructionSiteService)target;
        
        if (GUILayout.Button("Analyze Builds Tilemap"))
        {
            service.RefreshSites();
        }
    }
}

[CustomEditor(typeof(BigSnowdrifts))]
public class BigSnowdriftsEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        BigSnowdrifts service = (BigSnowdrifts)target;
        
        if (GUILayout.Button("Play hide"))
        {
            service.PlayHideAnimation();
        }
    }
}

[CustomEditor(typeof(BuildingAreaService))]
public class BuildingAreaServiceEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        BuildingAreaService service = (BuildingAreaService)target;

        Undo.RecordObject(service,"Create Areas");
        
        if (GUILayout.Button("Create Areas"))
        {
 
            service.CreateAllAreas();
        }
        
        

    }
}


[CustomEditor(typeof(DialoguePortrait))]
public class DialoguePortraitEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        DialoguePortrait portrait = (DialoguePortrait)target;

        if (GUILayout.Button("Show"))
        {
            portrait.Show();
        }

        if (GUILayout.Button("Hide"))
        {
            portrait.Hide();
        }
    }
}

[CustomEditor(typeof(CutSceneAnimator))]
public class CutSceneAnimatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        CutSceneAnimator portrait = (CutSceneAnimator)target;

        if (GUILayout.Button("PlayArrivalOfGuidAnimation"))
        {
            portrait.PlayArrivalOfGuidAnimation();
        }
    }
}
[CustomEditor(typeof(CutSceneEditor))]
public class CutSceneDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        CutSceneEditor portrait = (CutSceneEditor)target;

        if (GUILayout.Button("Set zoom"))
        {
            portrait.SetZoom();
        }
    
        if (GUILayout.Button("get time"))
        {
            portrait.GetTime();
        }
    }
}

