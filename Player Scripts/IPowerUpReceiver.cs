using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPowerUpReceiver
{
    void ApplyPowerUp(string type, int value);
}