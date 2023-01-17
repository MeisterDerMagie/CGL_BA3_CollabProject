//(c) copyright by Martin M. Klöckener
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Wichtel;

public class InstrumentsManager : MonoBehaviour
{
    #region Singleton
    private static InstrumentsManager instance;
    public static InstrumentsManager Instance => instance;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Debug.LogWarning("Singleton instance already exists. You should never initialize a second one.", this);
    }
    #endregion

    [SerializeField, ReadOnly] public List<Instrument> instruments = new List<Instrument>();

    public List<Instrument> GetAllInstrumentsOfCategory(InstrumentCategory category)
    {
        List<Instrument> instrumentsByCategory = new List<Instrument>();

        foreach (Instrument instrument in instruments)
        {
            if (instrument.category == category) instrumentsByCategory.Add(instrument);
        }

        return instrumentsByCategory;
    }

    public Instrument GetInstrument(int id)
    {
        foreach (Instrument instrument in instruments)
        {
            if (instrument.instrumentId == id) return instrument;
        }
        
        Debug.LogError($"You tried to get an instrument that doesn't exist. Id: {id.ToString()}");
        return null;
    }

    public bool IsValidInstrumentId(int id)
    {
        foreach (Instrument instrument in instruments)
        {
            if (instrument.instrumentId == id) return true;
        }

        return false;
    }
    
    #if UNITY_EDITOR
    private void OnValidate()
    {
        //find all instruments in the Asset database
        instruments = EditorUtilities.GetAssets<Instrument>();
        
        //check if there any duplicate ids
        var ids = new List<int>();
        foreach (Instrument instrument in instruments)
        {
            if (!ids.Contains(instrument.instrumentId))
                ids.Add(instrument.instrumentId);
            else
                Debug.LogError("There are two or more instruments with the same ID. Make sure that each instrument has a different ID!");
        }
        
        //order them by their ID
        instruments = instruments.OrderBy(character => character.instrumentId).ToList();
    }
    #endif
}