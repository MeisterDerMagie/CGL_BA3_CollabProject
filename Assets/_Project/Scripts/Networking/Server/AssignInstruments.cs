using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class AssignInstruments : MonoBehaviour
{
    private void Start()
    {
        //only run on server
        if (!NetworkManager.Singleton.IsServer) return;
        
        var assignedInstruments = new List<int>();
        
        //get random silly harmonic instrument
        assignedInstruments.Add(GetRandomInstrumentIdOfCategory(InstrumentCategory.SillyHarmonic));
        
        //get random silly rhythmic instrument
        assignedInstruments.Add(GetRandomInstrumentIdOfCategory(InstrumentCategory.SillyRhythmic));

        //get random harmonic instrument
        assignedInstruments.Add(GetRandomInstrumentIdOfCategory(InstrumentCategory.Harmonic));

        //get random rhythmic instrument
        assignedInstruments.Add(GetRandomInstrumentIdOfCategory(InstrumentCategory.Rhythmic));

        //assign random instruments to players
        foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
        {
            client.PlayerObject.GetComponent<PlayerData>().SetInstruments(assignedInstruments);
        }
    }

    private int GetRandomInstrumentIdOfCategory(InstrumentCategory category)
    {
        //get all instruments of category
        List<Instrument> allInstrumentsOfCategory = InstrumentsManager.Instance.GetAllInstrumentsOfCategory(category);
        
        //generate random instrument
        int randomIndex = Random.Range(0, allInstrumentsOfCategory.Count);
        
        //return instrument id
        return allInstrumentsOfCategory[randomIndex].instrumentId;
    }
}