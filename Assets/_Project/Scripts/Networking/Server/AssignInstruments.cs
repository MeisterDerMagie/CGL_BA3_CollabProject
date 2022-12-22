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
        
        //assign random instruments to players
        foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
        {
            List<Instrument> allInstruments = InstrumentsManager.Instance.instruments;
            var assignedInstruments = new List<uint>();
            
            for (int i = 0; i < Constants.INSTRUMENTS_AMOUNT; i++)
            {
                int iterations = 0;
                while (true)
                {
                    //safety exit
                    iterations += 1;
                    if (iterations == 1000)
                    {
                        Debug.LogError("Something went wrong and we got caught in a while loop while assigning instruments.", this);
                        break;
                    }
                    
                    //generate random instrument
                    int randomInstrumentsIndex = Random.Range(0, allInstruments.Count);
                    
                    //check if the instrument was already chosen. If yes, we try again.
                    if (assignedInstruments.Contains(allInstruments[randomInstrumentsIndex].instrumentId)) continue;
                    
                    //if the instrument wasn't chosen before, assign it.
                    assignedInstruments.Add(allInstruments[randomInstrumentsIndex].instrumentId);
                    break;
                }
            }
            
            //when all instruments have been randomly assigned, set them.
            client.PlayerObject.GetComponent<PlayerData>().SetInstruments(assignedInstruments);
        }
    }
}