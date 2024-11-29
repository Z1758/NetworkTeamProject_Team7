using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KSH_FriendsWindow : MonoBehaviour
{
    [SerializeField] GameObject _friendsWindow;

    private bool _isfrinds = false;

    private void OnEnable()
    {
        _friendsWindow.SetActive(false);
        _isfrinds = false;
    }

    public void FriendsWindow()
    {
        if (_isfrinds == true)
        {
            _friendsWindow.SetActive(false);
            _isfrinds = false;
        }
        else
        {
            _friendsWindow.SetActive(true);
            _isfrinds = true;
        }
    }
}
