using System;
using UnityEngine;
using UnityEngine.Events;

public static class EventManager
{
    public static UnityEvent<string> SelectedBuildingForProduction = new UnityEvent<string>();
    public static UnityEvent ProductionBuildingCompleted = new UnityEvent();
    
    public static UnityEvent<string> SelectedBuildingForInformation = new UnityEvent<string>();
    public static UnityEvent<Soldier> SelectedSoldierForInformation = new UnityEvent<Soldier>();
    
    // instead of checking with if statement on SelectedBuildingForInformation, we use different event so
    //its easier to add more spawning buildings to the game and make them open spawning tab on ui
    // if there will be another spawner building, we can swap barracks with different parent class
    public static UnityEvent<Barracks> SelectedBuildingForSpawning = new UnityEvent<Barracks>();

    public static UnityEvent<Vector3, IHaveHealth> ToBeAttackedObjectSelected = new UnityEvent<Vector3, IHaveHealth>();
}