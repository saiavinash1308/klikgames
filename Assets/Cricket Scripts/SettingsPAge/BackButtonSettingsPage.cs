using UnityEngine;

public class BackButtonSettingsPage : MonoBehaviour
{
    public GameObject Home;
    public GameObject ClassicSelect;
    public GameObject Wallet;

    // Start is called before the first frame update
    void Start()
    {
        // Initially activate the Home panel and deactivate the others
        // ActivatePanel(Home);
        Home.SetActive(true);
    }

    private void SetAllPanelsInactive()
    {
        Home.SetActive(false);
        Wallet.SetActive(false);
        ClassicSelect.SetActive(false);
    }

    public void ActivatePanel(GameObject panelToActivate)
    {
        SetAllPanelsInactive();
        panelToActivate.SetActive(true);
    }

   
}