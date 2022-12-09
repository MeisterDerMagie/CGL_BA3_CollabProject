using System.Collections;
using System.Collections.Generic;
using Doodlenite;
using UnityEngine;
using UnityEngine.SceneManagement;
using Wichtel.SceneManagement;

namespace Doodlenite {
public class HostLobby : MonoBehaviour
{
    public void Host() => ServerProviderCommunication.Instance.HostRequest();

    public void CancelHostRequest() => ServerProviderCommunication.Instance.CancelHostRequest();
}
}