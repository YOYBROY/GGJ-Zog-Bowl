using UnityEngine;

public class UsedGunSwitcher : MonoBehaviour
{
    [SerializeField] GameObject gunModel;
    [SerializeField] GameObject usedGunModel;
    [SerializeField] GameObject hideObject;

    private void Awake()
    {
        if (gunModel != null)
            gunModel.SetActive(true);
        if (usedGunModel != null)
            usedGunModel.SetActive(false);
    }

    public void SwitchGunModel()
    {
        if(gunModel != null)
        {
            gunModel.SetActive(false);
        }
        if (usedGunModel != null)
        {
            usedGunModel.SetActive(true);
        }
        if (hideObject != null)
        {
            hideObject.SetActive(false);
        }
    }
}